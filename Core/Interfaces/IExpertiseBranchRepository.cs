using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.ResponseModels.ENabiz;

namespace Core.Interfaces
{
    public interface IExpertiseBranchRepository : IRepository<ExpertiseBranch>
    {
        IQueryable<ExpertiseBranch> QueryableList();
        Task<ExpertiseBranch> GetById(CancellationToken cancellationToken, long id);
        Task UpdateAsync(ExpertiseBranch expertiseBranch);
        Task<List<ExpertiseBranch>> GetListRelatedWithProgramsByProfessionId(CancellationToken cancellationToken, long id);
        Task<List<ExpertiseBranch>> GetListForProtocolProgramByHospitalId(CancellationToken cancellationToken, long hospitalId);
        Task<List<long>> GetSubBrachIds(CancellationToken cancellationToken, long id);
        long? GetLastCurriculum(long expBranchId);
        Task<List<ExpertiseBranchResponseDTO>> ExpertiseBranchListEnabiz(CancellationToken cancellationToken);
    }
}
 