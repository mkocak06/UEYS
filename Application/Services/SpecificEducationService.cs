using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SpecificEducationService : BaseService, ISpecificEducationService
    {
        private readonly IMapper mapper;
        private readonly ISpecificEducationRepository specificEducationRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SpecificEducationService(IMapper mapper, IUnitOfWork unitOfWork, ISpecificEducationRepository specificEducationRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.specificEducationRepository = specificEducationRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseWrapper<SpecificEducationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            SpecificEducation specificEducation = await specificEducationRepository.GetByIdAsync(cancellationToken, id);
            specificEducationRepository.SoftDelete(specificEducation);
            await unitOfWork.CommitAsync(cancellationToken);
            return new() { Result = true };
        }

        public async Task<ResponseWrapper<SpecificEducationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            SpecificEducation specificEducation = await specificEducationRepository.GetByIdAsync(cancellationToken, id);

            SpecificEducationResponseDTO response = mapper.Map<SpecificEducationResponseDTO>(specificEducation);


            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<SpecificEducationResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<SpecificEducation> query = specificEducationRepository.IncludingQueryable(x => x.IsDeleted == false);

            List<SpecificEducation> specificEducations = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<SpecificEducationResponseDTO> response = mapper.Map<List<SpecificEducationResponseDTO>>(specificEducations);

            return new ResponseWrapper<List<SpecificEducationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<SpecificEducationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<SpecificEducation> ordersQuery = specificEducationRepository.IncludingQueryable(x => x.IsDeleted == false);
            FilterResponse<SpecificEducation> filterResponse = ordersQuery.ToFilterView(filter);
            var specificEducations = mapper.Map<List<SpecificEducationResponseDTO>>(await filterResponse.Query.ToListAsync());
            var response = new PaginationModel<SpecificEducationResponseDTO>
            {
                Items = specificEducations,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<SpecificEducationResponseDTO>> PostAsync(CancellationToken cancellationToken, SpecificEducationDTO specificEducationDTO)
        {
            SpecificEducation specificEducation = mapper.Map<SpecificEducation>(specificEducationDTO);

            await specificEducationRepository.AddAsync(cancellationToken, specificEducation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<SpecificEducationResponseDTO>(specificEducation);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<SpecificEducationResponseDTO>> Put(CancellationToken cancellationToken, long id, SpecificEducationDTO specificEducationDTO)
        {
            SpecificEducation specificEducation = await specificEducationRepository.GetByIdAsync(cancellationToken, id);
            SpecificEducation updatedSpecificEducation = mapper.Map(specificEducationDTO, specificEducation);
            specificEducationRepository.Update(updatedSpecificEducation);
            await unitOfWork.CommitAsync(cancellationToken);
            var response = mapper.Map<SpecificEducationResponseDTO>(updatedSpecificEducation);
            return new() { Result = true, Item = response };
        }
    }
}
