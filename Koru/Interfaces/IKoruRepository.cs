using Core.Entities;
using Core.Entities.Koru;
using Core.Models.Authorization;
using Koru.Data;
using Shared.ResponseModels.Authorization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Koru.Interfaces
{
    public interface IKoruRepository
    {
        public ICollection<Permission> GetPermissions();

        public Task<UserRole> AddRoleToUser(Role role, long userId);
        public Task<UserRoleProgram> AddUserRoleProgram(long userRoleId, long programId);
        public Task<UserRoleProgram> RemoveUserRoleProgram(long id);
        Task<UserRoleStudent> RemoveUserRoleStudent(long userId, long roleId, long studentId);
        Task<bool> AddRoleToUser(string roleName, long userId);
        public Role AddUpdateRole(Role role);
        Task<bool> RemoveRole(CancellationToken cancellationToken, long id);
        public Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken);
        Task<Role> GetRoleByRoleNameAsync(CancellationToken cancellationToken, string roleName);
        Task<Role> GetRoleByCodeAsync(CancellationToken cancellationToken, string code);
        public bool RemovePermissionFromRole(RolePermission rolePermission);
        Task<string[]> GetUserPermissionsAsync(long userId);
        Task<Role> AddRoleAsync(CancellationToken cancellationToken, string name, string description);
        Task<Role> UpdateRolePermissionsAsync(CancellationToken cancellationToken, long roleId, List<string> permissions);
        Task<Task> UpdateUserRolesAsync(CancellationToken cancellationToken, long userId, List<long> roleIds);
        Task<Task> CreateUserRolesAsync(CancellationToken cancellationToken, long userId, List<long> roleIds);
        Task<List<Permission>> GetUserPermissionsModelAsync(CancellationToken cancellationToken, long userId, long selectedRoleId);
        Task<List<Role>> GetRolesByUserIdAsync(CancellationToken cancellationToken, long userId);
        Task<RolePermission> AddPermissionToRole(CancellationToken cancellationToken, RolePermission rolePermission);
        Task<RolePermission> RemovePermissionToRole(CancellationToken cancellationToken, RolePermission rolePermission);
        Task<Role> UpdateRoleAsync(CancellationToken cancellationToken, long id, string name, string description);
        Task<Role> GetRoleById(CancellationToken cancellationToken, long id);
        Task<bool> AddMenuToRole(CancellationToken cancellationToken, long roleId, long menuId);
        Task<bool> RemoveMenuToRole(CancellationToken cancellationToken, long roleId, long menuId);
        Task<List<Role>> GetRolesByUserRole(CancellationToken cancellationToken, List<Role> role);
        Task<ZoneModel> GetZone(CancellationToken cancellationToken, long userId, List<Role> userRole, long selectedRoleId);
        Task<ZoneModel> GetZone(CancellationToken cancellationToken, long userId, long selectedRoleId);
        Task<Task> UpdateUserRolesAndZonesAsync(CancellationToken cancellationToken, long userId, List<ZoneRegisterDTO> zones);
        Task<Task> AddStudentZoneToUserRole(CancellationToken cancellationToken, long userId, long studentId, string roleCode);
        Task RemoveRoleFromUser(CancellationToken cancellationToken, long userId, long roleId);
        Task<Menu> GetMenuByMenuNameAsync(CancellationToken cancellationToken, string menuName);
    }
}
