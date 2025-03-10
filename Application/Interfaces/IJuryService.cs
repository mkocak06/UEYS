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
    public interface IJuryService
    {
        Task<ResponseWrapper<JuryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<JuryResponseDTO>> PostAsync(CancellationToken cancellationToken, JuryDTO juryDTO);
        Task<ResponseWrapper<JuryResponseDTO>> Delete(CancellationToken cancellationToken, long educatorId, long thesisId);
    }
}
