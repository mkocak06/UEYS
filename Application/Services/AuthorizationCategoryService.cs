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
    public class AuthorizationCategoryService : BaseService, IAuthorizationCategoryService
    {
        private readonly IMapper mapper;
        private readonly IAuthorizationCategoryRepository authorizationCategoryRepository;

        public AuthorizationCategoryService(IMapper mapper, IUnitOfWork unitOfWork, IAuthorizationCategoryRepository authorizationCategoryRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.authorizationCategoryRepository = authorizationCategoryRepository;
        }
        public async Task<ResponseWrapper<List<AuthorizationCategoryResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<AuthorizationCategory> authorizationCategories = await authorizationCategoryRepository.ListAsync(cancellationToken);

            List<AuthorizationCategoryResponseDTO> response = mapper.Map<List<AuthorizationCategoryResponseDTO>>(authorizationCategories);

            return new ResponseWrapper<List<AuthorizationCategoryResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            AuthorizationCategory authorizationCategory = await authorizationCategoryRepository.GetByIdAsync(cancellationToken, id);

            AuthorizationCategoryResponseDTO response = mapper.Map<AuthorizationCategoryResponseDTO>(authorizationCategory);

            return new ResponseWrapper<AuthorizationCategoryResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> PostAsync(CancellationToken cancellationToken, AuthorizationCategoryDTO authorizationCategoryDTO)
        {
            AuthorizationCategory authorizationCategory = mapper.Map<AuthorizationCategory>(authorizationCategoryDTO);

            //authorizationCategory.Order = authorizationCategoryRepository.GetOrderNumber();

            await authorizationCategoryRepository.AddAsync(cancellationToken, authorizationCategory);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<AuthorizationCategoryResponseDTO>(authorizationCategory);

            return new ResponseWrapper<AuthorizationCategoryResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Put(CancellationToken cancellationToken, long id, AuthorizationCategoryDTO authorizationCategoryDTO)
        {
            AuthorizationCategory authorizationCategory = await authorizationCategoryRepository.GetByIdAsync(cancellationToken, id);

            AuthorizationCategory updatedAuthorizationCategory = mapper.Map(authorizationCategoryDTO, authorizationCategory);

            authorizationCategoryRepository.Update(updatedAuthorizationCategory);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<AuthorizationCategoryResponseDTO>(updatedAuthorizationCategory);

            return new ResponseWrapper<AuthorizationCategoryResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            AuthorizationCategory authorizationCategory = await authorizationCategoryRepository.GetByIdAsync(cancellationToken, id);

            authorizationCategoryRepository.SoftDelete(authorizationCategory);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<AuthorizationCategoryResponseDTO> { Result = true };
        }

        public async Task<PaginationModel<AuthorizationCategoryResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<AuthorizationCategory> ordersQuery = authorizationCategoryRepository.Queryable();
            FilterResponse<AuthorizationCategory> filterResponse = ordersQuery.ToFilterView(filter);

            var authorizationCategories = mapper.Map<List<AuthorizationCategoryResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<AuthorizationCategoryResponseDTO>
            {
                Items = authorizationCategories,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        //public async Task<ResponseWrapper<bool>> ChangeOrder(CancellationToken cancellationToken, long id, bool isToUp)
        //{
        //    await authorizationCategoryRepository.ChangeOrder(cancellationToken, id, isToUp);
        //    await unitOfWork.CommitAsync(cancellationToken);

        //    var response = new ResponseWrapper<bool>
        //    {
        //        Result = true
        //    };

        //    return response;
        //}
    }
}
