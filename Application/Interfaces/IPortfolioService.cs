using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IPortfolioService
    {
        Task<PaginationModel<PortfolioResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<PortfolioResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<PortfolioResponseDTO>> PostAsync(CancellationToken cancellationToken, PortfolioDTO quotaRequestDTO);
        Task<ResponseWrapper<PortfolioResponseDTO>> Put(CancellationToken cancellationToken, long id, PortfolioDTO quotaRequestDTO);
        Task<ResponseWrapper<PortfolioResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
