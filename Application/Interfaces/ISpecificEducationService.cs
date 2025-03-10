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
    public interface ISpecificEducationService
    {
        Task<ResponseWrapper<List<SpecificEducationResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<SpecificEducationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<SpecificEducationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<SpecificEducationResponseDTO>> PostAsync(CancellationToken cancellationToken, SpecificEducationDTO specificEducationDTO);
        Task<ResponseWrapper<SpecificEducationResponseDTO>> Put(CancellationToken cancellationToken, long id, SpecificEducationDTO specificEducationDTO);
        Task<ResponseWrapper<SpecificEducationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
