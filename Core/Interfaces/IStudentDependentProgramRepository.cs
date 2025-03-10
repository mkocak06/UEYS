using Core.Entities;
using System.Linq;

namespace Core.Interfaces
{
    public interface IStudentDependentProgramRepository : IRepository<StudentDependentProgram>
    {
        IQueryable<StudentDependentProgram> GetListByStudentId(long studentId);
    }
}
