using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFacultyRepository : IRepository<Faculty>
    {
        Task<List<Faculty>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
    }
}
