using Application.Interfaces;
using Application.Reports.ExcelReports.UniversityReports;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.University;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class UniversityService : BaseService, IUniversityService
    {
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;
        private readonly IUniversityRepository universityRepository;

        public UniversityService(IMapper mapper, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.universityRepository = universityRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }
        public async Task<ResponseWrapper<List<UniversityResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<University> query = universityRepository.IncludingQueryable(x => !x.IsDeleted, x => x.Province, x => x.Institution);

            List<University> universities = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<UniversityResponseDTO> response = mapper.Map<List<UniversityResponseDTO>>(universities);

            return new() { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<bool>> UnDeleteUniversity(CancellationToken cancellationToken, long id)
        {
            var university = await universityRepository.GetByIdAsync(cancellationToken, id);

            if (university != null && university.IsDeleted == true)
            {
                university.IsDeleted = false;
                university.DeleteDate = null;

                universityRepository.Update(university);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }
        public async Task<PaginationModel<UniversityResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            IQueryable<University> ordersQuery = universityRepository.PaginateQuery(zone);



            //IQueryable<University> ordersQuery = universityRepository.IncludingQueryable(x => !x.IsDeleted, x => x.Institution, x => x.Province);
            FilterResponse<University> filterResponse = ordersQuery.ToFilterView(filter);

            var universities = mapper.Map<List<UniversityResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<UniversityResponseDTO>
            {
                Items = universities,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            
            return response;
        }
        public async Task<PaginationModel<UniversityResponseDTO>> GetAffiliationPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<University> ordersQuery = universityRepository.QueryableUniversitiesForAffilitation();
            FilterResponse<University> filterResponse = ordersQuery.ToFilterView(filter);

            var universities = mapper.Map<List<UniversityResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<UniversityResponseDTO>
            {
                Items = universities,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<PaginationModel<UniversityResponseDTO>> GetDeletedPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<University> ordersQuery = universityRepository.QueryableUniversities(x => x.IsDeleted);

            FilterResponse<University> filterResponse = ordersQuery.ToFilterView(filter);

            var universities = mapper.Map<List<UniversityResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<UniversityResponseDTO>
            {
                Items = universities,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<UniversityResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            University university = await universityRepository.GetByIdWithSubRecords(cancellationToken, id);
            UniversityResponseDTO response = mapper.Map<UniversityResponseDTO>(university);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<UniversityResponseDTO>> PostAsync(CancellationToken cancellationToken, UniversityDTO universityDTO)
        {
            University university = mapper.Map<University>(universityDTO);

            await universityRepository.AddAsync(cancellationToken, university);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<UniversityResponseDTO>(university);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<UniversityResponseDTO>> Put(CancellationToken cancellationToken, long id, UniversityDTO universityDTO)
        {
            University university = await universityRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.Faculties);

            University updatedUniversity = mapper.Map(universityDTO, university);

            universityRepository.Update(updatedUniversity);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<UniversityResponseDTO>(updatedUniversity);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<UniversityResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            University university = await universityRepository.GetByIdAsync(cancellationToken, id);

            universityRepository.SoftDelete(university);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<UniversityResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var university = await universityRepository.GetByIdAsync(cancellationToken, id);

            university.IsDeleted = false;
            university.DeleteDate = null;

            universityRepository.Update(university);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<UniversityResponseDTO>() { Result = true };
        }

        public async Task<ResponseWrapper<List<UniversityBreadcrumbDTO>>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId)
        {
            List<UniversityBreadcrumbDTO> universities = await universityRepository.GetListByExpertiseBranchId(cancellationToken, expertiseBranchId);

            return new() { Result = true, Item = universities };
        }

        public async Task<ResponseWrapper<List<UniversityResponseDTO>>> GetListByProvinceId(CancellationToken cancellationToken, long provinceId)
        {
            IQueryable<University> query = universityRepository.IncludingQueryable(x => x.ProvinceId == provinceId && x.IsDeleted == false);

            List<University> universities = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<UniversityResponseDTO> response = mapper.Map<List<UniversityResponseDTO>>(universities);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
        {
            filter.pageSize = int.MaxValue;
            filter.page = 1;

            var response = await GetPaginateList(cancellationToken, filter);

            var byteArray = ExportList.ExportListReport(response.Items);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }
        public async Task<ResponseWrapper<List<ReportResponseDTO>>> GetFilteredReport(CancellationToken cancellationToken, List<FilterDTO> filters)
        {
            IQueryable<University> ordersQuery = unitOfWork.UniversityRepository.QueryableUniversities(x => x.IsDeleted == false);

            List<ReportResponseDTO> report = filters.Select((dto, i) => new ReportResponseDTO()
            {
                Key = dto.Key,
                Value = ordersQuery.ToFilterView(dto).Count.ToString()
            }).ToList();

            return new ResponseWrapper<List<ReportResponseDTO>>() { Result = true, Item = report };
        }

        public async Task<ResponseWrapper<List<ReportResponseDTO>>> UniversityCountByParentInstitution()
        {
            var response = universityRepository.GetReports();

            return new ResponseWrapper<List<ReportResponseDTO>> { Item = response, Result = true };
        }
    }
}
