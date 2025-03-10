using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface ICurriculumPerfectionService
    {
        Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<CurriculumPerfectionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, CurriculumPerfectionDTO curriculumPerfectionDTO);
        Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, CurriculumPerfectionDTO curriculumPerfectionDTO);
        Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);

    }
}
