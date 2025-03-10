using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRelatedDependentProgramService
    {
        Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, RelatedDependentProgramDTO relatedDependentProgramDTO);
        Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, RelatedDependentProgramDTO relatedDependentProgramDTO);
        Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id);

    }
}
