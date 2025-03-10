using Shared.FilterModels.Base;
using Shared.Models;
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
    public interface IExitExamService
    {
        Task<ResponseWrapper<List<ExitExamResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<ExitExamResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<ExitExamResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<ExitExamResponseDTO>> PostAsync(CancellationToken cancellationToken, ExitExamDTO ExitExamDTO);
        Task<ResponseWrapper<ExitExamResponseDTO>> Put(CancellationToken cancellationToken, long id, ExitExamDTO ExitExamDTO);
        Task<ResponseWrapper<ExitExamResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ExitExamRulesModel>> GetExitExamRules(CancellationToken cancellationToken, long studentId);
    }
}
