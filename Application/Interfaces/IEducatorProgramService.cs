using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IEducatorProgramService
    {
        Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByEducatorIdAsync(CancellationToken cancellationToken, long educatorId);
        Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId);
        Task<ResponseWrapper<EducatorProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorProgramDTO educatorProgramDTO);
        Task<ResponseWrapper<EducatorProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorProgramDTO educatorProgramDTO);
        Task<ResponseWrapper<EducatorProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<EducatorProgramResponseDTO>> GetById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId); 
    }
}
