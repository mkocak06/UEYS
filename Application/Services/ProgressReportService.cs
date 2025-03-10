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
using Shared.Types;

namespace Application.Services
{
    public class ProgressReportService : BaseService, IProgressReportService
    {
        private readonly IMapper mapper;
        private readonly IProgressReportRepository progressReportRepository;
        private readonly IAdvisorThesisRepository advisorThesisRepository;
        private readonly IEducationTrackingRepository educationTrackingRepository;
        private readonly IThesisDefenceRepository thesisDefenceRepository;
        private readonly IThesisRepository thesisRepository;

        public ProgressReportService(IMapper mapper, IUnitOfWork unitOfWork, IProgressReportRepository progressReportRepository, IAdvisorThesisRepository advisorThesisRepository, IEducationTrackingRepository educationTrackingRepository, IThesisDefenceRepository thesisDefenceRepository, IThesisRepository thesisRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.progressReportRepository = progressReportRepository;
            this.advisorThesisRepository = advisorThesisRepository;
            this.educationTrackingRepository = educationTrackingRepository;
            this.thesisDefenceRepository = thesisDefenceRepository;
            this.thesisRepository = thesisRepository;
        }

        public async Task<PaginationModel<ProgressReportResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<ProgressReport> ordersQuery = progressReportRepository.IncludingQueryable(x => true);
            FilterResponse<ProgressReport> filterResponse = ordersQuery.ToFilterView(filter);

            List<ProgressReportResponseDTO> progressReports = mapper.Map<List<ProgressReportResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            return new PaginationModel<ProgressReportResponseDTO>()
            {
                Items = progressReports,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
        }

        public async Task<ResponseWrapper<ProgressReportResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            ProgressReport progressReport = await progressReportRepository.GetIncluding(cancellationToken, x => x.Id == id == x.IsDeleted == false, x => x.Thesis, x => x.Educator);
            ProgressReportResponseDTO response = mapper.Map<ProgressReportResponseDTO>(progressReport);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgressReportResponseDTO>> CalculateStartEndDates(CancellationToken cancellationToken, long thesisId, long studentId)
        {
            var thesis = await thesisRepository.GetByIdAsync(cancellationToken, thesisId);
            if (thesis.SubjectDetermineDate == null)
                return new() { Result = false, Message = "You have to add thesis subject determine date to add progress report!" };

            var coordinatorAdvisor = await advisorThesisRepository.GetByAsync(cancellationToken, x => x.IsCoordinator == true && x.ThesisId == thesisId && !x.IsDeleted);
            if (coordinatorAdvisor == null)
                return new() { Result = false, Message = "There is no coordinator advisor in this thesis. You have to add coordinator advisor to add progress report!" };

            var successfulThesisDefence = await thesisDefenceRepository.AnyAsync(cancellationToken, x => x.ThesisId == thesisId && !x.IsDeleted && (x.Result == DefenceResultType.Successful || x.Result == DefenceResultType.SuccessfulWithRevision));

            if (successfulThesisDefence)
                return new() { Result = false, Message = "You cannot add a thesis progress report because you have a successful thesis defense!" };

            var response = new ProgressReportResponseDTO();

            var lastProgressReport = await progressReportRepository.Queryable().OrderByDescending(x => x.BeginDate).FirstOrDefaultAsync(x => x.IsDeleted == false && x.ThesisId == thesisId, cancellationToken);

            if (lastProgressReport != null)
                response.BeginDate = lastProgressReport.EndDate?.AddDays(1);
            else
                response.BeginDate = thesis.SubjectDetermineDate;

            response.EndDate = response.BeginDate?.AddMonths(3);

            var timeIncreasings = await educationTrackingRepository.Queryable().OrderBy(x => x.ProcessDate).Where(x => x.IsDeleted == false && x.StudentId == studentId && x.ProcessType == ProcessType.TimeIncreasing && x.ProcessDate >= response.BeginDate && x.ReasonType != ReasonType.ExtensionByApplicationForSubjection && x.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation && x.ReasonType != ReasonType.ForceMajeure && x.ReasonType != ReasonType.DueToUnusualCircumstances).ToListAsync(cancellationToken);

            var timeWithoutEducation = await educationTrackingRepository.Queryable().OrderBy(x => x.ProcessDate).Where(x => !x.IsDeleted && x.StudentId == studentId && x.ProcessDate >= response.BeginDate && x.ReasonType != ReasonType.BranchChange && (x.ProcessType == ProcessType.Transfer || x.ProcessType == ProcessType.Assignment || x.ProcessType == ProcessType.Start) && (x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment || x.ReasonType == ReasonType.BeginningToAssignedInstitution || x.ReasonType == ReasonType.CompletionOfAssignment || x.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment || x.ReasonType == ReasonType.Other || x.ReasonType == ReasonType.ExcusedTransfer || x.ReasonType == ReasonType.UnexcusedTransfer)).ToListAsync(cancellationToken);

            for (int i = 1; i < timeWithoutEducation.Count; i++)
                if (i % 2 == 1)
                    timeIncreasings.Add(new() { ProcessDate = timeWithoutEducation[i].ProcessDate, AdditionalDays = (timeWithoutEducation[i].ProcessDate?.Date - timeWithoutEducation[i - 1].ProcessDate?.Date)?.Days ?? 0 });

            if (timeIncreasings != null)
                foreach (var item in timeIncreasings)
                    if (item.ProcessDate <= response.EndDate)
                        response.EndDate = response.EndDate?.AddDays(item.AdditionalDays ?? 0);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgressReportResponseDTO>> PostAsync(CancellationToken cancellationToken, ProgressReportDTO progressReportDTO)
        {
            ProgressReport progressReport = mapper.Map<ProgressReport>(progressReportDTO);

            await progressReportRepository.AddAsync(cancellationToken, progressReport);
            await unitOfWork.CommitAsync(cancellationToken);

            ProgressReportResponseDTO response = mapper.Map<ProgressReportResponseDTO>(progressReport);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgressReportResponseDTO>> Put(CancellationToken cancellationToken, long id, ProgressReportDTO progressReportDTO)
        {
            ProgressReport progressReport = await progressReportRepository.GetIncluding(cancellationToken, x => x.Id == id);

            ProgressReport updatedProgressReport = mapper.Map(progressReportDTO, progressReport);

            progressReportRepository.Update(updatedProgressReport);
            await unitOfWork.CommitAsync(cancellationToken);

            ProgressReportResponseDTO response = mapper.Map<ProgressReportResponseDTO>(updatedProgressReport);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ProgressReportResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            ProgressReport progressReport = await progressReportRepository.GetByIdAsync(cancellationToken, id);

            progressReportRepository.SoftDelete(progressReport);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<List<ProgressReportResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId)
        {
            IQueryable<ProgressReport> query = progressReportRepository.IncludingQueryable(x => x.ThesisId == thesisId && x.IsDeleted == false);

            List<ProgressReport> progressReports = await query.OrderBy(x => x.Description).ToListAsync(cancellationToken);

            List<ProgressReportResponseDTO> response = mapper.Map<List<ProgressReportResponseDTO>>(progressReports);

            return new() { Result = true, Item = response };
        }
    }
}
