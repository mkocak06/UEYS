using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IStudentCountService
    {
        Task<PaginationModel<StudentCountResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<StudentCountResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<StudentCountResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentCountDTO studentCountDTO);
        Task<ResponseWrapper<StudentCountResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentCountDTO studentCountDTO);
        Task<ResponseWrapper<StudentCountResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
