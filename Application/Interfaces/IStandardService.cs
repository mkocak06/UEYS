using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Standard;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IStandardService
    {
        Task<ResponseWrapper<List<StandardResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<StandardResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<StandardResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<List<ProgramStandardResponseDTO>> GetPaginateListByLatestCurriculumsExpertiseBranch(CancellationToken cancellationToken, long expBranchId);
        Task<ResponseWrapper<StandardResponseDTO>> PostAsync(CancellationToken cancellationToken, StandardDTO standardDTO);
        Task<ResponseWrapper<StandardResponseDTO>> Put(CancellationToken cancellationToken, long id, StandardDTO standardDTO);
        Task<ResponseWrapper<StandardResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        //Task<ResponseWrapper<bool>> ImportExcel(CancellationToken cancellationToken, IFormFile formFile);
        //Task<ResponseWrapper<bool>> ImportExcelGenelStandars(CancellationToken cancellationToken, IFormFile formFile);
    }
}