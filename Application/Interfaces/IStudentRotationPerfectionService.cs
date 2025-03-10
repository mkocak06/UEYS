using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Interfaces
{
    public interface IStudentRotationPerfectionService
    {
        Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentRotationPerfectionDTO StudentRotationPerfectionDTO);
        Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentRotationPerfectionDTO studentRotationDTO);
        Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> GetByStrIdAndPerfectionId(CancellationToken cancellationToken, long studentRotationId, long perfectionId);
    }
}
