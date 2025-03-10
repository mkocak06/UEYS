using Core.Entities;
using Shared.ResponseModels.ProtocolProgram;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDependentProgramRepository : IRepository<DependentProgram>
    {
    }
}
