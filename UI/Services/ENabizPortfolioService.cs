using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Services
{
    public interface IENabizPortfolioService
    {
        Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetByUserId(long? userId);
        Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetByProgramId(long? programId, UserType? userType = null);
        Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetChartDataByUserId(long? userId);
        Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetChartDataByProgramId(long? programId, UserType? userType = null);
        Task<ResponseWrapper<byte[]>> GetExcelByteArray(long? userId);
        Task<ResponseWrapper<byte[]>> GetExcelByteArrayByProgram(long? programId, UserType? userType = null);
    }
    public class ENabizPortfolioService : IENabizPortfolioService
    {
        private readonly IHttpService _httpService;

        public ENabizPortfolioService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetByUserId(long? userId)
        {
            return await _httpService.Get<ResponseWrapper<ENabizPortfolioResponseDTO>>($"ENabizPortfolio/GetByUserId/{userId}");
        }

        public async Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetByProgramId(long? programId, UserType? userType = null)
        {
            return await _httpService.Get<ResponseWrapper<ENabizPortfolioResponseDTO>>($"ENabizPortfolio/GetByProgramId?programId={programId}&userType={userType}");
        }

        public async Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetChartDataByUserId(long? userId)
        {
            return await _httpService.Get<ResponseWrapper<List<ENabizPortfolioChartModel>>>($"ENabizPortfolio/GetChartDataByUserId/{userId}");
        }

        public async Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetChartDataByProgramId(long? programId, UserType? userType = null)
        {
            return await _httpService.Get<ResponseWrapper<List<ENabizPortfolioChartModel>>>($"ENabizPortfolio/GetChartDataByProgramId?programId={programId}&userType={userType}");
        }

        public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(long? userId)
        {
            return await _httpService.Get<ResponseWrapper<byte[]>>($"ENabizPortfolio/ExcelExport/{userId}");
        }
        public async Task<ResponseWrapper<byte[]>> GetExcelByteArrayByProgram(long? programId, UserType? userType = null)
        {
            return await _httpService.Get<ResponseWrapper<byte[]>>($"ENabizPortfolio/ExcelExportByProgramId?programId={programId}&userType={userType}");
        }
    }
}
