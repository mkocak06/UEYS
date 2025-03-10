using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFormService
    {
        Task<ResponseWrapper<List<FormResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<FormResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<FormResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<FormResponseDTO>> PostAsync(CancellationToken cancellationToken, FormDTO FormDTO);
        Task<ResponseWrapper<FormResponseDTO>> Put(CancellationToken cancellationToken, long id, FormDTO FormDTO);
        Task<ResponseWrapper<FormResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
