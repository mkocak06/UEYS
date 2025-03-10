using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IExpertiseBranchService
    {
        Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<ExpertiseBranchResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<ExpertiseBranchResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListRelatedWithProgramsByProfessionIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListByProfessionIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ExpertiseBranchResponseDTO>> PostAsync(CancellationToken cancellationToken, ExpertiseBranchDTO expertiseBranchDTO);
        Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Put(CancellationToken cancellationToken, long id, ExpertiseBranchDTO expertiseBranchDTO);
        Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<bool>> ImportFromExcel(CancellationToken cancellationToken, IFormFile formFile); 
        Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListForProtocolProgramByHospitalId(CancellationToken cancellationToken, long hospitalId);
    }
}
