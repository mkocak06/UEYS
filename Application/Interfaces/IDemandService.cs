using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
	public interface IDemandService
    {
        Task<PaginationModel<DemandResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<DemandResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<DemandResponseDTO>> PostAsync(CancellationToken cancellationToken, DemandDTO demandDto);
        Task<ResponseWrapper<DemandResponseDTO>> Put(CancellationToken cancellationToken, long id, DemandDTO demandDto);
        Task<ResponseWrapper<DemandResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
