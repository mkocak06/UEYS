using Core.Entities;
using Shared.ResponseModels;
using Shared.ResponseModels.Authorization;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> Create(CancellationToken cancellationToken, User user, string password=null);
        Task<ResponseWrapper<UserForLoginResponseDTO>> Login(CancellationToken cancellationToken, string email, string password);
        Task<ResponseWrapper<UserForLoginResponseDTO>> LoginWithIdentityNo(CancellationToken cancellationToken, string identityNo);
        Task<bool> UserExists(CancellationToken cancellationToken, string email);
        Task<User> GetAdmin(CancellationToken cancellationToken, string email);
        Task<User> Update(CancellationToken cancellationToken, User admin);
        Task<bool> CheckRoleOfZone(CancellationToken cancellationToken, ZoneRegisterDTO zone);
    }
}
