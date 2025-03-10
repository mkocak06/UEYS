using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;

namespace Application.Interfaces
{
    public interface IEducationOfficerService
    {
        Task<ResponseWrapper<EducationOfficerResponseDTO>> PostAsync(CancellationToken cancellationToken, EducationOfficerDTO educationOfficerDTO);
        Task<ResponseWrapper<EducationOfficerResponseDTO>> ChangeProgramManager(CancellationToken cancellationToken, EducationOfficerDTO educationOfficerDTO);
        Task<PaginationModel<EducationOfficerResponseDTO>> GetPaginateListForProgramDetail(CancellationToken cancellationToken, FilterDTO filter);
        //Task<ResponseWrapper<EducationOfficerResponseDTO>> FinishDuty(CancellationToken cancellationToken, long programId, long educatorId);
        Task<ResponseWrapper<List<EducationOfficerResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId);
    }
}
