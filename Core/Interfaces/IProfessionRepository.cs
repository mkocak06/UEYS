using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProfessionRepository : IRepository<Profession>
    {
        Task<List<Profession>> GetByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<Profession> GetWithSubRecords(CancellationToken cancellationToken, long id);
    }
}
 