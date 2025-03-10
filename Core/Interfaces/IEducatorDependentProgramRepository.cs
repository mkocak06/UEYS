using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEducatorDependentProgramRepository : IRepository<EducatorDependentProgram>
    {
        Task<List<EducatorDependentProgram>> GetListWithSubRecords(CancellationToken cancellationToken, long dependProgId);
    }
}
