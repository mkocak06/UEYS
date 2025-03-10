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
    public class EthicCommitteeDecisionService : BaseService, IEthicCommitteeDecisionService
    {
        private readonly IMapper mapper;
        private readonly IEthicCommitteeDecisionRepository ethicCommitteeDecisionRepository;

        public EthicCommitteeDecisionService(IUnitOfWork unitOfWork, IMapper mapper, IEthicCommitteeDecisionRepository ethicCommitteeDecisionRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.ethicCommitteeDecisionRepository = ethicCommitteeDecisionRepository;
        }

        public async Task<ResponseWrapper<List<EthicCommitteeDecisionResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId)
        {
            List<EthicCommitteeDecision> ethicCommitteeDecisions = await ethicCommitteeDecisionRepository.GetAsync(cancellationToken, x => x.ThesisId == thesisId && x.IsDeleted == false);

            List<EthicCommitteeDecisionResponseDTO> response = mapper.Map<List<EthicCommitteeDecisionResponseDTO>>(ethicCommitteeDecisions);

            return new() { Result = true, Item = response };
        }

        public async Task<PaginationModel<EthicCommitteeDecisionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<EthicCommitteeDecision> ordersQuery = ethicCommitteeDecisionRepository.Queryable();
            FilterResponse<EthicCommitteeDecision> filterResponse = ordersQuery.ToFilterView(filter);

            List<EthicCommitteeDecisionResponseDTO> ethicCommitteeDecisions = mapper.Map<List<EthicCommitteeDecisionResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            return new PaginationModel<EthicCommitteeDecisionResponseDTO>()
            {
                Page = filter.page,
                PageSize = filter.pageSize,
                Items = ethicCommitteeDecisions,
                TotalItemCount = filterResponse.Count,
                TotalPages = filterResponse.PageNumber
            };
        }

        public async Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            EthicCommitteeDecision ethicCommitteeDecision = await ethicCommitteeDecisionRepository.GetByIdAsync(cancellationToken, id);

            EthicCommitteeDecisionResponseDTO response = mapper.Map<EthicCommitteeDecisionResponseDTO>(ethicCommitteeDecision);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> PostAsync(CancellationToken cancellationToken, EthicCommitteeDecisionDTO ethicCommitteeDecisionDTO)
        {
            EthicCommitteeDecision ethicCommitteeDecision = mapper.Map<EthicCommitteeDecision>(ethicCommitteeDecisionDTO);

            await ethicCommitteeDecisionRepository.AddAsync(cancellationToken, ethicCommitteeDecision);
            await unitOfWork.CommitAsync(cancellationToken);

            EthicCommitteeDecisionResponseDTO response = mapper.Map<EthicCommitteeDecisionResponseDTO>(ethicCommitteeDecision);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Put(CancellationToken cancellationToken, long id, EthicCommitteeDecisionDTO ethicCommitteeDecisionDTO)
        {
            EthicCommitteeDecision ethicCommitteeDecision = await ethicCommitteeDecisionRepository.GetIncluding(cancellationToken, x => x.Id == id);

            EthicCommitteeDecision updatedEthicCommitteeDecision = mapper.Map(ethicCommitteeDecisionDTO, ethicCommitteeDecision);

            ethicCommitteeDecisionRepository.Update(updatedEthicCommitteeDecision);
            await unitOfWork.CommitAsync(cancellationToken);

            EthicCommitteeDecisionResponseDTO response = mapper.Map<EthicCommitteeDecisionResponseDTO>(updatedEthicCommitteeDecision);
            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            EthicCommitteeDecision ethicCommitteeDecision = await ethicCommitteeDecisionRepository.GetByIdAsync(cancellationToken, id);

            ethicCommitteeDecisionRepository.SoftDelete(ethicCommitteeDecision);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}
