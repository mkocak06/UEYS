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
    public interface IFormStandardService
    {
        Task<ResponseWrapper<List<FormStandardResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<FormStandardResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<FormStandardResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<FormStandardResponseDTO>> PostAsync(CancellationToken cancellationToken, FormStandardDTO standardDTO);
        Task<ResponseWrapper<FormStandardResponseDTO>> Put(CancellationToken cancellationToken, long id, FormStandardDTO standardDTO);
        Task<ResponseWrapper<FormStandardResponseDTO>> Delete(CancellationToken cancellationToken, long id);

    }
}
