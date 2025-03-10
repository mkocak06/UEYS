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
    public interface IThesisService
    {
        Task<PaginationModel<ThesisResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ThesisResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<ThesisResponseDTO>> PostAsync(CancellationToken cancellationToken, ThesisDTO thesisDTO);
        Task<ResponseWrapper<ThesisResponseDTO>> Put(CancellationToken cancellationToken, long id, ThesisDTO thesisDTO);
        Task<ResponseWrapper<ThesisResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ThesisResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<ThesisResponseDTO>>> GetListForEreportByStudentId(CancellationToken cancellationToken, long studentId);
    }
}
