using Core.Entities;
using System.Collections.Generic;

namespace Core.Entities.Koru
{
    public class UserRole
    {
        public long Id { get; set; }
        public long? UserId { get; }
        public long? RoleId { get; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserRoleFaculty> UserRoleFaculties { get; set; }
        public virtual ICollection<UserRoleHospital> UserRoleHospitals { get; set; }
        public virtual ICollection<UserRoleProgram> UserRolePrograms { get; set; }
        public virtual ICollection<UserRoleProvince> UserRoleProvinces { get; set; }
        public virtual ICollection<UserRoleUniversity> UserRoleUniversities { get; set; }
        public virtual ICollection<UserRoleStudent> UserRoleStudents { get; set; }

        public UserRole() { }
        public UserRole(long userId, long roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
