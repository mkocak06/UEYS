using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.University;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUniversityService
    {
        Task<ResponseWrapper<List<UniversityResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginationModel<UniversityResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<PaginationModel<UniversityResponseDTO>> GetDeletedPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<UniversityResponseDTO>> PostAsync(CancellationToken cancellationToken, UniversityDTO universityDTO);
        Task<ResponseWrapper<UniversityResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<UniversityResponseDTO>> Put(CancellationToken cancellationToken, long id, UniversityDTO universityDTO);
        Task<ResponseWrapper<UniversityResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<UniversityBreadcrumbDTO>>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId);
        Task<ResponseWrapper<List<UniversityResponseDTO>>> GetListByProvinceId(CancellationToken cancellationToken, long provinceId);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<UniversityResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<ReportResponseDTO>>> GetFilteredReport(CancellationToken cancellationToken, List<FilterDTO> filters);
        Task<ResponseWrapper<List<ReportResponseDTO>>> UniversityCountByParentInstitution();
        Task<PaginationModel<UniversityResponseDTO>> GetAffiliationPaginateList(CancellationToken cancellationToken, FilterDTO filter);
    }
}
