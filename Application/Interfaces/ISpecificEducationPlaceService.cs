using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISpecificEducationPlaceService
    {
        Task<ResponseWrapper<List<SpecificEducationPlaceResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<SpecificEducationPlaceResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> PostAsync(CancellationToken cancellationToken, SpecificEducationPlaceDTO specificEducationPlaceDTO);
        Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Put(CancellationToken cancellationToken, long id, SpecificEducationPlaceDTO specificEducationPlaceDTO);
        Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
