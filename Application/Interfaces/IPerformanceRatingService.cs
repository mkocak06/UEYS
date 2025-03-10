using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.FilterModels.Base;

namespace Application.Interfaces
{
    public interface IPerformanceRatingService
    {
        Task<PaginationModel<PerformanceRatingResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> PostAsync(CancellationToken cancellationToken, PerformanceRatingDTO performanceRatingDTO);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> Put(CancellationToken cancellationToken, long id, PerformanceRatingDTO performanceRatingDTO);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
