using Core.Entities;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Core.Interfaces
{
    public interface IThesisDefenceRepository : IRepository<ThesisDefence>
    {
        Task<ThesisDefence> GetWithSubRecords(Expression<Func<ThesisDefence, bool>> predicate, CancellationToken cancellationToken);
        Task<ThesisDefence> UpdateWithSubRecords(CancellationToken cancellationToken, long id, ThesisDefence thesisDefence);
        Task<ThesisDefence> GetById(long id, CancellationToken cancellationToken);
    }
}
