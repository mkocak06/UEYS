using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<List<Menu>> GetListByRoleId(CancellationToken cancellationToken, long selectedRoleId);
    }
}