using Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthorizationCategoryRepository : IRepository<AuthorizationCategory>
    {
        //int GetOrderNumber();
        //Task ChangeOrder(CancellationToken cancellationTokenlong, long id, bool isToUp);
    }
}
 