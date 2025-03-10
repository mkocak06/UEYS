using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILogService
    {
        Task<PaginationModel<LogResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<LogResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<LogResponseDTO>> PostAsync(CancellationToken cancellationToken, LogDTO logDTO);
        //Task<ResponseWrapper<LogResponseDTO>> Put(CancellationToken cancellationToken, long id, LogDTO logDTO);
        //Task<ResponseWrapper<LogResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
