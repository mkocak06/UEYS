using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProfessionService
    {
        Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<ProfessionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetByUniversityIdAsync(CancellationToken cancellationToken, long uniId);
        Task<PaginationModel<ProfessionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<ProfessionResponseDTO>> PostAsync(CancellationToken cancellationToken, ProfessionDTO facultyDTO);
        Task<ResponseWrapper<ProfessionResponseDTO>> Put(CancellationToken cancellationToken, long id, ProfessionDTO facultyDTO);
        Task<ResponseWrapper<ProfessionResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
