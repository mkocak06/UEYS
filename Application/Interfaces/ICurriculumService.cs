using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface ICurriculumService
    {
        Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginationModel<CurriculumResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<CurriculumResponseDTO>> PostAsync(CancellationToken cancellationToken, CurriculumDTO curriculumDTO);
        Task<ResponseWrapper<CurriculumResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetByExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId);
        Task<ResponseWrapper<CurriculumResponseDTO>> GetLatestByBeginningDateAndExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId, DateTime beginningDate);
        Task<ResponseWrapper<CurriculumResponseDTO>> Put(CancellationToken cancellationToken, long id, CurriculumDTO curriculumDTO);
        Task<ResponseWrapper<CurriculumResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<bool>> UnDeleteCurriculum(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<CurriculumResponseDTO>> UploadExcel(CancellationToken cancellationToken, IFormFile file);
        Task<ResponseWrapper<CurriculumResponseDTO>> CreateCopy(CancellationToken cancellationToken, long id, CurriculumDTO curriculumDTO);
        Task<ResponseWrapper<CurriculumResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<byte[]>> CurriculumDetailsExport(CancellationToken cancellationToken);
    }
}
