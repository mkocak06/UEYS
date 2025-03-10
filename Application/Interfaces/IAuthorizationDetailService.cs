using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IAuthorizationDetailService
    {
        Task<ResponseWrapper<List<AuthorizationDetailResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId);
        Task<ResponseWrapper<AuthorizationDetailResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<AuthorizationDetailResponseDTO>> PostAsync(CancellationToken cancellationToken, AuthorizationDetailDTO authorizationDetailDTO);
        Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Put(CancellationToken cancellationToken, long id, AuthorizationDetailDTO authorizationDetailDTO);
        Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
