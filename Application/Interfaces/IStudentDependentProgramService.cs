using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IStudentDependentProgramService
    {
        Task<ResponseWrapper<StudentDependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentDependentProgramDTO studentDependentProgramDTO);
        Task<ResponseWrapper<List<StudentDependentProgramPaginateDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studenId);
    }
}
