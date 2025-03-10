using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPerformanceRatingRepository : IRepository<PerformanceRating>
    {
        Task<List<PerformanceRating>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<PerformanceRating> GetByRatingId(CancellationToken cancellationToken, long id);
        Task<EducatorProgram> GetEducatorByStudentId(CancellationToken cancellationToken, long? studentId, long userId);
    }
}
