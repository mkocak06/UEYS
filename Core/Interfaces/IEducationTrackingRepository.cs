using Core.Entities;
using Shared.RequestModels;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEducationTrackingRepository : IRepository<EducationTracking>
    {
        Task<List<EducationTracking>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<int> GetRemainingDaysForDependentProgram(CancellationToken cancellationToken, StudentDependentProgramPaginateDTO dependentProgramDTO);
        Task<List<EducationTracking>> GetTimeIncreasingRecordsByDate(CancellationToken cancellationToken, OpinionFormRequestDTO opinionForm);
        Task CheckEstimatedFinish(long studentId, CancellationToken cancellationToken);
    }
}
