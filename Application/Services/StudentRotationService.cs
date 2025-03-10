using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class StudentRotationService : BaseService, IStudentRotationService
    {
        private readonly IMapper mapper;
        private readonly IStudentRotationRepository studentRotationRepository;
        private readonly IRotationRepository rotationRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IEducationTrackingRepository educationTrackingRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IOpinionFormRepository opinionFormRepository;

        public StudentRotationService(IMapper mapper, IUnitOfWork unitOfWork, IStudentRotationRepository studentRotationRepository, IDocumentRepository documentRepository, IEducationTrackingRepository educationTrackingRepository, IStudentRepository studentRepository, IRotationRepository rotationRepository, IOpinionFormRepository opinionFormRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studentRotationRepository = studentRotationRepository;
            this.documentRepository = documentRepository;
            this.educationTrackingRepository = educationTrackingRepository;
            this.studentRepository = studentRepository;
            this.rotationRepository = rotationRepository;
            this.opinionFormRepository = opinionFormRepository;
        }
        public async Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            List<StudentRotationResponseDTO> response = studentRotationRepository.GetListByStudentIdQuery(studentId).Select(x => new StudentRotationResponseDTO
            {
                Id = x.Id,
                BeginDate = x.BeginDate,
                EndDate = x.EndDate,
                Educator = mapper.Map<EducatorResponseDTO>(x.Educator),
                EducatorId = x.EducatorId,
                IsSuccessful = x.IsSuccessful,
                Student = mapper.Map<StudentResponseDTO>(x.Student),
                StudentId = x.StudentId,
                Program = mapper.Map<ProgramResponseDTO>(x.Program),
                ProgramId = x.ProgramId,
                Rotation = mapper.Map<RotationResponseDTO>(x.Rotation),
                RotationId = x.RotationId,
                IsUncompleted = x.IsUncompleted,
                RemainingDays = x.RemainingDays,
                EducatorName = x.EducatorName,
                ProcessDateForExemption = x.ProcessDateForExemption,
                StudentRotationPerfections = x.StudentRotationPerfections != null ? x.StudentRotationPerfections.Select(y => new StudentRotationPerfectionResponseDTO
                {
                    Id = y.Id,
                    EducatorId = y.EducatorId,
                    StudentRotationId = y.StudentRotationId,
                    Perfection = y.Perfection != null ? new PerfectionResponseDTO
                    {
                        Id = y.Perfection.Id,
                        PerfectionType = y.Perfection.PerfectionType,
                        PName = mapper.Map<PropertyResponseDTO>(y.Perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName)),
                        LevelList = mapper.Map<List<PropertyResponseDTO>>(y.Perfection.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionLevel).ToList())
                    } : null,
                    PerfectionId = y.PerfectionId,
                    ProcessDate = y.ProcessDate,
                    IsSuccessful = y.IsSuccessful,
                    Educator = new EducatorResponseDTO { User = new UserAccountDetailInfoResponseDTO { Name = y.Educator.User.Name } }
                }).ToList() : null,
            }).ToList();

            return new ResponseWrapper<List<StudentRotationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerStudentsListByUserId(CancellationToken cancellationToken, long userId)
        {
            var response = studentRotationRepository.GetFormerStudentsListByUserId(cancellationToken, userId)?.Select(x => new StudentRotationResponseDTO
            {
                Id = x.Id,
                BeginDate = x.BeginDate,
                EndDate = x.EndDate,
                Educator = mapper.Map<EducatorResponseDTO>(x.Educator),
                EducatorId = x.EducatorId,
                IsSuccessful = x.IsSuccessful,
                Student = mapper.Map<StudentResponseDTO>(x.Student),
                StudentId = x.StudentId,
                Program = mapper.Map<ProgramResponseDTO>(x.Program),
                ProgramId = x.ProgramId,
                Rotation = mapper.Map<RotationResponseDTO>(x.Rotation),
                RotationId = x.RotationId,
                EducatorName = x.EducatorName,
                StudentRotationPerfections = x.StudentRotationPerfections.Select(y => new StudentRotationPerfectionResponseDTO
                {
                    Perfection = y.Perfection != null ? new PerfectionResponseDTO
                    {
                        Id = y.Perfection.Id,
                        PerfectionType = y.Perfection.PerfectionType,
                        PName = mapper.Map<PropertyResponseDTO>(y.Perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName)),
                        LevelList = mapper.Map<List<PropertyResponseDTO>>(y.Perfection.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionLevel).ToList())
                    } : null,
                    PerfectionId = y.PerfectionId,
                    ProcessDate = y.ProcessDate,
                    IsSuccessful = y.IsSuccessful,
                    Educator = new EducatorResponseDTO { User = new UserAccountDetailInfoResponseDTO { Name = y.Educator.User.Name } }
                }).ToList(),
            }).ToList();

            return new ResponseWrapper<List<StudentRotationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            StudentRotation studentRotation = await studentRotationRepository.GetIncluding(cancellationToken, x => x.Id == id && x.IsDeleted == false, x => x.Student, x => x.Rotation, x => x.Program.Hospital.Province, x => x.Program.ExpertiseBranch, x => x.Educator.User, x => x.StudentRotationPerfections);
            StudentRotationResponseDTO response = mapper.Map<StudentRotationResponseDTO>(studentRotation);

            response.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, studentRotation.Id, DocumentTypes.StudentRotation));

            return new ResponseWrapper<StudentRotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> GetByStudentAndRotationId(CancellationToken cancellationToken, long studentId, long rotationId)
        {
            StudentRotation studentRotation = await studentRotationRepository.GetWithSubRecords(cancellationToken, studentId, rotationId);

            var response = mapper.Map<StudentRotationResponseDTO>(studentRotation);
            var docs = await documentRepository.GetByEntityId(cancellationToken, studentRotation.Id, DocumentTypes.StudentRotation);

            var docsResponse = mapper.Map<List<DocumentResponseDTO>>(docs);
            if (studentRotation != null)
                response.Documents = docsResponse;

            return new ResponseWrapper<StudentRotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO)
        {
            StudentRotation studentRotation = mapper.Map<StudentRotation>(studentRotationDTO);

            await studentRotationRepository.AddAsync(cancellationToken, studentRotation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentRotationResponseDTO>(studentRotation);

            return new ResponseWrapper<StudentRotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentRotationDTO studentRotationDTO)
        {
            var studentRotation = await studentRotationRepository.GetByIdAsync(cancellationToken, id);

            StudentRotation updatedStudentRotation = mapper.Map(studentRotationDTO, studentRotation);

            studentRotationRepository.Update(updatedStudentRotation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentRotationResponseDTO>(updatedStudentRotation);

            return new ResponseWrapper<StudentRotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> GetEndDateByStartDate(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO)
        {
            var response = new StudentRotationResponseDTO() { BeginDate = studentRotationDTO.BeginDate };
            var rotation = await rotationRepository.GetByIdAsync(cancellationToken, studentRotationDTO.RotationId ?? 0);

            response.EndDate = response.BeginDate?.AddMonths(Convert.ToInt32(rotation.Duration) / 30).AddDays(-1);

            var timeIncreasings = await educationTrackingRepository.Queryable().OrderBy(x => x.ProcessDate).Where(x => x.IsDeleted == false && x.StudentId == studentRotationDTO.StudentId && x.ProcessType == ProcessType.TimeIncreasing && x.ProcessDate >= response.BeginDate && x.ReasonType != ReasonType.ExtensionByApplicationForSubjection && x.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation && x.ReasonType != ReasonType.ForceMajeure && x.ReasonType != ReasonType.DueToUnusualCircumstances).ToListAsync(cancellationToken);

            if (timeIncreasings != null)
                foreach (var item in timeIncreasings)
                    if (item.ProcessDate <= response.EndDate)
                        response.EndDate = response.EndDate?.AddDays(item.AdditionalDays ?? 0);

            return new ResponseWrapper<StudentRotationResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            StudentRotation studentRotation = await studentRotationRepository.GetByIdAsync(cancellationToken, id);

            var educationTrackings = await educationTrackingRepository.GetAsync(cancellationToken, x => x.StudentRotationId == id);

            if (educationTrackings != null)
                for (int i = 0; i < educationTrackings.Count; i++)
                    educationTrackingRepository.SoftDelete(educationTrackings[i]);

            studentRotationRepository.SoftDelete(studentRotation);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentRotationResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> SendStudentToRotation(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO)
        {
            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)studentRotationDTO.StudentId);
            var studentRotation = mapper.Map<StudentRotation>(studentRotationDTO);
            StudentRotation addedStudentRotation = new();
            if (studentRotation.Id == 0)
                addedStudentRotation = studentRotationRepository.Add(studentRotation);
            else
            {
                studentRotation.IsUncompleted = null;
                studentRotation.PreviousEndDate = studentRotation.EndDate?.Date;
                studentRotation.EndDate = null;
                studentRotationRepository.Update(studentRotation);
                addedStudentRotation = studentRotation;
            }

            await unitOfWork.CommitAsync(cancellationToken);

            var rotation = await rotationRepository.GetIncluding(cancellationToken, x => x.Id == addedStudentRotation.RotationId, x => x.ExpertiseBranch);

            var educationTracking = new EducationTracking
            {
                StudentId = addedStudentRotation.StudentId,
                ProgramId = addedStudentRotation.ProgramId,
                FormerProgramId = student.ProgramId,
                ProcessDate = addedStudentRotation.BeginDate,
                StudentRotationId = addedStudentRotation.Id,
                Description = rotation.ExpertiseBranch.Name,
                AdditionalDays = addedStudentRotation.RemainingDays != null ? addedStudentRotation.RemainingDays : Convert.ToInt32(rotation.Duration),
                ProcessType = ProcessType.Information,
                ReasonType = ReasonType.LeavingTheInstitutionDueToRotation
            };
            educationTrackingRepository.Add(educationTracking);

            student.ProgramId = addedStudentRotation.ProgramId;
            student.Status = StudentStatus.Rotation;

            studentRepository.Update(student);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true, Item = mapper.Map<StudentRotationResponseDTO>(addedStudentRotation) };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> FinishStudentsRotation(CancellationToken cancellationToken, long id, StudentRotationDTO studentRotationDTO)
        {
            var rotation = await rotationRepository.GetByIdAsync(cancellationToken, studentRotationDTO.RotationId ?? 0);

            if (Convert.ToInt32(rotation.Duration) > 90 || studentRotationDTO.RemainingDays > 90)
            {
                var checkOpinionFormNecessary = await studentRotationRepository.CheckOpinionForms(cancellationToken, studentRotationDTO);
                if (!checkOpinionFormNecessary.Result)
                    return new() { Result = false, Message = checkOpinionFormNecessary.Message };
            }

            var studentRotation = await studentRotationRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.Rotation.ExpertiseBranch);
            StudentRotation updatedStudentRotation = mapper.Map(studentRotationDTO, studentRotation);

            int remainingDays = 0;

            if (studentRotationDTO.IsUncompleted == true)
            {
                remainingDays = await studentRotationRepository.GetRemainingDays(cancellationToken, id, studentRotationDTO);
                updatedStudentRotation.RemainingDays = remainingDays;
            }

            studentRotationRepository.Update(updatedStudentRotation);

            var educationTracking = await educationTrackingRepository.Get(x => x.IsDeleted == false && x.StudentRotationId == id).OrderByDescending(x => x.ProcessDate).FirstOrDefaultAsync(cancellationToken);

            var newEducationTracking = new EducationTracking()
            {
                StudentId = studentRotation.StudentId,
                ProgramId = educationTracking?.FormerProgramId,
                FormerProgramId = studentRotation.ProgramId,
                ProcessDate = studentRotationDTO.EndDate,
                Description = studentRotation.Rotation.ExpertiseBranch.Name,
                AdditionalDays = (studentRotationDTO.EndDate?.Date - studentRotationDTO.BeginDate?.Date)?.Days,
                StudentRotationId = studentRotation.Id,
                RelatedEducationTrackingId = educationTracking?.Id,
                ProcessType = ProcessType.Information
            };

            if (studentRotationDTO.IsSuccessful == true)
                newEducationTracking.ReasonType = ReasonType.CompletionOfRotation;
            else if (studentRotationDTO.IsSuccessful == false)
            {
                newEducationTracking.ProcessType = ProcessType.TimeIncreasing;
                newEducationTracking.ReasonType = ReasonType.TimeExtensionDueToFailureInRotation;
            }
            else
                newEducationTracking.ReasonType = ReasonType.LeftWithoutCompletingRotation;

            if (newEducationTracking.ReasonType == ReasonType.TimeExtensionDueToFailureInRotation)
            {
                var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.IsDeleted == false && x.StudentId == studentRotation.StudentId && x.ReasonType == ReasonType.EstimatedFinish);
                estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.Date.AddDays(newEducationTracking.AdditionalDays ?? 0);
            }
            var addedEducationTracking = educationTrackingRepository.Add(newEducationTracking);
            await unitOfWork.CommitAsync(cancellationToken);

            educationTracking.RelatedEducationTrackingId = addedEducationTracking.Id;

            educationTrackingRepository.Update(educationTracking);

            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)studentRotationDTO.StudentId);

            student.Status = StudentStatus.EducationContinues;
            student.ProgramId = educationTracking.FormerProgramId;
            studentRepository.Update(student);

            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true, Item = mapper.Map<StudentRotationResponseDTO>(updatedStudentRotation) };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> AddPastRotation(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO)
        {
            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)studentRotationDTO.StudentId);

            var studentRotation = mapper.Map<StudentRotation>(studentRotationDTO);

            var rotation = await rotationRepository.GetIncluding(cancellationToken, x => x.Id == studentRotationDTO.RotationId, x => x.ExpertiseBranch);

            if (studentRotation.IsSuccessful == true)
            {
                studentRotation.StudentRotationPerfections = new List<StudentRotationPerfection>();
                var perfections = await studentRotationRepository.GetPerfectionListByCurrciulumAndRotationId(cancellationToken, (long)student.CurriculumId, (long)studentRotationDTO.RotationId);
                perfections.ForEach(x => studentRotation.StudentRotationPerfections.Add(new StudentRotationPerfection() { IsSuccessful = true, PerfectionId = x.Id }));
            }
            else
            {
                var estimatedFinish = await educationTrackingRepository.GetByAsync(cancellationToken, x => x.IsDeleted == false && x.StudentId == student.Id && x.ReasonType == ReasonType.EstimatedFinish);
                estimatedFinish.ProcessDate = estimatedFinish.ProcessDate?.AddDays(Convert.ToDouble(rotation.Duration));
                educationTrackingRepository.Update(estimatedFinish);
            }
            var addedStudentRotation = studentRotationRepository.Add(studentRotation);

            await unitOfWork.CommitAsync(cancellationToken);

            var educationTracking_1 = new EducationTracking
            {
                StudentId = addedStudentRotation.StudentId,
                ProgramId = addedStudentRotation.ProgramId,
                FormerProgramId = student.ProgramId,
                ProcessDate = addedStudentRotation.BeginDate,
                StudentRotationId = addedStudentRotation.Id,
                Description = rotation.ExpertiseBranch.Name + "(" + "Geçmiş Rotasyon" + ")",
                AdditionalDays = Convert.ToInt32(rotation.Duration),
                ProcessType = ProcessType.Information,
                ReasonType = ReasonType.LeavingTheInstitutionDueToRotation,
            };

            educationTrackingRepository.Add(educationTracking_1);
            await unitOfWork.CommitAsync(cancellationToken);

            var educationTracking_2 = new EducationTracking()
            {
                StudentId = studentRotation.StudentId,
                ProgramId = student?.ProgramId,
                FormerProgramId = studentRotation.ProgramId,
                ProcessDate = studentRotationDTO.EndDate,
                Description = rotation.ExpertiseBranch.Name + "(" + "Geçmiş Rotasyon" + ")",
                AdditionalDays = Convert.ToInt32(rotation.Duration),
                StudentRotationId = studentRotation.Id,
                ProcessType = studentRotation.IsSuccessful == true ? ProcessType.Information : ProcessType.TimeIncreasing,
                ReasonType = studentRotation.IsSuccessful == true ? ReasonType.CompletionOfRotation : ReasonType.TimeExtensionDueToFailureInRotation,
                RelatedEducationTrackingId = educationTracking_1.Id
            };

            educationTrackingRepository.Add(educationTracking_2);
            await unitOfWork.CommitAsync(cancellationToken);
            educationTracking_1.RelatedEducationTrackingId = educationTracking_2.Id;
            educationTrackingRepository.Update(educationTracking_1);
            return new() { Result = true, Item = mapper.Map<StudentRotationResponseDTO>(addedStudentRotation) };
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> DeleteActiveRotation(CancellationToken cancellationToken, long id)
        {

            // BU METHODA DÜŞMÜYOR SEBEBİNİ BUL



            StudentRotation studentRotation = await studentRotationRepository.GetByIdAsync(cancellationToken, id);

            if (studentRotation.RemainingDays == null)
                studentRotationRepository.SoftDelete(studentRotation);
            else
            {
                studentRotation.IsUncompleted = true;
                studentRotation.EndDate = studentRotation.PreviousEndDate?.Date;
                studentRotationRepository.Update(studentRotation);
            }
            var leavingInstitution = await educationTrackingRepository.Get(x => x.IsDeleted == false && x.StudentRotationId == id).OrderByDescending(x => x.ProcessDate).FirstOrDefaultAsync(cancellationToken);

            educationTrackingRepository.SoftDelete(leavingInstitution);

            var student = await studentRepository.GetByIdAsync(cancellationToken, (long)studentRotation.StudentId);

            student.Status = StudentStatus.EducationContinues;
            student.ProgramId = leavingInstitution.FormerProgramId;
            studentRepository.Update(student);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentRotationResponseDTO> { Result = true };
        }
    }
}
