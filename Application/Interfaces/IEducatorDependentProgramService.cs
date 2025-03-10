using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IEducatorDependentProgramService
    {
        Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetListByDependProgIdAsync(CancellationToken cancellationToken, long dependProgId);
        Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorDependentProgramDTO educatorDependentProgramDTO);
        Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Delete(CancellationToken cancellationToken, long educatorId, long dependentProgramId);
        Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorDependentProgramDTO educatorDependentProgramDTO);
    }
}
