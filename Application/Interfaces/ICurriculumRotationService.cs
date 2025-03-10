using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface ICurriculumRotationService
    {
        Task<ResponseWrapper<CurriculumRotationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<CurriculumRotationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<CurriculumRotationResponseDTO>> PostAsync(CancellationToken cancellationToken, CurriculumRotationDTO curriculumRotationDTO);
        Task<ResponseWrapper<CurriculumRotationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<CurriculumRotationResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
    }
}