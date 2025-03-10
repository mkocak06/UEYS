using Core.Entities;
using Core.Models.Authorization;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFormRepository : IRepository<Form>
    {
        IQueryable<Form> GetByZoneQueryable(ZoneModel zone);
        Task<FormResponseDTO> GetWithSubRecords(CancellationToken cancellationToken, long id);
    }
}
