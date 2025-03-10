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
    public class ThesisService : BaseService, IThesisService
    {
        private readonly IMapper mapper;
        private readonly IThesisRepository thesisRepository;
        private readonly IThesisDefenceRepository thesisDefenceRepository;
        private readonly IThesisDefenceService thesisDefenceService;

        public ThesisService(IMapper mapper, IUnitOfWork unitOfWork, IThesisRepository thesisRepository, IThesisDefenceRepository thesisDefenceRepository, IThesisDefenceService thesisDefenceService) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.thesisRepository = thesisRepository;
            this.thesisDefenceRepository = thesisDefenceRepository;
            this.thesisDefenceService = thesisDefenceService;
        }
        public async Task<PaginationModel<ThesisResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<ThesisResponseDTO> ordersQuery = thesisRepository.QueryableThesis();
            FilterResponse<ThesisResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var theses = await filterResponse.Query.ToListAsync();

            var response = new PaginationModel<ThesisResponseDTO>
            {
                Items = theses,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<ResponseWrapper<ThesisResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Thesis thesis = await thesisRepository.GetWithSubRecords(cancellationToken, id);

            ThesisResponseDTO response = mapper.Map<ThesisResponseDTO>(thesis);

            response.DeletedAdvisorTheses ??= new List<AdvisorThesisResponseDTO>();
            response.DeletedAdvisorTheses = response.AdvisorTheses.Where(at => at.IsDeleted).ToList();
            response.AdvisorTheses = response.AdvisorTheses.Where(at => !at.IsDeleted).ToList();

            return new ResponseWrapper<ThesisResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<ThesisResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            List<Thesis> theses = await thesisRepository.GetIncludingList(cancellationToken, x => x.StudentId == studentId && x.IsDeleted == false, x => x.AdvisorTheses);

            List<ThesisResponseDTO> response = mapper.Map<List<ThesisResponseDTO>>(theses);

            return new ResponseWrapper<List<ThesisResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<ThesisResponseDTO>>> GetListForEreportByStudentId(CancellationToken cancellationToken, long studentId)
        {
            List<Thesis> theses = await thesisRepository.GetListWithSubRecords(cancellationToken, studentId);

            List<ThesisResponseDTO> response = mapper.Map<List<ThesisResponseDTO>>(theses);

            return new ResponseWrapper<List<ThesisResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ThesisResponseDTO>> PostAsync(CancellationToken cancellationToken, ThesisDTO thesisDTO)
        {
            Thesis thesis = mapper.Map<Thesis>(thesisDTO);

            await thesisRepository.AddAsync(cancellationToken, thesis);
            await unitOfWork.CommitAsync(cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);
            var response = mapper.Map<ThesisResponseDTO>(thesis);
            return new ResponseWrapper<ThesisResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ThesisResponseDTO>> Put(CancellationToken cancellationToken, long id, ThesisDTO thesisDTO)
        {
            var thesis = await thesisRepository.GetByIdAsync(cancellationToken, id);
            Thesis updatedThesis = mapper.Map(thesisDTO, thesis);

            if (thesisDTO.SubjectDetermineDate > thesisDTO.ThesisTitleDetermineDate)
                return new() { Result = false, Message = "Tez başlığı belirleme tarihi tez konusu belirleme tarihinden küçük olamaz!" }; 

            if (updatedThesis.IsDeleted)
            {
                var thesisDefences = await thesisDefenceRepository.Queryable().OrderByDescending(x => x.DefenceOrder).Where(x => !x.IsDeleted && x.ThesisId == id).ToListAsync(cancellationToken);

                await unitOfWork.BeginTransactionAsync();

                foreach (var item in thesisDefences)
                {
                    var result = await thesisDefenceService.Delete(cancellationToken, item.Id, thesis.StudentId ?? 0);
                    if (!result.Result)
                        return new() { Result = false, Message = result.Message };
                }
            }

            thesisRepository.Update(updatedThesis);
            await unitOfWork.EndTransactionAsync(cancellationToken);

            var response = mapper.Map<ThesisResponseDTO>(updatedThesis);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ThesisResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Thesis thesis = await thesisRepository.GetByIdAsync(cancellationToken, id);

            thesisRepository.SoftDelete(thesis);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<ThesisResponseDTO> { Result = true };
        }
    }
}
