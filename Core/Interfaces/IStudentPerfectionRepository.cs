using Core.Entities;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStudentPerfectionRepository : IRepository<StudentPerfection>
    {
        Task<StudentPerfection> GetWithSubRecords(CancellationToken cancellationToken, long id);
        Task<StudentPerfection> GetByStudentAndPerfection(CancellationToken cancellationToken, long studentId, long perfectionId);
        Task<List<StudentPerfectionResponseDTO>> GetListByStudentId(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType);
        Task<List<StudentPerfectionResponseDTO>> GetListByStudentIdWithoutType(CancellationToken cancellationToken, long studentId);
    }
}
