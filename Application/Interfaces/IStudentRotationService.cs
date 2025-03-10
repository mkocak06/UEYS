using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IStudentRotationService
    {
        Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<StudentRotationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<StudentRotationResponseDTO>> GetByStudentAndRotationId(CancellationToken cancellationToken, long studentId, long rotationId);
        Task<ResponseWrapper<StudentRotationResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO);
        Task<ResponseWrapper<StudentRotationResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentRotationDTO studentRotationDTO);
        Task<ResponseWrapper<StudentRotationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<StudentRotationResponseDTO>> DeleteActiveRotation(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerStudentsListByUserId(CancellationToken cancellationToken, long userId);
        Task<ResponseWrapper<StudentRotationResponseDTO>> FinishStudentsRotation(CancellationToken cancellationToken,long id, StudentRotationDTO studentRotationDTO);
        Task<ResponseWrapper<StudentRotationResponseDTO>> SendStudentToRotation(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO);
        Task<ResponseWrapper<StudentRotationResponseDTO>> AddPastRotation(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO);
        Task<ResponseWrapper<StudentRotationResponseDTO>> GetEndDateByStartDate(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO);
    }
}
