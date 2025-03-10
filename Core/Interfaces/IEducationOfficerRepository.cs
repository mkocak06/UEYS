using Core.Entities;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;

namespace Core.Interfaces
{
    public interface IEducationOfficerRepository : IRepository<EducationOfficer>
    {
        Task<ResponseWrapper<EducationOfficerResponseDTO>> FinishEducationOfficersDuty(CancellationToken cancellationToken, long educationOfficerId, DateTime? dateTime = null);
        IQueryable<EducationOfficer> GetPaginateListForProgramDetailQuery(long programId);
        Task<ResponseWrapper<EducationOfficerResponseDTO>> FinishCurrentDuty(CancellationToken cancellationToken, long programId, DateTime? dateTime = null);
    }
}
