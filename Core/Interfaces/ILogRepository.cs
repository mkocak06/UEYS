using Core.Entities;
using Core.Models.LogInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILogRepository : IRepository<Log>
    {
        Task<List<string>> SystemLogInformation();
    }
}
 