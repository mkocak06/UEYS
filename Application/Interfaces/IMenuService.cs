using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Menu;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
	public interface IMenuService
    {
        Task<ResponseWrapper<MenuResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<MenuResponseDTO>> PostAsync(CancellationToken cancellationToken, MenuDTO menuDto);
        Task<ResponseWrapper<MenuResponseDTO>> Put(CancellationToken cancellationToken, long id, MenuDTO menuDto);
        Task<ResponseWrapper<MenuResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<MenuResponseDTO>>> GetListByUser(CancellationToken cancellationToken);
    }
}
