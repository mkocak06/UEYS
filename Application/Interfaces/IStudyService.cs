using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IStudyService
    {
        Task<ResponseWrapper<List<StudyResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<StudyResponseDTO>> PostAsync(CancellationToken cancellationToken, StudyDTO studyDTO);
        Task<ResponseWrapper<StudyResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<StudyResponseDTO>> Put(CancellationToken cancellationToken, long id, StudyDTO studyDTO);
        Task<ResponseWrapper<StudyResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<PaginationModel<StudyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
    }
}
