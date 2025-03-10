using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using ChustaSoft.Common.Helpers;
using Core.Entities;
using Core.Extentsions;
using Core.Helpers;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Shared.BaseModels;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Globalization;
using ExportList = Application.Reports.ExcelReports.StudentReports.ExportList;

namespace Application.Services
{
    public class StudentService : BaseService, IStudentService
    {
        private readonly IMapper mapper;
        private readonly IStudentRepository studentRepository;
        private readonly IEducationTrackingRepository educationTrackingRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICKYSService cKYSService;
        private readonly IKoruRepository koruRepository;
        private readonly IEducatorRepository educatorRepository;
        private readonly IEmailSender _emailSender;
        public StudentService(IMapper mapper, IUnitOfWork unitOfWork, IStudentRepository studentRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository, IEducationTrackingRepository educationTrackingRepository, IDocumentRepository documentRepository, ICKYSService cKYSService, IEducatorRepository educatorRepository, IEmailSender emailSender) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studentRepository = studentRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
            this.educationTrackingRepository = educationTrackingRepository;
            this.documentRepository = documentRepository;
            this.cKYSService = cKYSService;
            this.educatorRepository = educatorRepository;
            _emailSender = emailSender;
        }
        public async Task<ResponseWrapper<List<StudentResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Student> query = studentRepository.IncludingQueryable(x => x.IsDeleted == false && x.User.IsDeleted == false, x => x.User);

            List<Student> students = await query.OrderBy(x => x.User.Name).ToListAsync(cancellationToken);

            List<StudentResponseDTO> response = mapper.Map<List<StudentResponseDTO>>(students);

            return new ResponseWrapper<List<StudentResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<BreadCrumbSearchResponseDTO>>> GetListForBreadCrumb(CancellationToken cancellationToken)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var students = await studentRepository.GetListForBreadCrumb(zone).OrderBy(x => x.Name).Take(300).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<BreadCrumbSearchResponseDTO>> { Result = true, Item = students };
        }

        public async Task<PaginationModel<StudentPaginateResponseDTO>> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            IQueryable<StudentPaginateResponseDTO> ordersQuery = studentRepository.OnlyPaginateListQuery(zone);
            var mainListFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "MainList");
            if (mainListFilter != null)
            {
                ordersQuery = ordersQuery.Where(x => x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration && x.Status != StudentStatus.EducationEnded);
                filter.Filter.Filters.Remove(mainListFilter);
            }

            var originalExpertiseBranchNameFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "OriginalExpertiseBranchName");
            if (originalExpertiseBranchNameFilter != null)
            {
                if (originalExpertiseBranchNameFilter.Value.ToString().StartsWith("'") &&
                    originalExpertiseBranchNameFilter.Value.ToString().EndsWith("'"))
                {
                    filter.Filter.Filters.Remove(originalExpertiseBranchNameFilter);

                    ordersQuery = ordersQuery.Where(i => i.OriginalExpertiseBranchName.ToLower() == originalExpertiseBranchNameFilter.Value.ToString().ToLower().Trim('\''));
                }
                if (originalExpertiseBranchNameFilter.Value.ToString().StartsWith("\"") &&
                    originalExpertiseBranchNameFilter.Value.ToString().EndsWith("\""))
                {
                    filter.Filter.Filters.Remove(originalExpertiseBranchNameFilter);

                    ordersQuery = ordersQuery.Where(i => i.OriginalExpertiseBranchName.ToLower() == originalExpertiseBranchNameFilter.Value.ToString().ToLower().Trim('\"'));
                }
            }

            //program detayında  bulunan öğrenci listesideki default yazılmış status filtresinden dolayı hatalı çalışmasın diye operator == "contains" eklendi
            var statusFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "Status");
            if (statusFilter != null && statusFilter.Operator == "contains")
            {
                ordersQuery = ordersQuery.Where(x => x.Status == (StudentStatus)Enum.Parse(typeof(StudentStatus), statusFilter.Value.ToString()));
                filter.Filter.Filters.Remove(statusFilter);
            }

            FilterResponse<StudentPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var students = await filterResponse.Query.ToListAsync(cancellationToken);

            students?.ForEach(x => x.IdentityNo = x.IdentityNo != null ? StringHelpers.MaskIdentityNumber(x.IdentityNo) : null);

            var response = new PaginationModel<StudentPaginateResponseDTO>
            {
                Items = students,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<bool>> UnDeleteStudent(CancellationToken cancellationToken, long id)
        {
            var student = await studentRepository.GetByIdAsync(cancellationToken, id);

            if (student != null && student.IsDeleted == true)
            {
                student.IsDeleted = false;
                student.DeleteDate = null;
                student.DeleteReason = null;
                student.DeleteReasonType = null;

                studentRepository.Update(student);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }

        public async Task<PaginationModel<StudentResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Student> ordersQuery = studentRepository.IncludingQueryable(x => true, x => x.User, x => x.Program.Hospital.Province, x => x.Program.Faculty.University,
                x => x.Program.ExpertiseBranch, x => x.Program.Hospital, x => x.Program.ExpertiseBranch.Profession, x => x.Curriculum, x => x.Curriculum.ExpertiseBranch,
                x => x.Curriculum.ExpertiseBranch.Profession);
            FilterResponse<Student> filterResponse = ordersQuery.Where(x => x.UserId != null).ToFilterView(filter);

            var students = mapper.Map<List<StudentResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            if (students != null && students.Count > 0)
            {
                students.ForEach(x => x.User.IdentityNo = StringHelpers.MaskIdentityNumber(x.User?.IdentityNo));
            }

            var response = new PaginationModel<StudentResponseDTO>
            {
                Items = students,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            return response;
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id, bool isDeleted = false)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var query = studentRepository.GetWithSubRecords(zone);

            var student = await query.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == isDeleted && x.IsHardDeleted == false && x.Status != StudentStatus.EducationEnded && x.Status != StudentStatus.SentToRegistration, cancellationToken);

            if (student != null)
            {
                var studentExBrs = new List<StudentExpertiseBranchResponseDTO>();

                foreach (var item in student?.StudentExpertiseBranches ?? new List<StudentExpertiseBranch>())
                {
                    if (item.ExpertiseBranchId != null)
                    {
                        var stuExBr = new StudentExpertiseBranchResponseDTO
                        {
                            ExpertiseBranchName = item.ExpertiseBranch.Name,
                            ExpertiseBranchId = item.ExpertiseBranchId,
                            IsPrincipal = item.ExpertiseBranch.IsPrincipal,
                            RegistrationDate = item.RegistrationDate,
                            StudentId = item.StudentId,
                            Id = item.Id
                        };
                        studentExBrs.Add(stuExBr);
                    }
                }
                student.StudentExpertiseBranches = null;
                var response = mapper.Map<StudentResponseDTO>(student);
                response.User.IdentityNo = StringHelpers.MaskIdentityNumber(student.User.IdentityNo);
                response.StudentExpertiseBranches = studentExBrs;
                if (student.OriginalProgramId != null && student.OriginalProgramId > 0)
                {

                    var protocolProgram = await studentRepository.IsProtocolProgram((long)student.OriginalProgramId, cancellationToken);
                    if (protocolProgram != null)
                        response.ProtocolOrComplement = protocolProgram.Type;
                }

                var docs = await documentRepository.GetByEntityId(cancellationToken, student.Id, DocumentTypes.OsymResultDocument);
                var docs_1 = await documentRepository.GetByEntityId(cancellationToken, student.Id, DocumentTypes.PhotocopyOfIdentityCard);
                var docs_2 = await documentRepository.GetByEntityId(cancellationToken, student.Id, DocumentTypes.FeeReceipt);
                var docs_4 = await documentRepository.GetByEntityId(cancellationToken, student.Id, DocumentTypes.RegistrationControlForm);
                docs ??= new();
                docs_1.ForEach(x => docs.Add(x));
                docs_2.ForEach(x => docs.Add(x));
                docs_4.ForEach(x => docs.Add(x));

                response.Documents = mapper.Map<List<DocumentResponseDTO>>(docs);
                response.StartedOverWithExamForSameBranch = response.EducationTrackings.Any(x => x.ReasonType == ReasonType.StartingOverwithExamfortheSameExpertiseBranch || x.ReasonType == ReasonType.RestartByJudgment);

                return new ResponseWrapper<StudentResponseDTO> { Result = true, Item = response };
            }
            else
                return new() { Result = false, Message = "You are not authorized for this operation!" };
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> GetRegistrationStudentById(CancellationToken cancellationToken, long id)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var query = studentRepository.GetRegistrationStudentQuery(zone);

            var response = await query.Select(x => new StudentResponseDTO()
            {
                Id = x.Id,
                BeginningExam = x.BeginningExam,
                BeginningPeriod = x.BeginningPeriod,
                BeginningYear = x.BeginningYear,
                PlacementScore = x.PlacementScore,
                QuotaType = x.QuotaType,
                IsDeleted = x.IsDeleted,
                IsHardDeleted = x.IsHardDeleted,
                Status = x.Status,
                GraduatedDate = x.GraduatedDate,
                GraduatedSchool = x.GraduatedSchool,
                MedicineRegistrationDate = x.MedicineRegistrationDate,
                MedicineRegistrationNo = x.MedicineRegistrationNo,
                User = x.User == null ? null : new UserAccountDetailInfoResponseDTO()
                {
                    Id = x.UserId ?? 0,
                    IdentityNo = x.User.IdentityNo,
                    Name = x.User.Name,
                    Email = x.User.Email,
                    Phone = x.User.Phone
                },
                StudentExpertiseBranches = x.StudentExpertiseBranches.Select(x => new StudentExpertiseBranchResponseDTO()
                {
                    RegistrationDate = x.RegistrationDate,
                    ExpertiseBranch = x.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO() { Name = x.ExpertiseBranch.Name }

                }).ToList(),
                Program = x.Program == null ? null : new ProgramResponseDTO { Id = x.Program.Id, Hospital = x.Program.Hospital == null ? null : new HospitalResponseDTO { Name = x.Program.Hospital.Name, Province = x.Program.Hospital.Province == null ? null : new ProvinceResponseDTO() { Name = x.Program.Hospital.Province.Name } }, ExpertiseBranch = x.Program.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO { Name = x.Program.ExpertiseBranch.Name } },
                Curriculum = x.Curriculum == null ? null : new CurriculumResponseDTO() { Id = x.Curriculum.Id, Version = x.Curriculum.Version, Duration = x.Curriculum.Duration, ExpertiseBranch = x.Curriculum.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO { Name = x.Curriculum.ExpertiseBranch.Name } },
                EducationTrackings = x.EducationTrackings.Where(x => !x.IsDeleted).Select(x => new EducationTrackingResponseDTO()
                {
                    Id = x.Id,
                    ProcessType = x.ProcessType,
                    ReasonType = x.ReasonType,
                    Program = x.Program == null ? null : new ProgramResponseDTO { Id = x.Program.Id, Hospital = x.Program.Hospital == null ? null : new HospitalResponseDTO { Name = x.Program.Hospital.Name, Province = x.Program.Hospital.Province == null ? null : new ProvinceResponseDTO() { Name = x.Program.Hospital.Province.Name } }, ExpertiseBranch = x.Program.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO { Name = x.Program.ExpertiseBranch.Name } },
                    AdditionalDays = x.AdditionalDays,
                    ProcessDate = x.ProcessDate,
                    Description = x.Description,
                    StudentRotationId = x.StudentRotationId,
                    ThesisDefenceId = x.ThesisDefenceId
                }).ToList(),
                StudentRotations = x.StudentRotations.Where(x => !x.IsDeleted).Select(x => new StudentRotationResponseDTO()
                {
                    Id = x.Id,
                    EducatorName = x.EducatorName,
                    Educator = x.Educator == null ? null : new EducatorResponseDTO() { User = x.Educator.User == null ? null : new UserAccountDetailInfoResponseDTO() { Name = x.Educator.User.Name } },
                    BeginDate = x.BeginDate,
                    EndDate = x.EndDate,
                    IsSuccessful = x.IsSuccessful,
                    IsUncompleted = x.IsUncompleted,
                    Program = x.Program == null ? null : new ProgramResponseDTO { Id = x.Program.Id, Hospital = x.Program.Hospital == null ? null : new HospitalResponseDTO { Name = x.Program.Hospital.Name, Province = x.Program.Hospital.Province == null ? null : new ProvinceResponseDTO() { Name = x.Program.Hospital.Province.Name } }, ExpertiseBranch = x.Program.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO { Name = x.Program.ExpertiseBranch.Name } },
                    Rotation = x.Rotation == null ? null : new RotationResponseDTO
                    {
                        ExpertiseBranch = x.Rotation.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO { Name = x.Rotation.ExpertiseBranch.Name }
                    }
                }).ToList(),
                PerformanceRatings = x.PerformanceRatings.Where(x => !x.IsDeleted).Select(x => new PerformanceRatingResponseDTO()
                {
                    Id = x.Id,
                    Educator = x.Educator == null ? null : new EducatorResponseDTO() { User = x.Educator.User == null ? null : new UserAccountDetailInfoResponseDTO() { Name = x.Educator.User.Name } },
                    Result = x.Result,
                    Altruism = x.Altruism,
                    AppropriateAppealToPeople = x.AppropriateAppealToPeople,
                    CommunicationObstacleRemove = x.CommunicationObstacleRemove,
                    ConflictResolution = x.ConflictResolution,
                    CrisisManagement = x.CrisisManagement,
                    EmbraceLearningAndTeaching = x.EmbraceLearningAndTeaching,
                    Empathy = x.Empathy,
                    Fair = x.Fair,
                    FeedBack = x.FeedBack,
                    FightAddiction = x.FightAddiction,
                    MotivatePeople = x.MotivatePeople,
                    HealthProtectionVolunteer = x.HealthProtectionVolunteer,
                    HealthRiskAwareness = x.HealthRiskAwareness,
                    HumanValues = x.HumanValues,
                    Leadership = x.Leadership,
                    LegalLiabilityAwareness = x.LegalLiabilityAwareness,
                    LegalLiabilityCompletion = x.LegalLiabilityCompletion,
                    LifeStyleChangeRoleModel = x.LifeStyleChangeRoleModel,
                    Listening = x.Listening,
                    ManagementTechniquesApply = x.ManagementTechniquesApply,
                    MeetingManagement = x.MeetingManagement,
                    NegativeNews = x.NegativeNews,
                    Period = x.Period,
                    SafetyProviding = x.SafetyProviding,
                    ScientificThinking = x.ScientificThinking,
                    StandUpForTeam = x.StandUpForTeam,
                    TeachingEffort = x.TeachingEffort,
                    WorkInTeam = x.WorkInTeam,
                    WorkPlaceManagement = x.WorkPlaceManagement
                }).ToList(),
                OpinionForms = x.OpinionForms.Where(x => !x.IsDeleted && x.FormStatusType == 0).Select(x => new OpinionFormResponseDTO()
                {
                    Period = x.Period,
                    ProgramManager = x.ProgramManager == null ? null : new EducatorResponseDTO() { User = x.ProgramManager.User == null ? null : new UserAccountDetailInfoResponseDTO() { Name = x.ProgramManager.User.Name }, StaffTitle = x.ProgramManager.StaffTitle == null ? null : new TitleResponseDTO() { Name = x.ProgramManager.StaffTitle.Name } },
                    Educator = x.Educator == null ? null : new EducatorResponseDTO() { User = x.Educator.User == null ? null : new UserAccountDetailInfoResponseDTO() { Name = x.Educator.User.Name }, StaffTitle = x.Educator.StaffTitle == null ? null : new TitleResponseDTO() { Name = x.Educator.StaffTitle.Name } },
                    Result = x.Result,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    AdditionalExplanation = x.AdditionalExplanation
                }).ToList(),
                Theses = x.Theses.Where(x => !x.IsDeleted).Select(x => new ThesisResponseDTO()
                {
                    Id = x.Id,
                    Subject = x.Subject,
                    SubjectDetermineDate = x.SubjectDetermineDate,
                    ThesisSubjectType_1 = x.ThesisSubjectType_1,
                    ThesisSubjectType_2 = x.ThesisSubjectType_2,
                    Status = x.Status,
                    ThesisDefences = x.ThesisDefences.Where(x => !x.IsDeleted).Select(x => new ThesisDefenceResponseDTO()
                    {
                        Id = x.Id,
                        DefenceOrder = x.DefenceOrder,
                        Description = x.Description,
                        Hospital = x.Hospital == null ? null : new HospitalResponseDTO() { Name = x.Hospital.Name, Province = x.Hospital.Province == null ? null : new ProvinceResponseDTO() { Name = x.Hospital.Province.Name } },
                        Result = x.Result,
                        ExamDate = x.ExamDate,
                        Juries = x.Juries.Where(x => !x.IsDeleted).Select(x => new JuryResponseDTO()
                        {
                            Id = x.Id,
                            JuryType = x.JuryType,

                            Educator = x.Educator == null ? null : new EducatorResponseDTO()
                            {
                                Id = x.Educator.Id,
                                UserId = x.Educator.User.Id,
                                StaffTitle = x.Educator.StaffTitle == null ? null : new TitleResponseDTO() { Name = x.Educator.StaffTitle.Name },
                                User = x.Educator.User == null ? null : new UserAccountDetailInfoResponseDTO() { Id = x.Educator.User.Id, Name = x.Educator.User.Name },
                                EducatorExpertiseBranches = x.Educator.EducatorExpertiseBranches.Select(x => new EducatorExpertiseBranchResponseDTO()
                                {
                                    RegistrationNo = x.RegistrationNo,
                                    ExpertiseBranch = x.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO() { Id = x.ExpertiseBranch.Id, Name = x.ExpertiseBranch.Name }
                                }).ToList(),
                            }
                        }).ToList(),
                    }).ToList(),
                }).ToList(),
                ExitExams = x.ExitExams.Where(x => !x.IsDeleted).Select(x => new ExitExamResponseDTO()
                {
                    Id = x.Id,
                    ExamDate = x.ExamDate,
                    AbilityExamNote = x.AbilityExamNote,
                    PracticeExamNote = x.PracticeExamNote,
                    Hospital = x.Hospital == null ? null : new HospitalResponseDTO() { Name = x.Hospital.Name, Province = x.Hospital.Province == null ? null : new ProvinceResponseDTO() { Name = x.Hospital.Province.Name } },
                    Secretary = x.Secretary == null ? null : new UserResponseDTO() { Name = x.Secretary.Name},
                    ExamStatus = x.ExamStatus,
                    Description = x.Description,
                    EducationTrackingId = x.EducationTrackingId,
                    Juries = x.Juries.Where(x => !x.IsDeleted).Select(x => new JuryResponseDTO()
                    {
                        Id = x.Id,
                        JuryType = x.JuryType,

                        Educator = x.Educator == null ? null : new EducatorResponseDTO()
                        {
                            Id = x.Educator.Id,
                            UserId = x.Educator.User.Id,
                            StaffTitle = x.Educator.StaffTitle == null ? null : new TitleResponseDTO() { Name = x.Educator.StaffTitle.Name },
                            User = x.Educator.User == null ? null : new UserAccountDetailInfoResponseDTO() { Id = x.Educator.User.Id, Name = x.Educator.User.Name },
                            EducatorExpertiseBranches = x.Educator.EducatorExpertiseBranches.Select(x => new EducatorExpertiseBranchResponseDTO()
                            {
                                RegistrationNo = x.RegistrationNo,
                                ExpertiseBranch = x.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO() { Id = x.ExpertiseBranch.Id, Name = x.ExpertiseBranch.Name }
                            }).ToList(),
                        }
                    }).ToList(),
                }).ToList(),
            }).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false && x.IsHardDeleted == false && x.Status == StudentStatus.SentToRegistration, cancellationToken);

            if (response != null)
            {
                response.User.IdentityNo = StringHelpers.MaskIdentityNumber(response.User.IdentityNo);

                var docs = await documentRepository.GetByEntityId(cancellationToken, response.Id, DocumentTypes.OsymResultDocument);
                var docs_1 = await documentRepository.GetByEntityId(cancellationToken, response.Id, DocumentTypes.PhotocopyOfIdentityCard);
                var docs_2 = await documentRepository.GetByEntityId(cancellationToken, response.Id, DocumentTypes.FeeReceipt);
                var docs_4 = await documentRepository.GetByEntityId(cancellationToken, response.Id, DocumentTypes.RegistrationControlForm);

                docs ??= new();
                docs_1.ForEach(x => docs.Add(x));
                docs_2.ForEach(x => docs.Add(x));
                docs_4.ForEach(x => docs.Add(x));

                foreach (var item in response.StudentRotations)
                {
                    var rotationDocs = await documentRepository.GetByEntityId(cancellationToken, item.Id ?? -1, DocumentTypes.StudentRotation);
                    item.Documents = mapper.Map<List<DocumentResponseDTO>>(rotationDocs);
                }

                foreach (var item in response.EducationTrackings)
                {
                    List<Document> eduTrackingDocs = new();
                    if (item.ReasonType == ReasonType.CompletionOfRotation || item.ReasonType == ReasonType.ExemptionOfRotation || item.ReasonType == ReasonType.LeavingTheInstitutionDueToRotation || item.ReasonType == ReasonType.LeftWithoutCompletingRotation || item.ReasonType == ReasonType.TimeExtensionDueToFailureInRotation)
                        eduTrackingDocs = await documentRepository.GetByEntityId(cancellationToken, item.StudentRotationId ?? -1, DocumentTypes.StudentRotation);
                    else if (item.ReasonType == ReasonType.TimeExtensionDueToFirstThesisDefence || item.ReasonType == ReasonType.SuccessfulFirstThesisDefence || item.ReasonType == ReasonType.FirstThesisDefenceDateDetermined || item.ReasonType == ReasonType.UnsuccessfulFirstThesisDefence || item.ReasonType == ReasonType.SecondThesisDefenceDateDetermined || item.ReasonType == ReasonType.SuccessfulSecondThesisDefence || item.ReasonType == ReasonType.TimeDecreasingDueToSecondThesisDefence || item.ReasonType == ReasonType.UnsuccessfulSecondThesisDefence)
                        eduTrackingDocs = await documentRepository.GetByEntityId(cancellationToken, item.ThesisDefenceId ?? -1, DocumentTypes.ThesisDefence);

                    eduTrackingDocs = await documentRepository.GetByEntityId(cancellationToken, item.Id ?? -1, DocumentTypes.EducationTimeTracking);
                    item.Documents = mapper.Map<List<DocumentResponseDTO>>(eduTrackingDocs);
                }

                response.Documents = mapper.Map<List<DocumentResponseDTO>>(docs);

                return new ResponseWrapper<StudentResponseDTO> { Result = true, Item = response };
            }
            else
                return new() { Result = false, Message = "You are not authorized for this operation!" };
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentDTO studentDTO)
        {
            Student student = mapper.Map<Student>(studentDTO);
            if (await studentRepository.AnyAsync(cancellationToken, x => x.UserId == student.UserId && !x.IsDeleted && !x.IsHardDeleted && x.Status != StudentStatus.EducationEnded && !x.User.IsDeleted))
            {
                return new ResponseWrapper<StudentResponseDTO> { Result = false, Message = "Student already exists" };
            }
            else
            {
                if (student.CurriculumId != null)
                {
                    var studentsCurriculum = await unitOfWork.CurriculumRepository.GetByIdAsync(cancellationToken, student.CurriculumId.Value);
                    if (student.EducationTrackings != null && studentsCurriculum?.Duration != null)
                    {
                        student.EducationTrackings.Add(new() { ProcessType = ProcessType.EstimatedFinish, ReasonType = ReasonType.EstimatedFinish, ProcessDate = student.EducationTrackings.FirstOrDefault()?.ProcessDate?.AddYears(studentsCurriculum.Duration.Value) });
                    }
                }
                student.User = null;
                await studentRepository.AddAsync(cancellationToken, student);
                await unitOfWork.CommitAsync(cancellationToken);

                var response = mapper.Map<StudentResponseDTO>(student);

                return new ResponseWrapper<StudentResponseDTO> { Result = true, Item = response };
            }
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentDTO studentDTO)
        {
            var existStudent = studentRepository.Queryable().AsSplitQuery()
                .Include(x => x.User).ThenInclude(x => x.Country)
                .Include(x => x.Theses).ThenInclude(x => x.ThesisDefences)
                .Include(x => x.OriginalProgram).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.EducationOfficers).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                .Include(x => x.Curriculum).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.StudentExpertiseBranches).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.PrincipalBranches).FirstOrDefault(x => x.IsHardDeleted == false && x.Id == id);
            var updatedStudent = mapper.Map(studentDTO, existStudent);

            if (studentDTO.IsDeleted == true)
            {
                updatedStudent.User.IsDeleted = true;
                updatedStudent.User.DeleteDate = DateTime.UtcNow;
            }


            studentRepository.Update(updatedStudent);
            await unitOfWork.CommitAsync(cancellationToken);

            var studentUpdated = studentRepository.Queryable().AsSplitQuery()
                .Include(x => x.User).ThenInclude(x => x.Country)
                .Include(x => x.Theses).ThenInclude(x => x.ThesisDefences)
                .Include(x => x.OriginalProgram).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.EducationOfficers).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                .Include(x => x.Curriculum).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.StudentExpertiseBranches).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.PrincipalBranches).FirstOrDefault(x => x.IsHardDeleted == false && x.Id == id);
            return new ResponseWrapper<StudentResponseDTO> { Result = true, Item = mapper.Map<StudentResponseDTO>(studentUpdated) };
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> AddStudentToProgram(CancellationToken cancellationToken, long studentId, long programId)
        {
            Student student = await studentRepository.GetByIdAsync(cancellationToken, studentId);

            student.ProgramId = programId;
            studentRepository.Update(student);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentResponseDTO>(student);

            return new ResponseWrapper<StudentResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> Delete(CancellationToken cancellationToken, long id, string reason)
        {
            Student student = await studentRepository.GetByIdAsync(cancellationToken, id);

            student.DeleteReason = reason;

            studentRepository.SoftDelete(student);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> CompletelyDelete(CancellationToken cancellationToken, long id)
        {
            Student student = await studentRepository.GetByIdAsync(cancellationToken, id);

            student.IsHardDeleted = true;

            studentRepository.Update(student);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<StudentResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var student = await studentRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.User);

            if (await educatorRepository.AnyAsync(cancellationToken, x => x.IsDeleted == false && x.UserId == student.UserId) == true || await studentRepository.AnyAsync(cancellationToken, x => x.IsDeleted == false && x.IsHardDeleted == false && x.UserId == student.UserId))
                return new() { Result = false, Message = "Geri almak istediğiniz kişinin aktif öğrenci ya da eğiticisi vardır. Bu kişiyi geri alamazsınız!" };

            await studentRepository.UnDeleteStudent(cancellationToken, id);
            return new ResponseWrapper<StudentResponseDTO>() { Result = true };
        }

        public async Task<ResponseWrapper<List<StudentResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId)
        {
            IQueryable<Student> query = studentRepository.IncludingQueryable(x => x.ProgramId == programId && x.IsDeleted == false, x => x.User);

            var students = await query.OrderBy(x => x.User.Name).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<StudentResponseDTO>> { Result = true, Item = mapper.Map<List<StudentResponseDTO>>(students) };
        }

        public ResponseWrapper<List<CountsByMonthsResponse>> GetCountsByMonthsResponse()
        {
            var result = studentRepository.GetCountsByMonthsResponse();
            return new() { Item = result, Result = true };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
        {
            filter.pageSize = int.MaxValue;
            filter.page = 1;

            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<StudentExcelExportModel> ordersQuery = studentRepository.ExportExcelQuery(zone);
            var mainListFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "MainList");
            if (mainListFilter != null)
            {
                ordersQuery = ordersQuery.Where(x => x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration && x.Status != StudentStatus.EducationEnded);
                filter.Filter.Filters.Remove(mainListFilter);
            }
            var originalExpertiseBranchNameFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "OriginalExpertiseBranchName");
            if (originalExpertiseBranchNameFilter != null)
            {
                if (originalExpertiseBranchNameFilter.Value.ToString().StartsWith("'") &&
                    originalExpertiseBranchNameFilter.Value.ToString().EndsWith("'"))
                {
                    filter.Filter.Filters.Remove(originalExpertiseBranchNameFilter);

                    ordersQuery = ordersQuery.Where(i => i.OriginalExpertiseBranchName.ToLower() == originalExpertiseBranchNameFilter.Value.ToString().ToLower(new CultureInfo("tr-TR")).Trim('\''));
                }
                if (originalExpertiseBranchNameFilter.Value.ToString().StartsWith("\"") &&
                    originalExpertiseBranchNameFilter.Value.ToString().EndsWith("\""))
                {
                    filter.Filter.Filters.Remove(originalExpertiseBranchNameFilter);

                    ordersQuery = ordersQuery.Where(i => i.OriginalExpertiseBranchName.ToLower() == originalExpertiseBranchNameFilter.Value.ToString().ToLower(new CultureInfo("tr-TR")).Trim('\"'));
                }
            }
            var statusFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "Status");
            if (statusFilter != null && statusFilter.Operator == "contains")
            {
                ordersQuery = ordersQuery.Where(x => x.Status == (StudentStatus)Enum.Parse(typeof(StudentStatus), statusFilter.Value.ToString()));
                filter.Filter.Filters.Remove(statusFilter);
            }

            FilterResponse<StudentExcelExportModel> filterResponse = ordersQuery.ToFilterView(filter);

            var students = await filterResponse.Query.ToListAsync(cancellationToken);

            var byteArray = ExportList.ExportListReport(students);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }

        public async Task<ResponseWrapper<List<StudentCountByProperty>>> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = studentRepository.QueryableStudentsForCharts(zone);

            FilterResponse<StudentChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false).GroupBy(x => x.IsUniversityPrivate)
                                                     .Select(x => new StudentCountByProperty()
                                                     {
                                                         Value = x.Key == true ? "Foundation University" : (x.Key == false ? "Public University" : "Other"),
                                                         Count = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<StudentCountByProperty>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<StudentCountByProperty>>> CountByExamType(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = studentRepository.QueryableStudentsForCharts(zone);

            FilterResponse<StudentChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false).GroupBy(x => x.PlacementExamType)
                                                     .Select(x => new StudentCountByProperty()
                                                     {
                                                         Value = x.Key.HasValue ? x.Key.Value.GetDescription() : "Belirtilmemiş",
                                                         Count = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<StudentCountByProperty>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<StudentCountByProperty>>> CountByProfession(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = studentRepository.QueryableStudentsForCharts(zone);

            FilterResponse<StudentChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.IsDeleted == false).GroupBy(x => x.ProfessionName)
                                                     .Select(x => new StudentCountByProperty()
                                                     {
                                                         Value = x.Key ?? "Bilinmiyor",
                                                         Count = x.Count()
                                                     }).ToListAsync(cancellationToken);

            return new ResponseWrapper<List<StudentCountByProperty>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<StudentQuotaChartModel>>> CountByQuotas(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryablePrograms = studentRepository.QueryableStudentsForCharts(zone);

            FilterResponse<StudentChartModel> filterResponse = queryablePrograms.ToFilterView(filter);

            var response = await filterResponse.Query.Where(x => x.QuotaType != null).GroupBy(x => x.QuotaType)
                                                     .Select(x => new StudentQuotaChartModel()
                                                     {
                                                         SeriesName = x.Key.Value,
                                                         //ChemistCount = x.Count(s => s.QuotaType == QuotaType.Chemist),
                                                         //DKKCount = x.Count(s => s.QuotaType == QuotaType.DKK),
                                                         //EAHCount = x.Count(s => s.QuotaType == QuotaType.SB),
                                                         //HKKCount = x.Count(s => s.QuotaType == QuotaType.HKK),
                                                         //JGKCount = x.Count(s => s.QuotaType == QuotaType.JGK),
                                                         //KKKCount = x.Count(s => s.QuotaType == QuotaType.KKK),
                                                         //KKTCFullTimeCount = x.Count(s => s.QuotaType == QuotaType.KKTCFullTime),
                                                         //KKTCHalfTimeCount = x.Count(s => s.QuotaType == QuotaType.KKTCHalfTime),
                                                         //PharmacistCount = x.Count(s => s.QuotaType == QuotaType.Pharmacist),
                                                         //SBACount = x.Count(s => s.QuotaType == QuotaType.SBA),
                                                         //University_PrivateCount = x.Count(s => s.QuotaType == QuotaType.Uni_Private),
                                                         //University_StateCount = x.Count(s => s.QuotaType == QuotaType.Uni_State),
                                                         //VetCount = x.Count(s => s.QuotaType == QuotaType.Vet)
                                                         Count = x.Count()
                                                     }).OrderByDescending(x => x.Count).ToListAsync(cancellationToken);



            return new ResponseWrapper<List<StudentQuotaChartModel>> { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            var queryableStudents = studentRepository.QueryableStudentsForCharts(zone);

            FilterResponse<StudentChartModel> filterResponse = queryableStudents.ToFilterView(filter);

            var response = await filterResponse.Query.GroupBy(x => new { x.ParentInstitutionName, x.ProfessionName })
                                                     .Select(x => new CountsByProfessionInstitutionModel
                                                     {
                                                         ParentInstitutionName = x.Key.ParentInstitutionName ?? "Bilinmiyor",
                                                         ProfessionName = x.Key.ProfessionName ?? "Bilinmiyor",
                                                         Count = x.Count()
                                                     }
                                                     ).ToListAsync(cancellationToken);

            var result = new List<CountsByProfessionInstitutionModel>();
            foreach (var item in response.GroupBy(x => x.ParentInstitutionName))
            {
                result.Add(new CountsByProfessionInstitutionModel() { ParentInstitutionName = item.Key });
                foreach (var item2 in item)
                {
                    var selectedItem = result.FirstOrDefault(x => x.ParentInstitutionName == item.Key);
                    if (selectedItem != null)
                        if (item2.ProfessionName == "Tıp")
                            selectedItem.MedicineCount = item2.Count;
                        else if (item2.ProfessionName == "Diş Hekimliği")
                            selectedItem.DentistryCount = item2.Count;
                        else
                            selectedItem.PharmaceuticsCount = item2.Count;
                }
            }

            return new ResponseWrapper<List<CountsByProfessionInstitutionModel>> { Item = result, Result = true };
        }

        public async Task<ResponseWrapper<List<RestartStudentUserModel>>> GetRestartStudents(CancellationToken cancellationToken)
        {
            return new() { Item = await studentRepository.GetRestartStudents(cancellationToken), Result = true };
        }

        public async Task<PaginationModel<StudentPaginateResponseDTO>> GetExpiredStudents(CancellationToken cancellationToken)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            IQueryable<StudentPaginateResponseDTO> ordersQuery = studentRepository.GetExpiredStudents(zone);

            var students = await ordersQuery.ToListAsync(cancellationToken);

            return new PaginationModel<StudentPaginateResponseDTO>
            {
                Items = students,
                Page = 1,
                PageSize = int.MaxValue,
                TotalItemCount = students.Count
            };
        }
    }
}

