using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using System.Linq.Expressions;
using Shared.RequestModels;

namespace Core.Interfaces
{
    public interface IThesisRepository : IRepository<Thesis>
    {
        public IQueryable<ThesisResponseDTO> QueryableThesis();
        public Task<Thesis> GetWithSubRecords(CancellationToken cancellationToken, long id);
        Task<Thesis> UpdateWithSubRecords(CancellationToken cancellationToken, long id, Thesis thesis);
        Task<List<Thesis>> GetListWithSubRecords(CancellationToken cancellationToken, long studentId);
    }
}
