using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class StandardCategoryService : BaseService , IStandardCategoryService
    {
        private readonly IMapper mapper;
        private readonly IStandardCategoryRepository standardCategoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public StandardCategoryService(IMapper mapper, IUnitOfWork unitOfWork, IStandardCategoryRepository standardCategoryRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.standardCategoryRepository = standardCategoryRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseWrapper<StandardCategoryResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            StandardCategory category = await standardCategoryRepository.GetByIdAsync(cancellationToken, id);

            standardCategoryRepository.SoftDelete(category);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<StandardCategoryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            StandardCategory category = await standardCategoryRepository.GetByAsync(cancellationToken, x => x.Id == id && x.IsDeleted == false);

            StandardCategoryResponseDTO response = mapper.Map<StandardCategoryResponseDTO>(category);

            return new() { Result = true, Item = response };
        }

        public  async Task<ResponseWrapper<List<StandardCategoryResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<StandardCategory> query = standardCategoryRepository.IncludingQueryable(x => x.IsDeleted ==false);

            List<StandardCategory> categories = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<StandardCategoryResponseDTO> response = mapper.Map<List<StandardCategoryResponseDTO>>(categories);

            return new ResponseWrapper<List<StandardCategoryResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<StandardCategoryResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<StandardCategory> ordersQuery = standardCategoryRepository.IncludingQueryable(x => true);
            FilterResponse<StandardCategory> filterResponse = ordersQuery.ToFilterView(filter);

            var categories = mapper.Map<List<StandardCategoryResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<StandardCategoryResponseDTO>
            {
                Items = categories,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public  async Task<ResponseWrapper<StandardCategoryResponseDTO>> PostAsync(CancellationToken cancellationToken, StandardCategoryDTO categoryDTO)
        {
            StandardCategory category = mapper.Map<StandardCategory>(categoryDTO);

            await standardCategoryRepository.AddAsync(cancellationToken, category);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StandardCategoryResponseDTO>(category);

            return new() { Result = true, Item = response };
        }

        public  async Task<ResponseWrapper<StandardCategoryResponseDTO>> Put(CancellationToken cancellationToken, long id, StandardCategoryDTO categoryDTO)
        {
            StandardCategory category = await standardCategoryRepository.GetByIdAsync(cancellationToken, id);

            StandardCategory updatedStandardCategory = mapper.Map(categoryDTO, category);

            standardCategoryRepository.Update(updatedStandardCategory);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StandardCategoryResponseDTO>(updatedStandardCategory);

            return new() { Result = true, Item = response };
        }
    }
}
