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
    public class StudyService : BaseService, IStudyService
    {
        private readonly IMapper mapper;
        private readonly IStudyRepository studyRepository;

        public StudyService(IMapper mapper, IUnitOfWork unitOfWork, IStudyRepository studyRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studyRepository = studyRepository;
        }
        public async Task<ResponseWrapper<List<StudyResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Study> authorizationCategories = await studyRepository.ListAsync(cancellationToken);

            List<StudyResponseDTO> response = mapper.Map<List<StudyResponseDTO>>(authorizationCategories);

            return new ResponseWrapper<List<StudyResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudyResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Study study = await studyRepository.GetByIdAsync(cancellationToken, id);

            StudyResponseDTO response = mapper.Map<StudyResponseDTO>(study);

            return new ResponseWrapper<StudyResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudyResponseDTO>> PostAsync(CancellationToken cancellationToken, StudyDTO studyDTO)
        {
            Study study = mapper.Map<Study>(studyDTO);

            await studyRepository.AddAsync(cancellationToken, study);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudyResponseDTO>(study);

            return new ResponseWrapper<StudyResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudyResponseDTO>> Put(CancellationToken cancellationToken, long id, StudyDTO studyDTO)
        {
            Study study = await studyRepository.GetByIdAsync(cancellationToken, id);

            Study updatedStudy = mapper.Map(studyDTO, study);

            studyRepository.Update(updatedStudy);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudyResponseDTO>(updatedStudy);

            return new ResponseWrapper<StudyResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudyResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Study study = await studyRepository.GetByIdAsync(cancellationToken, id);

            studyRepository.SoftDelete(study);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudyResponseDTO> { Result = true };
        }

        public async Task<PaginationModel<StudyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Study> ordersQuery = studyRepository.IncludingQueryable(x => true);
            FilterResponse<Study> filterResponse = ordersQuery.ToFilterView(filter);

            var authorizationCategories = mapper.Map<List<StudyResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));



            var response = new PaginationModel<StudyResponseDTO>
            {
                Items = authorizationCategories,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
    }
}
