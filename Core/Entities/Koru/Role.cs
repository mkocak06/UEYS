using System.Collections.Generic;

namespace Core.Entities.Koru
{
    public class Role
    {
        public long Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public long? CategoryId { get; set; }
        public int Level { get; set; }//grupta ekleme yetkisi
        public bool IsAddRole { get; set; } = false;
        public bool IsDeleted { get; set; }
        public string Code { get; set; }
        public bool? IsAutomated { get; set; }
        public virtual RoleCategory Category { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
        public virtual ICollection<RoleField> RoleFields { get; set; }
    }
}
