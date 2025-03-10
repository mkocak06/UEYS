using Core.Entities;
using Core.Interfaces;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EducationTrackingRepository : EfRepository<EducationTracking>, IEducationTrackingRepository
    {
        private readonly ApplicationDbContext dbContext;
        public EducationTrackingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<EducationTracking>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.EducationTrackings.AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.ProcessOwner)
                .Include(x => x.FormerProgram)
                .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Where(x => x.StudentId == studentId && x.IsDeleted == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetRemainingDaysForDependentProgram(CancellationToken cancellationToken, StudentDependentProgramPaginateDTO studentDependentProgramDTO)
        {
            var dependentProgram = await dbContext.DependentPrograms.FirstOrDefaultAsync(x => x.Id == studentDependentProgramDTO.DependentProgramId);

            var leavingInstitution = await dbContext.EducationTrackings.OrderByDescending(x => x.ProcessDate).FirstOrDefaultAsync(x => x.IsDeleted == false && x.StudentId == studentDependentProgramDTO.StudentId && x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment && x.ProgramId == dependentProgram.ProgramId && x.AssignmentType == AssignmentType.UnderProtocolProgram);

            var eduTrList = await dbContext.EducationTrackings.AsSplitQuery()
                .Where(x => x.StudentId == studentDependentProgramDTO.StudentId && x.IsDeleted == false && x.ProcessDate > leavingInstitution.ProcessDate && x.ProcessDate < studentDependentProgramDTO.EndDate && (x.ProcessType == ProcessType.TimeIncreasing || x.ReasonType == ReasonType.CompletionOfRotation || x.ReasonType == ReasonType.LeftWithoutCompletingRotation)).Select(x => x.AdditionalDays)
                .ToListAsync(cancellationToken);

            var totalDaysInEducation = (studentDependentProgramDTO.EndDate?.Date - leavingInstitution.ProcessDate?.Date)?.Days;
            int totalExtendedDays = 0;
            foreach (var item in eduTrList)
                totalExtendedDays += (int)item;
            return (int)((studentDependentProgramDTO.RemainingDays == null ? (dependentProgram.Duration * 30) : studentDependentProgramDTO.RemainingDays) + totalExtendedDays - totalDaysInEducation);
        }

        public async Task<List<EducationTracking>> GetTimeIncreasingRecordsByDate(CancellationToken cancellationToken, OpinionFormRequestDTO opinionForm)
        {
            return await dbContext.EducationTrackings.AsSplitQuery()
                                                     .Where(x => x.StudentId == opinionForm.StudentId && x.IsDeleted == false && x.ProcessDate >= opinionForm.StartDate && x.ProcessDate < opinionForm.EndDate && x.ProcessType == ProcessType.TimeIncreasing && x.ReasonType != ReasonType.ExtensionByApplicationForSubjection && x.ReasonType != ReasonType.TimeExtensionDueToFailureInRotation && x.ReasonType != ReasonType.ForceMajeure && x.ReasonType != ReasonType.DueToUnusualCircumstances)
                                                     .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                                     .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                                     .ToListAsync(cancellationToken);
        }

        public async Task CheckEstimatedFinish(long studentId, CancellationToken cancellationToken)
        {
            var student = await dbContext.Students.Include(x => x.EducationTrackings.Where(x => x.ProcessType == ProcessType.EstimatedFinish)).FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);

            if (student.EducationTrackings.FirstOrDefault()?.ProcessDate <= DateTime.UtcNow)
                student.Status = StudentStatus.EducationContinues;
        }
    }
}
