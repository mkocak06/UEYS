using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IEducatorExpertiseBranchService
    {
        Task<ResponseWrapper<List<EducatorExpertiseBranchResponseDTO>>> GetListByEducatorIdAsync(CancellationToken cancellationToken, long educatorId);
        Task<ResponseWrapper<EducatorExpertiseBranchResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorExpertiseBranchDTO educatorExpertiseBranchDTO);
        Task<ResponseWrapper<EducatorExpertiseBranchResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
