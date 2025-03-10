using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJuryRepository : IRepository<Jury>
    {
        Task<Jury> GetWithSubRecords(CancellationToken cancellationToken, long id); 
    }
}
