using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IQuotaRequestService
    {
        Task<PaginationModel<QuotaRequestResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<QuotaRequestResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<QuotaRequestResponseDTO>> PostAsync(CancellationToken cancellationToken, QuotaRequestDTO quotaRequestDTO);
        Task<ResponseWrapper<QuotaRequestResponseDTO>> Put(CancellationToken cancellationToken, long id, QuotaRequestDTO quotaRequestDTO);
        Task<ResponseWrapper<QuotaRequestResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
