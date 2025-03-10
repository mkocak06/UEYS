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
    public class FormStandardService : BaseService, IFormStandardService
    {
        private readonly IMapper mapper;
        private readonly IFormStandardRepository formStandardRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FormStandardService(IMapper mapper, IUnitOfWork unitOfWork, IFormStandardRepository formStandardRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.formStandardRepository = formStandardRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseWrapper<FormStandardResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            FormStandard formStandard = await formStandardRepository.GetByIdAsync(cancellationToken, id);

            formStandardRepository.SoftDelete(formStandard);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<FormStandardResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            FormStandard formStandard = await formStandardRepository.GetIncluding(cancellationToken, x => x.Id == id && x.IsDeleted == false, x => x.Form, x => x.Standard);

            FormStandardResponseDTO response = mapper.Map<FormStandardResponseDTO>(formStandard);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<FormStandardResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<FormStandard> query = formStandardRepository.IncludingQueryable(x => x.IsDeleted == false);

            List<FormStandard> formStandards = await query.ToListAsync(cancellationToken);

            List<FormStandardResponseDTO> response = mapper.Map<List<FormStandardResponseDTO>>(formStandards);

            return new ResponseWrapper<List<FormStandardResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<FormStandardResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<FormStandard> ordersQuery = formStandardRepository.IncludingQueryable(x => true);
            FilterResponse<FormStandard> filterResponse = ordersQuery.ToFilterView(filter);

            var formStandards = mapper.Map<List<FormStandardResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<FormStandardResponseDTO>
            {
                Items = formStandards,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<FormStandardResponseDTO>> PostAsync(CancellationToken cancellationToken, FormStandardDTO formStandardDTO)
        {
            FormStandard formStandard = mapper.Map<FormStandard>(formStandardDTO);

            await formStandardRepository.AddAsync(cancellationToken, formStandard);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<FormStandardResponseDTO>(formStandard);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<FormStandardResponseDTO>> Put(CancellationToken cancellationToken, long id, FormStandardDTO formStandardDTO)
        {

            FormStandard formStandard = await formStandardRepository.GetByIdAsync(cancellationToken, id);

            FormStandard updatedFormStandard = mapper.Map(formStandardDTO, formStandard);

            formStandardRepository.Update(updatedFormStandard);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<FormStandardResponseDTO>(updatedFormStandard);

            return new() { Result = true, Item = response };
        }
    }
}
