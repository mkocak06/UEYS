using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using Core.Models.Authorization;
using Shared.Types;

namespace Infrastructure.Data
{
    public class MenuRepository : EfRepository<Menu>, IMenuRepository
    {
        private readonly ApplicationDbContext dbContext;

        public MenuRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Menu>> GetListByRoleId(CancellationToken cancellationToken, long selectedRoleId)
        {
            var selectedRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Id == selectedRoleId, cancellationToken);
            if (selectedRole?.CategoryId is (int)RoleCategoryType.Admin)
                return await dbContext.Menus.ToListAsync(cancellationToken);
            return await (from r in dbContext.RoleMenus
                          join m in dbContext.Menus on r.MenuId equals m.Id
                          where r.RoleId == selectedRoleId
                          select m).Distinct().ToListAsync(cancellationToken);
        }
    }
}