using Core.Entities;
using System.Linq;

namespace Core.Interfaces
{
    public interface ICurriculumRotationRepository : IRepository<CurriculumRotation>
    {
        IQueryable<CurriculumRotation> GetWithSubRecords();
    }
}
