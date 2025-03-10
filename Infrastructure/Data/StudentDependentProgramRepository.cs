using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data
{
    public class StudentDependentProgramRepository : EfRepository<StudentDependentProgram>, IStudentDependentProgramRepository
    {
        private readonly ApplicationDbContext dbContext;
        public StudentDependentProgramRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<StudentDependentProgram> GetListByStudentId(long studentId)
        {
            return dbContext.StudentDependentPrograms.AsSplitQuery()
                                                     .Include(x => x.DependentProgram).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital)
                                                     .Include(x => x.DependentProgram).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                                     .Include(x => x.DependentProgram).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                                     .Where(x => x.StudentId == studentId && x.DependentProgram.RelatedDependentProgram.ProtocolProgram.ParentProgramId != x.DependentProgram.ProgramId);
        }
    }
}
