using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class EducatorProgramService : BaseService, IEducatorProgramService
    {

        private readonly IMapper mapper;
        private readonly IEducatorProgramRepository educatorProgramRepository;
        private readonly IEducationOfficerRepository educationOfficerRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IUserRepository userRepository;
        private readonly IEducatorRepository educatorRepository;

        public EducatorProgramService(IMapper mapper, IUnitOfWork unitOfWork, IEducatorProgramRepository educatorProgramRepository, IKoruRepository koruRepository, IEducationOfficerRepository educationOfficerRepository, IHttpContextAccessor httpContextAccessor, IDocumentRepository documentRepository, IUserRepository userRepository, IEducatorRepository educatorRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.educatorProgramRepository = educatorProgramRepository;
            this.koruRepository = koruRepository;
            this.educationOfficerRepository = educationOfficerRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.documentRepository = documentRepository;
            this.userRepository = userRepository;
            this.educatorRepository = educatorRepository;
        }

        public async Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByEducatorIdAsync(CancellationToken cancellationToken, long educatorId)
        {
            List<EducatorProgram> educatorProgram = await educatorProgramRepository.GetIncludingList(cancellationToken, x => x.EducatorId == educatorId && x.Educator.IsDeleted == false && x.Program.IsDeleted == false, x => x.Program);
            List<EducatorProgramResponseDTO> response = mapper.Map<List<EducatorProgramResponseDTO>>(educatorProgram);

            return new ResponseWrapper<List<EducatorProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> GetById(CancellationToken cancellationToken, long id)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var educatorProgram = await educatorProgramRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.Program);

            bool result = IsAuthorized(educatorProgram, zone);

            if (!result)
                return new ResponseWrapper<EducatorProgramResponseDTO>() { Result = false, Message = "You are not authorized for this operation!" };

            var response = await educatorProgramRepository.Queryable().Select(x => new EducatorProgramResponseDTO()
            {
                Id = x.Id,
                EducatorId = x.EducatorId,
                ProgramId = x.ProgramId,
                Program = new() { HospitalId = x.Program.HospitalId, Hospital = new() { Province = new() { Name = x.Program.Hospital.Province.Name }, ProvinceId = x.Program.Hospital.ProvinceId, Name = x.Program.Hospital.Name, FacultyId = x.Program.Hospital.FacultyId, Faculty = new() { UniversityId = x.Program.Hospital.Faculty.UniversityId } }, ExpertiseBranchId = x.Program.ExpertiseBranchId, ExpertiseBranch = new() { Name = x.Program.ExpertiseBranch.Name } },
                DutyType = x.DutyType,
                DutyStartDate = x.DutyStartDate,
                DutyEndDate = x.DutyEndDate,
                IsEducationOfficer = false
            }).FirstOrDefaultAsync(x => x.Id == id);
            var educationOfficer = await educationOfficerRepository.GetByAsync(cancellationToken, x => x.EducatorId == response.EducatorId && x.ProgramId == response.ProgramId && x.EndDate == null);
            if (educationOfficer != null)
            {
                response.IsEducationOfficer = true;
                response.EducationOfficerId = educationOfficer.Id;
                response.EducationOfficerDutyEndDate = educationOfficer.EndDate;
                response.EducationOfficerDutyStartDate = educationOfficer.StartDate;
                response.EducationOfficerDocuments = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, educationOfficer.Id, DocumentTypes.EducationOfficerAssignmentLetter));
            }
            response.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, response.Id ?? 0, DocumentTypes.PlaceOfDuty));

            return new ResponseWrapper<EducatorProgramResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId)
        {
            List<EducatorProgram> educatorProgram = await educatorProgramRepository.GetListByHospitalId(hospitalId).ToListAsync(cancellationToken);
            List<EducatorProgramResponseDTO> response = mapper.Map<List<EducatorProgramResponseDTO>>(educatorProgram);

            return new ResponseWrapper<List<EducatorProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId)
        {
            return new()
            {
                Result = true,
                Item = await educatorProgramRepository.Queryable().OrderBy(x => x.Educator.User.Name).Where(x => x.ProgramId == programId && x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false).Select(x => new EducatorProgramResponseDTO()
                {
                    Id = x.Id,
                    EducatorId = x.Educator.Id,
                    ProgramId = x.ProgramId,
                    Educator = x.Educator == null ? null : new EducatorResponseDTO()
                    {
                        Id = x.Educator.Id,
                        UserId = x.Educator.User.Id,
                        IsDeleted = x.Educator.IsDeleted,
                        User = x.Educator.User == null ? null : new UserAccountDetailInfoResponseDTO()
                        {
                            Id = x.Educator.User.Id,
                            Name = x.Educator.User.Name,
                            IsDeleted = x.Educator.User.IsDeleted
                        }
                    }
                }).ToListAsync(cancellationToken)
            };
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorProgramDTO educatorProgramDTO)
        {
            EducatorProgram educatorProgram = mapper.Map<EducatorProgram>(educatorProgramDTO);
            var educator = await educatorRepository.GetByIdAsync(cancellationToken, educatorProgramDTO.EducatorId ?? 0);

            if (educatorProgramDTO.IsEducationOfficer == true)
                await AddEducationOfficer(educatorProgramDTO, cancellationToken);

            var addedEducatorProgram = await educatorProgramRepository.AddAsync(cancellationToken, educatorProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            foreach (var item in educatorProgramDTO.Documents)
            {
                var document = await documentRepository.GetByIdAsync(cancellationToken, item.Id);
                document.EntityId = addedEducatorProgram.Id;
            }

            var response = await GetById(cancellationToken, addedEducatorProgram.Id);

            return new ResponseWrapper<EducatorProgramResponseDTO> { Result = true, Item = response.Item };
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorProgramDTO educatorProgramDTO)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var existEducatorProgram = await educatorProgramRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.Program.Hospital.Faculty);
            var existEducationOfficer = await educationOfficerRepository.GetByAsync(cancellationToken, x => x.EducatorId == educatorProgramDTO.EducatorId && x.ProgramId == educatorProgramDTO.ProgramId && x.EndDate == null);

            bool result = IsAuthorized(existEducatorProgram, zone);

            if (!result)
                return new ResponseWrapper<EducatorProgramResponseDTO>() { Result = false, Message = "You are not authorized for this operation!" };

            if (educatorProgramDTO.EducationOfficerId != null)
                existEducationOfficer.StartDate = educatorProgramDTO.EducationOfficerDutyStartDate;


            if (existEducationOfficer != null && (educatorProgramDTO.IsEducationOfficer == false || educatorProgramDTO.EducationOfficerDutyEndDate != null || educatorProgramDTO.DutyEndDate > DateTime.UtcNow))
            {
                await educationOfficerRepository.FinishEducationOfficersDuty(cancellationToken, educatorProgramDTO.EducationOfficerId ?? 0, educatorProgramDTO.DutyEndDate > DateTime.UtcNow ? educatorProgramDTO.DutyEndDate : educatorProgramDTO.EducationOfficerDutyEndDate);
            }
            else if (existEducationOfficer == null && educatorProgramDTO.IsEducationOfficer == true)
            {
                await AddEducationOfficer(educatorProgramDTO, cancellationToken);
            }

            var mappedEducatorProgram = mapper.Map(educatorProgramDTO, existEducatorProgram);

            educatorProgramRepository.Update(mappedEducatorProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<EducatorProgramResponseDTO>(mappedEducatorProgram);

            return new ResponseWrapper<EducatorProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<bool> AddEducationOfficer(EducatorProgramDTO educatorProgramDTO, CancellationToken cancellationToken)
        {
            var educator = await educatorRepository.GetByIdAsync(cancellationToken, educatorProgramDTO.EducatorId ?? 0);
            var besRole = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);
            await educationOfficerRepository.FinishCurrentDuty(cancellationToken, educatorProgramDTO.ProgramId ?? 0);
            var addedEducationOfficer = await educationOfficerRepository.AddAsync(cancellationToken, new() { EducatorId = educatorProgramDTO.EducatorId, ProgramId = educatorProgramDTO.ProgramId, StartDate = educatorProgramDTO.EducationOfficerDutyStartDate, EndDate = educatorProgramDTO.EducationOfficerDutyEndDate });
            await unitOfWork.CommitAsync(cancellationToken);
            foreach (var item in educatorProgramDTO.EducationOfficerDocuments)
            {
                var document = await documentRepository.GetByIdAsync(cancellationToken, item.Id);
                document.EntityId = addedEducationOfficer.Id;
            }
            var addedUserRole = await koruRepository.AddRoleToUser(besRole, educator.UserId ?? 0);
            if (addedUserRole.Id != 0)
                await koruRepository.AddUserRoleProgram(addedUserRole.Id, educatorProgramDTO.ProgramId ?? 0);
            return true;
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var educatorProgram = await educatorProgramRepository.GetByIdAsync(cancellationToken, id);

            var educatorPrograms = await educatorProgramRepository.GetAsync(cancellationToken, x => x.EducatorId == educatorProgram.EducatorId);
            if (educatorPrograms?.Count < 2)
                return new ResponseWrapper<EducatorProgramResponseDTO>() { Result = false, Message = "Educator must have at least one program!" };

            bool result = IsAuthorized(educatorProgram, zone);
            if (!result)
                return new ResponseWrapper<EducatorProgramResponseDTO>() { Result = false, Message = "You are not authorized for this operation!" };

            unitOfWork.BeginTransaction();

            var educationOfficer = await educationOfficerRepository.GetByAsync(cancellationToken, x => x.ProgramId == educatorProgram.ProgramId && x.EducatorId == educatorProgram.EducatorId && x.EndDate == null);
            if (educationOfficer != null)
                await educationOfficerRepository.FinishEducationOfficersDuty(cancellationToken, educationOfficer.Id);

            educatorProgramRepository.HardDelete(educatorProgram);

            await unitOfWork.EndTransactionAsync(cancellationToken);

            return new ResponseWrapper<EducatorProgramResponseDTO> { Result = true };
        }

        public bool IsAuthorized(EducatorProgram educatorProgram, ZoneModel zone)
        {
            bool response = true;
            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
            {
                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    response = provinceIds.Contains(educatorProgram.Program.Hospital.ProvinceId ?? 0);
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    response = universityIds.Contains(educatorProgram.Program.Hospital.Faculty.UniversityId);
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    response = facultyIds.Contains(educatorProgram.Program.Hospital.FacultyId ?? 0);
                }
                if (zone.Hospitals != null && zone.Hospitals.Any())
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    response = hospitalIds.Contains(educatorProgram.Program.HospitalId ?? 0);
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    response = programIds.Contains(educatorProgram.ProgramId ?? 0);
                }
            }
            return response;
        }
    }
}
