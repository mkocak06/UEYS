using Core.Entities;
using System.Linq;

namespace Core.Interfaces
{
    public interface ICurriculumPerfectionRepository : IRepository<CurriculumPerfection>
    {
        IQueryable<CurriculumPerfection> GetWithSubRecords();
    }
}
