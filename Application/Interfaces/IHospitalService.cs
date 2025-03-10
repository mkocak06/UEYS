using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.FilterModels;

namespace Application.Interfaces
{
    public interface IHospitalService
    {
        Task<ResponseWrapper<List<HospitalResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginationModel<HospitalResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<PaginationModel<HospitalResponseDTO>> GetDeletedPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<HospitalResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<ResponseWrapper<List<HospitalBreadcrumbDTO>>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expId);
        Task<ResponseWrapper<HospitalResponseDTO>> PostAsync(CancellationToken cancellationToken, HospitalDTO hospitalDTO);
        Task<ResponseWrapper<HospitalResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<HospitalResponseDTO>> Put(CancellationToken cancellationToken, long id, HospitalDTO hospitalDTO);
        Task<ResponseWrapper<HospitalResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<MapDTO>>> GetListForMap(CancellationToken cancellationToken, long? universityId);
        Task<ResponseWrapper<UserHospitalDetailDTO>> GetUserHospitalDetail(CancellationToken cancellationToken);
        Task<ResponseWrapper<bool>> GetLatLongDetailsFromGoogle(CancellationToken cancellationToken);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<HospitalResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetHospitalCountByParentInstitution(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<bool>> ImportFromExcel(CancellationToken cancellationToken, IFormFile formFile);
        Task<ResponseWrapper<byte[]>> DetailedExcelExport(CancellationToken cancellationToken, ProgramFilter filterX);
    }
}
