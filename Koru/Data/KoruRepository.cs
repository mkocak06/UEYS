using Core.Entities;
using Core.Entities.Koru;
using Core.Models.Authorization;
using Koru.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels.Authorization;
using Shared.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Koru
{
    public class KoruRepository : IKoruRepository
    {
        static ConcurrentDictionary<string, Permission> Permissions = new();
        private readonly DbContext dbContext;

        public KoruRepository(DbContext dbContext) : base()
        {
            this.dbContext = dbContext;
        }

        internal static void Add(string permissionName, string description, string group)
        {
            Permission permission = new Permission(permissionName, description, group);
            Permissions.AddOrUpdate(permission.Name, permission, (key, oldValue) => permission);
        }

        public ICollection<Permission> GetPermissions()
        {
            return SyncPermissions();
        }

        private List<Permission> SyncPermissions()
        {
            //try
            //{
            //    var dbPermissions1 = dbContext.Set<Permission>().Where(p => p.IsActive).ToList();
            //    var permissions1 = Permissions.Select(x => x.Key).ToList();
            //    var aaa = dbPermissions1.Where(x => !permissions1.Any(a => a == x.Name)&&x.Name!= AccountType.SuperAdmin.ToString() && x.Name!= AccountType.Admin.ToString()
            //    && x.Name != AccountType.ReadOnly.ToString()).ToList();
            //    dbContext.Set<Permission>().RemoveRange(aaa);
            //    dbContext.SaveChanges();
            //}
            //catch (System.Exception ex)
            //{

            //}
            var permissionsSet = dbContext.Set<Permission>();
            var dbPermissions = dbContext.Set<Permission>().Where(p => p.IsActive).ToDictionary(p => p.Name);
            foreach (var item in Permissions)
            {
                if (!dbPermissions.ContainsKey(item.Key))
                {
                    item.Value.IsActive = true;
                    permissionsSet.Add(item.Value);
                }
                else if (dbPermissions[item.Key] != item.Value)
                {
                    dbPermissions[item.Key].PermissionGroup = item.Value.PermissionGroup;
                    dbPermissions[item.Key].Description = item.Value.Description;
                    dbPermissions[item.Key].IsActive = true;
                    permissionsSet.Update(dbPermissions[item.Key]);
                }
            }
            dbContext.SaveChanges(true);
            return permissionsSet.ToList();
        }

        public async Task<UserRole> AddRoleToUser(Role role, long userId)
        {
            if (role.Id == 0)
            {
                dbContext.Add<Role>(role);
            }
            else if (await dbContext.Set<UserRole>().FirstOrDefaultAsync(x => x.RoleId == role.Id && x.UserId == userId) != null)//already has role
            {
                return await dbContext.Set<UserRole>().FirstOrDefaultAsync(x => x.RoleId == role.Id && x.UserId == userId);
            }

            var response = dbContext.Add<UserRole>(new UserRole(userId, role.Id));
            dbContext.SaveChanges();
            return response.Entity;
        }

        public async Task<UserRoleProgram> AddUserRoleProgram(long userRoleId, long programId)
        {
            if (await dbContext.Set<UserRoleProgram>().FirstOrDefaultAsync(x => x.UserRoleId == userRoleId && x.ProgramId == programId) != null)//already has userRoleProgram
                return new UserRoleProgram();

            var response = dbContext.Add<UserRoleProgram>(new UserRoleProgram() { ProgramId = programId, UserRoleId = userRoleId });
            dbContext.SaveChanges();
            return response.Entity;
        }

        public async Task<UserRoleProgram> RemoveUserRoleProgram(long id)
        {
            var userRoleProgram = await dbContext.Set<UserRoleProgram>().FirstOrDefaultAsync(x => x.Id == id);

            if (userRoleProgram != null)//already has userRoleProgram
                dbContext.Set<UserRoleProgram>().Remove(userRoleProgram);

            dbContext.SaveChanges();
            return new();
        }

        public async Task<UserRoleStudent> RemoveUserRoleStudent(long userId, long roleId, long studentId)
        {
            var userRoleStudent = await dbContext.Set<UserRoleStudent>().FirstOrDefaultAsync(x => x.UserRole.RoleId == roleId && x.UserRole.UserId == userId && x.StudentId == studentId);

            if (userRoleStudent != null)//already has userRoleStudent
                dbContext.Set<UserRoleStudent>().Remove(userRoleStudent);

            dbContext.SaveChanges();
            return new();
        }

        public async Task<bool> AddRoleToUser(string roleName, long userId)
        {
            var existRole = await dbContext.Set<Role>().FirstOrDefaultAsync(x => x.RoleName == roleName);
            if (existRole != null)
            {

                dbContext.Add<UserRole>(new UserRole(userId, existRole.Id));
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Role AddUpdateRole(Role role)
        {
            if (role.Id == 0)
                dbContext.Add<Role>(role);
            else
            {
                dbContext.Update<Role>(role);
            }
            dbContext.SaveChanges();
            return role;
        }

        public async Task<Role> AddRoleAsync(CancellationToken cancellationToken, string name, string description)
        {
            var role = new Role();
            role.RoleName = name;
            role.Description = description;
            var result = await dbContext.AddAsync(role, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task<bool> RemoveRole(CancellationToken cancellationToken, long id)
        {
            var role = await dbContext.Set<Role>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (role != null)
            {
                role.IsDeleted = true;
                return await dbContext.SaveChangesAsync(cancellationToken) == 1;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Set<Role>().AsNoTracking().AsSplitQuery()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .OrderBy(x => x.RoleName)
                .Where(x => x.IsDeleted == false)
                .ToListAsync<Role>(cancellationToken);
        }
        public async Task<Role> GetRoleByRoleNameAsync(CancellationToken cancellationToken, string roleName)
        {
            return await dbContext.Set<Role>()
                .FirstOrDefaultAsync<Role>(x => x.RoleName == roleName, cancellationToken);
        }

        public async Task<Role> GetRoleByCodeAsync(CancellationToken cancellationToken, string code)
        {
            return await dbContext.Set<Role>()
                .FirstOrDefaultAsync<Role>(x => x.Code == code, cancellationToken);
        }

        public async Task<List<Role>> GetRolesByUserIdAsync(CancellationToken cancellationToken, long userId)
        {
            return await dbContext.Set<UserRole>().Where(x => x.UserId == userId).Join(
                dbContext.Set<Role>(),
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => role
                ).ToListAsync(cancellationToken);

        }

        public async Task<Role> UpdateRolePermissionsAsync(CancellationToken cancellationToken, long roleId, List<string> permissions)
        {
            var role = await dbContext.Set<Role>()
                .Include(r => r.RolePermissions)
                .Where(r => r.Id == roleId)
                .FirstOrDefaultAsync(cancellationToken);
            if (role == null)
                throw new KeyNotFoundException($"Could not found the Role with id: {roleId}");
            role.RolePermissions.Clear();
            var rpList = await dbContext.Set<Permission>()
                .Where(p => permissions.Contains(p.Name))
                .Select(p => new RolePermission(roleId, p.Id)).ToListAsync();
            await dbContext.Set<RolePermission>().AddRangeAsync(rpList, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return role;
        }

        public bool RemovePermissionFromRole(RolePermission rolePermission)
        {
            dbContext.Remove(rolePermission);
            return dbContext.SaveChanges() == 1;
        }

        public async Task<string[]> GetUserPermissionsAsync(long userId)
        {
            var result = await dbContext.Set<UserRole>().Where(u => u.UserId == userId).Join(
                dbContext.Set<RolePermission>(),
                userRole => userRole.RoleId,
                rolePermission => rolePermission.RoleId,
                (userRole, rolePermission) => rolePermission.Permission.Name
                ).ToListAsync();
            return result.ToArray();
        }
        public async Task<List<Permission>> GetUserPermissionsModelAsync(CancellationToken cancellationToken, long userId, long selectedRoleId)
        {
            var result = await dbContext.Set<UserRole>().Where(u => u.UserId == userId && u.RoleId == selectedRoleId).Join(
                dbContext.Set<RolePermission>(),
                userRole => userRole.RoleId,
                rolePermission => rolePermission.RoleId,
                (userRole, rolePermission) => rolePermission.Permission
                ).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Task> CreateUserRolesAsync(CancellationToken cancellationToken, long userId, List<long> roleIds)
        {
            var oldList = dbContext.Set<UserRole>().Where(u => u.UserId == userId);
            dbContext.Set<UserRole>().RemoveRange(oldList);
            await dbContext.Set<UserRole>().AddRangeAsync(roleIds.Select(r => new UserRole(userId, r)), cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task<RolePermission> AddPermissionToRole(CancellationToken cancellationToken, RolePermission rolePermission)
        {
            await dbContext.Set<RolePermission>().AddAsync(rolePermission);
            await dbContext.SaveChangesAsync(cancellationToken);
            return rolePermission;
        }
        public async Task<RolePermission> RemovePermissionToRole(CancellationToken cancellationToken, RolePermission rolePermission)
        {
            var _rolePermission = await dbContext.Set<RolePermission>().FirstOrDefaultAsync(x => x.PermissionId == rolePermission.PermissionId && x.RoleId == x.RoleId, cancellationToken);
            dbContext.Remove(_rolePermission);
            await dbContext.SaveChangesAsync(cancellationToken);
            return _rolePermission;
        }
        public async Task<Role> UpdateRoleAsync(CancellationToken cancellationToken, long id, string name, string description)
        {
            var role = await dbContext.Set<Role>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (role != null)
            {
                role.RoleName = name;
                role.Description = description;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            return role;
        }

        public async Task<Role> GetRoleById(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Set<Role>()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                 .Include(m => m.RoleMenus)
                     .ThenInclude(mr => mr.Menu)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public async Task<Menu> GetMenuByMenuNameAsync(CancellationToken cancellationToken, string menuName)
        {
            return await dbContext.Set<Menu>()
                .FirstOrDefaultAsync(x => x.Name == menuName, cancellationToken);
        }
        public async Task<bool> AddMenuToRole(CancellationToken cancellationToken, long roleId, long menuId)
        {
            await dbContext.Set<RoleMenu>().AddAsync(new RoleMenu { RoleId = roleId, MenuId = menuId }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RemoveMenuToRole(CancellationToken cancellationToken, long roleId, long menuId)
        {
            var roleMenu = await dbContext.Set<RoleMenu>().FirstOrDefaultAsync(x => x.RoleId == roleId && x.MenuId == menuId, cancellationToken);
            dbContext.Set<RoleMenu>().Remove(roleMenu);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<Role>> GetRolesByUserRole(CancellationToken cancellationToken, List<Role> roleUserRoles)
        {
            if (roleUserRoles.Any(x => x.CategoryId == (int)RoleCategoryType.Admin))
            {
                return await dbContext.Set<Role>().Where(x => x.IsAutomated != true && !x.IsDeleted).ToListAsync(cancellationToken);
            }
            List<Role> roles = new List<Role>();
            foreach (var item in roleUserRoles)
            {
                if (item.IsAddRole)
                {

                    if (item.CategoryId == (long)RoleCategoryType.Hospital)
                    {
                        var roleListWithHospitalCategory = await dbContext.Set<Role>().Where(x => x.CategoryId > item.CategoryId && item.Level > x.Level && !x.IsDeleted).ToListAsync(cancellationToken);

                        if (roleListWithHospitalCategory != null && roleListWithHospitalCategory.Count > 0)
                            roles.AddRange(roleListWithHospitalCategory);
                    }

                    var roleList = await dbContext.Set<Role>().Where(x => x.CategoryId == item.CategoryId && item.Level > x.Level && !x.IsDeleted).ToListAsync(cancellationToken);
                    if (roleList != null && roleList.Count > 0)
                        roles.AddRange(roleList);
                }
            }
            return roles;

        }
        public async Task<ZoneModel> GetZone(CancellationToken cancellationToken, long userId, List<Role> userRoles, long selectedRoleId)
        {
            var selectedRole = await dbContext.Set<Role>().FirstOrDefaultAsync(x => x.Id == selectedRoleId);
            List<ZoneModel> zoneModels = new();
            ZoneModel zoneModel = new();
            foreach (var item in userRoles)
            {
                if (item.CategoryId == (int)RoleCategoryType.Admin)
                {
                    if (selectedRole.CategoryId == (int)RoleCategoryType.Province)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Province;
                        zoneModel.Provinces = await dbContext.Set<Province>().ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.University)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.University;
                        zoneModel.Universities = await dbContext.Set<University>().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Faculty)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Faculty;
                        zoneModel.Faculties = await dbContext.Set<Faculty>().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Hospital)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Hospital;
                        zoneModel.Hospitals = await dbContext.Set<Hospital>().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Program)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Program;
                        zoneModel.Programs = await dbContext.Set<Program>().AsSplitQuery().AsNoTracking()
                                                .Include(x => x.Faculty).ThenInclude(x => x.University)
                                                .Include(x => x.Hospital).ThenInclude(x => x.Province)
                                                .Include(x => x.ExpertiseBranch)
                            .Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Student)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Student;
                        zoneModel.Students = await dbContext.Set<Student>().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                }
                else
                {
                    if (selectedRole.CategoryId == (int)RoleCategoryType.Province)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Province;
                        zoneModel.Provinces = await (from ur in dbContext.Set<UserRole>()
                                                     join up in dbContext.Set<UserRoleProvince>() on ur.Id equals up.UserRoleId
                                                     join p in dbContext.Set<Province>() on up.ProvinceId equals p.Id
                                                     where ur.RoleId == item.Id && ur.UserId == userId
                                                     select p).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.University)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.University;
                        zoneModel.Universities = await (from ur in dbContext.Set<UserRole>()
                                                        join up in dbContext.Set<UserRoleUniversity>() on ur.Id equals up.UserRoleId
                                                        join p in dbContext.Set<University>() on up.UniversityId equals p.Id
                                                        where ur.RoleId == item.Id && ur.UserId == userId
                                                        select p).Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Faculty)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Faculty;
                        zoneModel.Faculties = await (from ur in dbContext.Set<UserRole>()
                                                     join up in dbContext.Set<UserRoleFaculty>() on ur.Id equals up.UserRoleId
                                                     join p in dbContext.Set<Faculty>() on up.FacultyId equals p.Id
                                                     where ur.RoleId == item.Id && ur.UserId == userId
                                                     select p).Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Hospital)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Hospital;
                        zoneModel.Hospitals = await (from ur in dbContext.Set<UserRole>()
                                                     join up in dbContext.Set<UserRoleHospital>() on ur.Id equals up.UserRoleId
                                                     join p in dbContext.Set<Hospital>() on up.HospitalId equals p.Id
                                                     where ur.RoleId == item.Id && ur.UserId == userId
                                                     select p).Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Program)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Program;
                        zoneModel.Programs = await (from ur in dbContext.Set<UserRole>()
                                                    join up in dbContext.Set<UserRoleProgram>() on ur.Id equals up.UserRoleId
                                                    join p in dbContext.Set<Program>().AsSplitQuery().AsNoTracking()
                                                .Include(x => x.Faculty).ThenInclude(x => x.University)
                                                .Include(x => x.Hospital).ThenInclude(x => x.Province)
                                                .Include(x => x.ExpertiseBranch)
                                                on up.ProgramId equals p.Id
                                                    where ur.RoleId == item.Id && ur.UserId == userId
                                                    select p).Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                    else if (selectedRole.CategoryId == (int)RoleCategoryType.Student)
                    {
                        zoneModel.RoleCategory = RoleCategoryType.Student;
                        zoneModel.Students = await (from ur in dbContext.Set<UserRole>()
                                                    join up in dbContext.Set<UserRoleStudent>() on ur.Id equals up.UserRoleId
                                                    join p in dbContext.Set<Student>() on up.StudentId equals p.Id
                                                    where ur.RoleId == item.Id && ur.UserId == userId
                                                    select p).Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
                    }
                }
            }

            return zoneModel;
        }
        public async Task<ZoneModel> GetZone(CancellationToken cancellationToken, long userId, long selectedRoleId)
        {
            var selectedRole = await dbContext.Set<Role>().FirstOrDefaultAsync(x => x.Id == selectedRoleId);
            ZoneModel zoneModel = new();
            if (selectedRole.CategoryId is (int)RoleCategoryType.Admin)
                zoneModel.RoleCategory = RoleCategoryType.Admin;
            else if (selectedRole.CategoryId is (int)RoleCategoryType.Ministry)
                zoneModel.RoleCategory = RoleCategoryType.Ministry;
            else if (selectedRole.CategoryId is (int)RoleCategoryType.Registration)
                zoneModel.RoleCategory = RoleCategoryType.Registration;
            else
                zoneModel.RoleCategory = RoleCategoryType.None;

            if (selectedRole.CategoryId == (int)RoleCategoryType.Province)
            {
                if (zoneModel.Provinces != null && zoneModel.Provinces.Count > 0)
                {
                    var provinces = await (from ur in dbContext.Set<UserRole>()
                                           join up in dbContext.Set<UserRoleProvince>() on ur.Id equals up.UserRoleId
                                           join p in dbContext.Set<Province>() on up.ProvinceId equals p.Id
                                           where ur.RoleId == selectedRole.Id && ur.UserId == userId
                                           select p).ToListAsync(cancellationToken);
                    if (provinces != null && provinces.Count > 0)
                    {
                        zoneModel.Provinces.AddRange(provinces);
                    }
                }
                else
                {
                    zoneModel.Provinces = await (from ur in dbContext.Set<UserRole>()
                                                 join up in dbContext.Set<UserRoleProvince>() on ur.Id equals up.UserRoleId
                                                 join p in dbContext.Set<Province>() on up.ProvinceId equals p.Id
                                                 where ur.RoleId == selectedRole.Id && ur.UserId == userId
                                                 select p).ToListAsync(cancellationToken);
                }
            }
            else if (selectedRole.CategoryId == (int)RoleCategoryType.University)
            {
                if (zoneModel.Universities != null && zoneModel.Universities.Count > 0)
                {
                    var universities = await (from ur in dbContext.Set<UserRole>()
                                              join up in dbContext.Set<UserRoleUniversity>() on ur.Id equals up.UserRoleId
                                              join p in dbContext.Set<University>() on up.UniversityId equals p.Id
                                              where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted
                                              select p).ToListAsync(cancellationToken);
                    if (universities != null && universities.Count > 0)
                    {
                        zoneModel.Universities.AddRange(universities);
                    }
                }
                else
                {
                    zoneModel.Universities = await (from ur in dbContext.Set<UserRole>()
                                                    join up in dbContext.Set<UserRoleUniversity>() on ur.Id equals up.UserRoleId
                                                    join p in dbContext.Set<University>() on up.UniversityId equals p.Id
                                                    where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted
                                                    select p).ToListAsync(cancellationToken);
                }
            }
            else if (selectedRole.CategoryId == (int)RoleCategoryType.Faculty)
            {
                if (zoneModel.Faculties != null && zoneModel.Faculties.Count > 0)
                {
                    var faculties = await (from ur in dbContext.Set<UserRole>()
                                           join up in dbContext.Set<UserRoleFaculty>() on ur.Id equals up.UserRoleId
                                           join p in dbContext.Set<Faculty>() on up.FacultyId equals p.Id
                                           where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted
                                           select p).ToListAsync(cancellationToken);
                    if (faculties != null && faculties.Count > 0)
                    {
                        zoneModel.Faculties.AddRange(faculties);
                    }
                }
                else
                {
                    zoneModel.Faculties = await (from ur in dbContext.Set<UserRole>()
                                                 join up in dbContext.Set<UserRoleFaculty>() on ur.Id equals up.UserRoleId
                                                 join p in dbContext.Set<Faculty>() on up.FacultyId equals p.Id
                                                 where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted
                                                 select p).ToListAsync(cancellationToken);
                }
            }
            else if (selectedRole.CategoryId == (int)RoleCategoryType.Hospital)
            {
                //zoneModel.Hospitals = await (from ur in dbContext.Set<UserRole>()
                //                             join up in dbContext.Set<UserRoleHospital>() on ur.Id equals up.UserRoleId
                //                             join p in dbContext.Set<Hospital>() on up.HospitalId equals p.Id
                //                             where ur.RoleId == userRole.Id && ur.UserId == userId
                //                             select p).ToListAsync(cancellationToken);


                if (zoneModel.Hospitals != null && zoneModel.Hospitals.Count > 0)
                {
                    var hospitals = zoneModel.Hospitals = await dbContext
                        .Set<UserRoleHospital>().Where(x => x.UserRole.RoleId == selectedRole.Id && x.UserRole.UserId == userId && !x.Hospital.IsDeleted)
                                                                             .Select(x => x.Hospital)
                                                                             .ToListAsync(cancellationToken);

                    if (hospitals != null && hospitals.Count > 0)
                    {
                        zoneModel.Hospitals.AddRange(hospitals);
                    }
                }
                else
                {
                    zoneModel.Hospitals = await dbContext.Set<UserRoleHospital>()
                                                .Where(x => x.UserRole.RoleId == selectedRole.Id && x.UserRole.UserId == userId && !x.Hospital.IsDeleted)
                                                                            .Select(x => x.Hospital)
                                                                            .ToListAsync(cancellationToken);
                }
            }
            else if (selectedRole.CategoryId == (int)RoleCategoryType.Program)
            {
                if (zoneModel.Programs != null && zoneModel.Programs.Count > 0)
                {
                    var programs = await (from ur in dbContext.Set<UserRole>()
                                          join up in dbContext.Set<UserRoleProgram>() on ur.Id equals up.UserRoleId
                                          join p in dbContext.Set<Program>().AsSplitQuery().AsNoTracking()
                                          .Include(x => x.Faculty).ThenInclude(x => x.University)
                                          .Include(x => x.Hospital).ThenInclude(x => x.Province)
                                          .Include(x => x.ExpertiseBranch)
                                          on up.ProgramId equals p.Id
                                          where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted
                                          select p).ToListAsync(cancellationToken);

                    if (programs != null && programs.Count > 0)
                    {
                        zoneModel.Programs.AddRange(programs);
                    }
                }
                else
                {
                    zoneModel.Programs = await (from ur in dbContext.Set<UserRole>()
                                                join up in dbContext.Set<UserRoleProgram>() on ur.Id equals up.UserRoleId
                                                join p in dbContext.Set<Program>().AsSplitQuery().AsNoTracking()
                                                .Include(x => x.Faculty).ThenInclude(x => x.University)
                                                .Include(x => x.Hospital).ThenInclude(x => x.Province)
                                                .Include(x => x.ExpertiseBranch)
                                                on up.ProgramId equals p.Id
                                                where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted
                                                select p).ToListAsync(cancellationToken);
                }
            }
            else if (selectedRole.CategoryId == (int)RoleCategoryType.Student)
            {
                if (zoneModel.Students != null && zoneModel.Students.Count > 0)
                {
                    var students = await (from ur in dbContext.Set<UserRole>()
                                          join up in dbContext.Set<UserRoleStudent>() on ur.Id equals up.UserRoleId
                                          join p in dbContext.Set<Student>() on up.StudentId equals p.Id
                                          where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted && !p.IsHardDeleted
                                          select p).ToListAsync(cancellationToken);

                    if (students != null && students.Count > 0)
                    {
                        zoneModel.Students.AddRange(students);
                    }
                }
                else
                {
                    zoneModel.Students = await (from ur in dbContext.Set<UserRole>()
                                                join up in dbContext.Set<UserRoleStudent>() on ur.Id equals up.UserRoleId
                                                join p in dbContext.Set<Student>() on up.StudentId equals p.Id
                                                where ur.RoleId == selectedRole.Id && ur.UserId == userId && !p.IsDeleted && !p.IsHardDeleted
                                                select p).ToListAsync(cancellationToken);
                }
            }

            return zoneModel;
        }

        public async Task<Task> AddStudentZoneToUserRole(CancellationToken cancellationToken, long userId, long studentId, string roleCode)
        {
            var role = dbContext.Set<Role>().FirstOrDefault(x => x.Code == roleCode);
            var userRole = dbContext.Set<UserRole>().Include(x => x.UserRoleStudents).FirstOrDefault(x => x.UserId == userId && x.RoleId == role.Id);
            var user = dbContext.Set<User>().FirstOrDefault(x => x.Id == userId);
            if (user.IsDeleted == true)
                user.IsDeleted = false;
            if (userRole == null)
            {
                UserRole userRoleToAdd = new(userId, role.Id)
                {
                    UserRoleStudents = [new() { StudentId = studentId }]
                };

                dbContext.Add(userRoleToAdd);
            }
            else if (userRole.UserRoleStudents?.Any(x => x.StudentId == studentId) == false)
            {
                UserRoleStudent userRoleStudent = new() { UserRoleId = userRole.Id, StudentId = studentId };
                dbContext.Set<UserRoleStudent>().Add(userRoleStudent);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return Task.CompletedTask;
        }
        public async Task<Task> UpdateUserRolesAndZonesAsync(CancellationToken cancellationToken, long userId, List<ZoneRegisterDTO> zones)
        {

            var oldList = dbContext.Set<UserRole>().Where(u => u.UserId == userId);
            if (oldList != null)
            {
                dbContext.Set<UserRole>().RemoveRange(oldList);
            }
            foreach (var item in zones)
            {
                var role = await dbContext.Set<Role>().FirstOrDefaultAsync(x => x.Id == item.RoleId, cancellationToken);

                if (role != null)
                {
                    UserRole userRole = new(userId, role.Id);
                    if (role.CategoryId == (int)RoleCategoryType.Province)
                    {
                        userRole.UserRoleProvinces = item.ZoneIds.Select(x => new UserRoleProvince { ProvinceId = x }).ToList();
                    }
                    else if (role.CategoryId == (int)RoleCategoryType.University)
                    {
                        userRole.UserRoleUniversities = item.ZoneIds.Select(x => new UserRoleUniversity { UniversityId = x }).ToList();
                    }
                    else if (role.CategoryId == (int)RoleCategoryType.Faculty)
                    {
                        userRole.UserRoleFaculties = item.ZoneIds.Select(x => new UserRoleFaculty { FacultyId = x, ExpertiseBranchId = item.ExpertiseBranchId }).ToList();
                    }
                    else if (role.CategoryId == (int)RoleCategoryType.Hospital)
                    {
                        userRole.UserRoleHospitals = item.ZoneIds.Select(x => new UserRoleHospital { HospitalId = x }).ToList();
                    }
                    else if (role.CategoryId == (int)RoleCategoryType.Program)
                    {
                        userRole.UserRolePrograms = item.ZoneIds.Select(x => new UserRoleProgram { ProgramId = x }).ToList();
                    }
                    else if (role.CategoryId == (int)RoleCategoryType.Student)
                    {
                        userRole.UserRoleStudents = item.ZoneIds.Select(x => new UserRoleStudent { StudentId = x }).ToList();
                    }
                    await dbContext.Set<UserRole>().AddAsync(userRole);
                }
                //var setting = await dbContext.Set<UserSetting>().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
                //if (role != null && setting == null)
                //{
                //    await dbContext.Set<UserSetting>().AddAsync(new() { SelectedRoleId = role.Id, UserId = userId }, cancellationToken);
                //}
            }
            await dbContext.SaveChangesAsync(cancellationToken);
            return Task.CompletedTask;
        }
        public async Task<Task> UpdateUserRolesAsync(CancellationToken cancellationToken, long userId, List<long> roleIds)
        {
            var oldList = dbContext.Set<UserRole>().Where(u => u.UserId == userId);
            dbContext.Set<UserRole>().RemoveRange(oldList);
            await dbContext.Set<UserRole>().AddRangeAsync(roleIds.Select(r => new UserRole(userId, r)), cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task RemoveRoleFromUser(CancellationToken cancellationToken, long userId, long roleId)
        {
            var userRole = await dbContext.Set<UserRole>().FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == roleId, cancellationToken);
            if (userRole != null)
            {
                dbContext.Set<UserRole>().Remove(userRole);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            var user = await dbContext.Set<User>().Include(x => x.UserRoles).FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (!user.UserRoles.Any())
            {
                user.IsDeleted = true;
                user.DeleteDate = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
