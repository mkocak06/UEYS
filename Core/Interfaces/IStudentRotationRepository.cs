using Core.Entities;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Shared.ResponseModels;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;

namespace Core.Interfaces
{
    public interface IStudentRotationRepository : IRepository<StudentRotation>
    {
        Task<List<StudentRotation>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<StudentRotation> GetWithSubRecords(CancellationToken cancellationToken, long studentId, long rotationId);
        IQueryable<StudentRotation> GetFormerStudentsListByUserId(CancellationToken cancellationToken, long userId);
        IQueryable<StudentRotation> GetListByStudentIdQuery(long studentId);
        Task<int> GetRemainingDays(CancellationToken cancellationToken, long id, StudentRotationDTO studentRotationDTO);
        Task<List<Perfection>> GetPerfectionListByCurrciulumAndRotationId(CancellationToken cancellationToken, long curriculumId, long rotationId);
        Task<ResponseWrapper<StudentRotationResponseDTO>> CheckOpinionForms(CancellationToken cancellationToken, StudentRotationDTO studentRotationDTO);
        Task CheckStudentRotations(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO);
    }
}
