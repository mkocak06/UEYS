using Core.Entities;
using Shared.ResponseModels.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStandardRepository : IRepository<Standard>
    {
        Task<List<ProgramStandardResponseDTO>> GetByLatestCurriculumsExpertiseBranch(long expBranchId);
    }
}
