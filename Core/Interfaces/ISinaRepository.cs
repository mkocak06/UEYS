using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces;

public interface ISinaRepository : IRepository<Sina>
{
    Task UpdateSinaTable();
}