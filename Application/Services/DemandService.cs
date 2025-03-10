using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.UnitOfWork;
using Infrastructure.Data;
using Shared.FilterModels.Base;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class DemandService : BaseService, IDemandService
    {
        private readonly IMapper _mapper;
        private readonly IDemandRepository _demandRepository;

        public DemandService(IUnitOfWork unitOfWork, IMapper mapper, IDemandRepository demandRepository) : base(unitOfWork)
        {
            _mapper = mapper;
            _demandRepository = demandRepository;
        }
        public async Task<PaginationModel<DemandResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Demand> ordersQuery = _demandRepository.Get(x => !x.IsDeleted);
            FilterResponse<Demand> filterResponse = ordersQuery.ToFilterView(filter);

            var demands = _mapper.Map<List<DemandResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<DemandResponseDTO>
            {
                Items = demands,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<ResponseWrapper<List<DemandResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Demand> demands = await _demandRepository.GetAsync(cancellationToken, x => !x.IsDeleted);

            List<DemandResponseDTO> response = _mapper.Map<List<DemandResponseDTO>>(demands);
            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DemandResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Demand demand = await _demandRepository.FirstOrDefaultAsync(cancellationToken, x => x.Id == id);

            DemandResponseDTO response = _mapper.Map<DemandResponseDTO>(demand);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DemandResponseDTO>> PostAsync(CancellationToken cancellationToken, DemandDTO demandDTO)
        {
            Demand demand = _mapper.Map<Demand>(demandDTO);

            await unitOfWork.DemandRepository.AddAsync(cancellationToken, demand);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<DemandResponseDTO>(demand);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DemandResponseDTO>> Put(CancellationToken cancellationToken, long id, DemandDTO demandDTO)
        {
            Demand demand = await _demandRepository.GetByIdAsync(cancellationToken, id);

            Demand updatedDemand = _mapper.Map(demandDTO, demand);

            unitOfWork.DemandRepository.Update(updatedDemand);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<DemandResponseDTO>(updatedDemand);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DemandResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Demand demand = await _demandRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.DemandRepository.SoftDelete(demand);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}