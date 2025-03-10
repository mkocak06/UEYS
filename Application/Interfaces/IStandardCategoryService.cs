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
    public interface IStandardCategoryService
    {
        Task<ResponseWrapper<List<StandardCategoryResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<StandardCategoryResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> PostAsync(CancellationToken cancellationToken, StandardCategoryDTO standardCategoryDTO);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> Put(CancellationToken cancellationToken, long id, StandardCategoryDTO standardCategoryDTO);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
