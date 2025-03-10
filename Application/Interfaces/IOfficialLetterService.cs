using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IOfficialLetterService
    {
        Task<PaginationModel<OfficialLetterResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<OfficialLetterResponseDTO>> PostAsync(CancellationToken cancellationToken, OfficialLetterDTO progressReportDTO);
        Task<ResponseWrapper<OfficialLetterResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<OfficialLetterResponseDTO>> Put(CancellationToken cancellationToken, long id, OfficialLetterDTO progressReportDTO);
        Task<ResponseWrapper<OfficialLetterResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<OfficialLetterResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId);
    }
}
