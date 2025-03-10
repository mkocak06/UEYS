using Core.Entities;
using Core.Entities.Koru;
using Core.Interfaces;
using Koru;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels;
using Shared.ResponseModels.Authorization;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AuthRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<User> Create(CancellationToken cancellationToken, User admin, string password)
        {
            if (password != null)
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                admin.PasswordHash = passwordHash;
                admin.PasswordSalt = passwordSalt;
            }
            admin.CreateDate = DateTime.UtcNow;

            await dbContext.Users.AddAsync(admin, cancellationToken); // Adding the user to context of users.
            await dbContext.SaveChangesAsync(cancellationToken); // Save changes to database.

            return admin;
        }
        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Create hash using password salt.
                for (int i = 0; i < computedHash.Length; i++)
                { // Loop through the byte array
                    if (computedHash[i] != passwordHash[i]) return false; // if mismatch
                }
            }
            return true; //if no mismatches.
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(CancellationToken cancellationToken, string email)
        {
            if (await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken))
                return true;
            return false;
        }

        public async Task<User> GetAdmin(CancellationToken cancellationToken, string email)
        {
            var usr = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
            return usr;
        }

        public async Task<ResponseWrapper<UserForLoginResponseDTO>> Login(CancellationToken cancellationToken, string email, string password)
        {
            var admin = await dbContext.Users.AsNoTracking().Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.Email == email && x.IsDeleted == false, cancellationToken); //Get user from database.
            if (admin == null)
                return new() { Result = false, Message = "User does not exist!" };  // User does not exist.


            if (!VerifyPassword(password, admin.PasswordHash, admin.PasswordSalt))
            {
                if (admin.IsPassive)
                    return new() { Result = false, Message = "User account is passive." };

                admin.LastLoginDate = DateTime.UtcNow;
                return new()
                {
                    Result = false,
                    Message = "User password wrong",
                    Item = new()
                    {
                        Email = admin.Email
                    }
                }; ;
            }

            return new()
            {
                Result = true,
                Item = new()
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Name = admin.Name,
                    ProfilePhoto = admin.ProfilePhoto,
                    SelectedRoleId = admin.SelectedRoleId,
                    UserRoleIds = admin.UserRoles?.Where(x => x.RoleId != null).Select(x => x.RoleId.Value).ToList(),
                    RoleCategoryType = (RoleCategoryType)(admin.UserRoles?.FirstOrDefault(x => x.RoleId == admin.SelectedRoleId)?.Role?.CategoryId ?? 0)
                }
            };
        }

        public async Task<ResponseWrapper<UserForLoginResponseDTO>> LoginWithIdentityNo(CancellationToken cancellationToken, string identityNo)
        {
            var admin = await dbContext.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.IdentityNo == identityNo && x.IsDeleted == false, cancellationToken); //Get user from database.
            if (admin == null)
                return new() { Result = false, Message = "User does not exist!" };  // User does not exist.
            if (admin.IsPassive)
                return new() { Result = false, Message = "User account is passive." };
            admin.LastLoginDate = DateTime.UtcNow;
            return new()
            {
                Result = true,
                Item = new()
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Name = admin.Name,
                    ProfilePhoto = admin.ProfilePhoto,
                    SelectedRoleId = admin.SelectedRoleId,
                    UserRoleIds = admin.UserRoles?.Where(x => x.RoleId != null).Select(x => x.RoleId.Value).ToList(),
                    RoleCategoryType = (RoleCategoryType)(admin.UserRoles?.FirstOrDefault(x => x.RoleId == admin.SelectedRoleId)?.Role?.CategoryId ?? 0)
                }
            };
        }

        public async Task<User> Update(CancellationToken cancellationToken, User admin)
        {
            var savedUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == admin.Id, cancellationToken);


            dbContext.Entry(savedUser).CurrentValues.SetValues(savedUser);
            await dbContext.SaveChangesAsync(cancellationToken);

            return savedUser;
        }

        public async Task<bool> CheckRoleOfZone(CancellationToken cancellationToken, ZoneRegisterDTO zone)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Id == zone.RoleId);
            switch ((RoleCategoryType)(role.CategoryId ?? 0))
            {
                case RoleCategoryType.University:
                    return await dbContext.UserRoleUniversities.AnyAsync(x => x.UniversityId == zone.ZoneIds.FirstOrDefault() && x.UserRole.RoleId == zone.RoleId, cancellationToken);
                case RoleCategoryType.Faculty:
                    return await dbContext.UserRoleFaculties.AnyAsync(x => x.FacultyId == zone.ZoneIds.FirstOrDefault() && x.UserRole.RoleId == zone.RoleId, cancellationToken);
            }
            return false;
        }
    }
}
