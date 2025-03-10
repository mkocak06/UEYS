using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IStudentExpertiseBranchService
    {
        Task<ResponseWrapper<List<StudentExpertiseBranchResponseDTO>>> GetListByStudentIdAsync(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<StudentExpertiseBranchResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentExpertiseBranchDTO studentExpertiseBranchDTO);
        Task<ResponseWrapper<StudentExpertiseBranchResponseDTO>> Delete(CancellationToken cancellationToken, long studentId, long expBranchId);
    }
}
