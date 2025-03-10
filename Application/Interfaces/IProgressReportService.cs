using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IProgressReportService
    {
        Task<PaginationModel<ProgressReportResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<ProgressReportResponseDTO>> PostAsync(CancellationToken cancellationToken, ProgressReportDTO progressReportDTO);
        Task<ResponseWrapper<ProgressReportResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ProgressReportResponseDTO>> Put(CancellationToken cancellationToken, long id, ProgressReportDTO progressReportDTO);
        Task<ResponseWrapper<ProgressReportResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<ProgressReportResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId);
        Task<ResponseWrapper<ProgressReportResponseDTO>> CalculateStartEndDates(CancellationToken cancellationToken, long thesisId, long studentId);
    }
}
