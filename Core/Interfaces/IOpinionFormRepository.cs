using Core.Entities;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOpinionFormRepository : IRepository<OpinionForm>
    {
        Task<List<OpinionForm>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<OpinionForm> GetByOpinionId(CancellationToken cancellationToken, long id);
        Task<Educator> GetEducatorByStudentId(CancellationToken cancellationToken, long studentId, long userId);
        Task<EducationOfficer> GetProgramManagerByStudentId(CancellationToken cancellationToken, long studentId);
        Task<List<OpinionForm>> GetListByStudentAndProgramId(CancellationToken cancellationToken, long studentId, long programId);
        Task ChangeDateByTimeIncreasing(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO, int? dayDiff = null);
        Task<ResponseWrapper<EducationTrackingDTO>> CheckTransferPossible(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO);
    }
}
