using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITitleService
    {
        Task<ResponseWrapper<List<TitleResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<List<TitleResponseDTO>>> GetListByTypeAsync(CancellationToken cancellationToken, TitleType titleType);
        Task<ResponseWrapper<TitleResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<TitleResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<TitleResponseDTO>> PostAsync(CancellationToken cancellationToken, TitleDTO titleDTO);
        Task<ResponseWrapper<TitleResponseDTO>> Put(CancellationToken cancellationToken, long id, TitleDTO titleDTO);
        Task<ResponseWrapper<TitleResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
