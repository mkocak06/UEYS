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
    public class EducatorCountContributionFormulaService : BaseService, IEducatorCountContributionFormulaService
    {
        private readonly IMapper mapper;
        private readonly IEducatorCountContributionFormulaRepository quotaRequestRepository;

        public EducatorCountContributionFormulaService(IMapper mapper, IUnitOfWork unitOfWork, IEducatorCountContributionFormulaRepository quotaRequestRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.quotaRequestRepository = quotaRequestRepository;
        }

        public async Task<PaginationModel<EducatorCountContributionFormulaResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<EducatorCountContributionFormula> ordersQuery = quotaRequestRepository.Queryable();
            FilterResponse<EducatorCountContributionFormula> filterResponse = ordersQuery.ToFilterView(filter);

            var quotaRequests = mapper.Map<List<EducatorCountContributionFormulaResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<EducatorCountContributionFormulaResponseDTO>
            {
                Items = quotaRequests,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            EducatorCountContributionFormula quotaRequest = await quotaRequestRepository.GetByIdAsync(cancellationToken, id);

            return new ResponseWrapper<EducatorCountContributionFormulaResponseDTO> { Result = true, Item = mapper.Map<EducatorCountContributionFormulaResponseDTO>(quotaRequest) };
        }

        public async Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorCountContributionFormulaDTO quotaRequestDTO)
        {
            EducatorCountContributionFormula quotaRequest = mapper.Map<EducatorCountContributionFormula>(quotaRequestDTO);

            await quotaRequestRepository.AddAsync(cancellationToken, quotaRequest);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<EducatorCountContributionFormulaResponseDTO> { Result = true, Item = mapper.Map<EducatorCountContributionFormulaResponseDTO>(quotaRequest) };
        }

        public async Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorCountContributionFormulaDTO quotaRequestDTO)
        {
            EducatorCountContributionFormula quotaRequest = await quotaRequestRepository.GetByIdAsync(cancellationToken, id);
            var updatedEducatorCountContributionFormula = mapper.Map(quotaRequestDTO, quotaRequest);

            quotaRequestRepository.Update(updatedEducatorCountContributionFormula);

            var response = mapper.Map<EducatorCountContributionFormulaResponseDTO>(quotaRequest);

            return new ResponseWrapper<EducatorCountContributionFormulaResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            EducatorCountContributionFormula quotaRequest = await quotaRequestRepository.GetByIdAsync(cancellationToken, id);

            quotaRequestRepository.SoftDelete(quotaRequest);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<EducatorCountContributionFormulaResponseDTO> { Result = true };
        }
    }
}
