using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Types;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ExitExamRepository : EfRepository<ExitExam>, IExitExamRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ExitExamRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ExitExam> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.ExitExams.AsSplitQuery().AsNoTracking()
                                               .Include(x => x.Juries).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               .Include(x => x.Juries).ThenInclude(x => x.Educator).ThenInclude(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.Student).ThenInclude(x => x.User)
                                               .Include(x => x.Hospital)
                                               .Include(x => x.EducationTracking)
                                               .Include(x => x.Secretary)
                                               .FirstOrDefaultAsync(x => x.Id == id, cancellationToken); ;
        }

        public IQueryable<ExitExam> QueryableExitExam(Expression<Func<ExitExam, bool>> predicate)
        {
            return dbContext.ExitExams.AsSplitQuery().AsNoTracking()
                                               .Include(x => x.Juries.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                                               .Include(x => x.Juries.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Educator).ThenInclude(x => x.AcademicTitle)
                                               .Include(x => x.Juries.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Educator).ThenInclude(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.Student).ThenInclude(x => x.User)
                                               .Include(x => x.Hospital)
                                               .Include(x => x.EducationTracking)
                                               .Include(x => x.Secretary)
                                               .Where(predicate);
        }

        public async Task<ExitExam> UpdateWithSubRecords(CancellationToken cancellationToken, long id, ExitExam exitExam)
        {
            ExitExam existExitExam = await GetWithSubRecords(cancellationToken, id);
            dbContext.Entry(exitExam).State = EntityState.Modified;

            if (exitExam.Juries != null)
            {

                foreach (var item in exitExam.Juries)
                {
                    var existJury = existExitExam.Juries.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existJury == null ? EntityState.Added : EntityState.Modified;

                }
                var JuriesIds = exitExam.Juries.Select(r => r.Id).ToList();
                if (existExitExam?.Juries != null)
                {
                    foreach (var item in existExitExam.Juries.Where(x => !JuriesIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }


            dbContext.Entry(exitExam.EducationTracking).State = existExitExam.EducationTracking == null ? EntityState.Added : EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return exitExam;
        }

        public async Task<Student> GetStudentWithSubRecords(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.Students.AsSplitQuery().AsNoTracking()
                .Include(x => x.Theses.Where(x => x.IsDeleted == false))
                .Include(x => x.EducationTrackings.Where(x => x.IsDeleted == false))
                .Include(x => x.OriginalProgram.ExpertiseBranch)
                .Include(x => x.Curriculum).ThenInclude(x => x.CurriculumRotations.Where(x => x.Rotation.IsRequired == true && x.IsDeleted == false && x.Rotation.IsDeleted == false)).ThenInclude(x=>x.Rotation)
                .Include(x => x.StudentRotations.Where(x => (x.IsSuccessful == true || x.ProcessDateForExemption != null) && x.IsDeleted == false && x.Rotation.IsRequired == true)).ThenInclude(x => x.Rotation)
                .Include(x => x.OpinionForms.Where(x => x.FormStatusType == FormStatusType.Active && x.IsDeleted == false))
                .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);
        }
    }
}
