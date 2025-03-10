using Core.Entities;
using System.Linq;

namespace Core.Interfaces
{
    public interface IEducatorProgramRepository : IRepository<EducatorProgram>
    {
        IQueryable<EducatorProgram> GetListByHospitalId(long hospitalId);
    }
}
