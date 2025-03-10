using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Entities.Koru
{
    public class Permission
    {
        public long Id { get; set; }
        public string PermissionGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public string MainGroup { get; set; }
        public string SubGroup { get; set; }
        public string Description2 { get; set; }


        public List<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<Field> Fields { get; set; }
        public Permission(string name, string description, string permissionGroup)
        {
            Name = name;
            Description = description;
            PermissionGroup = permissionGroup;
        }

        public override bool Equals(object obj)
        {
            return obj is Permission permission &&
                   PermissionGroup == permission.PermissionGroup &&
                   Name == permission.Name &&
                   Description == permission.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PermissionGroup, Name, Description);
        }
    }
}
