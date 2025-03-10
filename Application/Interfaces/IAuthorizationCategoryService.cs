using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IAuthorizationCategoryService
    {
        Task<ResponseWrapper<List<AuthorizationCategoryResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> PostAsync(CancellationToken cancellationToken, AuthorizationCategoryDTO authorizationCategoryDTO);
        Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Put(CancellationToken cancellationToken, long id, AuthorizationCategoryDTO authorizationCategoryDTO);
        Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<PaginationModel<AuthorizationCategoryResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        //Task<ResponseWrapper<bool>> ChangeOrder(CancellationToken cancellationToken, long authCategoryId, bool isToUp);
    }
}
