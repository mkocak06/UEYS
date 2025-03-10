using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.Constants;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class ThesisDefenceService : BaseService, IThesisDefenceService
    {
        private readonly IMapper mapper;
        private readonly IThesisDefenceRepository thesisDefenceRepository;
        private readonly IThesisRepository thesisRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IEducationTrackingRepository educationTrackingRepository;
        private readonly IStringLocalizer<Shared.ApiResource> apiResource;
        private readonly IProgressReportRepository progressReportRepository;
        private readonly IEducationTrackingService educationTrackingService;
        private readonly IDocumentRepository documentRepository;
        private readonly IExitExamRepository exitExamRepository;
        private readonly IJuryRepository juryRepository;
        private readonly IKoruRepository koruRepository;

        public ThesisDefenceService(IMapper mapper, IUnitOfWork unitOfWork, IThesisDefenceRepository thesisDefenceRepository, IThesisRepository thesisRepository, IStudentRepository studentRepository, IEducationTrackingRepository educationTrackingRepository, IProgressReportRepository progressReportRepository, IStringLocalizer<Shared.ApiResource> apiResource, IEducationTrackingService educationTrackingService, IDocumentRepository documentRepository, IExitExamRepository exitExamRepository, IJuryRepository juryRepository, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.thesisDefenceRepository = thesisDefenceRepository;
            this.thesisRepository = thesisRepository;
            this.studentRepository = studentRepository;
            this.educationTrackingRepository = educationTrackingRepository;
            this.progressReportRepository = progressReportRepository;
            this.apiResource = apiResource;
            this.educationTrackingService = educationTrackingService;
            this.documentRepository = documentRepository;
            this.exitExamRepository = exitExamRepository;
            this.juryRepository = juryRepository;
            this.koruRepository = koruRepository;
        }

        public async Task<ResponseWrapper<List<ThesisDefenceResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId)
        {
            IQueryable<ThesisDefence> query = thesisDefenceRepository.IncludingQueryable(x => x.ThesisId == thesisId && x.IsDeleted == false);

            List<ThesisDefence> thesisDefences = await query.OrderBy(x => x.Description).ToListAsync(cancellationToken);

            List<ThesisDefenceResponseDTO> response = mapper.Map<List<ThesisDefenceResponseDTO>>(thesisDefences);

            return new() { Result = true, Item = response };
        }

        public async Task<PaginationModel<ThesisDefenceResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<ThesisDefence> ordersQuery = thesisDefenceRepository.IncludingQueryable(x => true);
            FilterResponse<ThesisDefence> filterResponse = ordersQuery.ToFilterView(filter);

            List<ThesisDefenceResponseDTO> thesisDefences = mapper.Map<List<ThesisDefenceResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            return new PaginationModel<ThesisDefenceResponseDTO>()
            {
                Items = thesisDefences,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> GetById(CancellationToken cancellationToken, long id)
        {
            ThesisDefence thesisDefence = await thesisDefenceRepository.GetById(id, cancellationToken);
            ThesisDefenceResponseDTO response = mapper.Map<ThesisDefenceResponseDTO>(thesisDefence);

            return new() { Result = true, Item = response };
        }

        // İlk eklenen tez savunması hep tarih belirleme olarak eklenir
        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> PostAsync(CancellationToken cancellationToken, ThesisDefenceDTO thesisDefenceDTO)
        {
            var thesis = await thesisRepository.GetByIdAsync(cancellationToken, thesisDefenceDTO.ThesisId ?? 0);

            //var isAddable = await IsThesisDefenceAddable(cancellationToken, thesisDefenceDTO.ThesisId ?? 0, thesisDefenceDTO.ExamDate ?? DateTime.UtcNow);
            //if (!isAddable.Result)
            //    return new() { Result = false, Message = isAddable.Message };

            if (thesisDefenceDTO.DefenceOrder == 2)
            {
                var firstThesisDefence = await thesisDefenceRepository.GetByAsync(cancellationToken, x => !x.IsDeleted && x.ThesisId == thesis.Id && x.DefenceOrder == 1);
                if (thesisDefenceDTO.ExamDate <= firstThesisDefence.ExamDate)
                    return new() { Result = false, Message = "2. tez savunmasının tarihi, ilk tez savunmasının tarihinden büyük olmalıdır!" };
            }

            var lastProggressReport = await progressReportRepository.Queryable().OrderByDescending(x => x.EndDate).FirstOrDefaultAsync(x => !x.IsDeleted && x.ThesisId == thesisDefenceDTO.ThesisId, cancellationToken);

            if (lastProggressReport == null || lastProggressReport.EndDate?.AddMonths(3) < thesisDefenceDTO.ExamDate)
                return new() { Result = false, Message = "There cannot be more than 3 months between the due date of the last thesis progress report and the date of the thesis defense you attended. Add any missing thesis progress reports." };

            ThesisDefence thesisDefence = mapper.Map<ThesisDefence>(thesisDefenceDTO);

            foreach (var item in thesisDefence.Juries)
                await koruRepository.AddStudentZoneToUserRole(cancellationToken, item.UserId ?? 0, thesis.StudentId ?? 0, RoleCodeConstants.TEZ_SAVUNMASI_JURISI);

            var educationTracking = new EducationTracking()
            {
                StudentId = thesis.StudentId,
                ProcessDate = thesisDefence.ExamDate,
                ReasonType = thesisDefence.DefenceOrder == 1 ? ReasonType.FirstThesisDefenceDateDetermined : thesisDefence.DefenceOrder == 2 ? ReasonType.SecondThesisDefenceDateDetermined : ReasonType.ThesisDefence,
                ProcessType = ProcessType.Information,
            };

            thesisDefence.EducationTrackings = [educationTracking];

            await thesisDefenceRepository.AddAsync(cancellationToken, thesisDefence);
            await unitOfWork.CommitAsync(cancellationToken);

            ThesisDefenceResponseDTO response = mapper.Map<ThesisDefenceResponseDTO>(thesisDefence);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> Put(CancellationToken cancellationToken, long id, ThesisDefenceDTO thesisDefenceDTO)
        {
            var existThesisDefence = await thesisDefenceRepository.GetById(id, cancellationToken);
            var thesis = await thesisRepository.GetByIdAsync(cancellationToken, thesisDefenceDTO.ThesisId ?? 0);
            var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => !x.IsDeleted && x.StudentId == thesis.StudentId && x.ReasonType == ReasonType.EstimatedFinish);
            var student = await studentRepository.GetByIdAsync(cancellationToken, thesis.StudentId ?? 0);

            bool isStatusConcluded = thesisDefenceDTO.Result != DefenceResultType.InProgress && existThesisDefence.Result == DefenceResultType.InProgress;

            if (isStatusConcluded)
            {
                if (existThesisDefence.ExamDate > thesisDefenceDTO.ExamDate)
                    return new() { Result = false, Message = "Tez savunmasının sonuç tarihi, belirlenme tarihinden önce bir tarih olamaz!" };

                var educationTracking = new EducationTracking()
                {
                    StudentId = thesis.StudentId,
                    ProcessDate = thesisDefenceDTO.ExamDate,
                    ProcessType = ProcessType.Information,
                    ThesisDefenceId = id,
                    Description = "Uzmanlık eğitimi bitirme sınavına girmek için 30 iş gününüz vardır. Detaylı bilgi için Tıpta ve Diş Hekimliğinde Uzmanlık Eğitimi Yönetmeliği Madde 20'yi inceleyiniz."
                };

                if (thesisDefenceDTO.Result is DefenceResultType.Successful or DefenceResultType.SuccessfulWithRevision)
                {
                    educationTracking.ReasonType = thesisDefenceDTO.DefenceOrder == 1 ? ReasonType.SuccessfulFirstThesisDefence : thesisDefenceDTO.DefenceOrder == 2 ? ReasonType.SuccessfulSecondThesisDefence : ReasonType.ThesisDefence;

                    if (thesisDefenceDTO.DefenceOrder == 2)
                    {
                        var failedFirstThesisDefence = await educationTrackingRepository.GetByAsync(cancellationToken, x => !x.IsDeleted && x.StudentId == student.Id && x.ReasonType == ReasonType.TimeExtensionDueToFirstThesisDefence);
                        if (failedFirstThesisDefence != null)
                        {
                            var realEstimatedFinishDate = estimatedFinish.ProcessDate?.AddDays(-failedFirstThesisDefence.AdditionalDays ?? 0);
                            var timeDecreasingEducationTracking = new EducationTracking()
                            {
                                StudentId = student.Id,
                                ProcessType = ProcessType.Information,
                                ReasonType = ReasonType.TimeDecreasingDueToSecondThesisDefence,
                                ThesisDefenceId = id,
                                AdditionalDays = realEstimatedFinishDate < thesisDefenceDTO.ExamDate ? (estimatedFinish.ProcessDate - thesisDefenceDTO.ExamDate)?.Days : (estimatedFinish.ProcessDate - realEstimatedFinishDate)?.Days,
                                ProcessDate = thesisDefenceDTO.ExamDate
                            };
                            educationTrackingRepository.Add(timeDecreasingEducationTracking);
                            estimatedFinish.ProcessDate = realEstimatedFinishDate < thesisDefenceDTO.ExamDate ? thesisDefenceDTO.ExamDate : realEstimatedFinishDate;
                        }
                        else
                            return new() { Result = false, Message = "Birinci başarısız tez savunması bulunamadı!" };
                    }
                }
                else
                {
                    educationTracking.ReasonType = thesisDefenceDTO.DefenceOrder == 1 ? ReasonType.UnsuccessfulFirstThesisDefence : thesisDefenceDTO.DefenceOrder == 2 ? ReasonType.UnsuccessfulSecondThesisDefence : ReasonType.ThesisDefence;
                    if (thesisDefenceDTO.DefenceOrder == 1)
                    {
                        var timeIncreasingEducationTracking = new EducationTracking()
                        {
                            StudentId = thesis.StudentId,
                            ProcessDate = thesisDefenceDTO.ExamDate,
                            ProcessType = ProcessType.Information,
                            ThesisDefenceId = id,
                            ReasonType = ReasonType.TimeExtensionDueToFirstThesisDefence,
                            AdditionalDays = (thesisDefenceDTO.ExamDate?.AddMonths(6) - estimatedFinish.ProcessDate)?.Days,
                            Description = $"2. tez savunması için son tarih : {thesisDefenceDTO.ExamDate?.AddMonths(6).ToShortDateString()}",
                            SecondThesisDeadline = thesisDefenceDTO.ExamDate?.AddMonths(6)
                        };
                        educationTrackingRepository.Add(timeIncreasingEducationTracking);
                        estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(timeIncreasingEducationTracking.AdditionalDays ?? 0);
                    }
                    else
                    {
                        var dismissedEducationTracking = new EducationTracking()
                        {
                            StudentId = thesis.StudentId,
                            ProcessDate = thesisDefenceDTO.ExamDate,
                            ProcessType = ProcessType.Information,
                            ThesisDefenceId = id,
                            ReasonType = ReasonType.Dismission,
                            Description = "2. tez savunmasının başarısız olması nedeniyle ilişiğin kesilmesi"
                        };
                        educationTrackingRepository.Add(dismissedEducationTracking);
                        student.Status = StudentStatus.Dismissed;
                        studentRepository.Update(student);
                    }
                }

                educationTrackingRepository.Update(estimatedFinish);
                educationTrackingRepository.Add(educationTracking);

                thesis.Status = thesisDefenceDTO.Result is DefenceResultType.Successful or DefenceResultType.SuccessfulWithRevision ? ThesisStatusType.Successful : thesisDefenceDTO.Result == DefenceResultType.Failed ? ThesisStatusType.Unsuccessful : ThesisStatusType.Continues;
                thesisRepository.Update(thesis);
            }

            //jüri silinmişse rolleriyle beraber juriyi sil
            foreach (var item in existThesisDefence.Juries)
            {
                if (!thesisDefenceDTO.Juries.Any(x => x.Id == item.Id))
                {
                    juryRepository.SoftDelete(item);
                    var anyOtherJury = await juryRepository.AnyAsync(cancellationToken, x => !x.IsDeleted && x.Id != item.Id && x.ThesisDefenceId != null && (item.EducatorId != null ? x.EducatorId == item.EducatorId : x.UserId == item.UserId));
                    var thesisJuryRole = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.TEZ_SAVUNMASI_JURISI);

                    if (!anyOtherJury)
                        await koruRepository.RemoveRoleFromUser(cancellationToken, item.Educator?.UserId ?? item.UserId ?? 0, thesisJuryRole.Id);
                    else
                        await koruRepository.RemoveUserRoleStudent(item.Educator?.UserId ?? item.UserId ?? 0, thesisJuryRole.Id, thesis.StudentId ?? 0);
                }
            }
            //yeni juri eklenmişse rolüyle beraber ekle
            foreach (var item in thesisDefenceDTO.Juries)
            {
                if (item.Id == null)
                {
                    await koruRepository.AddStudentZoneToUserRole(cancellationToken, item.UserId ?? 0, thesis.StudentId ?? 0, RoleCodeConstants.TEZ_SAVUNMASI_JURISI);
                    juryRepository.Add(mapper.Map<Jury>(item));
                }
            }
            var thesisDefence = mapper.Map<ThesisDefence>(thesisDefenceDTO);
            thesisDefenceRepository.Update(thesisDefence);
            await unitOfWork.CommitAsync(cancellationToken);

            var updatedThesisDefence = await thesisDefenceRepository.GetById(id, cancellationToken);

            ThesisDefenceResponseDTO response = mapper.Map<ThesisDefenceResponseDTO>(updatedThesisDefence);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> Delete(CancellationToken cancellationToken, long id, long studentId)
        {
            ThesisDefence thesisDefence = await thesisDefenceRepository.GetByIdAsync(cancellationToken, id);
            var thesis = await thesisRepository.GetByIdAsync(cancellationToken, thesisDefence.ThesisId ?? 0);

            var isLastDefence = await thesisDefenceRepository.Queryable().Where(x => x.ThesisId == thesis.Id && !x.IsDeleted).OrderByDescending(x => x.DefenceOrder).Select(x => x.Id).FirstOrDefaultAsync() == id;

            var hasExitExam = await exitExamRepository.AnyAsync(cancellationToken, x => !x.IsDeleted && x.StudentId == studentId);

            if (!isLastDefence)
                return new() { Result = false, Message = "Silmek isteğiniz savunma teze ait olan son savunma değildir. Tez savunmalarını sondan başa doğru silebilirsiniz." };
            else if (hasExitExam)
                return new() { Result = false, Message = "Tez savunmasını silmek istediğiniz öğrencinin bitirme sınavı mevcuttur, bu savunmayı silmek için önce bitirme sınavını silmelisiniz." };
            else
            {
                thesisDefenceRepository.SoftDelete(thesisDefence);

                var previousThesisDefence = await thesisDefenceRepository.Queryable().OrderByDescending(x => x.DefenceOrder).FirstOrDefaultAsync(x => x.ThesisId == thesisDefence.ThesisId && !x.IsDeleted && x.DefenceOrder != thesisDefence.DefenceOrder);

                thesis.Status = previousThesisDefence?.Result is DefenceResultType.Successful or DefenceResultType.SuccessfulWithRevision ? ThesisStatusType.Successful : previousThesisDefence?.Result is DefenceResultType.Failed ? ThesisStatusType.Unsuccessful : ThesisStatusType.Continues;

                thesisRepository.Update(thesis);

                var educationTrackings = await educationTrackingRepository.GetAsync(cancellationToken, x => !x.IsDeleted && x.ThesisDefenceId == id);

                if (educationTrackings != null)
                    foreach (var item in educationTrackings)
                        await educationTrackingService.Delete(cancellationToken, item?.Id ?? 0);

                var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.StudentId == studentId && x.ReasonType == ReasonType.EstimatedFinish && !x.IsDeleted);
                var student = await studentRepository.GetByIdAsync(cancellationToken, studentId);

                if (thesisDefence.Result == DefenceResultType.Failed && thesisDefence.DefenceOrder == 1)
                {
                    var timeExtensionEducationTracking = educationTrackings?.FirstOrDefault(x => x.ReasonType == ReasonType.TimeExtensionDueToFirstThesisDefence);
                    if (timeExtensionEducationTracking != null)
                        estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(-(timeExtensionEducationTracking?.AdditionalDays) ?? 0);
                }
                else if (thesisDefence.DefenceOrder == 2)
                {
                    if (thesisDefence.Result == DefenceResultType.Failed)
                        student.Status = estimatedFinish.ProcessDate < DateTime.UtcNow ? StudentStatus.EstimatedFinishDatePast : StudentStatus.EducationContinues;
                    else
                    {
                        var timeDecreasingEducationTracking = educationTrackings?.FirstOrDefault(x => x.ReasonType == ReasonType.TimeDecreasingDueToSecondThesisDefence);
                        if (timeDecreasingEducationTracking != null)
                            estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(timeDecreasingEducationTracking?.AdditionalDays ?? 0);
                    }
                }

                await unitOfWork.CommitAsync(cancellationToken);
                return new() { Result = true };
            }
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> IsThesisDefenceAddable(CancellationToken cancellationToken, long thesisId, DateTime? date)
        {
            var thesis = await thesisRepository.GetByIdAsync(default, thesisId);
            var educationTracking = await educationTrackingRepository.FirstOrDefaultAsync(default, x => !x.IsDeleted && x.ReasonType == ReasonType.EstimatedFinish && x.StudentId == thesis.StudentId);
            if (educationTracking != null && (date != null ? date?.AddMonths(3) : DateTime.Now.Date.AddMonths(3)) >= educationTracking?.ProcessDate?.Date)
                return new() { Result = true };
            else return new() { Result = false, Message = "You cannot add a thesis defense on this date. A thesis defense can be added 3 months before the student's estimated finish date." };
        }
    }
}
