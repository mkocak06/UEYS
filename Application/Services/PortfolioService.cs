using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class PortfolioService : BaseService, IPortfolioService
    {
        private readonly IMapper mapper;
        private readonly IPortfolioRepository portfolioRepository;

        public PortfolioService(IMapper mapper, IUnitOfWork unitOfWork, IPortfolioRepository portfolioRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<PaginationModel<PortfolioResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Portfolio> ordersQuery = portfolioRepository.IncludingQueryable(x => true, x => x.ExpertiseBranch);
            FilterResponse<Portfolio> filterResponse = ordersQuery.ToFilterView(filter);

            var portfolios = mapper.Map<List<PortfolioResponseDTO>>(await filterResponse.Query.OrderBy(x => x.ExpertiseBranch.Name).ToListAsync());

            var response = new PaginationModel<PortfolioResponseDTO>
            {
                Items = portfolios,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<PortfolioResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Portfolio portfolio = await portfolioRepository.GetIncluding(cancellationToken, x => x.IsDeleted == false && x.Id == id, x=>x.ExpertiseBranch);

            return new ResponseWrapper<PortfolioResponseDTO> { Result = true, Item = mapper.Map<PortfolioResponseDTO>(portfolio) };
        }

        public async Task<ResponseWrapper<PortfolioResponseDTO>> PostAsync(CancellationToken cancellationToken, PortfolioDTO portfolioDTO)
        {
            Portfolio portfolio = mapper.Map<Portfolio>(portfolioDTO);

            await portfolioRepository.AddAsync(cancellationToken, portfolio);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<PortfolioResponseDTO> { Result = true, Item = mapper.Map<PortfolioResponseDTO>(portfolio) };
        }

        public async Task<ResponseWrapper<PortfolioResponseDTO>> Put(CancellationToken cancellationToken, long id, PortfolioDTO portfolioDTO)
        {
            Portfolio portfolio = await portfolioRepository.GetByIdAsync(cancellationToken, id);
            var updatedPortfolio = mapper.Map(portfolioDTO, portfolio);

            portfolioRepository.Update(updatedPortfolio);

            var response = mapper.Map<PortfolioResponseDTO>(portfolio);

            return new ResponseWrapper<PortfolioResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PortfolioResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Portfolio portfolio = await portfolioRepository.GetByIdAsync(cancellationToken, id);

            portfolioRepository.SoftDelete(portfolio);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<PortfolioResponseDTO> { Result = true };
        }
    }
}
