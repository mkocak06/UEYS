using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface ISubQuotaRequestPortfolioService
    {
        Task<ResponseWrapper<SubQuotaRequestPortfolioResponseDTO>> PostAsync(CancellationToken cancellationToken, SubQuotaRequestPortfolioDTO subQuotaRequestPortfolioDTO);
        Task<ResponseWrapper<SubQuotaRequestPortfolioResponseDTO>> Put(CancellationToken cancellationToken, long id, SubQuotaRequestPortfolioDTO subQuotaRequestPortfolioDTO);
        Task<ResponseWrapper<SubQuotaRequestPortfolioResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
