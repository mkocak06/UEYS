using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAdvisorThesisRepository : IRepository<AdvisorThesis>
    {
        Task<AdvisorThesis> GetById(CancellationToken cancellationToken, long id);
    }
}
