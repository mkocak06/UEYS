using AutoMapper;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
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
using Core.Entities;
using Application.Interfaces;
using Application.Services.Base;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace Application.Services
{
    public class SpecificEducationPlaceService : BaseService, ISpecificEducationPlaceService
    {
        private readonly IMapper mapper;
        private readonly ISpecificEducationPlaceRepository specificEducationPlaceRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SpecificEducationPlaceService(IMapper mapper, IUnitOfWork unitOfWork, ISpecificEducationPlaceRepository specificEducationPlaceRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.specificEducationPlaceRepository = specificEducationPlaceRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            SpecificEducationPlace specificEducationPlace = await specificEducationPlaceRepository.GetByIdAsync(cancellationToken, id);
            specificEducationPlaceRepository.SoftDelete(specificEducationPlace);
            await unitOfWork.CommitAsync(cancellationToken);
            return new() { Result = true };
        }

        public async Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            SpecificEducationPlace specificEducationPlace = await specificEducationPlaceRepository.GetIncluding(cancellationToken, x=>x.Id == id, x=> x.Province);
            SpecificEducationPlaceResponseDTO response = mapper.Map<SpecificEducationPlaceResponseDTO>(specificEducationPlace);
         

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<SpecificEducationPlaceResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<SpecificEducationPlace> query = specificEducationPlaceRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.Province);

            List<SpecificEducationPlace> specificEducationPlace = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<SpecificEducationPlaceResponseDTO> response = mapper.Map<List<SpecificEducationPlaceResponseDTO>>(specificEducationPlace);

            return new ResponseWrapper<List<SpecificEducationPlaceResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<SpecificEducationPlaceResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<SpecificEducationPlace> ordersQuery = specificEducationPlaceRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.Province);
            FilterResponse<SpecificEducationPlace> filterResponse = ordersQuery.ToFilterView(filter);
            var specificEducationPlaces = mapper.Map<List<SpecificEducationPlaceResponseDTO>>(await filterResponse.Query.ToListAsync());
            var response = new PaginationModel<SpecificEducationPlaceResponseDTO>
            {
                Items = specificEducationPlaces,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> PostAsync(CancellationToken cancellationToken, SpecificEducationPlaceDTO specificEducationPlaceDTO)
        {
            SpecificEducationPlace specificEducationPlace = mapper.Map<SpecificEducationPlace>(specificEducationPlaceDTO);

            await specificEducationPlaceRepository.AddAsync(cancellationToken, specificEducationPlace);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<SpecificEducationPlaceResponseDTO>(specificEducationPlace);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Put(CancellationToken cancellationToken, long id, SpecificEducationPlaceDTO specificEducationPlaceDTO)
        {
            SpecificEducationPlace specificEducationPlace = await specificEducationPlaceRepository.GetByIdAsync(cancellationToken, id);
            SpecificEducationPlace updatedSpecificEducationPlace = mapper.Map(specificEducationPlaceDTO, specificEducationPlace);
            specificEducationPlaceRepository.Update(updatedSpecificEducationPlace);
            await unitOfWork.CommitAsync(cancellationToken);
            var response = mapper.Map<SpecificEducationPlaceResponseDTO>(updatedSpecificEducationPlace);
            return new() { Result = true, Item = response };
        }
    }
}

