using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface ISubQuotaRequestService
    {
        Task<PaginationModel<SubQuotaRequestPaginateResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<SubQuotaRequestResponseDTO>> PostAsync(CancellationToken cancellationToken, SubQuotaRequestDTO subQuotaRequestDTO);
        Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Put(CancellationToken cancellationToken, long id, SubQuotaRequestDTO subQuotaRequestDTO);
        Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetByProgramId(CancellationToken cancellationToken, long programId);

    }
}
