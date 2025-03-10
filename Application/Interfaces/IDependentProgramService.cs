using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IDependentProgramService
    {
        Task<ResponseWrapper<DependentProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, DependentProgramDTO dependentProgramDTO);
        Task<ResponseWrapper<DependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, DependentProgramDTO dependentProgramDTO);
        Task<ResponseWrapper<DependentProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
