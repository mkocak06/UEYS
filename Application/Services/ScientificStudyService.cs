using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class ScientificStudyService : BaseService, IScientificStudyService
    {
        private readonly IMapper mapper;
        private readonly IScientificStudyRepository scientificStudyRepository;
        private readonly IDocumentRepository documentRepository;

        public ScientificStudyService(IMapper mapper, IUnitOfWork unitOfWork, IScientificStudyRepository scientificStudyRepository, IDocumentRepository documentRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.scientificStudyRepository = scientificStudyRepository;
            this.documentRepository = documentRepository;
        }
        public async Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            List<ScientificStudy> scientificStudies = await scientificStudyRepository.GetIncludingList(cancellationToken, x => x.StudentId == studentId && x.IsDeleted == false, x => x.Study);

            List<ScientificStudyResponseDTO> response = mapper.Map<List<ScientificStudyResponseDTO>>(scientificStudies);

            return new ResponseWrapper<List<ScientificStudyResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<ScientificStudyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<ScientificStudy> ordersQuery = scientificStudyRepository.IncludingQueryable(x => true, x => x.Study);
            FilterResponse<ScientificStudy> filterResponse = ordersQuery.ToFilterView(filter);

            var scientificStudies = mapper.Map<List<ScientificStudyResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            if (scientificStudies != null)
            {
                foreach (var item in scientificStudies)
                {
                    item.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, (long)item.Id, Shared.Types.DocumentTypes.ScientificStudy));
                }
            }

            var response = new PaginationModel<ScientificStudyResponseDTO>
            {
                Items = scientificStudies,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<ScientificStudyResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            ScientificStudy scientificStudy = await scientificStudyRepository.GetIncluding(cancellationToken, x => x.Id == id && x.IsDeleted, x => x.Study);

            ScientificStudyResponseDTO response = mapper.Map<ScientificStudyResponseDTO>(scientificStudy);

            return new ResponseWrapper<ScientificStudyResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ScientificStudyResponseDTO>> PostAsync(CancellationToken cancellationToken, ScientificStudyDTO scientificStudyDTO)
        {
            ScientificStudy scientificStudy = mapper.Map<ScientificStudy>(scientificStudyDTO);

            await scientificStudyRepository.AddAsync(cancellationToken, scientificStudy);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ScientificStudyResponseDTO>(scientificStudy);

            return new ResponseWrapper<ScientificStudyResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ScientificStudyResponseDTO>> Put(CancellationToken cancellationToken, long id, ScientificStudyDTO scientificStudyDTO)
        {
            ScientificStudy scientificStudy = await scientificStudyRepository.GetByIdAsync(cancellationToken, id);

            ScientificStudy updatedScientificStudy = mapper.Map(scientificStudyDTO, scientificStudy);

            scientificStudyRepository.Update(updatedScientificStudy);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ScientificStudyResponseDTO>(updatedScientificStudy);

            return new ResponseWrapper<ScientificStudyResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ScientificStudyResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            ScientificStudy scientificStudy = await scientificStudyRepository.GetByIdAsync(cancellationToken, id);

            scientificStudyRepository.SoftDelete(scientificStudy);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ScientificStudyResponseDTO> { Result = true };
        }
    }
}
