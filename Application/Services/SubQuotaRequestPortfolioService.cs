using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class SubQuotaRequestPortfolioService : BaseService, ISubQuotaRequestPortfolioService
    {

        private readonly IMapper mapper;
        private readonly ISubQuotaRequestPortfolioRepository subQuotaRequestPortfolioRepository;

        public SubQuotaRequestPortfolioService(IMapper mapper, IUnitOfWork unitOfWork, ISubQuotaRequestPortfolioRepository subQuotaRequestPortfolioRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.subQuotaRequestPortfolioRepository = subQuotaRequestPortfolioRepository;
        }

        public async Task<ResponseWrapper<SubQuotaRequestPortfolioResponseDTO>> PostAsync(CancellationToken cancellationToken, SubQuotaRequestPortfolioDTO subQuotaRequestPortfolioDTO)
        {
            SubQuotaRequestPortfolio subQuotaRequestPortfolio = mapper.Map<SubQuotaRequestPortfolio>(subQuotaRequestPortfolioDTO);

            await subQuotaRequestPortfolioRepository.AddAsync(cancellationToken, subQuotaRequestPortfolio);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<SubQuotaRequestPortfolioResponseDTO>(subQuotaRequestPortfolio);

            return new ResponseWrapper<SubQuotaRequestPortfolioResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<SubQuotaRequestPortfolioResponseDTO>> Put(CancellationToken cancellationToken, long id, SubQuotaRequestPortfolioDTO subQuotaRequestPortfolioDTO)
        {
            var subQuotaRequestPortfolio = await subQuotaRequestPortfolioRepository.GetByIdAsync(cancellationToken, id);

            var updatedSubQuotaRequestPortfolio = mapper.Map(subQuotaRequestPortfolioDTO, subQuotaRequestPortfolio);

            subQuotaRequestPortfolioRepository.Update(updatedSubQuotaRequestPortfolio);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<SubQuotaRequestPortfolioResponseDTO>(updatedSubQuotaRequestPortfolio);

            return new ResponseWrapper<SubQuotaRequestPortfolioResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<SubQuotaRequestPortfolioResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            unitOfWork.BeginTransaction();
            SubQuotaRequestPortfolio subQuotaRequestPortfolio = await subQuotaRequestPortfolioRepository.GetByIdAsync(cancellationToken, id);

            subQuotaRequestPortfolioRepository.HardDelete(subQuotaRequestPortfolio);
            await unitOfWork.EndTransactionAsync(cancellationToken);

            return new ResponseWrapper<SubQuotaRequestPortfolioResponseDTO> { Result = true };
        }
    }
}
