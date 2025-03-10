using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StudentRotationRepository : EfRepository<StudentRotation>, IStudentRotationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StudentRotationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<StudentRotation> GetWithSubRecords(CancellationToken cancellationToken, long studentId, long rotationId)
        {
            return await dbContext.StudentRotations.AsSplitQuery().AsNoTracking()
                                                   .Include(x => x.Educator).ThenInclude(x => x.User)
                                                   .Include(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                                                   .Include(x => x.Program).ThenInclude(x => x.Hospital)
                                                   .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                                   .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                                                   .FirstOrDefaultAsync(x => x.StudentId == studentId && x.RotationId == rotationId && x.IsDeleted == false, cancellationToken);
        }

        public async Task<List<StudentRotation>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.StudentRotations.AsSplitQuery().AsNoTracking()
                                                   .Include(x => x.StudentRotationPerfections).ThenInclude(x => x.Perfection)
                                                   .Include(x => x.StudentRotationPerfections).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                                   .Include(x => x.Educator).ThenInclude(x => x.User)
                                                   .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                                                   .Include(x => x.Rotation).ThenInclude(x => x.Perfections)
                                                   .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                                   .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                                   .Where(x => x.StudentId == studentId && x.IsDeleted == false)
                                                   .ToListAsync(cancellationToken);
        }

        public IQueryable<StudentRotation> GetListByStudentIdQuery(long studentId)
        {
            return dbContext.StudentRotations
                            .AsSplitQuery().AsNoTracking()
                            .Include(x => x.StudentRotationPerfections).ThenInclude(x => x.Perfection).ThenInclude(x => x.PerfectionProperties).ThenInclude(x => x.Property)
                            .Include(x => x.StudentRotationPerfections).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                            .Include(x => x.Educator).ThenInclude(x => x.User)
                            .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                            .Include(x => x.Rotation).ThenInclude(x => x.Perfections)
                            .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                            .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                            .Where(x => x.StudentId == studentId && x.IsDeleted == false);
        }

        public IQueryable<StudentRotation> GetFormerStudentsListByUserId(CancellationToken cancellationToken, long userId)
        {
            var formerStudent = dbContext.Students.FirstOrDefault(x => x.UserId == userId && x.EducationTrackings.Any(y => y.ReasonType == ReasonType.BranchChange_End && y.IsDeleted == false));
            if (formerStudent != null)
            {
                return dbContext.StudentRotations.AsSplitQuery().AsNoTracking()
                                                 .Include(x => x.StudentRotationPerfections).ThenInclude(x => x.Perfection).ThenInclude(x => x.PerfectionProperties).ThenInclude(x => x.Property)
                                                 .Include(x => x.StudentRotationPerfections).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                                 .Include(x => x.Educator).ThenInclude(x => x.User)
                                                 .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                                                 .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                                 .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                                 .Where(x => x.StudentId == formerStudent.Id && x.IsDeleted == false);
            }
            else
                return null;
        }

        public async Task<int> GetRemainingDays(CancellationToken cancellationToken, long id, StudentRotationDTO studentRotationDTO)
        {
            var studentRotation = await dbContext.StudentRotations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            var leavingInstitution = await dbContext.EducationTrackings.OrderByDescending(x => x.ProcessDate).FirstOrDefaultAsync(x => x.IsDeleted == false && x.StudentRotationId == id, cancellationToken);

            var daysInRotation = (studentRotationDTO.EndDate?.Date - leavingInstitution.ProcessDate?.Date)?.Days;

            var eduTrList = await dbContext.EducationTrackings
                .AsSplitQuery()
                .Where(x => x.IsDeleted == false && x.StudentId == studentRotationDTO.StudentId && x.ProcessType == ProcessType.TimeIncreasing && x.ProcessDate > leavingInstitution.ProcessDate && x.ProcessDate < studentRotationDTO.EndDate).Select(x => x.AdditionalDays).ToListAsync(cancellationToken);
            int totalExtendedDays = 0;
            foreach (var item in eduTrList)
                totalExtendedDays += (int)item;

            return (int)((studentRotation.RemainingDays == null ? Convert.ToInt32(studentRotation.Rotation.Duration) : studentRotation.RemainingDays) - daysInRotation + totalExtendedDays);
        }

        public async Task<List<Perfection>> GetPerfectionListByCurrciulumAndRotationId(CancellationToken cancellationToken, long curriculumId, long rotationId)
        {
            return await (from cr in dbContext.CurriculumRotations
                          join p in dbContext.Perfections on cr.Id equals p.CurriculumRotationId
                          where p.IsDeleted == false && cr.IsDeleted == false && cr.Rotation.IsDeleted == false && cr.Curriculum.IsDeleted == false && cr.CurriculumId == curriculumId && cr.RotationId == rotationId
                          select p).ToListAsync(cancellationToken);
        }

        public async Task<ResponseWrapper<StudentRotationResponseDTO>> CheckOpinionForms(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO)
        {
            var educationStart = await dbContext.EducationTrackings.OrderBy(x => x.ProcessDate).FirstOrDefaultAsync(x => x.StudentId == studentRotationDTO.StudentId && x.IsDeleted == false && x.ProcessType == ProcessType.Start, cancellationToken);
            var estimatedFinish = await dbContext.EducationTrackings.FirstOrDefaultAsync(x => x.StudentId == studentRotationDTO.StudentId && x.IsDeleted == false && x.ReasonType == ReasonType.EstimatedFinish, cancellationToken);

            var student = await dbContext.Students.Include(x => x.Curriculum).FirstOrDefaultAsync(x => x.Id == studentRotationDTO.StudentId, cancellationToken);

            var opinionForms = new List<OpinionForm>() { new() { Period = PeriodType.Period_1, StartDate = educationStart?.ProcessDate, EndDate = educationStart?.ProcessDate?.AddMonths(6).AddDays(-1) } };

            var timeIncreasings = await dbContext.EducationTrackings.OrderBy(x => x.ProcessDate).Where(x => x.IsDeleted == false && x.StudentId == studentRotationDTO.StudentId && x.ProcessType == ProcessType.TimeIncreasing && x.ProcessDate >= opinionForms.FirstOrDefault().StartDate && x.ReasonType != ReasonType.ExtensionByApplicationForSubjection && x.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation && x.ReasonType != ReasonType.ForceMajeure && x.ReasonType != ReasonType.DueToUnusualCircumstances).ToListAsync(cancellationToken);
            var timeWithoutEducation = await dbContext.EducationTrackings.OrderBy(x => x.ProcessDate).Where(x => !x.IsDeleted && x.StudentId == studentRotationDTO.StudentId && x.ReasonType != ReasonType.BranchChange && (x.ProcessType == ProcessType.Transfer || x.ProcessType == ProcessType.Assignment || x.ProcessType == ProcessType.Start) && (x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment || x.ReasonType == ReasonType.BeginningToAssignedInstitution || x.ReasonType == ReasonType.CompletionOfAssignment || x.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment || x.ReasonType == ReasonType.Other || x.ReasonType == ReasonType.ExcusedTransfer || x.ReasonType == ReasonType.UnexcusedTransfer)).ToListAsync(cancellationToken);

            var totalPeriodCount = student.Curriculum?.Duration * 2;

            for (int i = 1; i < timeWithoutEducation.Count; i++)
                if (i % 2 == 1)
                    timeIncreasings.Add(new() { ProcessDate = timeWithoutEducation[i].ProcessDate, AdditionalDays = (timeWithoutEducation[i].ProcessDate?.Date - timeWithoutEducation[i - 1].ProcessDate?.Date)?.Days ?? 0 });

            if (timeIncreasings != null)
                foreach (var item in timeIncreasings)
                    if (item.ProcessDate <= opinionForms.FirstOrDefault().EndDate)
                        opinionForms.FirstOrDefault().EndDate = opinionForms.FirstOrDefault().EndDate?.AddDays(item.AdditionalDays ?? 0);

            for (int i = 2; i < totalPeriodCount - 1; i++)
            {
                var opinionFormToAdd = new OpinionForm()
                {
                    Period = (PeriodType)i,
                    StartDate = opinionForms.FirstOrDefault(x => x.Period == (PeriodType)(i - 1))?.EndDate?.AddDays(1),
                    EndDate = opinionForms.FirstOrDefault(x => x.Period == (PeriodType)(i - 1))?.EndDate?.AddMonths(6)
                };

                if (opinionFormToAdd.StartDate < estimatedFinish.ProcessDate?.Date)
                {
                    if (opinionFormToAdd.EndDate > estimatedFinish?.ProcessDate?.Date)
                        opinionFormToAdd.EndDate = estimatedFinish?.ProcessDate?.Date;

                    opinionForms.Add(opinionFormToAdd);
                    var timeIncreasingsInPeriod = timeIncreasings.Where(x => x.ProcessDate >= opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i)?.StartDate);

                    if (timeIncreasingsInPeriod != null)
                        foreach (var item in timeIncreasingsInPeriod)
                            if (item.ProcessDate < opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i).EndDate)
                                opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i).EndDate = opinionForms.FirstOrDefault(x => x.Period == (PeriodType)i).EndDate?.AddDays(item.AdditionalDays ?? 0);
                }
            }
            var timeIncreasingsInRotation = timeIncreasings.Where(x => x.ProcessDate?.Date >= studentRotationDTO.BeginDate?.Date && x.ProcessDate?.Date.AddDays(x.AdditionalDays ?? 0) <= studentRotationDTO.EndDate?.Date).Sum(x => x.AdditionalDays);
            var timeInEducation = (studentRotationDTO.EndDate?.Date - studentRotationDTO.BeginDate?.Date)?.Days - timeIncreasingsInRotation;

            if (timeInEducation < 91)
                return new() { Result = true };

            bool response = true;

            var opFormWhichRotationStartIn = opinionForms.FirstOrDefault(x => x.StartDate <= studentRotationDTO.BeginDate && x.EndDate >= studentRotationDTO.BeginDate);
            var opFormWhichRotationEndIn = opinionForms.FirstOrDefault(x => x.StartDate <= studentRotationDTO.EndDate && x.EndDate >= studentRotationDTO.EndDate);

            var timeFromRotationStart = (opFormWhichRotationStartIn.EndDate?.Date - studentRotationDTO.BeginDate?.Date)?.Days;
            var timeFromOpFormStart = (studentRotationDTO.EndDate?.Date - opFormWhichRotationEndIn?.StartDate?.Date)?.Days;


            if (timeFromRotationStart > 90 || timeFromOpFormStart > 90)
            {
                response = await dbContext.OpinionForms.AnyAsync(x => x.IsDeleted == false && x.FormStatusType == FormStatusType.Active && x.StudentId == student.Id && x.Period == (timeFromRotationStart > 90 ? opFormWhichRotationStartIn.Period : opFormWhichRotationEndIn.Period), cancellationToken);
            }

            int? fullPeriodCount = opFormWhichRotationEndIn.Period - opFormWhichRotationStartIn.Period - 1;

            if (fullPeriodCount > 0)
            {
                for (int i = 0; i < fullPeriodCount; i++)
                {
                    response = await dbContext.OpinionForms.AnyAsync(x => x.IsDeleted == false && x.FormStatusType == FormStatusType.Active && x.StudentId == student.Id && x.Period == opFormWhichRotationStartIn.Period + i + 1, cancellationToken);
                }
            }

            return new() { Result = response, Message = !response ? "You cannot finish the rotation without saving the student's opinion information!" : null };
        }

        public async Task CheckStudentRotations(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO)
        {
            var studentRotations = await dbContext.StudentRotations.Where(x => x.IsDeleted == false && x.StudentId == educationTrackingDTO.StudentId && x.BeginDate <= educationTrackingDTO.ProcessDate && x.EndDate >= educationTrackingDTO.ProcessDate).ToListAsync(cancellationToken);
            var educationTracking = await dbContext.EducationTrackings.FirstOrDefaultAsync(x => x.IsDeleted == false && x.StudentRotationId == studentRotations.FirstOrDefault().Id && x.ReasonType == ReasonType.CompletionOfRotation);

            if (studentRotations != null && studentRotations.Any() && educationTracking != null)
            {
                studentRotations.FirstOrDefault().EndDate = studentRotations.FirstOrDefault().EndDate?.AddDays(educationTrackingDTO.AdditionalDays ?? 0);
                educationTracking.ProcessDate = educationTracking.ProcessDate?.AddDays(educationTrackingDTO.AdditionalDays ?? 0);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}