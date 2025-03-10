using Core.Entities;
using Core.Entities.Koru;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Koru.Data
{
    public class KoruDataSeeder
    {
        public static async Task SeedAsync(DbContext applicationDbContext)
        {
            var count = await applicationDbContext.Set<Role>().CountAsync();
            if (count == 0)
            {
                var user = await applicationDbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == "admin@saglik.gov.tr");
                var permission = new Permission(AccountType.SuperAdmin.ToString(), AccountType.SuperAdmin.GetDescription(), "Authorization");
                var role = new Role() { RoleName = AccountType.SuperAdmin.ToString() };
                applicationDbContext.Set<Permission>().Add(permission);
                applicationDbContext.Set<Role>().Add(role);
                await applicationDbContext.SaveChangesAsync();
                var userRole = new UserRole(user.Id, role.Id);
                applicationDbContext.Set<RolePermission>().Add(new RolePermission(role.Id, permission.Id));
                await applicationDbContext.SaveChangesAsync();
                applicationDbContext.Set<UserRole>().Add(userRole);
                await applicationDbContext.SaveChangesAsync();
            }
            var existUser = await applicationDbContext.Set<Role>().AnyAsync(x => x.RoleName == AccountType.Admin.ToString());
            if (!existUser)
            {
                var permission = new Permission(AccountType.Admin.ToString(), AccountType.Admin.GetDescription(), "Authorization");
                var role = new Role() { RoleName = AccountType.Admin.ToString() };
                applicationDbContext.Set<Permission>().Add(permission);
                applicationDbContext.Set<Role>().Add(role);
                await applicationDbContext.SaveChangesAsync();
                applicationDbContext.Set<RolePermission>().Add(new RolePermission(role.Id, permission.Id));
                await applicationDbContext.SaveChangesAsync();
            }
            var existReadOnly = await applicationDbContext.Set<Role>().AnyAsync(x => x.RoleName == AccountType.ReadOnly.ToString());
            if (!existReadOnly)
            {
                var permission = new Permission(AccountType.ReadOnly.ToString(), AccountType.ReadOnly.GetDescription(), "Authorization");
                var role = new Role() { RoleName = AccountType.ReadOnly.ToString() };
                applicationDbContext.Set<Permission>().Add(permission);
                applicationDbContext.Set<Role>().Add(role);
                await applicationDbContext.SaveChangesAsync();
                applicationDbContext.Set<RolePermission>().Add(new RolePermission(role.Id, permission.Id));
                await applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
