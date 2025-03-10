using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class PerfectionRepository : EfRepository<Perfection>, IPerfectionRepository
    {
        private readonly ApplicationDbContext dbContext;
        public PerfectionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Perfection>> GetList(CancellationToken cancellationToken)
        {
            return await dbContext.Perfections.AsSplitQuery()
                .Include(x => x.CurriculumPerfections).ThenInclude(x => x.Curriculum)
                .Where(x => x.IsDeleted == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Perfection>> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId)
        {
            return await dbContext.CurriculumPerfections.AsSplitQuery()
               .Include(x => x.Curriculum)
               .Where(x => x.IsDeleted == false && x.CurriculumId == curriculumId)
               .Select(x => x.Perfection)
               .ToListAsync(cancellationToken);
        }

        public IQueryable<Perfection> GetByStudentIdQuery(long studentId)
        {
            var student = dbContext.Students.FirstOrDefault(x => x.Id == studentId);
            return dbContext.CurriculumPerfections
                            .Include(x => x.Perfection).ThenInclude(x => x.PerfectionProperties).ThenInclude(x => x.Property)
                            .Include(x => x.Perfection).ThenInclude(x => x.StudentPerfections.Where(x => x.IsDeleted == false && x.Student.CurriculumId == student.CurriculumId))
                            .Where(x => x.IsDeleted == false && x.CurriculumId == student.CurriculumId)
                            .Select(x => x.Perfection);
        }
        public IQueryable<Perfection> GetWithSubRecords()
        {
            return dbContext.Perfections.AsSplitQuery().AsNoTracking()
                                        .Include(x => x.CurriculumPerfections).ThenInclude(x => x.Curriculum)
                                        .Include(x => x.PerfectionProperties).ThenInclude(x => x.Property)
                                        .Where(x => x.IsDeleted == false);
        }
        public async Task<Perfection> GetByIdWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Perfections
                                  .Include(x => x.CurriculumPerfections).ThenInclude(x => x.Curriculum)
                                  .Include(x => x.PerfectionProperties).ThenInclude(x => x.Property)
                                  .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
