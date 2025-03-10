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
    public class EducationTrackingService : BaseService, IEducationTrackingService
    {
        private readonly IMapper mapper;
        private readonly IEducationTrackingRepository educationTrackingRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IDependentProgramRepository dependentProgramRepository;
        private readonly IStudentDependentProgramRepository studentDependentProgramRepository;
        private readonly IOpinionFormRepository opinionFormRepository;
        private readonly ICurriculumRepository curriculumRepository;
        private readonly IProgramRepository programRepository;
        private readonly IStudentRotationRepository studentRotationRepository;
        private readonly IUserRepository userRepository;

        public EducationTrackingService(IMapper mapper, IUnitOfWork unitOfWork, IEducationTrackingRepository educationTrackingRepository, IDocumentRepository documentRepository, IStudentRepository studentRepository, IDependentProgramRepository dependentProgramRepository, IStudentDependentProgramRepository studentDependentProgramRepository, IOpinionFormRepository opinionFormRepository, ICurriculumRepository curriculumRepository, IProgramRepository programRepository, IStudentRotationRepository studentRotationRepository, IUserRepository userRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.educationTrackingRepository = educationTrackingRepository;
            this.documentRepository = documentRepository;
            this.studentRepository = studentRepository;
            this.dependentProgramRepository = dependentProgramRepository;
            this.studentDependentProgramRepository = studentDependentProgramRepository;
            this.opinionFormRepository = opinionFormRepository;
            this.curriculumRepository = curriculumRepository;
            this.programRepository = programRepository;
            this.studentRotationRepository = studentRotationRepository;
            this.userRepository = userRepository;
        }

        public async Task<PaginationModel<EducationTrackingResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<EducationTracking> ordersQuery = educationTrackingRepository.IncludingQueryable(x => true, x => x.Student, x => x.ProcessOwner);
            FilterResponse<EducationTracking> filterResponse = ordersQuery.ToFilterView(filter);

            var educationTrackings = mapper.Map<List<EducationTrackingResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            if (educationTrackings != null)
            {
                foreach (var item in educationTrackings)
                {
                    item.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, (long)item.Id, DocumentTypes.EducationTimeTracking));
                }
            }

            var response = new PaginationModel<EducationTrackingResponseDTO>
            {
                Items = educationTrackings,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> GetById(CancellationToken cancellationToken, long id)
        {
            EducationTracking educationTracking = await educationTrackingRepository.GetIncluding(cancellationToken, x => x.IsDeleted == false && x.Id == id, x => x.Student, x => x.ProcessOwner);

            EducationTrackingResponseDTO response = mapper.Map<EducationTrackingResponseDTO>(educationTracking);

            return new ResponseWrapper<EducationTrackingResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetListByStudentIdAsync(CancellationToken cancellationToken, long studentId)
        {
            List<EducationTracking> educationTrackings = await educationTrackingRepository.GetListByStudentId(cancellationToken, studentId);
            List<EducationTrackingResponseDTO> response = mapper.Map<List<EducationTrackingResponseDTO>>(educationTrackings);

            if (response != null)
            {
                foreach (var item in response)
                {
                    item.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, (long)item.Id, DocumentTypes.EducationTimeTracking));
                }
            }

            return new ResponseWrapper<List<EducationTrackingResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> PostAsync(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO)
        {
            EducationTracking educationTracking = mapper.Map<EducationTracking>(educationTrackingDTO);


            var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.StudentId == educationTracking.StudentId && x.ProcessType == ProcessType.EstimatedFinish);
            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)educationTracking.StudentId);
            var failedThesisDefence = await educationTrackingRepository.GetByAsync(cancellationToken, x => !x.IsDeleted && x.StudentId == student.Id && (x.ReasonType == ReasonType.FirstThesisDefence || x.ReasonType == ReasonType.TimeExtensionDueToFirstThesisDefence));

            if (failedThesisDefence != null && educationTracking.ProcessDate < failedThesisDefence.ProcessDate)
            {
                var realEducationEstimatedFinishDate = estimatedFinish.ProcessDate?.AddDays(-failedThesisDefence.AdditionalDays ?? 0);
                if (realEducationEstimatedFinishDate < educationTracking.ProcessDate)
                    return new() { Result = false, Message = "eğitimin gereçek bitiş tarihinden sonra bu işlemleri gerçekleştiremezsiniz" };
            }
            if (estimatedFinish != null)
            {
                #region SetProgramIdIfNull
                // nakil ile başlayan öğrencilerin educationTracking programId'si boş gelir, son basladığı programın id'si educationTracking'e eklenir
                if (educationTracking.ReasonType != ReasonType.CompletionOfAssignmentAbroad && educationTracking.AssignmentType != AssignmentType.EducationAbroad && educationTracking.ProcessType != ProcessType.Finish)
                {
                    var startRecord = await educationTrackingRepository.Queryable().OrderByDescending(x => x.ProcessDate).FirstOrDefaultAsync(x => x.ProcessDate <= educationTracking.ProcessDate && x.ProcessType == ProcessType.Start && x.IsDeleted == false && x.StudentId == educationTracking.StudentId, cancellationToken);
                    if (startRecord == null)
                        return new() { Result = false, Message = "İşlem tarihi, eğitimin başlangıç tarihinden önce olamaz!" };
                    educationTracking.ProgramId ??= startRecord.ProgramId;
                }
                #endregion

                #region EndOfEducation
                if (educationTracking.ProcessType == ProcessType.Finish && educationTracking.ReasonType != ReasonType.KKTCHalfTimed)
                    await studentRepository.DeleteStudent(cancellationToken, educationTrackingDTO);
                #endregion

                #region TransferOrAssingnment
                if ((educationTracking.ProcessType == ProcessType.Transfer && educationTracking.ReasonType != ReasonType.BranchChange_End) || (educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment && educationTracking.AssignmentType != AssignmentType.EducationAbroad) || educationTracking.ReasonType == ReasonType.CompletionOfAssignment || educationTracking.ReasonType == ReasonType.KKTCHalfTimed)
                {
                    if (educationTracking.ProcessType == ProcessType.Transfer || educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                    {
                        var checkTransferPossible = await opinionFormRepository.CheckTransferPossible(cancellationToken, educationTrackingDTO);
                        if (!checkTransferPossible.Result)
                            return new() { Result = false, Message = checkTransferPossible.Message };
                    }
                    educationTracking.FormerProgramId = student.ProgramId;

                    if (educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                    {
                        student.ProgramId = educationTracking.ProgramId;
                        var beginningToAssignedIns = await educationTrackingRepository.Queryable().OrderByDescending(x => x.ProcessDate).FirstOrDefaultAsync(x => x.IsDeleted == false && x.StudentId == educationTracking.StudentId && x.ReasonType == ReasonType.BeginningToAssignedInstitution && x.ProcessDate < educationTracking.ProcessDate, cancellationToken);
                        var leavingForAssignedIns = await educationTrackingRepository.GetByIdAsync(cancellationToken, (long)beginningToAssignedIns.RelatedEducationTrackingId);
                        leavingForAssignedIns.AdditionalDays = (educationTracking.ProcessDate?.Date - beginningToAssignedIns.ProcessDate?.Date)?.Days;
                        beginningToAssignedIns.AdditionalDays = (educationTracking.ProcessDate?.Date - beginningToAssignedIns.ProcessDate?.Date)?.Days;
                        educationTrackingRepository.Update(leavingForAssignedIns);
                        educationTrackingRepository.Update(beginningToAssignedIns);
                    }
                    else
                    {
                        if (educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment)
                            student.Status = StudentStatus.Assigned;
                        if (educationTracking.ProcessType == ProcessType.Transfer)
                        {
                            student.OriginalProgramId = educationTracking.ProgramId;
                            if (student.Status == StudentStatus.TransferDueToNegativeOpinion)
                                student.TransferredDueToOpinion = true;
                        }
                        student.ProgramId = educationTracking.ProgramId;
                    }

                    EducationTrackingDTO startETT = new()
                    {
                        StudentId = student.Id,
                        ProcessType = ProcessType.Start,
                        ReasonType = educationTracking.ReasonType,
                        ExcusedType = educationTracking.ExcusedType,
                        FormerProgramId = student.ProgramId,
                        AssignmentType = educationTracking.AssignmentType,
                        ProgramId = educationTracking.ProgramId
                    };

                    if (educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment)
                        startETT.ReasonType = ReasonType.BeginningToAssignedInstitution;

                    if (educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                        startETT.ReasonType = ReasonType.BeginningToOwnInstitutionAfterAssignment;

                    var addedStartETT = await educationTrackingRepository.AddAsync(cancellationToken, mapper.Map<EducationTracking>(startETT));


                    #region NegativeOpinion
                    if (educationTracking.ExcusedType == ExcusedType.NegativeOpinion)
                    {
                        EducationTrackingDTO timeIncreasingETT = new()
                        {
                            StudentId = student.Id,
                            ProcessType = ProcessType.TimeIncreasing,
                            ReasonType = ReasonType.RelocationAndTimeExtensionDueToNegativeOpinion,
                            ProcessDate = educationTracking.ProcessDate,
                            AdditionalDays = 365,
                            AssignmentType = educationTracking.AssignmentType,
                            ProgramId = educationTracking.ProgramId
                        };

                        await educationTrackingRepository.AddAsync(cancellationToken, mapper.Map<EducationTracking>(timeIncreasingETT));
                        estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddYears(1);
                    }
                    #endregion

                    await unitOfWork.CommitAsync(cancellationToken);

                    educationTracking.RelatedEducationTrackingId = addedStartETT.Id;
                }
                #endregion

                #region EducationAbroad
                if (educationTracking.AssignmentType == AssignmentType.EducationAbroad)
                    student.Status = StudentStatus.Abroad;
                if (educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad)
                {
                    var leavingForEducationAbroad = await educationTrackingRepository.Queryable().OrderByDescending(x => x.ProcessDate).FirstOrDefaultAsync(x => x.StudentId == educationTracking.StudentId && x.IsDeleted == false && x.AssignmentType == AssignmentType.EducationAbroad);
                    educationTracking.RelatedEducationTrackingId = leavingForEducationAbroad?.Id;
                }
                #endregion

                #region InCaseOfHealthReport
                if (educationTracking.ReasonType == ReasonType.HealthReport)
                {
                    var conflictingRecord = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.IsDeleted == false && x.ReasonType == ReasonType.HealthReport && x.StudentId == educationTracking.StudentId && x.ProcessDate.Value.AddDays(x.AdditionalDays ?? 0) > educationTracking.ProcessDate && x.ProcessDate <= educationTracking.ProcessDate);
                    if (conflictingRecord != null)
                    {
                        estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays((int)(educationTracking.ProcessDate?.Date - conflictingRecord.ProcessDate?.Date.AddDays(conflictingRecord.AdditionalDays ?? 0))?.Days);
                        conflictingRecord.PreviousAdditionalDays = conflictingRecord.AdditionalDays;
                        conflictingRecord.PreviousDescription = conflictingRecord.Description;
                        conflictingRecord.AdditionalDays = (educationTracking.ProcessDate?.Date - conflictingRecord.ProcessDate?.Date)?.Days;
                        conflictingRecord.Description += $" (Rapor tarihi içerisinde ikinci bir sağlık raporu girildiği için, {educationTracking.ProcessDate?.ToString("dd.MM.yyyy")} itibariyle rapor sonlandırılmıştır.)";
                        educationTrackingRepository.Update(conflictingRecord);
                    }
                }
                #endregion

                #region TimeIncreasingOrDecreasing
                if (educationTracking.AdditionalDays != null && (educationTracking.ProcessType == ProcessType.TimeIncreasing || educationTracking.ProcessType == ProcessType.TimeDecreasing))
                {
                    estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(Convert.ToDouble(educationTracking.ProcessType == ProcessType.TimeIncreasing ? educationTracking.AdditionalDays : -educationTracking.AdditionalDays));

                    //aşağıdaki nedenler dışında süre uzatma girildiğinde, kanaat formunun bitiş tarihi uzar
                    if (educationTracking.ProcessType == ProcessType.TimeIncreasing && educationTrackingDTO.ReasonType != ReasonType.RelocationAndTimeExtensionDueToNegativeOpinion && educationTrackingDTO.ReasonType != ReasonType.ExtensionByApplicationForSubjection && educationTrackingDTO.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation)
                        await opinionFormRepository.ChangeDateByTimeIncreasing(cancellationToken, educationTrackingDTO);

                    await studentRotationRepository.CheckStudentRotations(cancellationToken, educationTrackingDTO);
                }
                #endregion

                #region BranchChange
                if (educationTracking.ReasonType == ReasonType.BranchChange_End)
                {
                    var program = await programRepository.GetByIdAsync(cancellationToken, educationTracking.ProgramId ?? 0);
                    var curriculum = await curriculumRepository.Queryable().OrderByDescending(x => x.EffectiveDate).FirstOrDefaultAsync(x => x.IsDeleted == false && x.ExpertiseBranchId == program.ExpertiseBranchId && x.EffectiveDate <= educationTracking.ProcessDate, cancellationToken);
                    if (curriculum == null)
                        return new() { Result = false, Message = "Uygun müfredat bulunamadı. Sistem yöneticisi ile iletişime geçiniz!" };
                    educationTracking.FormerProgramId = student.OriginalProgramId;
                    educationTrackingRepository.SoftDelete(estimatedFinish);
                    student.IsDeleted = true;
                    student.DeleteDate = educationTracking.ProcessDate;
                    student.DeleteReason = ReasonType.BranchChange_End.ToString();
                    student.DeleteReasonType = ReasonType.BranchChange;

                    StudentDTO newStudent = new()
                    {
                        UserId = student.UserId,
                        ProgramId = educationTracking.ProgramId,
                        OriginalProgramId = educationTracking.ProgramId,
                        CurriculumId = curriculum.Id,
                        Status = StudentStatus.EducationContinues,
                        GraduatedDate = student.GraduatedDate,
                        GraduatedSchool = student.GraduatedSchool,
                        MedicineRegistrationDate = student.MedicineRegistrationDate,
                        MedicineRegistrationNo = student.MedicineRegistrationNo,
                        EducationTrackings = new() {
                            new() { ProcessType = ProcessType.Start, ReasonType = ReasonType.BranchChange, ProcessDate = educationTracking.ProcessDate?.Date, ProgramId = educationTracking.ProgramId },
                            new() { ProcessType = ProcessType.EstimatedFinish, ReasonType = ReasonType.EstimatedFinish, ProcessDate = educationTracking.ProcessDate?.Date.AddYears(curriculum.Duration ?? 0) }
                        },
                        StudentDependentPrograms = new()
                    };

                    var dependentPrograms = await dependentProgramRepository.GetAsync(cancellationToken, x => x.RelatedDependentProgram.IsActive == true && x.RelatedDependentProgram.ProtocolProgram.CancelingProtocolNo == null && x.RelatedDependentProgram.ProtocolProgram.ParentProgramId == newStudent.OriginalProgramId);

                    //program protokollü ise bağlı programları eklenir
                    if (dependentPrograms != null && dependentPrograms.Count > 0)
                        foreach (var item in dependentPrograms)
                            newStudent.StudentDependentPrograms.Add(new() { DependentProgramId = item.Id });
                    var dto = mapper.Map<Student>(newStudent);
                    await studentRepository.AddAsync(cancellationToken, dto);
                }
                #endregion

                #region SetStudentStatusToEducationContinues
                if (student.Status == StudentStatus.TransferDueToNegativeOpinion || educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad || educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                    student.Status = StudentStatus.EducationContinues;
                #endregion

                educationTrackingRepository.Update(estimatedFinish);
            }

            studentRepository.Update(student);

            await educationTrackingRepository.AddAsync(cancellationToken, educationTracking);
            await unitOfWork.CommitAsync(cancellationToken);

            if (educationTracking.RelatedEducationTrackingId != null)
            {
                var relatedTracking = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.Id == educationTracking.RelatedEducationTrackingId);
                relatedTracking.RelatedEducationTrackingId = educationTracking.Id;
                educationTrackingRepository.Update(relatedTracking);
                await unitOfWork.CommitAsync(cancellationToken);
            }

            var response = mapper.Map<EducationTrackingResponseDTO>(educationTracking);

            await educationTrackingRepository.CheckEstimatedFinish(educationTrackingDTO.StudentId ?? 0, cancellationToken);

            return new ResponseWrapper<EducationTrackingResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> Put(CancellationToken cancellationToken, long id, EducationTrackingDTO educationTrackingDTO)
        {
            EducationTracking educationTracking = await educationTrackingRepository.GetByIdAsync(cancellationToken, id);

            var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.StudentId == educationTracking.StudentId && x.IsDeleted == false && x.ProcessType == ProcessType.EstimatedFinish);

            if (educationTracking.ProcessType == ProcessType.Start && educationTrackingDTO.ProcessDate != educationTracking.ProcessDate)
            {
                int diff;
                if (educationTracking.ProcessDate == null)
                {
                    var relatedTracking = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.IsDeleted == false && x.RelatedEducationTrackingId == educationTracking.Id && x.StudentId == educationTracking.StudentId);
                    diff = (educationTrackingDTO.ProcessDate.Value.Date - relatedTracking.ProcessDate.Value.Date).Days;
                }
                else
                    diff = (educationTrackingDTO.ProcessDate.Value.Date - educationTracking.ProcessDate.Value.Date).Days;

                if (educationTracking.ProcessType == ProcessType.Start && educationTracking.ReasonType != ReasonType.BeginningToAssignedInstitution && educationTracking.ReasonType != ReasonType.BeginningToOwnInstitutionAfterAssignment)
                {
                    var diffDay = (educationTrackingDTO?.ProcessDate?.Date - educationTracking.ProcessDate?.Date)?.Days;
                    await opinionFormRepository.ChangeDateByTimeIncreasing(cancellationToken, mapper.Map<EducationTrackingDTO>(educationTracking), diffDay ?? 0);
                }

                estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(diff);
            }
            else if (educationTracking.ReasonType != ReasonType.LeavingTheInstitutionDueToAssignment)
            {
                if (educationTracking.AdditionalDays != null && educationTrackingDTO.AdditionalDays != null && educationTrackingDTO.AdditionalDays != educationTracking.AdditionalDays)
                {
                    if (educationTracking.ProcessType != ProcessType.Information)
                    {

                        var diff = Convert.ToDouble(educationTrackingDTO.AdditionalDays - educationTracking.AdditionalDays);

                        estimatedFinish.ProcessDate = educationTracking.ProcessType == ProcessType.TimeIncreasing ? estimatedFinish.ProcessDate?.AddDays(diff) : estimatedFinish.ProcessDate?.AddDays(-diff);

                        if (educationTrackingDTO.ReasonType != ReasonType.RelocationAndTimeExtensionDueToNegativeOpinion && educationTrackingDTO.ReasonType != ReasonType.ExtensionByApplicationForSubjection && educationTrackingDTO.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation)
                            await opinionFormRepository.ChangeDateByTimeIncreasing(cancellationToken, educationTrackingDTO, (int)diff);
                    }
                }
                else if (educationTrackingDTO.AdditionalDays != educationTracking.AdditionalDays)
                {
                    var diff = Convert.ToDouble(educationTrackingDTO.AdditionalDays ?? 0 - educationTracking.AdditionalDays ?? 0);
                    estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(diff);

                }
            }

            if ((educationTracking.ReasonType == ReasonType.Rotation || educationTracking.ReasonType == ReasonType.TimeExtensionDueToFailureInRotation) && educationTracking.ReasonType != educationTrackingDTO.ReasonType)
            {
                if (educationTrackingDTO.ReasonType == ReasonType.Rotation)
                    estimatedFinish.ProcessDate = estimatedFinish.ProcessDate.Value.Date.AddDays(-(int)educationTrackingDTO.AdditionalDays);
                else
                    estimatedFinish.ProcessDate = estimatedFinish.ProcessDate.Value.Date.AddDays((int)educationTrackingDTO.AdditionalDays);
            }

            EducationTracking updatedEducationTracking = mapper.Map(educationTrackingDTO, educationTracking);

            educationTrackingRepository.Update(estimatedFinish);
            educationTrackingRepository.Update(updatedEducationTracking);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<EducationTrackingResponseDTO>(updatedEducationTracking);

            return new ResponseWrapper<EducationTrackingResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            EducationTracking educationTracking = await educationTrackingRepository.GetByIdAsync(cancellationToken, id);

            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)educationTracking.StudentId);
            var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.StudentId == educationTracking.StudentId && x.IsDeleted == false && x.ProcessType == ProcessType.EstimatedFinish);

            if (estimatedFinish != null)
            {
                if (educationTracking.ProcessType == ProcessType.TimeIncreasing || educationTracking.ProcessType == ProcessType.TimeDecreasing)
                {
                    if (educationTracking.ReasonType == ReasonType.HealthReport)
                    {
                        var conflictingRecord = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.ReasonType == ReasonType.HealthReport && x.IsDeleted == false && x.Id != educationTracking.Id && x.StudentId == educationTracking.StudentId && x.ProcessDate.Value.AddDays(x.AdditionalDays ?? 0) >= educationTracking.ProcessDate && x.ProcessDate <= educationTracking.ProcessDate);
                        if (conflictingRecord != null)
                        {
                            estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays((int)(educationTracking.AdditionalDays - conflictingRecord.AdditionalDays));
                            educationTrackingRepository.Update(estimatedFinish);
                            conflictingRecord.AdditionalDays = conflictingRecord.PreviousAdditionalDays;
                            conflictingRecord.Description = conflictingRecord.PreviousDescription;
                            conflictingRecord.PreviousAdditionalDays = null;
                            conflictingRecord.PreviousDescription = null;
                            educationTrackingRepository.Update(conflictingRecord);
                        }
                    }
                    var diff = Convert.ToDouble(educationTracking.AdditionalDays);
                    estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(educationTracking.ProcessType == ProcessType.TimeIncreasing ? -diff : diff);

                    if (educationTracking.ReasonType != ReasonType.RelocationAndTimeExtensionDueToNegativeOpinion && educationTracking.ReasonType != ReasonType.ExtensionByApplicationForSubjection && educationTracking.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation)
                        await opinionFormRepository.ChangeDateByTimeIncreasing(cancellationToken, mapper.Map<EducationTrackingDTO>(educationTracking), -(int)diff);
                }

                //if (educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToRotation || educationTracking.ReasonType == ReasonType.CompletionOfRotation || educationTracking.ReasonType == ReasonType.TimeExtensionDueToFailureInRotation)
                //{
                //    var relatedTracking = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.RelatedEducationTrackingId == educationTracking.Id && x.IsDeleted == false);
                //    if (relatedTracking != null)
                //    {
                //        educationTrackingRepository.SoftDelete(relatedTracking);
                //        if (relatedTracking.ReasonType == ReasonType.TimeExtensionDueToFailureInRotation)
                //        {
                //            var diff = Convert.ToDouble(educationTracking.AdditionalDays);
                //            estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(-diff);
                //            educationTrackingRepository.Update(estimatedFinish);
                //        }
                //    }
                //}

                if (educationTracking.ProcessType == ProcessType.Transfer || educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment || educationTracking.ReasonType == ReasonType.CompletionOfAssignment || educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad || educationTracking.ReasonType == ReasonType.KKTCHalfTimed)
                {
                    var relatedTracking = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.RelatedEducationTrackingId == educationTracking.Id && x.IsDeleted == false);

                    if (relatedTracking != null)
                        educationTrackingRepository.SoftDelete(relatedTracking);

                    if (relatedTracking?.ProcessDate != null && educationTracking.ProcessDate != null)
                    {
                        estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays((educationTracking.ProcessDate?.Date - relatedTracking.ProcessDate?.Date)?.Days ?? 0);
                        educationTrackingRepository.Update(estimatedFinish);
                    }

                    if (educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment)
                        student.Status = StudentStatus.EducationContinues;
                    else if (educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                        student.Status = StudentStatus.Assigned;
                    else if (educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad)
                        student.Status = StudentStatus.Abroad;
                    else if (educationTracking.ProcessType == ProcessType.Transfer)
                    {
                        student.OriginalProgramId = educationTracking.FormerProgramId;
                        if (educationTracking.ExcusedType == ExcusedType.NegativeOpinion)
                        {
                            student.Status = StudentStatus.TransferDueToNegativeOpinion;
                            var ett = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.IsDeleted == false && x.ReasonType == ReasonType.RelocationAndTimeExtensionDueToNegativeOpinion && x.StudentId == educationTracking.StudentId);
                            educationTrackingRepository.SoftDelete(ett);
                            estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddYears(-1);
                        }
                    }
                    student.ProgramId = educationTracking.FormerProgramId;
                    studentRepository.Update(student);
                }
            }

            educationTrackingRepository.SoftDelete(educationTracking);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<EducationTrackingResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> SendStudentToDependentProgram(CancellationToken cancellationToken, long studentDependentProgramId, StudentDependentProgramPaginateDTO studentDependentProgramDTO)
        {
            var datelessRecord = await educationTrackingRepository.AnyAsync(cancellationToken, x => x.IsDeleted == false && x.StudentId == studentDependentProgramDTO.StudentId && x.ProcessType == ProcessType.Start && x.AssignmentType == AssignmentType.UnderProtocolProgram && x.ProcessDate == null);
            if (!datelessRecord)
            {
                var student = await studentRepository.GetByIdAsync(cancellationToken, (long)studentDependentProgramDTO.StudentId);
                var dependentProgram = await dependentProgramRepository.GetByIdAsync(cancellationToken, (long)studentDependentProgramDTO.DependentProgramId);

                var educationTracking = await educationTrackingRepository.AddAsync(cancellationToken, new()
                {
                    StudentId = studentDependentProgramDTO.StudentId,
                    ProcessType = ProcessType.Assignment,
                    ProcessDate = studentDependentProgramDTO.StartDate,
                    ReasonType = ReasonType.LeavingTheInstitutionDueToAssignment,
                    FormerProgramId = student.OriginalProgramId,
                    AssignmentType = AssignmentType.UnderProtocolProgram,
                    ProgramId = dependentProgram.ProgramId,
                    AdditionalDays = dependentProgram.Duration * 30
                });

                var studentDependentProgram = mapper.Map<StudentDependentProgram>(studentDependentProgramDTO);
                studentDependentProgram.IsActive = true;
                studentDependentProgram.IsUnCompleted = false;
                studentDependentProgram.EndDate = null;
                studentDependentProgram.RemainingDays = null;
                studentDependentProgramRepository.Update(studentDependentProgram);

                await unitOfWork.CommitAsync(cancellationToken);

                var educationTracking_2 = await educationTrackingRepository.AddAsync(cancellationToken, new()
                {
                    StudentId = studentDependentProgramDTO.StudentId,
                    ProcessType = ProcessType.Start,
                    ReasonType = ReasonType.BeginningToAssignedInstitution,
                    FormerProgramId = student.OriginalProgramId,
                    AssignmentType = AssignmentType.UnderProtocolProgram,
                    ProgramId = dependentProgram.ProgramId,
                    RelatedEducationTrackingId = educationTracking.Id
                });
                student.ProgramId = dependentProgram.ProgramId;
                student.ProtocolProgramId = dependentProgram.ProgramId;
                studentRepository.Update(student);
                await unitOfWork.CommitAsync(cancellationToken);

                educationTracking.RelatedEducationTrackingId = educationTracking_2.Id;
                educationTrackingRepository.Update(educationTracking);

                await unitOfWork.CommitAsync(cancellationToken);
                return new() { Item = new(), Result = true };
            }
            else
                return new() { Result = false, Message = "Eğitim süre takibinde işlem tarihi girilmemiş kayıt bulunmaktadır. Tarihi girilmemiş kayıtlar en üstte görünür." };
        }

        public async Task<ResponseWrapper<int>> GetRemainingDaysForDependentProgram(CancellationToken cancellationToken, StudentDependentProgramPaginateDTO studentDependentProgramDTO)
        {
            return new() { Result = true, Item = await educationTrackingRepository.GetRemainingDaysForDependentProgram(cancellationToken, studentDependentProgramDTO) };
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> ReturnToMainProgramInProtocol(CancellationToken cancellationToken, long studentDependentProgramId, StudentDependentProgramPaginateDTO studentDependentProgramDTO)
        {
            var datelessRecord = await educationTrackingRepository.AnyAsync(cancellationToken, x => x.IsDeleted == false && x.StudentId == studentDependentProgramDTO.StudentId && x.ProcessType == ProcessType.Start && x.AssignmentType == AssignmentType.UnderProtocolProgram && x.ProcessDate == null);
            if (!datelessRecord)
            {
                var remainingDays = await educationTrackingRepository.GetRemainingDaysForDependentProgram(cancellationToken, studentDependentProgramDTO);
                var student = await studentRepository.GetByIdAsync(cancellationToken, (long)studentDependentProgramDTO.StudentId);
                var studentDependentProgram = mapper.Map<StudentDependentProgram>(studentDependentProgramDTO);
                if (studentDependentProgramDTO.IsUnCompleted == true)
                    studentDependentProgram.RemainingDays = remainingDays;
                else
                    studentDependentProgram.IsCompleted = true;

                studentDependentProgram.IsActive = false;
                studentDependentProgramRepository.Update(studentDependentProgram);

                var educationTracking = await educationTrackingRepository.AddAsync(cancellationToken, new()
                {
                    StudentId = studentDependentProgramDTO.StudentId,
                    ProcessType = ProcessType.Assignment,
                    ProcessDate = studentDependentProgramDTO.EndDate,
                    ReasonType = ReasonType.CompletionOfAssignment,
                    ProgramId = student.OriginalProgramId,
                    AssignmentType = AssignmentType.UnderProtocolProgram,
                    FormerProgramId = student.ProgramId,
                    AdditionalDays = remainingDays
                });

                await unitOfWork.CommitAsync(cancellationToken);


                var educationTracking_2 = await educationTrackingRepository.AddAsync(cancellationToken, new()
                {
                    StudentId = studentDependentProgramDTO.StudentId,
                    ProcessType = ProcessType.Start,
                    ReasonType = ReasonType.BeginningToOwnInstitutionAfterAssignment,
                    ProgramId = student.OriginalProgramId,
                    AssignmentType = AssignmentType.UnderProtocolProgram,
                    FormerProgramId = student.ProgramId,
                    RelatedEducationTrackingId = educationTracking.Id
                });
                student.ProgramId = student.OriginalProgramId;
                student.ProtocolProgramId = null;
                studentRepository.Update(student);
                await unitOfWork.CommitAsync(cancellationToken);

                educationTracking.RelatedEducationTrackingId = educationTracking_2.Id;
                educationTrackingRepository.Update(educationTracking);

                await unitOfWork.CommitAsync(cancellationToken);
                return new() { Item = new(), Result = true };
            }
            else
                return new() { Result = false, Message = "Eğitim süre takibinde işlem tarihi girilmemiş kayıt bulunmaktadır. Tarihi girilmemiş kayıtlar en üstte görünür." };
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> GetEducationStartByStudentId(CancellationToken cancellationToken, long studentId)
        {
            return new() { Result = true, Item = mapper.Map<EducationTrackingResponseDTO>(await educationTrackingRepository.GetByAsync(cancellationToken, x => x.StudentId == studentId && x.IsDeleted == false && x.ReasonType == ReasonType.BeginningSpecializationEducation)) };
        }

        public async Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetTimeIncreasingRecordsByDate(CancellationToken cancellationToken, OpinionFormRequestDTO opinionForm)
        {
            return new() { Result = true, Item = mapper.Map<List<EducationTrackingResponseDTO>>(await educationTrackingRepository.GetTimeIncreasingRecordsByDate(cancellationToken, opinionForm)) };
        }
    }
}
