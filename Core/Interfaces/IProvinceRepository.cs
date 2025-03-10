using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Shared.ResponseModels.StatisticModels;
using System.Threading.Tasks;
using System.Threading;

namespace Core.Interfaces
{
    public interface IProvinceRepository : IRepository<Province>
    {
        Task<List<CityDetailsForMapModel>> DetailsForMap(CancellationToken cancellationToken);
    }
}
 