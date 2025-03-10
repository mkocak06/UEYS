using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IScientificStudyService
    {
        Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<PaginationModel<ScientificStudyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<ScientificStudyResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ScientificStudyResponseDTO>> PostAsync(CancellationToken cancellationToken, ScientificStudyDTO scientificStudyDTO);
        Task<ResponseWrapper<ScientificStudyResponseDTO>> Put(CancellationToken cancellationToken, long id, ScientificStudyDTO scientificStudyDTO);
        Task<ResponseWrapper<ScientificStudyResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
