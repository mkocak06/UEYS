using Shared.ResponseModels.ENabizPortfolio;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Interfaces
{
    public interface IENabizPortfolioService
    {
        Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetTotalOperationsByUserIdAsync(CancellationToken cancellationToken, long userId);
        Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetTotalOperationsByProgramIdAsync(CancellationToken cancellationToken, long programId, UserType? userType);
        Task<ResponseWrapper<List<ENabizPortfolioResponseDTO>>> GetAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetUserOperationsChartData(CancellationToken cancellationToken, long userId);
        Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetUserOperationsChartDataByProgramId(CancellationToken cancellationToken, long programId, UserType? userType);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, long userId);
        Task<ResponseWrapper<byte[]>> ExcelExportByProgramId(CancellationToken cancellationToken, long programId, UserType? userType);
    }
}
