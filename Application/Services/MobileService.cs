using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Helpers;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.Mobile;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class MobileService : BaseService, IMobileService
    {
        private readonly IMapper mapper;
        private readonly IProgramRepository programRepository;
        private readonly IEducatorRepository educatorRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IUserRepository userRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;
        public MobileService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository, IProgramRepository programRepository, IEducatorRepository educatorRepository, IStudentRepository studentRepository, IUserRepository userRepository, IDocumentRepository documentRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.programRepository = programRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
            this.educatorRepository = educatorRepository;
            this.studentRepository = studentRepository;
            this.userRepository = userRepository;
            this.documentRepository = documentRepository;
        }
        public async Task<PaginationModel<MobileProgramPaginateResponseDTO>> ProgramGetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var ordersQuery = programRepository.PaginateListQuery(zone);
            var filterResponse = ordersQuery.ToFilterView(filter);

            var programs = await filterResponse.Query.ToListAsync(cancellationToken);

            var response = new PaginationModel<MobileProgramPaginateResponseDTO>
            {
                Items = mapper.Map<List<MobileProgramPaginateResponseDTO>>(programs),
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<MobileProgramResponseDTO>> ProgramGetById(CancellationToken cancellationToken, long id)
        {
            var program = await programRepository.GetWithSubRecords(cancellationToken, id);
            return new()
            {
                Result = true,
                Item = new MobileProgramResponseDTO()
                {
                    ExpertiseBranchId = program.ExpertiseBranchId,
                    ExpertiseBranchName = program.ExpertiseBranch?.Name,
                    FacultyId = program.FacultyId,
                    FacultyName = program.Faculty?.Name,
                    HospitalId = program.HospitalId,
                    HospitalName = program.Hospital?.Name,
                    UniversityId = program.Faculty?.UniversityId,
                    UniversityName = program.Faculty?.University?.Name,
                    AuthorizationDetails = mapper.Map<List<MobileAuthDetailResponseDTO>>(program.AuthorizationDetails)
                }
            };
        }

        public async Task<PaginationModel<MobileEducatorPaginateResponseDTO>> EducatorGetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var ordersQuery = educatorRepository.OnlyPaginateListQuery(zone);

            var roleFilter = filter?.Filter?.Filters?.Where(x => x.Field == "Role");
            if (roleFilter != null)
            {
                foreach (var role in roleFilter.ToList())
                {
                    ordersQuery = ordersQuery.Where(x => x.Roles.Contains((string)role.Value));
                    filter.Filter.Filters.Remove(role);
                }
            }
            var titleFilter = filter?.Filter?.Filters?.Where(x => x.Field == "AdminTitle");
            if (titleFilter != null)
            {
                foreach (var title in titleFilter.ToList())
                {
                    ordersQuery = ordersQuery.Where(x => x.EducatorAdministrativeTitles.Contains((string)title.Value));
                    filter.Filter.Filters.Remove(title);
                }
            }
            FilterResponse<EducatorPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var educators = await filterResponse.Query.ToListAsync(cancellationToken);
            educators?.ForEach(x => x.IdentityNo = StringHelpers.MaskIdentityNumber(x.IdentityNo));

            var response = new PaginationModel<MobileEducatorPaginateResponseDTO>
            {
                Items = mapper.Map<List<MobileEducatorPaginateResponseDTO>>(educators),
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<EducatorResponseDTO>> EducatorGetById(CancellationToken cancellationToken, long id)
        {
            Educator educator = await educatorRepository.GetWithSubRecords(cancellationToken, id);

            var educatorExBrs = new List<EducatorExpertiseBranchResponseDTO>();
            var eduAdministrativeTitles = new List<EducatorAdministrativeTitleResponseDTO>();

            if (educator.EducatorAdministrativeTitles?.Count > 0)
            {
                foreach (var item in educator.EducatorAdministrativeTitles)
                {
                    eduAdministrativeTitles.Add(new EducatorAdministrativeTitleResponseDTO { Id = item.Id, AdministrativeTitle = mapper.Map<TitleResponseDTO>(item.AdministrativeTitle), AdministrativeTitleId = item.AdministrativeTitleId, EducatorId = item.EducatorId });
                }
                educator.EducatorAdministrativeTitles = null;
            }

            if (educator.EducatorExpertiseBranches?.Count > 0)
            {
                foreach (var item in educator.EducatorExpertiseBranches)
                {
                    var eduExBr = new EducatorExpertiseBranchResponseDTO
                    {
                        ExpertiseBranchName = item.ExpertiseBranch.Name,
                        ExpertiseBranchId = item.ExpertiseBranchId,
                        //IsPrincipal = item.ExpertiseBranch.IsPrincipal,
                        RegistrationDate = item.RegistrationDate,
                        RegistrationNo = item.RegistrationNo,
                        RegistrationBranchName = item.RegistrationBranchName,
                        RegistrationGraduationSchool = item.RegistrationGraduationSchool,
                        EducatorId = item.EducatorId,
                        Id = item.Id,
                        SubBranchIds = item.ExpertiseBranch.SubBranches.Select(x => x.SubBranchId).ToList()
                    };

                    item.ExpertiseBranch.SubBranches = null;
                    eduExBr.ExpertiseBranch = mapper.Map<ExpertiseBranchResponseDTO>(item.ExpertiseBranch);
                    educatorExBrs.Add(eduExBr);
                }
                educator.EducatorExpertiseBranches = null;
            }

            EducatorResponseDTO response = mapper.Map<EducatorResponseDTO>(educator);

            response.User.IdentityNo = StringHelpers.MaskIdentityNumber(educator.User.IdentityNo);

            response.EducatorExpertiseBranches = educatorExBrs;
            response.EducatorAdministrativeTitles = eduAdministrativeTitles;

            var docs = await documentRepository.GetByEntityId(cancellationToken, id, DocumentTypes.AssociateProfessorship);
            var docs_1 = await documentRepository.GetByEntityId(cancellationToken, id, DocumentTypes.DeclarationDocument);
            var docs_2 = await documentRepository.GetByEntityId(cancellationToken, id, DocumentTypes.SpecializationBoardChairman);
            var docs_3 = await documentRepository.GetByEntityId(cancellationToken, id, DocumentTypes.SpecializationBoardMember);

            docs_1.ForEach(x => docs.Add(x));
            docs_2.ForEach(x => docs.Add(x));
            docs_3.ForEach(x => docs.Add(x));

            response.Documents = mapper.Map<List<DocumentResponseDTO>>(docs); ;

            if (response.EducatorPrograms != null)
                foreach (var item in response.EducatorPrograms)
                    item.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, (long)item.Id, DocumentTypes.PlaceOfDuty));

            if (response.EducationOfficers != null)
                foreach (var item in response.EducationOfficers)
                    item.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, (long)item.Id, DocumentTypes.EducationOfficerAssignmentLetter));

            return new ResponseWrapper<EducatorResponseDTO> { Result = true, Item = response };
        }

        public async Task<PaginationModel<MobileStudentPaginateResponseDTO>> StudentGetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var ordersQuery = studentRepository.OnlyPaginateListQuery(zone);

            var filterResponse = ordersQuery.ToFilterView(filter);

            var students = await filterResponse.Query.ToListAsync(cancellationToken);

            students?.ForEach(x => x.IdentityNo = StringHelpers.MaskIdentityNumber(x.IdentityNo));

            var response = new PaginationModel<MobileStudentPaginateResponseDTO>
            {
                Items = mapper.Map<List<MobileStudentPaginateResponseDTO>>(students),
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> StudentGetById(CancellationToken cancellationToken, long id, bool isDeleted = false)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var query = studentRepository.GetWithSubRecords(zone);

            var student = await query.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == isDeleted && x.IsHardDeleted == false, cancellationToken);

            if (student != null)
            {
                var studentExBrs = new List<StudentExpertiseBranchResponseDTO>();

                foreach (var item in student?.StudentExpertiseBranches ?? new List<StudentExpertiseBranch>())
                {
                    if (item.ExpertiseBranchId != null)
                    {
                        var stuExBr = new StudentExpertiseBranchResponseDTO
                        {
                            ExpertiseBranchName = item.ExpertiseBranch.Name,
                            ExpertiseBranchId = item.ExpertiseBranchId,
                            IsPrincipal = item.ExpertiseBranch.IsPrincipal,
                            RegistrationDate = item.RegistrationDate,
                            StudentId = item.StudentId,
                            Id = item.Id
                        };
                        studentExBrs.Add(stuExBr);
                    }
                }
                student.StudentExpertiseBranches = null;
                var response = mapper.Map<StudentResponseDTO>(student);
                response.User.IdentityNo = StringHelpers.MaskIdentityNumber(student.User.IdentityNo);
                response.StudentExpertiseBranches = studentExBrs;
                if (student.OriginalProgramId != null && student.OriginalProgramId > 0)
                {

                    var protocolProgram = await studentRepository.IsProtocolProgram((long)student.OriginalProgramId, cancellationToken);
                    if (protocolProgram != null)
                        response.ProtocolOrComplement = protocolProgram.Type;
                }

                return new ResponseWrapper<StudentResponseDTO> { Result = true, Item = response };
            }
            else
                return new() { Result = false, Message = "You are not authorized for this operation!" };
        }

        public async Task<ResponseWrapper<MobileUserResponseDTO>> UserGetById(CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            if (userId == 0) return new();
            User user = await userRepository.GetByIdWithSubRecords(cancellationToken, userId);
            return new() { Result = true, Item = mapper.Map<MobileUserResponseDTO>(user) };
        }
    }
}
