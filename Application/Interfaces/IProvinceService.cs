using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using Shared.FilterModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.ResponseModels.StatisticModels;

namespace Application.Interfaces
{
    public interface IProvinceService
    {
        Task<ResponseWrapper<List<ProvinceResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<ProvinceResponseDTO>> PostAsync(CancellationToken cancellationToken, ProvinceDTO provinceDTO);
        Task<ResponseWrapper<ProvinceResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<ProvinceResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<ProvinceResponseDTO>> Put(CancellationToken cancellationToken, long id, ProvinceDTO provinceDTO);
        Task<ResponseWrapper<ProvinceResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<CityDetailsForMapModel>>> ProvinceDetailsForMap(CancellationToken cancellationToken);
    }
}
