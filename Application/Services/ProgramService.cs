using System.Globalization;
using System.Text;
using Application.Interfaces;
using Application.Reports.ExcelReports.ProgramReports;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using ExcelDataReader;
using Infrastructure.Data;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class ProgramService : BaseService, IProgramService
    {
        private readonly IMapper mapper;
        private readonly IProgramRepository programRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;
        public ProgramService(IMapper mapper, IUnitOfWork unitOfWork, IProgramRepository programRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.programRepository = programRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }
        public async Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<ProgramPaginateResponseDTO> ordersQuery = programRepository.PaginateListQuery(zone);
            FilterResponse<ProgramPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var programs = await filterResponse.Query.ToListAsync(cancellationToken);

            var response = new PaginationModel<ProgramPaginateResponseDTO>
            {
                Items = programs,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListAll(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<ProgramPaginateResponseDTO> ordersQuery = programRepository.AllPaginateListQuery();
            FilterResponse<ProgramPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var programs = await filterResponse.Query.ToListAsync(cancellationToken);

            var response = new PaginationModel<ProgramPaginateResponseDTO>
            {
                Items = programs,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<PaginationModel<ProgramPaginateForQuotaResponseDTO>> GetPaginateListForQuota(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<ProgramPaginateForQuotaResponseDTO> ordersQuery = programRepository.GetProgramsForQuota(zone);
            FilterResponse<ProgramPaginateForQuotaResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var programs = await filterResponse.Query.ToListAsync(cancellationToken);

            var response = new PaginationModel<ProgramPaginateForQuotaResponseDTO>
            {
                Items = mapper.Map<List<ProgramPaginateForQuotaResponseDTO>>(programs),
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<bool>> UnDeleteProgram(CancellationToken cancellationToken, long id)
        {
            var program = await programRepository.GetByIdAsync(cancellationToken, id);

            if (program != null && program.IsDeleted == true)
            {
                program.IsDeleted = false;
                program.DeleteDate = null;

                programRepository.Update(program);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }
        public async Task<PaginationModel<ProgramResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Program> ordersQuery = programRepository.QueryablePrograms(x => true);

            FilterResponse<Program> filterResponse = ordersQuery.ToFilterView(filter);

            List<ProgramResponseDTO> programs = mapper.Map<List<ProgramResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<ProgramResponseDTO>
            {
                Items = programs,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            return response;
        }

        public async Task<PaginationModel<ProgramResponseDTO>> GetPaginateListForProtocolProgram(CancellationToken cancellationToken, FilterDTO filter)
        {
            var jArray = (JArray)filter.Filter.Filters.FirstOrDefault(x => x.Field == "ExpertiseBranchIds")?.Value;

            if (jArray?.Count > 0)
            {
                var idList = jArray.Select(x => (long)x).ToList();
                var authCategoryList = new List<string>() { "2", "3" };

                IQueryable<Program> ordersQuery = programRepository.QueryablePrograms(x => idList.Contains((long)x.ExpertiseBranchId) && authCategoryList.Contains(x.AuthorizationDetails.OrderByDescending(x => x.AuthorizationDate).FirstOrDefault(x => x.AuthorizationDate != null).AuthorizationCategory.Name));

                filter.Filter.Filters.Remove(filter.Filter.Filters.FirstOrDefault(x => x.Field == "ExpertiseBranchIds"));

                FilterResponse<Program> filterResponse = ordersQuery.ToFilterView(filter);

                List<ProgramResponseDTO> programs = mapper.Map<List<ProgramResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

                return new PaginationModel<ProgramResponseDTO>
                {
                    Items = programs,
                    TotalPages = filterResponse.PageNumber,
                    Page = filter.page,
                    PageSize = filter.pageSize,
                    TotalItemCount = filterResponse.Count
                };
            }
            else
                return new() { Items = new() };

        }

        public async Task<PaginationModel<ProgramResponseDTO>> GetListForSearchByExpertiseBranchId(CancellationToken cancellationToken, FilterDTO filter, long exBranchId, bool getAll = false)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<ProgramPaginateResponseDTO> ordersQuery = programRepository.PaginateListQuery(zone, x => x.IsDeleted == false && x.ExpertiseBranchId == exBranchId, true);

            //ordersQuery = programRepository.QueryableProgramsByUserRole(x => x.IsDeleted == false && x.ExpertiseBranchId == exBranchId);

            if (filter.Filter.Filters.Count == 0)
                return new PaginationModel<ProgramResponseDTO> { };

            var value = filter.Filter.Filters[0].Value;

            var fullTextSearchFilter = new FilterDTO()
            {
                Filter = new Filter()
                {
                    Logic = "or",
                    Filters = new()
                {
                    new Filter()
                    {
                        Field = "UniversityName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "ProvinceName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "ProfessionName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "HospitalName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "ExpertiseBranchName",
                        Operator = "contains",
                        Value = value,
                    }
                }
                },
                page = 1,
                pageSize = int.MaxValue
            };

            //yetki kategorisi 1,2,3 olan programlar gelecek
            if (!getAll)
                ordersQuery = ordersQuery.Where(x => new List<string> { "1", "2", "3" }.Contains(x.AuthorizationCategory));

            FilterResponse<ProgramPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(fullTextSearchFilter);

            var mainList = await filterResponse.Query.ToListAsync(cancellationToken);

            var loopTime = (filter.Filter.Filters.Count) / 5;

            int filterFactor = 5;

            List<string> searchParameterList = new List<string>();

            for (int i = 1; i < loopTime; i++)
            {
                searchParameterList.Add(filter.Filter.Filters[filterFactor].Value.ToString());

                filterFactor += 5;
            }

            //var mappedPrograms = mapper.Map<List<ProgramResponseDTO>>(mainList);

            var responseDTO = mainList.Select(x => new ProgramResponseDTO()
            {
                Id = x.Id,
                Hospital = new HospitalResponseDTO() { Id = (long)x.HospitalId, Name = x.HospitalName, Province = new ProvinceResponseDTO() { Id = (long)x.ProvinceId, Name = x.ProvinceName } },
                Faculty = new FacultyResponseDTO() { Id = (long)x.FacultyId, Name = x.FacultyName },
                ExpertiseBranch = new ExpertiseBranchResponseDTO() { Id = (long)x.ExpertiseBranchId, Name = x.ExpertiseBranchName, IsPrincipal = x.IsPrincipal },
            }).ToList();

            foreach (var item in searchParameterList)
            {
                responseDTO = responseDTO.Where(x => x.Name.ToLower().Contains(item)).ToList();
            }

            var response = new PaginationModel<ProgramResponseDTO>
            {
                Items = responseDTO,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            return response;
        }
        public async Task<PaginationModel<ProgramResponseDTO>> GetListForSearch(CancellationToken cancellationToken, FilterDTO filter, bool getAll)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            if (getAll)
                zone.RoleCategory = RoleCategoryType.Admin;

            IQueryable<ProgramPaginateResponseDTO> ordersQuery = programRepository.PaginateListQuery(zone, null);

            var protocolFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "ProtocolProgram");
            if (protocolFilter != null)
            {
                filter.Filter.Filters.Remove(protocolFilter);
                ordersQuery = ordersQuery.Where(x => x.CanMakeProtocol == true);
            }

            var complementFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "ComplementProgram");
            if (complementFilter != null)
            {
                filter.Filter.Filters.Remove(complementFilter);
                ordersQuery = ordersQuery.Where(x => x.CanMakeProtocol == false);
            }

            if (filter.Filter.Filters.Count == 0)
                return new PaginationModel<ProgramResponseDTO> { };

            var value = filter.Filter.Filters[0].Value;

            var fullTextSearchFilter = new FilterDTO()
            {
                Filter = new Filter()
                {
                    Logic = "or",
                    Filters = new()
                {
                    new Filter()
                    {
                        Field = "UniversityName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "ProvinceName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "ProfessionName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "HospitalName",
                        Operator = "contains",
                        Value = value,
                    },new Filter()
                    {
                        Field = "ExpertiseBranchName",
                        Operator = "contains",
                        Value = value,
                    }
                }
                },
                page = 1,
                pageSize = int.MaxValue
            };

            FilterResponse<ProgramPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(fullTextSearchFilter);

            var mainList = await filterResponse.Query.ToListAsync(cancellationToken); // buraya performansı etkilediği için take() koyulduğunda istenilen programı getirmeme durumu oluyor

            var loopTime = (filter.Filter.Filters.Count) / 5;

            int filterFactor = 5;

            List<string> searchParameterList = new List<string>();

            for (int i = 1; i < loopTime; i++)
            {
                searchParameterList.Add(filter.Filter.Filters[filterFactor].Value.ToString());

                filterFactor = filterFactor + 5;
            }

            //var mappedPrograms = mapper.Map<List<ProgramResponseDTO>>(mainList);

            var responseDTO = mainList.Select(x => new ProgramResponseDTO()
            {
                Id = x.Id,
                Hospital = new HospitalResponseDTO() { Id = (long)x.HospitalId, Name = x.HospitalName, Province = new ProvinceResponseDTO() { Id = (long)x.ProvinceId, Name = x.ProvinceName } },
                Faculty = new FacultyResponseDTO() { Id = (long)x.FacultyId, Name = x.FacultyName },
                ExpertiseBranch = new ExpertiseBranchResponseDTO() { Id = (long)x.ExpertiseBranchId, Name = x.ExpertiseBranchName },
                PrincipalBrancIdList = x.PrincipalBrancIdList
            }).ToList();

            foreach (var item in searchParameterList)
            {
                responseDTO = responseDTO.Where(x => x.Name.ToLower().Contains(item.ToLower())).ToList();
            }

            var response = new PaginationModel<ProgramResponseDTO>
            {
                Items = responseDTO,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            return response;
        }

        public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            List<Program> programs = await programRepository.GetListByUniversityId(cancellationToken, uniId);

            List<ProgramResponseDTO> response = mapper.Map<List<ProgramResponseDTO>>(programs);

            return new ResponseWrapper<List<ProgramResponseDTO>> { Result = true, Item = response };
        }
        
        public async Task<ResponseWrapper<List<ProgramBreadcrumbSimpleDTO>>> GetProgramListByHospitalIdBreadCrumb(CancellationToken cancellationToken, long hospitalId)
        {
            List<ProgramBreadcrumbSimpleDTO> programs = await programRepository.GetProgramListByHospitalIdBreadCrumb(cancellationToken, hospitalId);

            return new ResponseWrapper<List<ProgramBreadcrumbSimpleDTO>> { Result = true, Item = programs };
        }

        public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Program> programs = await programRepository.GetAsync(cancellationToken, x => true);

            List<ProgramResponseDTO> response = mapper.Map<List<ProgramResponseDTO>>(programs);

            return new ResponseWrapper<List<ProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByHospitalIdAsync(CancellationToken cancellationToken, long hospitalId)
        {
            List<Program> programs = await programRepository.GetListByHospitalId(cancellationToken, hospitalId);

            List<ProgramResponseDTO> response = mapper.Map<List<ProgramResponseDTO>>(programs);

            return new ResponseWrapper<List<ProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId)
        {
            List<Program> programs = await programRepository.GetListByExpertiseBranchId(cancellationToken, expertiseBranchId);

            List<ProgramResponseDTO> response = mapper.Map<List<ProgramResponseDTO>>(programs);

            return new ResponseWrapper<List<ProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchIdExceptOneAsync(CancellationToken cancellationToken, long expertiseBranchId)
        {
            List<Program> programs = await programRepository.GetIncludingList(cancellationToken, x => x.ExpertiseBranchId != expertiseBranchId && x.IsDeleted == false);

            List<ProgramResponseDTO> response = mapper.Map<List<ProgramResponseDTO>>(programs);

            return new ResponseWrapper<List<ProgramResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Program program = await programRepository.GetWithSubRecords(cancellationToken, id);

            ProgramResponseDTO response = mapper.Map<ProgramResponseDTO>(program);

            return new ResponseWrapper<ProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> GetByStudentIdAsync(CancellationToken cancellationToken, long studentId)
        {
            Program program = await programRepository.GetByStudentId(cancellationToken, studentId);

            ProgramResponseDTO response = mapper.Map<ProgramResponseDTO>(program);

            return new ResponseWrapper<ProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> GetByHospitalAndBranchIdAsync(CancellationToken cancellationToken, long hospitalId, long expertiseBranchId)
        {
            var response = await programRepository.GetByHospitalAndBranchId(cancellationToken, hospitalId, expertiseBranchId);

            return new ResponseWrapper<ProgramResponseDTO> { Result = true, Item = response };
        }


        public async Task<ResponseWrapper<ProgramBreadcrumbResponseDTO>> GetByIdWithBreadcrumbAsync(CancellationToken cancellationToken, long id)
        {
            Program program = await programRepository.GetWithSubRecords(cancellationToken, id);
            var protocolProgramResponse = await programRepository.CheckProtocolProgram(cancellationToken, id);

            ProgramResponseDTO response = mapper.Map<ProgramResponseDTO>(program);
            response.IsMainProgram = protocolProgramResponse.IsMainProgram;
            response.IsDependentProgram = protocolProgramResponse.IsDependentProgram;
            response.ProtocolOrComplement = protocolProgramResponse.ProtocolOrComplement;
            var programBreadcrumb = new ProgramBreadcrumbResponseDTO()
            {
                Program = response,
                //RelatedPrograms = response?.Faculty?.UniversityId != null ? await programRepository.GetListByUniversityIdBreadCrumbModel(cancellationToken, response.Faculty.UniversityId) : null,
                //Universities = response?.ExpertiseBranchId != null ? await unitOfWork.UniversityRepository.GetListByExpertiseBranchId(cancellationToken, program.ExpertiseBranch.Id) : null,
                //Hospitals = response?.ExpertiseBranchId != null ? await unitOfWork.HospitalRepository.GetListByExpertiseBranchId(cancellationToken, program.ExpertiseBranch.Id) : null
            };

            return new ResponseWrapper<ProgramBreadcrumbResponseDTO> { Result = true, Item = programBreadcrumb };
        }

        public async Task<ResponseWrapper<List<ProgramsLocationResponseDTO>>> GetLocationsByExpertiseBranchId(CancellationToken cancellationToken, long? expBrId, long? authCategoryId)
        {
            var hospitals = await programRepository.GetLocationsByExpertiseBranchId(cancellationToken, expBrId, authCategoryId);
            return new() { Result = true, Item = hospitals };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, ProgramDTO programDTO)
        {
            Program program = mapper.Map<Program>(programDTO);

            var programs = await programRepository.GetAsync(cancellationToken);
            var hospital = await unitOfWork.HospitalRepository.GetIncluding(cancellationToken, x => x.Id == program.HospitalId, x => x.Province);
            var expertiseBranch = await unitOfWork.ExpertiseBranchRepository.GetIncluding(cancellationToken, x => x.Id == program.ExpertiseBranchId, x => x.Profession);

            program.Code = hospital.Province.Code.ConcatenateIntegers(hospital.Code, expertiseBranch.Profession?.Code ?? 0, expertiseBranch.Code);



            if (await programRepository.AnyAsync(cancellationToken, x => x.ExpertiseBranchId == program.ExpertiseBranchId && x.HospitalId == program.HospitalId && x.IsDeleted == false))
            {
                return new ResponseWrapper<ProgramResponseDTO> { Result = false, Message = "YUEP Programı sistemde bulunmaktadır." };
            }

            await programRepository.AddAsync(cancellationToken, program);
            await unitOfWork.CommitAsync(cancellationToken);

            ProgramResponseDTO response = mapper.Map<ProgramResponseDTO>(program);

            return new ResponseWrapper<ProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> AllProgramUpdate(CancellationToken cancellationToken, ProgramDTO programDTO)
        {
            //Program program = mapper.Map<Program>(programDTO);



            var programs = await programRepository.GetAsync(cancellationToken);



            foreach (var program in programs)
            {
                var hospital = await unitOfWork.HospitalRepository.GetIncluding(cancellationToken, x => x.Id == program.HospitalId, x => x.Province);
                var expertiseBranch = await unitOfWork.ExpertiseBranchRepository.GetIncluding(cancellationToken, x => x.Id == program.ExpertiseBranchId, x => x.Profession);



                program.Code = hospital.Province?.Code.ConcatenateIntegers(hospital.Code, expertiseBranch.Profession?.Code ?? 0, expertiseBranch.Code);



                programRepository.Update(program);
            }



            await unitOfWork.CommitAsync(cancellationToken);



            return new ResponseWrapper<ProgramResponseDTO> { Result = false, Message = "YUEP Programı sistemde bulunmaktadır." };





            //if (await programRepository.AnyAsync(cancellationToken, x => x.ExpertiseBranchId == program.ExpertiseBranchId && x.HospitalId == program.HospitalId && x.IsDeleted == false))
            //{
            //    return new ResponseWrapper<ProgramResponseDTO> { Result = false, Message = "YUEP Programı sistemde bulunmaktadır." };
            //}



            //await programRepository.AddAsync(cancellationToken, program);
            //await unitOfWork.CommitAsync(cancellationToken);



            //ProgramResponseDTO response = mapper.Map<ProgramResponseDTO>(program);



            //return new ResponseWrapper<ProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, ProgramDTO programDTO)
        {
            Program program = await programRepository.GetByIdAsync(cancellationToken, id);

            Program updatedProgram = mapper.Map(programDTO, program);

            programRepository.Update(updatedProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            ProgramResponseDTO response = mapper.Map<ProgramResponseDTO>(program);

            return new ResponseWrapper<ProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Program program = await programRepository.GetByIdAsync(cancellationToken, id);

            programRepository.SoftDelete(program);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ProgramResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<ProgramResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var program = await programRepository.GetByIdAsync(cancellationToken, id);

            if (await programRepository.AnyAsync(cancellationToken, x => x.ExpertiseBranchId == program.ExpertiseBranchId && x.HospitalId == program.HospitalId))
            {
                return new() { Result = false, Message = "In the hospital where the program you want to activate, there is an active program for the same expertise branch!" };
            }

            program.IsDeleted = false;
            program.DeleteDate = null;

            programRepository.Update(program);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ProgramResponseDTO>() { Result = true };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
        {
            filter.pageSize = int.MaxValue;
            filter.page = 1;

            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<ProgramExportResponseDTO> ordersQuery = programRepository.ExportListQuery(zone);

            var acIsActiveFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "AuthorizationCategoryIsActive");
            if (acIsActiveFilter != null)
            {
                ordersQuery = ordersQuery.Where(x => x.AuthorizationDetails.Any(a => a.AuthorizationCategoryIsActive == (bool)acIsActiveFilter.Value));
                filter.Filter.Filters.Remove(acIsActiveFilter);
            }
            FilterResponse<ProgramExportResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var programs = await filterResponse.Query.ToListAsync(cancellationToken);

            var byteArray = ExportList.ExportListReport(programs);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }
        public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramsForDashboard(CancellationToken cancellationToken)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<ProgramPaginateResponseDTO> ordersQuery = programRepository.PaginateListQuery(zone);

            var filter = new FilterDTO()
            {
                Filter = new Filter()
                {
                    Logic = "and",
                    Filters = new List<Filter>()
                        {
                            new() { Field="IsDeleted", Operator="eq", Value=false},
                            new() { Field="AuthorizationCategoryIsActive", Operator="eq", Value=true}
                        },
                },
                page = 1,
                pageSize = 1
            };

            FilterResponse<ProgramPaginateResponseDTO> activeResponse = ordersQuery.ToFilterView(filter);

            //var passiveFilter = new FilterDTO()
            //{
            //    Filter = new Filter()
            //    {
            //        Logic = "and",
            //        Filters = new List<Filter>()
            //            {
            //                new() { Field="IsDeleted", Operator="eq", Value=false},
            //                new() { Field="AuthorizationCategoryIsActive", Operator="eq", Value=false}
            //            },
            //    },
            //    page = 1,
            //    pageSize = 1
            //};
            //FilterResponse<ProgramPaginateResponseDTO> passiveResponse = ordersQuery.ToFilterView(passiveFilter);

            List<ActivePassiveResponseModel> programCount = programRepository.ProgramCountForDashboard();

            programCount.Add(new ActivePassiveResponseModel()
            {
                ActiveRecordsCount = (int)activeResponse.Count,
                //PassiveRecordsCount = (int)passiveResponse.Count,
                SeriesName = "Other Programs"
            });

            return new ResponseWrapper<List<ActivePassiveResponseModel>> { Item = programCount, Result = true };
        }
        public async Task<ResponseWrapper<bool>> AddProgramsByExpertiseBranches(CancellationToken cancellationToken)
        {
            var expertiseBranches = await unitOfWork.ExpertiseBranchRepository.GetAsync(cancellationToken);

            foreach (var expertiseBranch in expertiseBranches)
            {
                var hospitals = await unitOfWork.HospitalRepository.GetAsync(cancellationToken, x => x.Faculty.ProfessionId == expertiseBranch.ProfessionId);

                foreach (var hospital in hospitals)
                {
                    if (hospital.Id != 3 && !await unitOfWork.ProgramRepository.AnyAsync(cancellationToken, x => x.HospitalId == hospital.Id && x.ExpertiseBranchId == expertiseBranch.Id && x.FacultyId == hospital.FacultyId))
                    {
                        try
                        {
                            var program = new Program()
                            {
                                HospitalId = hospital.Id,
                                FacultyId = hospital.FacultyId,
                                ExpertiseBranchId = expertiseBranch.Id,
                                AuthorizationDetails = new List<AuthorizationDetail>()
                                {
                                    new AuthorizationDetail()
                                    {
                                        AuthorizationCategoryId = 6,
                                    }
                                }
                            };

                            await programRepository.AddAsync(cancellationToken, program);
                            await unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return new ResponseWrapper<bool> { Item = true, Result = true };

        }

        public async Task<ResponseWrapper<List<ProgramsCountByUniversityTypeModel>>> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = programRepository.QueryableProgramsForCharts(zone);

            FilterResponse<ProgramChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false && x.AuthorizationCategoryIsActive == true).GroupBy(x => x.IsUniversityPrivate)
                                                    .Select(x => new ProgramsCountByUniversityTypeModel()
                                                    {
                                                        IsPrivate = x.Key,
                                                        Count = x.Count()
                                                    }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<ProgramsCountByUniversityTypeModel>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetFieldNamesCountForDashboard(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = programRepository.QueryableProgramsForCharts(zone);

            FilterResponse<ProgramChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false && x.AuthorizationCategoryIsActive == true)
                                                     .GroupBy(x => x.ProfessionName)
                                                     .Select(x => new ActivePassiveResponseModel()
                                                     {
                                                         SeriesName = x.Key ?? "Bilinmiyor",
                                                         ActiveRecordsCount = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<ActivePassiveResponseModel>> { Item = response, Result = true };
        }
        public async Task<ResponseWrapper<List<AuthorizationCategoryChartModel>>> CountByAuthorizationCategory(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = programRepository.QueryableProgramsForCharts(zone);

            FilterResponse<ProgramChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false && x.AuthorizationCategoryIsActive == true)
                                                     .GroupBy(x => x.AuthorizationCategory)
                                                     .Select(x => new AuthorizationCategoryChartModel()
                                                     {
                                                         SeriesName = x.Key ?? "Bilinmiyor",
                                                         Count = x.Count(),
                                                         ColorCode = x.FirstOrDefault().AuthorizationCategoryColorCode
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<AuthorizationCategoryChartModel>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<ProgramMapModel>>> GetProgramCountByProvince(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = programRepository.QueryableProgramsForCharts(zone);

            FilterResponse<ProgramChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false && x.AuthorizationCategoryIsActive == true)
                                                     .GroupBy(x => x.ProvincePlateCode)
                                                     .Select(x => new ProgramMapModel()
                                                     {
                                                         PlateCode = x.Key ?? "Bilinmiyor",
                                                         ProgramCount = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<ProgramMapModel>> { Item = response, Result = true };
        }
        public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramCountByParentInstitution(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = programRepository.QueryableProgramsForCharts(zone);

            FilterResponse<ProgramChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false && x.AuthorizationCategoryIsActive == true)
                                                     .GroupBy(x => x.ParentInstitutionName)
                                                     .Select(x => new ActivePassiveResponseModel()
                                                     {
                                                         SeriesName = x.Key ?? "Bilinmiyor",
                                                         ActiveRecordsCount = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<ActivePassiveResponseModel>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = programRepository.QueryableProgramsForCharts(zone);

            FilterResponse<ProgramChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false && x.AuthorizationCategoryIsActive == true).GroupBy(x => new { x.ParentInstitutionName, x.ProfessionName })
                                                     .Select(x => new CountsByProfessionInstitutionModel
                                                     {
                                                         ParentInstitutionName = x.Key.ParentInstitutionName ?? "Bilinmiyor",
                                                         ProfessionName = x.Key.ProfessionName ?? "Bilinmiyor",
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

        //public async Task<ResponseWrapper<bool>> CreateAllBranchProgramsByHospitalId(CancellationToken cancellationToken, long hospitalId)
        //{
        //    var hospitals = await unitOfWork.HospitalRepository.GetIncludingList(cancellationToken, x => x.Id != 3 && x.Faculty.ProfessionId != 2, x => x.Faculty);

        //    foreach (var hospital in hospitals)
        //    {
        //        var expertiseBranches = await unitOfWork.ExpertiseBranchRepository.GetAsync(cancellationToken, x => x.ProfessionId == hospital.Faculty.ProfessionId && x.Id != 142);
        //        var authCategory = await unitOfWork.AuthorizationCategoryRepository.GetByAsync(cancellationToken, x => x.IsActive == false);

        //        var affiliated = await unitOfWork.AffiliationRepository.GetByAsync(cancellationToken, x => x.HospitalId == hospital.Id);

        //        foreach (var expertiseBranch in expertiseBranches)
        //        {
        //            if (affiliated == null)
        //            {
        //                if (!await programRepository.AnyAsync(cancellationToken, x => x.HospitalId == hospital.Id && x.ExpertiseBranchId == expertiseBranch.Id && x.FacultyId == hospital.FacultyId))
        //                {
        //                    var program = new Program()
        //                    {
        //                        HospitalId = hospital.Id,
        //                        FacultyId = hospital.FacultyId,
        //                        ExpertiseBranchId = expertiseBranch.Id,
        //                        AuthorizationDetails = (authCategory != null ? new List<AuthorizationDetail>
        //                        {
        //                            new AuthorizationDetail()
        //                            {
        //                                AuthorizationCategoryId = authCategory.Id
        //                            }
        //                        } : null),
        //                    };
        //                    await programRepository.AddAsync(cancellationToken, program);
        //                }
        //            }
        //            else
        //            {
        //                if (!await programRepository.AnyAsync(cancellationToken, x => x.HospitalId == hospital.Id && x.ExpertiseBranchId == expertiseBranch.Id && x.FacultyId == affiliated.FacultyId))
        //                {
        //                    var program = new Program()
        //                    {
        //                        HospitalId = hospital.Id,
        //                        FacultyId = affiliated.FacultyId,
        //                        ExpertiseBranchId = expertiseBranch.Id,
        //                        AuthorizationDetails = (authCategory != null ? new List<AuthorizationDetail>
        //                        {
        //                            new AuthorizationDetail()
        //                            {
        //                                AuthorizationCategoryId = authCategory.Id
        //                            }
        //                        } : null),
        //                    };
        //                    await programRepository.AddAsync(cancellationToken, program);
        //                }
        //            }
        //        }
        //    }

        //    await unitOfWork.CommitAsync(cancellationToken);

        //    return new ResponseWrapper<bool>()
        //    {
        //        Result = true
        //    };
        //}

        public async Task<ResponseWrapper<bool>> CreateAllBranchProgramsByHospitalId(CancellationToken cancellationToken, long hospitalId)
        {
            var hospital = await unitOfWork.HospitalRepository.GetIncluding(cancellationToken, x => x.Id == hospitalId && x.Id != 3, x => x.Programs, x => x.Faculty);

            if (hospital?.Faculty?.ProfessionId != null)
            {
                var expBranchIdList = hospital.Programs?.Select(x => x.ExpertiseBranchId).ToList() ?? new List<long?>();

                var expertiseBranches = await unitOfWork.ExpertiseBranchRepository.GetAsync(cancellationToken, x => x.ProfessionId == hospital.Faculty.ProfessionId && x.Id != 142 &&
                                                                                                                    !expBranchIdList.Contains(x.Id));

                var authCategory = await unitOfWork.AuthorizationCategoryRepository.GetByAsync(cancellationToken, x => x.IsActive == false);

                var affiliated = await unitOfWork.AffiliationRepository.GetByAsync(cancellationToken, x => x.HospitalId == hospital.Id);

                foreach (var expertiseBranch in expertiseBranches)
                {
                    var program = new Program()
                    {
                        HospitalId = hospital.Id,
                        FacultyId = hospital.FacultyId,
                        ExpertiseBranchId = expertiseBranch.Id,
                        AuthorizationDetails = new List<AuthorizationDetail>
                        {
                            new AuthorizationDetail()
                            {
                                AuthorizationCategoryId = authCategory?.Id
                            }
                        },
                    };

                    if (affiliated != null)
                    {
                        program.FacultyId = affiliated.FacultyId;
                    }

                    await programRepository.AddAsync(cancellationToken, program);
                }

                await unitOfWork.CommitAsync(cancellationToken);
            }

            return new ResponseWrapper<bool>()
            {
                Result = true
            };
        }

        #region Import
        public async Task<ResponseWrapper<ProgramResponseDTO>> ImportFromExcel(CancellationToken cancellationToken, IFormFile formFile)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            };


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int count = 1;
                    while (reader.Read()) //Each row of the file
                    {
                        if (count < 4 || string.IsNullOrWhiteSpace(reader.GetValue(0)?.ToString()))
                        {
                            count++;
                            continue;
                        }

                        var university = reader.GetValue(3) != null ? (await unitOfWork.UniversityRepository.GetIncluding(cancellationToken, x => x.Name.Trim() == reader.GetValue(3).ToString().Trim() && x.IsDeleted == false, x => x.Faculties)) : null;
                        if (university == null)
                        {
                            university = new University()
                            {
                                Name = reader.GetValue(3)?.ToString().Trim(),
                                InstitutionId = await GetInstitutionId(cancellationToken, reader.GetValue(2)),
                                IsPrivate = reader.GetValue(2)?.ToString().Trim() == "YÖK-Üni/Vakıf",
                                ProvinceId = await GetProvinceId(cancellationToken, reader.GetValue(1)),
                            };
                            // await unitOfWork.UniversityRepository.AddAsync(cancellationToken, university);
                        }

                        var faculty = reader.GetValue(4) != null ? (await unitOfWork.FacultyRepository.GetByAsync(cancellationToken, x => x.Name.Trim() == reader.GetValue(4).ToString().Trim() && x.IsDeleted == false)) : null;
                        if (faculty == null)
                        {
                            faculty = new Faculty()
                            {
                                University = university,
                                Name = reader.GetValue(4)?.ToString().Trim(),
                                Address = reader.GetValue(1)?.ToString().Trim(),
                            };
                            //await unitOfWork.FacultyRepository.AddAsync(cancellationToken, faculty);
                        }

                        var hospital = reader.GetValue(5) != null ? (await unitOfWork.HospitalRepository.GetByAsync(cancellationToken, x => x.Name.Trim() == reader.GetValue(5).ToString().Trim() && x.IsDeleted == false)) : null;
                        if (hospital == null)
                        {
                            hospital = new Hospital()
                            {
                                ProvinceId = await GetProvinceId(cancellationToken, reader.GetValue(1)),
                                Name = reader.GetValue(5)?.ToString().Trim(),
                                InstitutionId = await GetInstitutionId(cancellationToken, reader.GetValue(2)),
                                Faculty = faculty
                            };
                            //await unitOfWork.HospitalRepository.AddAsync(cancellationToken, hospital);
                        }

                        //await unitOfWork.CommitAsync(cancellationToken);

                        var programFacultyId = faculty?.Id;

                        var affiliatedUniversityName = reader.GetValue(7)?.ToString().Trim();
                        var afilliatedUniversity = reader.GetValue(7) != null ? (await unitOfWork.UniversityRepository.GetIncluding(cancellationToken, x => x.Name.Trim() == affiliatedUniversityName && x.IsDeleted == false, x => x.Faculties)) : null;

                        var affiliatedFacultyName = reader.GetValue(8)?.ToString().Trim();
                        var affiliatedFaculty = affiliatedFacultyName != null ? (await unitOfWork.FacultyRepository.GetByAsync(cancellationToken, x => x.Name.Trim() == affiliatedFacultyName && x.IsDeleted == false)) : null;

                        if (affiliatedUniversityName != null && afilliatedUniversity == null)
                        {
                            afilliatedUniversity = new University()
                            {
                                Name = reader.GetValue(7)?.ToString().Trim(),
                                InstitutionId = await GetInstitutionId(cancellationToken, reader.GetValue(2)),
                                IsPrivate = false,
                                ProvinceId = await GetProvinceId(cancellationToken, reader.GetValue(1)),
                            };
                            //await unitOfWork.UniversityRepository.AddAsync(cancellationToken, afilliatedUniversity);

                            if (affiliatedFacultyName != null && affiliatedFaculty == null)
                            {
                                affiliatedFaculty = new Faculty()
                                {
                                    University = afilliatedUniversity,
                                    Name = affiliatedFacultyName,
                                };
                                //await unitOfWork.FacultyRepository.AddAsync(cancellationToken, affiliatedFaculty);
                            }

                            //await unitOfWork.CommitAsync(cancellationToken);
                        }

                        if (affiliatedUniversityName == null && reader.GetValue(8) != null)
                        {
                            continue;
                        }
                        var afilliatedFaculty = reader.GetValue(8) != null ? afilliatedUniversity?.Faculties?.FirstOrDefault(x => x.Name.Trim() == affiliatedFacultyName && x.IsDeleted == false) : null;

                        if (afilliatedUniversity != null && afilliatedFaculty != null)
                        {
                            var affilliation = await unitOfWork.AffiliationRepository.GetByAsync(cancellationToken, x => x.HospitalId == hospital.Id && x.FacultyId == afilliatedFaculty.Id && x.IsDeleted == false);

                            if (affilliation == null)
                            {
                                affilliation = new Affiliation()
                                {
                                    HospitalId = hospital?.Id,
                                    FacultyId = afilliatedFaculty?.Id,
                                };

                                //unitOfWork.AffiliationRepository.Add(affilliation);
                                //await unitOfWork.CommitAsync(cancellationToken);
                            }
                            programFacultyId = afilliatedFaculty?.Id;
                        }

                        var expBranchId = await GetExpertiseBranchId(cancellationToken, reader.GetValue(6));
                        if (expBranchId == null)
                        {

                        }

                        var program = await unitOfWork.ProgramRepository.GetByAsync(cancellationToken, x => x.HospitalId == hospital.Id && x.ExpertiseBranchId == expBranchId && x.IsDeleted == false);
                        if (program != null && program?.FacultyId != programFacultyId)
                        {
                            program.FacultyId = programFacultyId;

                            programRepository.Update(program);
                            await unitOfWork.CommitAsync(cancellationToken);
                        }

                        if (program == null)
                        {
                            program = new Program()
                            {
                                HospitalId = hospital.Id,
                                FacultyId = programFacultyId,
                                ExpertiseBranchId = expBranchId,
                            };
                            //await programRepository.AddAsync(cancellationToken, program);
                            //await unitOfWork.CommitAsync(cancellationToken);
                        }


                        //var hospital = await unitOfWork.HospitalRepository.GetByAsync(cancellationToken, x => x.Name.Trim() == reader.GetValue(6).ToString().Trim());
                        //var hospitalId = hospital?.Id;
                        //var expertiseBranchId = (long)await GetExpertiseBranchId(cancellationToken, reader.GetValue(7));
                        //var program = await unitOfWork.ProgramRepository.GetIncluding(cancellationToken, x => x.HospitalId == hospitalId && x.ExpertiseBranchId == expertiseBranchId, x => x.AuthorizationDetails);
                        //if (program != null && !program.AuthorizationDetails.Any())

                        var authorizationDetails = new List<AuthorizationDetail>
                        {
                            // Current AuthorizationDetail
                            new AuthorizationDetail()
                            {
                                AuthorizationEndDate = DateTimeParser(reader.GetValue(9)),
                                AuthorizationCategoryId = GetAuthorizationCategoryId(reader.GetValue(10)),
                                VisitDate = DateTimeParser(reader.GetValue(11)),
                                AuthorizationDate = DateTimeParser(reader.GetValue(12)),
                                AuthorizationDecisionNo = reader.GetValue(13)?.ToString(),
                                ProgramId = program.Id
                            }
                        };
                        var column = 14;
                        while (column <= 54)
                        {
                            if (Validation(reader.GetValue(column)) || Validation(reader.GetValue(column + 1)) || Validation(reader.GetValue(column + 2)) || Validation(reader.GetValue(column + 3)))
                            {
                                authorizationDetails.Add(new AuthorizationDetail()
                                {
                                    AuthorizationCategoryId = GetAuthorizationCategoryId(reader.GetValue(column)),
                                    VisitDate = DateTimeParser(reader.GetValue(column + 1)),
                                    AuthorizationDate = DateTimeParser(reader.GetValue(column + 2)),
                                    AuthorizationDecisionNo = reader.GetValue(column + 3)?.ToString(),
                                    ProgramId = program.Id
                                });
                            }
                            column += 4;
                        }
                        //await unitOfWork.AuthorizationDetailRepository.AddRangeAsync(cancellationToken, authorizationDetails);
                        try
                        {
                            //await unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return new();
        }
        private static bool Validation(object value)
        {
            return (value?.ToString() != null && value?.ToString()?.ToUpper()?.Trim() != "N/A" && value?.ToString()?.ToUpper()?.Trim() != "YOK");
        }
        private static DateTime? DateTimeParser(object value)
        {
            if (value != null)
            {
                if (Validation(value))
                {
                    return DateTimeOffset.Parse(value.ToString()).DateTime.AddHours(3).ToUniversalTime();
                }
            }
            return null;
        }
        private static long? GetAuthorizationCategoryId(object value)
        {
            if (value?.ToString() == "0")
            {
                return 1;
            }
            else if (value?.ToString() == "1")
            {
                return 2;
            }
            else if (value?.ToString() == "2")
            {
                return 3;
            }
            else if (value?.ToString() == "3")
            {
                return 4;
            }
            else if (value?.ToString() == "9")
            {
                return 5;
            }
            else
            {
                return null;
            }
        }
        private async Task<long?> GetProvinceId(CancellationToken cancellationToken, object value)
        {
            if (value != null)
            {
                var cities = await unitOfWork.ProvinceRepository.GetAsync(cancellationToken, x => x.Name.ToLower() == value.ToString().Trim().ToLower(new CultureInfo("tr-TR")));
                return cities.FirstOrDefault()?.Id;
            }

            return null;
        }
        private async Task<long?> GetExpertiseBranchId(CancellationToken cancellationToken, object value)
        {
            if (value != null)
            {
                var expertiseBranche = await unitOfWork.ExpertiseBranchRepository.GetByAsync(cancellationToken, x => x.Name == value.ToString().Trim());
                return expertiseBranche?.Id;
            }
            return null;
        }
        private async Task<long?> GetInstitutionId(CancellationToken cancellationToken, object value)
        {
            var institutionName =
                (value.ToString().Trim() == "YÖK-Üni/Vakıf" || value.ToString().Trim() == "YÖK-Üni/Vakıf")
                    ? "Yükseköğretim Kurumu (YÖK)"
                    : value.ToString().Trim();

            if (value != null)
            {
                var instutiton = await unitOfWork.InstitutionRepository.GetByAsync(cancellationToken, x => x.Name.Contains(value.ToString().Trim()));
                return instutiton?.Id;
            }
            return null;
        }
        #endregion

        public async Task<ResponseWrapper<List<string>>> YUEPExcelCheck(CancellationToken cancellationToken, IFormFile formFile)
        {
            var missingPrograms = new List<string>();

            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            };


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int count = 1;
                    while (reader.Read()) //Each row of the file
                    {
                        if (count < 4 || string.IsNullOrWhiteSpace(reader.GetValue(0)?.ToString()))
                        {
                            count++;
                            continue;
                        }

                        var program = await unitOfWork.ProgramRepository.GetByAsync(cancellationToken, x => x.Hospital.Name == reader.GetValue(5).ToString().Trim() &&
                                                                                                            x.ExpertiseBranch.Name == reader.GetValue(6).ToString().Trim() &&
                                                                                                            x.AuthorizationDetails.Any(x => x.IsDeleted == false));

                        if (program == null)
                        {
                            missingPrograms.Add(reader.GetValue(5)?.ToString().Trim() ?? "");
                        }
                    }
                }
            }

            return new ResponseWrapper<List<string>>
            {
                Item = missingPrograms
            };
        }
    }
}
