using Core.Entities;
using Core.Entities.Koru;
using Core.Models.Authorization;
using Core.Models.LogInformation;
using Shared.Models.SMSModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User Get(string email);
        Task<List<User>> GetList(CancellationToken cancellationToken);
        Task<User> GetUserWithEducationalInfo(CancellationToken cancellationToken, string identityNo);
        Task<User> GetUserByIdentity(CancellationToken cancellationToken, string identityNo);
        Task<User> GetByIdWithSubRecords(CancellationToken cancellationToken, long id);
        Task<User> GetByIdentityNoWithSubRecords(CancellationToken cancellationToken, string identityNo);
        bool IsExistingIdentity(CancellationToken cancellationToken, string identityNo);
        IQueryable<UserPaginateResponseDTO> PaginateQuery(ZoneModel zone);
        List<ActivePassiveResponseModel> GetActivePassiveResponse();
        Task SendWelcomeMessage(string phone);
        Task SendMessage(SMSModel model);
        Task<List<DetailedLogInformation>> UserLogInformation();
        Task<UserRole> AddUserRole(UserRole userRole);
        Task<UserRoleProgram> AddUserRoleProgram(UserRoleProgram userRoleProgram);
    }
}
