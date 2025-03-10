using Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPerfectionRepository : IRepository<Perfection>
    {
        Task<Perfection> GetByIdWithSubRecords(CancellationToken cancellationToken, long id);
        Task<List<Perfection>> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId);
        IQueryable<Perfection> GetByStudentIdQuery(long studentId);
        IQueryable<Perfection> GetWithSubRecords();
    }
}
