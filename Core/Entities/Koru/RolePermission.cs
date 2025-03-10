using AutoMapper.Configuration.Annotations;

namespace Core.Entities.Koru
{
    public class RolePermission
    {
        public long RoleId { get; set; }
        [Ignore]
        public Role Role { get; set; }
        public long PermissionId { get; set; }
        public Permission Permission { get; set; }

        private RolePermission() { } //needed by EF Core

        public RolePermission(long roleId, long permissionId)
        {
            PermissionId = permissionId;
            RoleId = roleId;
        }
    }
}
