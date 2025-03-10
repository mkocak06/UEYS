using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Extentsions;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Types;

namespace Application.Services
{
    public class TitleService : BaseService, ITitleService
    {
        private readonly IMapper mapper;
        private readonly ITitleRepository titleRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TitleService(IMapper mapper, IUnitOfWork unitOfWork, ITitleRepository titleRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.titleRepository = titleRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaginationModel<TitleResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<TitleResponseDTO> ordersQuery = titleRepository.QueryableTitles();
            
            FilterResponse<TitleResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var titles = filterResponse.Query.ToList();

            var response = new PaginationModel<TitleResponseDTO>
            {
                Items = titles,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<ResponseWrapper<List<TitleResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Title> query = titleRepository.Queryable();

            List<Title> titles = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<TitleResponseDTO> response = mapper.Map<List<TitleResponseDTO>>(titles);

            return new ResponseWrapper<List<TitleResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<TitleResponseDTO>>> GetListByTypeAsync(CancellationToken cancellationToken, TitleType titleType)
        {
            IQueryable<Title> query = titleRepository.IncludingQueryable(x => x.TitleType == titleType);

            List<Title> titles = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<TitleResponseDTO> response = mapper.Map<List<TitleResponseDTO>>(titles);

            return new ResponseWrapper<List<TitleResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<TitleResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Title title = await titleRepository.GetByIdAsync(cancellationToken, id);

            TitleResponseDTO response = mapper.Map<TitleResponseDTO>(title);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<TitleResponseDTO>> PostAsync(CancellationToken cancellationToken, TitleDTO titleDTO)
        {
            Title title = mapper.Map<Title>(titleDTO);

            await titleRepository.AddAsync(cancellationToken, title);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<TitleResponseDTO>(title);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<TitleResponseDTO>> Put(CancellationToken cancellationToken, long id, TitleDTO titleDTO)
        {
            Title title = await titleRepository.GetByIdAsync(cancellationToken, id);

            Title updatedTitle = mapper.Map(titleDTO, title);

            titleRepository.Update(updatedTitle);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<TitleResponseDTO>(updatedTitle);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<TitleResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Title title = await titleRepository.GetByIdAsync(cancellationToken, id);

            titleRepository.SoftDelete(title);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}
