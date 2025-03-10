using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Extentsions;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Types;
using Infrastructure.Data;
using Shared.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Castle.Core.Logging;

namespace Application.Services
{
    public class ExitExamService : BaseService, IExitExamService
    {
        private readonly IMapper mapper;
        private readonly IExitExamRepository exitExamRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ExitExamService(IMapper mapper, IUnitOfWork unitOfWork, IExitExamRepository exitExamRepository, IHttpContextAccessor httpContextAccessor, IStudentRepository studentRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.exitExamRepository = exitExamRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.studentRepository = studentRepository;
        }
        public async Task<PaginationModel<ExitExamResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            var asdsadas = exitExamRepository.Queryable().Select(x => new ExitExamResponseDTO()
            {
                Id = x.Id,
                AbilityExamNote = x.AbilityExamNote,
                PracticeExamNote = x.PracticeExamNote,
                Description = x.Description,
                EducationTrackingId = x.EducationTrackingId,
                ExamDate = x.ExamDate,
                ExamStatus = x.ExamStatus,
                HospitalId = x.HospitalId,
                StudentId = x.StudentId,
                SecretaryId = x.SecretaryId,
                IsDeleted = x.IsDeleted,
                Secretary = x.Secretary == null ? null : new UserResponseDTO() { Id = x.Secretary.Id, Name = x.Secretary.Name },
                Hospital = x.Hospital == null ? null : new HospitalResponseDTO()
                {
                    Id = x.Hospital.Id,
                    Name = x.Hospital.Name,
                },
                EducationTracking = x.EducationTracking == null ? null : new EducationTrackingResponseDTO()
                {
                    Id = x.EducationTracking.Id,
                    ReasonType = x.EducationTracking.ReasonType,
                    ProcessDate = x.EducationTracking.ProcessDate,
                    Description = x.EducationTracking.Description,
                    StudentId = x.EducationTracking.Student.Id

                },
                Juries = x.Juries.Where(x => !x.IsDeleted).Select(x => new JuryResponseDTO()
                {
                    Id = x.Id,
                    ExitExamId = x.ExitExamId,
                    EducatorId = x.EducatorId,
                    JuryType = x.JuryType,
                    User = x.User == null ? null : new UserResponseDTO() { Name = x.User.Name, Id = x.User.Id},
                    Educator = x.Educator == null ? null : new EducatorResponseDTO()
                    {
                        Id = x.Educator.Id,
                        AcademicTitleId = x.Educator.AcademicTitleId,
                        AcademicTitle = x.Educator.AcademicTitle == null ? null : new TitleResponseDTO() { Id = x.Educator.AcademicTitle.Id, Name = x.Educator.AcademicTitle.Name },
                        UserId = x.Educator.UserId,
                        User = x.Educator.User == null ? null : new UserAccountDetailInfoResponseDTO() { Id = x.Educator.User.Id, Name = x.Educator.User.Name },
                        EducatorExpertiseBranches = x.Educator.EducatorExpertiseBranches.Select(x => new EducatorExpertiseBranchResponseDTO()
                        {
                            Id = x.Id,
                            EducatorId = x.EducatorId,
                            ExpertiseBranchId = x.ExpertiseBranchId,
                            ExpertiseBranch = x.ExpertiseBranch == null ? null : new ExpertiseBranchResponseDTO()
                            {
                                Id = x.ExpertiseBranch.Id,
                                Name = x.ExpertiseBranch.Name,
                            }
                        }).ToList(),
                    }
                }).ToList()

            });
            FilterResponse<ExitExamResponseDTO> filterResponse = asdsadas.ToFilterView(filter);

            var exitExams = mapper.Map<List<ExitExamResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<ExitExamResponseDTO>
            {
                Items = exitExams,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }
        public async Task<ResponseWrapper<List<ExitExamResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<ExitExam> query = exitExamRepository.Queryable();

            List<ExitExam> exitExams = await query.OrderBy(x => x.ExamDate).ToListAsync(cancellationToken);

            List<ExitExamResponseDTO> response = mapper.Map<List<ExitExamResponseDTO>>(exitExams);

            return new ResponseWrapper<List<ExitExamResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<ExitExamRulesModel>> GetExitExamRules(CancellationToken cancellationToken, long studentId)
        {
            Student student = await exitExamRepository.GetStudentWithSubRecords(cancellationToken, studentId);
            ExitExamRulesModel rulesModel = new ExitExamRulesModel();

            var estimatedFinish = student.EducationTrackings.FirstOrDefault(x => x.ReasonType == ReasonType.EstimatedFinish);

            var requiredRotationExpertiseBranchIds = student.Curriculum.CurriculumRotations.Select(x => x.Rotation.ExpertiseBranchId).ToList();
            var successfulRequiredStudentRotationExpertiseBranchIds = student.StudentRotations.Select(x => x.Rotation.ExpertiseBranchId).ToList();

            rulesModel.AreRotationsCompleted = !requiredRotationExpertiseBranchIds.Except(successfulRequiredStudentRotationExpertiseBranchIds).Any();

            rulesModel.AreOpinionFormsCompleted = student.Curriculum.Duration * 2 <= student.OpinionForms.Count || student.OpinionForms.OrderByDescending(x => x.EndDate).FirstOrDefault()?.EndDate <= estimatedFinish?.ProcessDate;

            rulesModel.IsThesisSuccess = student.OriginalProgram.ExpertiseBranch.IsPrincipal == false ? true : student.Theses.Any(x => x.Status == ThesisStatusType.Successful);

            rulesModel.EstimatedEndDate = estimatedFinish?.ProcessDate?.Date;
            return new ResponseWrapper<ExitExamRulesModel> { Result = true, Item = rulesModel };
        }
        public async Task<ResponseWrapper<ExitExamResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            ExitExam exitExam = await exitExamRepository.GetByIdAsync(cancellationToken, id);

            ExitExamResponseDTO response = mapper.Map<ExitExamResponseDTO>(exitExam);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ExitExamResponseDTO>> PostAsync(CancellationToken cancellationToken, ExitExamDTO exitExamDTO)
        {
            ExitExam exitExam = mapper.Map<ExitExam>(exitExamDTO);

            await exitExamRepository.AddAsync(cancellationToken, exitExam);
            if (exitExam.PracticeExamNote >= 60 && exitExam.AbilityExamNote >= 60)
            {
                var student = await studentRepository.GetByIdAsync(cancellationToken, exitExam.StudentId ?? 0);
                student.Status = StudentStatus.Gratuated;
                studentRepository.Update(student);
            }

            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<ExitExamResponseDTO>(exitExam);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ExitExamResponseDTO>> Put(CancellationToken cancellationToken, long id, ExitExamDTO exitExamDTO)
        {
            var exitExam = mapper.Map<ExitExam>(exitExamDTO);

            await exitExamRepository.UpdateWithSubRecords(cancellationToken, id, exitExam);

            var updatedExitExam = await exitExamRepository.GetWithSubRecords(cancellationToken, id);

            if (exitExamDTO.PracticeExamNote >= 60 && exitExamDTO.AbilityExamNote >= 60)
            {
                var student = await studentRepository.GetByIdAsync(cancellationToken, exitExam.StudentId ?? 0);
                student.Status = StudentStatus.Gratuated;
                studentRepository.Update(student);
            }
            var response = mapper.Map<ExitExamResponseDTO>(updatedExitExam);
            await unitOfWork.CommitAsync(cancellationToken);
            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<ExitExamResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            ExitExam exitExam = await exitExamRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.EducationTracking);

            if (exitExam.EducationTrackingId != 0 && exitExam.EducationTrackingId != null)
                unitOfWork.EducationTrackingRepository.SoftDelete(exitExam.EducationTracking);
            exitExamRepository.SoftDelete(exitExam);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}
