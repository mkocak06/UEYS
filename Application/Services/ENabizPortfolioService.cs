using Application.Interfaces;
using Application.Reports.ExcelReports.AsistanHekimENabiz;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class ENabizPortfolioService : BaseService, IENabizPortfolioService
    {
        private readonly IMapper mapper;
        private readonly IENabizPortfolioRepository enabizPortfolioRepository;

        public ENabizPortfolioService(IMapper mapper, IUnitOfWork unitOfWork, IENabizPortfolioRepository enabizPortfolioRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.enabizPortfolioRepository = enabizPortfolioRepository;
        }

        public async Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetTotalOperationsByUserIdAsync(CancellationToken cancellationToken, long userId)
        {
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var asistanHekimIslemleri = await enabizPortfolioRepository.GetTotalOperationsByIdentityNo(cancellationToken, user.IdentityNo, null, null);

            ENabizPortfolioResponseDTO response = mapper.Map<ENabizPortfolioResponseDTO>(asistanHekimIslemleri);

            return new ResponseWrapper<ENabizPortfolioResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ENabizPortfolioResponseDTO>> GetTotalOperationsByProgramIdAsync(CancellationToken cancellationToken, long programId, UserType? userType)
        {
            var asistanHekimIslemleri = await enabizPortfolioRepository.GetTotalOperationsByProgramId(cancellationToken, programId, userType, null, null);

            ENabizPortfolioResponseDTO response = mapper.Map<ENabizPortfolioResponseDTO>(asistanHekimIslemleri);

            return new ResponseWrapper<ENabizPortfolioResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ENabizPortfolioResponseDTO>>> GetAsync(CancellationToken cancellationToken)
        {
            var islemler = await enabizPortfolioRepository.GetListByAsync(cancellationToken, x => true);

            List<ENabizPortfolioResponseDTO> response = mapper.Map<List<ENabizPortfolioResponseDTO>>(islemler);

            return new ResponseWrapper<List<ENabizPortfolioResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetUserOperationsChartData(CancellationToken cancellationToken, long userId)
        {
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);

            var response = await enabizPortfolioRepository.OperationsChart(cancellationToken, user.IdentityNo, null, null);

            return new ResponseWrapper<List<ENabizPortfolioChartModel>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ENabizPortfolioChartModel>>> GetUserOperationsChartDataByProgramId(CancellationToken cancellationToken, long programId, UserType? userType)
        {
            var response = await enabizPortfolioRepository.OperationsChartByProgramId(cancellationToken, programId, userType, null, null);

            return new ResponseWrapper<List<ENabizPortfolioChartModel>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, long userId)
        {
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);

            var response = await enabizPortfolioRepository.GetListByAsync(cancellationToken, x => x.hekim_kimlik_numarasi == user.IdentityNo);

            var byteArray = ExportList.Report(response);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExportByProgramId(CancellationToken cancellationToken, long programId, UserType? userType)
        {
            var response = await enabizPortfolioRepository.GetListByProgramIdAsync(cancellationToken, programId, userType);

            var byteArray = ExportList.Report(response);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }
    }
}
