using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Shared.ResponseModels.Curriculum;

namespace Core.Interfaces
{
    public interface ICurriculumRepository : IRepository<Curriculum>
    {
        Task<Curriculum> GetByIdWithSubRecords(CancellationToken cancellationToken, long id);
        IQueryable<Curriculum> QueryableCurriculums(Expression<Func<Curriculum, bool>> predicate);
        Task<List<Curriculum>> GetListWithSubRecords(CancellationToken cancellationToken);
        Task<List<CurriculumExportModel>> CurriculumExportModel(CancellationToken cancellationToken);
    }
}
 