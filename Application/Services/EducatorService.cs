using Application.Interfaces;
using Application.Reports.ExcelReports.EducatorReports;
using Application.Services.Base;
using AutoMapper;
using Core.EkipModels;
using Core.Entities;
using Core.Extentsions;
using Core.Helpers;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.Ekip;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Globalization;

namespace Application.Services
{
    public class EducatorService : BaseService, IEducatorService
    {
        private readonly IMapper mapper;
        private readonly IEducatorRepository educatorRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IEducationOfficerRepository educationOfficerRepository;
        private readonly IExpertiseBranchRepository expertiseBranchRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;
        private readonly IUserRepository userRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IEkipService ekipService;
        private readonly ICKYSService cKYSService;
        private readonly IEducationTrackingRepository educationTrackingRepository;
        private readonly IThesisRepository thesisRepository;

        public EducatorService(IMapper mapper, IUnitOfWork unitOfWork, IEducatorRepository educatorRepository, IDocumentRepository documentRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository, IEducationOfficerRepository educationOfficerRepository, IExpertiseBranchRepository expertiseBranchRepository, IUserRepository userRepository, IStudentRepository studentRepository, IEkipService ekipService, ICKYSService cKYSService, IEducationTrackingRepository educationTrackingRepository, IThesisRepository thesisRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.educatorRepository = educatorRepository;
            this.documentRepository = documentRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
            this.educationOfficerRepository = educationOfficerRepository;
            this.expertiseBranchRepository = expertiseBranchRepository;
            this.userRepository = userRepository;
            this.studentRepository = studentRepository;
            this.ekipService = ekipService;
            this.cKYSService = cKYSService;
            this.educationTrackingRepository = educationTrackingRepository;
            this.thesisRepository = thesisRepository;
        }

        public async Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);

            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<EducatorPaginateResponseDTO> ordersQuery = educatorRepository.OnlyPaginateListQuery(zone);

            var roleFilter = filter.Filter.Filters.Where(x => x.Field == "Role");
            if (roleFilter != null)
            {
                foreach (var role in roleFilter.ToList())
                {
                    ordersQuery = ordersQuery.Where(x => x.Roles.Contains((string)role.Value));
                    filter.Filter.Filters.Remove(role);
                }
            }
            var titleFilter = filter.Filter.Filters.Where(x => x.Field == "AdminTitle");
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

            var response = new PaginationModel<EducatorPaginateResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<Educator> query = educatorRepository.QueryList(zone);

            List<Educator> educators = await query.OrderBy(x => x.User.Name).ToListAsync(cancellationToken);

            List<EducatorResponseDTO> response = mapper.Map<List<EducatorResponseDTO>>(educators);

            foreach (var item in response)
            {
                if (item.UserId != null)
                    item.User.IdentityNo = StringHelpers.MaskIdentityNumber(item.User.IdentityNo);
            }

            return new ResponseWrapper<List<EducatorResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<AdvisorPaginateResponseDTO>> GetPaginateListForAdvisor(CancellationToken cancellationToken, FilterDTO filter)
        {
            var ordersQuery = from x in educatorRepository.Queryable().AsNoTracking().AsQueryable()
                              let principalProgram = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program
                              let subProgram = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).Program
                              select new AdvisorPaginateResponseDTO()
                              {
                                  DutyPlaceHospital = principalProgram.Hospital.Name ?? subProgram.Hospital.Name,
                                  DutyPlaceHospitalId = principalProgram.HospitalId ?? subProgram.HospitalId,
                                  AcademicTitle = x.AcademicTitle.Name,
                                  ExpertiseBranches = x.EducatorExpertiseBranches.Select(x => new EducatorExpertiseBranchResponseDTO() { ExpertiseBranch = new() { Id = x.ExpertiseBranch.Id, Name = x.ExpertiseBranch.Name } }),
                                  ExpertiseBranchIds = x.EducatorExpertiseBranches.Select(x => x.ExpertiseBranchId),
                                  Id = x.Id,
                                  Name = x.User.Name,
                                  Phone = x.User.Phone,
                                  Type = x.EducatorType,
                                  IsDeleted = x.IsDeleted,
                                  UserIsDeleted = x.User.IsDeleted,
                                  UserId = x.User.Id,
                                  IdentityNo = x.User.IdentityNo
                              };

            //IQueryable<EducatorExp> ordersQuery = educatorRepository.PaginateListForAdvisorQuery(eduPlaceId);

            //if (filter.Filter.Filters.Any(x => x.Field == "EducatorExpertiseBranches"))
            //{
            //    var filteredTypeItem = filter.Filter.Filters.FirstOrDefault(x => x.Field == "EducatorExpertiseBranches");
            //    ordersQuery = ordersQuery.Where(x => x.Educator.EducatorExpertiseBranches.Any((a => a.ExpertiseBranch.Name.ToLower().Contains(filteredTypeItem.Value.ToString().ToLower(new CultureInfo("tr-TR"))))));
            //    filter.Filter.Filters.Remove(filteredTypeItem);
            //}
            //if (filter.Filter.Filters.Any(x => x.Field == "EducatorPrograms"))
            //{
            //    var filteredTypeItem = filter.Filter.Filters.FirstOrDefault(x => x.Field == "EducatorPrograms");
            //    ordersQuery = ordersQuery.Where(x => x.Educator.EducatorPrograms.Any((a => a.Program.Hospital.Name.ToLower().Contains(filteredTypeItem.Value.ToString().ToLower(new CultureInfo("tr-TR"))) && a.DutyEndDate == null)));
            //    filter.Filter.Filters.Remove(filteredTypeItem);
            //}

            var expBrFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "ExpertiseBranchName");
            var expBrIdFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "ExpertiseBranchId");

            if (expBrFilter != null)
            {
                ordersQuery = ordersQuery.Where(x => x.ExpertiseBranches.Any(a => a.ExpertiseBranch.Name.ToLower().Contains(expBrFilter.Value.ToString().ToLower(new CultureInfo("tr-TR")))));
                filter.Filter.Filters.Remove(expBrFilter);
            }

            if (expBrIdFilter != null)
            {
                ordersQuery = ordersQuery.Where(x => x.ExpertiseBranches.Any(x => x.ExpertiseBranch.Id == (long)expBrIdFilter.Value));
                filter.Filter.Filters.Remove(expBrIdFilter);
            }

            FilterResponse<AdvisorPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var educators = await filterResponse.Query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            if (educators != null && educators.Count > 0)
                foreach (var educator in educators)
                    educator.IdentityNo = StringHelpers.MaskIdentityNumber(educator.IdentityNo);

            var response = new PaginationModel<AdvisorPaginateResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            return response;
        }

        public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Educator> ordersQuery = educatorRepository.PaginateListQuery();

            if (filter?.Filter?.Filters?.Any(x => x.Field == "EducatorExpertiseBranches") == true)
            {
                var filteredTypeItem = filter.Filter.Filters.FirstOrDefault(x => x.Field == "EducatorExpertiseBranches");
                ordersQuery = ordersQuery.Where(x => x.EducatorExpertiseBranches.Any((a => a.ExpertiseBranch.Name.ToLower().Contains(filteredTypeItem.Value.ToString().ToLower(new CultureInfo("tr-TR"))))));
                filter.Filter.Filters.Remove(filteredTypeItem);
            }
            if (filter?.Filter?.Filters?.Any(x => x.Field == "EducatorPrograms") == true)
            {
                var filteredTypeItem = filter.Filter.Filters.FirstOrDefault(x => x.Field == "EducatorPrograms");
                ordersQuery = ordersQuery.Where(x => x.EducatorPrograms.Any((a => a.Program.Hospital.Name.ToLower().Contains(filteredTypeItem.Value.ToString().ToLower(new CultureInfo("tr-TR"))) && a.DutyEndDate == null)));
                filter.Filter.Filters.Remove(filteredTypeItem);
            }


            FilterResponse<Educator> filterResponse = ordersQuery.ToFilterView(filter);


            var educators = mapper.Map<List<EducatorResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            if (educators != null && educators.Count > 0)
            {
                foreach (var educator in educators)
                {
                    educator.User.IdentityNo = StringHelpers.MaskIdentityNumber(educator?.User?.IdentityNo);
                }
            }

            var response = new PaginationModel<EducatorResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };


            return response;
        }

        public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByCore(CancellationToken cancellationToken, FilterDTO filter, long studentId)
        {
            IQueryable<Educator> ordersQuery = educatorRepository.PaginateListQueryByCore(studentId);
            FilterResponse<Educator> filterResponse = ordersQuery.ToFilterView(filter);
            var educators = mapper.Map<List<EducatorResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            if (educators != null && educators.Count > 0)
            {
                foreach (var educator in educators)
                {
                    educator.User.IdentityNo = StringHelpers.MaskIdentityNumber(educator.User?.IdentityNo);
                }
            }

            var response = new PaginationModel<EducatorResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListByProgramId(CancellationToken cancellationToken, FilterDTO filter, long programId)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<EducatorPaginateResponseDTO> ordersQuery = educatorRepository.OnlyPaginateListQuery(zone);

            FilterResponse<EducatorPaginateResponseDTO> filterResponse = ordersQuery.Where(x => x.PrincipalBranchDutyPlaceId == programId || x.SubBranchDutyPlaceId == programId).ToFilterView(filter);

            var educators = await filterResponse.Query.ToListAsync(cancellationToken);
            educators?.ForEach(x => x.IdentityNo = StringHelpers.MaskIdentityNumber(x.IdentityNo));

            var response = new PaginationModel<EducatorPaginateResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByUniversityId(CancellationToken cancellationToken, FilterDTO filter, long uniId)
        {
            IQueryable<Educator> ordersQuery = educatorRepository.GetByUniversityIdQuery(uniId);
            FilterResponse<Educator> filterResponse = ordersQuery.ToFilterView(filter);

            var educators = mapper.Map<List<EducatorResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<EducatorResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByHospitalId(CancellationToken cancellationToken, FilterDTO filter, long hospitalId)
        {
            IQueryable<Educator> ordersQuery = educatorRepository.GetByHospitalIdQuery(hospitalId);
            FilterResponse<Educator> filterResponse = ordersQuery.ToFilterView(filter);

            var educators = mapper.Map<List<EducatorResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<EducatorResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<EducatorResponseDTO>> GetByIdentityNo(CancellationToken cancellationToken, string identityNo)
        {
            Educator educator = await educatorRepository.GetIncluding(cancellationToken, x => x.IsDeleted == false && x.User.IdentityNo == identityNo, x => x.User);

            EducatorResponseDTO response = mapper.Map<EducatorResponseDTO>(educator);

            return new ResponseWrapper<EducatorResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            IQueryable<Educator> ordersQuery = educatorRepository.GetByUniversityIdQuery(uniId);

            var educators = await ordersQuery.ToListAsync(cancellationToken);
            List<EducatorResponseDTO> response = mapper.Map<List<EducatorResponseDTO>>(educators);

            return new ResponseWrapper<List<EducatorResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorResponseDTO>> GetByIdForJuryList(CancellationToken cancellationToken, long id)
        {
            var educator = await educatorRepository.GetByIdForJuryList(cancellationToken, id);

            EducatorResponseDTO response = mapper.Map<EducatorResponseDTO>(educator);

            return new ResponseWrapper<EducatorResponseDTO> { Result = true, Item = response };

        }

        public async Task<ResponseWrapper<EducatorResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            var educator = await educatorRepository.GetWithSubRecordsWithZone(cancellationToken, zone, id);
            // Educator educator = await educatorRepository.GetWithSubRecords(cancellationToken, id);
            //var educator = await query.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);


            if (educator != null)
            {
                EducatorResponseDTO response = mapper.Map<EducatorResponseDTO>(educator);

                if (response.EducatorExpertiseBranches?.Count > 0)
                    for (int i = 0; i < response.EducatorExpertiseBranches.Count; i++)
                        if (response.EducatorExpertiseBranches[i].ExpertiseBranch.IsPrincipal == true)
                            response.EducatorExpertiseBranches[i].SubBranchIds = await expertiseBranchRepository.GetSubBrachIds(cancellationToken, (long)response.EducatorExpertiseBranches[i].ExpertiseBranchId);

                response.User.IdentityNo = StringHelpers.MaskIdentityNumber(educator.User.IdentityNo);

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
            else
            {
                return new() { Result = false, Message = "You are not authorized for this operation!" };
            }
        }

        public async Task<ResponseWrapper<EducatorResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorDTO EducatorDTO)
        {
            Educator educator = mapper.Map<Educator>(EducatorDTO);

            await educatorRepository.AddAsync(cancellationToken, educator);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<EducatorResponseDTO>(educator);

            return new ResponseWrapper<EducatorResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorDTO educatorDTO)
        {
            var educator = mapper.Map<Educator>(educatorDTO);

            //foreach (var item in educator.EducationOfficers)
            //{
            //    if (item.Id == 0)
            //    {
            //        await educationOfficerRepository.FinishEducationOfficersDuty(cancellationToken, (long)item.ProgramId);
            //        var role = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);
            //        koruRepository.AddRoleToUser(role, (long)educator.UserId);

            //    }
            //}
            await educatorRepository.UpdateWithSubRecords(cancellationToken, id, educator);

            var updatedEducator = await educatorRepository.GetWithSubRecords(cancellationToken, id);

            var educatorExBrs = new List<EducatorExpertiseBranchResponseDTO>();


            //if (educatorDTO.EducatorPrograms?.Count > 0)
            //    foreach (var item in educatorDTO.EducatorPrograms)
            //        foreach (var doc in item.Documents)
            //        {
            //            doc.EntityId = (long)item.Id;
            //            documentRepository.Update(mapper.Map<Document>(doc));
            //        }

            //if (educatorDTO.EducationOfficers?.Where(x => x.EndDate == null)?.ToList()?.Count > 0)
            //    foreach (var item in educatorDTO.EducationOfficers.Where(x => x.EndDate == null).ToList())
            //        foreach (var doc in item.Documents)
            //        {
            //            doc.EntityId = (long)updatedEducator.EducationOfficers.FirstOrDefault(x => x.DocumentOrder == item.DocumentOrder)?.Id;
            //            documentRepository.Update(mapper.Map<Document>(doc));
            //        }

            if (educator.IsDeleted)
            {
                //var user = await userRepository.GetByIdAsync(cancellationToken, educator.UserId ?? 0);
                //userRepository.SoftDelete(user);
                //await userRepository.DeleteStudentOrEducatorRole(cancellationToken, educator.UserId ?? 0);

                await educatorRepository.DeleteEducator(cancellationToken, educatorDTO);
            }

            await unitOfWork.CommitAsync(cancellationToken);


            foreach (var item in updatedEducator.EducatorExpertiseBranches)
            {
                var eduExBr = new EducatorExpertiseBranchResponseDTO
                {
                    ExpertiseBranchName = item.ExpertiseBranch.Name,
                    ExpertiseBranchId = item.ExpertiseBranchId,
                    //IsPrincipal = item.ExpertiseBranch.IsPrincipal,
                    RegistrationDate = item.RegistrationDate,
                    RegistrationNo = item.RegistrationNo,
                    EducatorId = item.EducatorId,
                    Id = item.Id,
                    SubBranchIds = item.ExpertiseBranch.SubBranches.Select(x => x.SubBranchId).ToList()
                };
                educatorExBrs.Add(eduExBr);
            }
            updatedEducator.EducatorExpertiseBranches = null;

            var response = mapper.Map<EducatorResponseDTO>(updatedEducator);

            response.User.IdentityNo = StringHelpers.MaskIdentityNumber(response.User.IdentityNo);
            response.EducatorExpertiseBranches = educatorExBrs;
            return new ResponseWrapper<EducatorResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var educator = await educatorRepository.GetByIdAsync(cancellationToken, id);

            if (await educatorRepository.AnyAsync(cancellationToken, x => x.IsDeleted == false && x.UserId == educator.UserId) == true || await studentRepository.AnyAsync(cancellationToken, x => x.IsDeleted == false && x.IsHardDeleted == false && x.UserId == educator.UserId))
                return new() { Result = false, Message = "Geri almak istediğiniz kişinin aktif öğrenci ya da eğiticisi vardır. Bu kişiyi geri alamazsınız!" };

            await educatorRepository.UnDeleteEducator(cancellationToken, id);

            return new ResponseWrapper<EducatorResponseDTO>() { Result = true };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
        {
            filter.pageSize = int.MaxValue;
            filter.page = 1;
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<EducatorPaginateResponseDTO> ordersQuery = educatorRepository.OnlyPaginateListQuery(zone);

            var roleFilter = filter.Filter.Filters.Where(x => x.Field == "Role");
            if (roleFilter != null)
            {
                foreach (var role in roleFilter.ToList())
                {
                    ordersQuery = ordersQuery.Where(x => x.Roles.Contains((string)role.Value));
                    filter.Filter.Filters.Remove(role);
                }
            }
            var titleFilter = filter.Filter.Filters.Where(x => x.Field == "AdminTitle");
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
            var response = new PaginationModel<EducatorPaginateResponseDTO>
            {
                Items = educators,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            var byteArray = ExportList.ExportListReport(response.Items);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }

        public async Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByAcademicTitle(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = educatorRepository.QueryableEducatorsForCharts(zone);

            FilterResponse<EducatorChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false).GroupBy(x => x.AcademicTitle)
                                                     .Select(x => new EducatorCountByProperty()
                                                     {
                                                         SeriesName = x.Key ?? "Bilinmiyor",
                                                         Count = x.Count(),
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<EducatorCountByProperty>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByProfession(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = educatorRepository.QueryableEducatorsForCharts(zone);

            FilterResponse<EducatorChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false).GroupBy(x => x.ProfessionName)
                                                     .Select(x => new EducatorCountByProperty()
                                                     {
                                                         SeriesName = x.Key ?? "Bilinmiyor",
                                                         Count = x.Count(),
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<EducatorCountByProperty>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryableEducators = educatorRepository.QueryableEducatorsForCharts(zone);

            FilterResponse<EducatorChartModel> filterResponse = queryableEducators.ToFilterView(filter);

            var response = await filterResponse.Query.GroupBy(x => new { x.ParentInstitutionName, x.ProfessionName })
                                                     .Select(x => new CountsByProfessionInstitutionModel
                                                     {
                                                         ParentInstitutionName = x.Key.ParentInstitutionName ?? "Bilinmiyor",
                                                         ProfessionName = x.Key.ProfessionName ?? "Aktif Değil",
                                                         Count = x.Count()
                                                     }
                                                     ).ToListAsync(cancellationToken);

            var result = new List<CountsByProfessionInstitutionModel>();
            foreach (var item in response.GroupBy(x => x.ParentInstitutionName))
            {
                result.Add(new CountsByProfessionInstitutionModel() { ParentInstitutionName = item.Key });
                foreach (var item2 in item)
                {
                    var selectedItem = result.FirstOrDefault(x => x.ParentInstitutionName == item.Key);
                    if (selectedItem != null)
                        if (item2.ProfessionName == "Tıp")
                            selectedItem.MedicineCount = item2.Count;
                        else if (item2.ProfessionName == "Diş Hekimliği")
                            selectedItem.DentistryCount = item2.Count;
                        else
                            selectedItem.PharmaceuticsCount = item2.Count;
                }
            }

            return new ResponseWrapper<List<CountsByProfessionInstitutionModel>> { Item = result, Result = true };
        }

        public async Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = educatorRepository.QueryableEducatorsForCharts(zone);

            FilterResponse<EducatorChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false).GroupBy(x => x.IsUniversityPrivate)
                                                     .Select(x => new EducatorCountByProperty()
                                                     {
                                                         SeriesName = x.Key == true ? "Foundation University" : (x.Key == false ? "Public University" : "Other"),
                                                         Count = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<EducatorCountByProperty>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<PersonelHareketiResponseDTO>>> WorkingLifeById(CancellationToken cancellationToken, long educatorId)
        {
            var educator = await unitOfWork.EducatorRepository.GetByIdAsync(cancellationToken, educatorId);
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, educator.UserId.Value);

            var workingPlaces = await ekipService.GetListByAsync<PersonelHareketi>(cancellationToken, x => x.tckn == user.IdentityNo);

            List<PersonelHareketiResponseDTO> response = mapper.Map<List<PersonelHareketiResponseDTO>>(workingPlaces);

            return new ResponseWrapper<List<PersonelHareketiResponseDTO>> { Item = response, Result = true };
        }
    }
}
