using Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRotationRepository : IRepository<Rotation>
    {
        Task<Rotation> GetByIdWithSubRecords(CancellationToken cancellationToken, long id);
        Task<List<Rotation>> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId);
        Task<Rotation> GetByCurriculumIdAndExpBranchName(CancellationToken cancellationToken, long curriculumId, string rotationName);
        Task<List<CurriculumRotation>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<List<Rotation>> GetFormerStudentListByStudentId(CancellationToken cancellationToken, long studentId);
        Task SetZoneOfEducators(CancellationToken cancellationToken);
        Task ReArrangeEducationTime(CancellationToken cancellationToken);
        Task SetZoneOfStudents(CancellationToken cancellationToken);
    }
}
