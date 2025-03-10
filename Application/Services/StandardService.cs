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
using Shared.ResponseModels.Standard;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class StandardService : BaseService, IStandardService
    {
        private readonly IMapper mapper;
        private readonly IStandardRepository standardRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public StandardService(IMapper mapper, IUnitOfWork unitOfWork, IStandardRepository standardRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.standardRepository = standardRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseWrapper<StandardResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Standard standard = await standardRepository.GetByIdAsync(cancellationToken, id);

            standardRepository.SoftDelete(standard);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public Task<ResponseWrapper<bool>> ImportExcel(CancellationToken cancellationToken, IFormFile formFile)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<bool>> ImportExcelGenelStandars(CancellationToken cancellationToken, IFormFile formFile)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseWrapper<StandardResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Standard standard = await standardRepository.GetIncluding(cancellationToken, x => x.Id == id && x.IsDeleted == false, x => x.StandardCategory);

            StandardResponseDTO response = mapper.Map<StandardResponseDTO>(standard);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<StandardResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Standard> query = standardRepository.IncludingQueryable(x => x.IsDeleted == false);

            List<Standard> standards = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<StandardResponseDTO> response = mapper.Map<List<StandardResponseDTO>>(standards);

            return new ResponseWrapper<List<StandardResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<StandardResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Standard> ordersQuery = standardRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.Curriculum.ExpertiseBranch, x => x.StandardCategory);
            FilterResponse<Standard> filterResponse = ordersQuery.ToFilterView(filter);

            var standards = await filterResponse.Query.OrderBy(x => x.StandardCategory.CategoryCode).ThenBy(x => x.StandardCategory.Name).ThenBy(x => x.Name).ToListAsync(cancellationToken);

            var response = mapper.Map<List<StandardResponseDTO>>(standards);

            return new PaginationModel<StandardResponseDTO>
            {
                Items = response,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
        }
        public async Task<List<ProgramStandardResponseDTO>> GetPaginateListByLatestCurriculumsExpertiseBranch(CancellationToken cancellationToken, long expBranchId)
        {
            return await standardRepository.GetByLatestCurriculumsExpertiseBranch(expBranchId);
        }
        public async Task<ResponseWrapper<StandardResponseDTO>> PostAsync(CancellationToken cancellationToken, StandardDTO standardDTO)
        {
            Standard standard = mapper.Map<Standard>(standardDTO);

            await standardRepository.AddAsync(cancellationToken, standard);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StandardResponseDTO>(standard);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StandardResponseDTO>> Put(CancellationToken cancellationToken, long id, StandardDTO standardDTO)
        {

            Standard standard = await standardRepository.GetByIdAsync(cancellationToken, id);

            Standard updatedStandard = mapper.Map(standardDTO, standard);

            standardRepository.Update(updatedStandard);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StandardResponseDTO>(updatedStandard);

            return new() { Result = true, Item = response };
        }
    }
}
