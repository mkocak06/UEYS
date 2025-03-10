using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IThesisDefenceService
    {
        Task<PaginationModel<ThesisDefenceResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> PostAsync(CancellationToken cancellationToken, ThesisDefenceDTO thesisDefenceDTO);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> GetById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> Put(CancellationToken cancellationToken, long id, ThesisDefenceDTO thesisDefenceDTO);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> IsThesisDefenceAddable(CancellationToken cancellationToken, long thesisId, DateTime? date);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> Delete(CancellationToken cancellationToken, long id, long studentId);
        Task<ResponseWrapper<List<ThesisDefenceResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId);
    }
}
