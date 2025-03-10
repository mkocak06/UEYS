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
    public class OfficialLetterService : BaseService, IOfficialLetterService
    {
        private readonly IMapper mapper;
        private readonly IOfficialLetterRepository officialLetterRepository;

        public OfficialLetterService(IMapper mapper, IUnitOfWork unitOfWork, IOfficialLetterRepository officialLetterRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.officialLetterRepository = officialLetterRepository;
        }

        public async Task<ResponseWrapper<List<OfficialLetterResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId)
        {
            IQueryable<OfficialLetter> query = officialLetterRepository.IncludingQueryable(x => x.ThesisId == thesisId && x.IsDeleted == false);

            List<OfficialLetter> officialLetters = await query.OrderBy(x => x.Description).ToListAsync(cancellationToken);

            List<OfficialLetterResponseDTO> response = mapper.Map<List<OfficialLetterResponseDTO>>(officialLetters);

            return new() { Result = true, Item = response };
        }

        public async Task<PaginationModel<OfficialLetterResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<OfficialLetter> ordersQuery = officialLetterRepository.IncludingQueryable(x => true);
            FilterResponse<OfficialLetter> filterResponse = ordersQuery.ToFilterView(filter);

            List<OfficialLetterResponseDTO> officialLetters = mapper.Map<List<OfficialLetterResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            return new PaginationModel<OfficialLetterResponseDTO>()
            {
                Items = officialLetters,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
        }

        public async Task<ResponseWrapper<OfficialLetterResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            OfficialLetter officialLetter = await officialLetterRepository.GetIncluding(cancellationToken, x => x.Id == id == x.IsDeleted == false, x => x.Thesis);
            OfficialLetterResponseDTO response = mapper.Map<OfficialLetterResponseDTO>(officialLetter);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<OfficialLetterResponseDTO>> PostAsync(CancellationToken cancellationToken, OfficialLetterDTO officialLetterDTO)
        {
            OfficialLetter officialLetter = mapper.Map<OfficialLetter>(officialLetterDTO);

            await officialLetterRepository.AddAsync(cancellationToken, officialLetter);
            await unitOfWork.CommitAsync(cancellationToken);

            OfficialLetterResponseDTO response = mapper.Map<OfficialLetterResponseDTO>(officialLetter);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<OfficialLetterResponseDTO>> Put(CancellationToken cancellationToken, long id, OfficialLetterDTO officialLetterDTO)
        {
            OfficialLetter officialLetter = await officialLetterRepository.GetIncluding(cancellationToken, x => x.Id == id);

            OfficialLetter updatedOfficialLetter = mapper.Map(officialLetterDTO, officialLetter);

            officialLetterRepository.Update(updatedOfficialLetter);
            await unitOfWork.CommitAsync(cancellationToken);

            OfficialLetterResponseDTO response = mapper.Map<OfficialLetterResponseDTO>(updatedOfficialLetter);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<OfficialLetterResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            OfficialLetter officialLetter = await officialLetterRepository.GetByIdAsync(cancellationToken, id);

            officialLetterRepository.SoftDelete(officialLetter);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}
