using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IEducatorCountContributionFormulaService
    {
        Task<PaginationModel<EducatorCountContributionFormulaResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorCountContributionFormulaDTO educatorCountContributionFormulaDTO);
        Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorCountContributionFormulaDTO educatorCountContributionFormulaDTO);
        Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
