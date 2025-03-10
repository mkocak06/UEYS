using Core.Entities;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IOpinionFormService
    {
        Task<PaginationModel<OpinionFormResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<OpinionFormResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<OpinionFormResponseDTO>> PostAsync(CancellationToken cancellationToken, OpinionFormDTO opinionFormDTO);
        Task<ResponseWrapper<OpinionFormResponseDTO>> Put(CancellationToken cancellationToken, long id, OpinionFormDTO opinionFormDTO);
        Task<ResponseWrapper<OpinionFormResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<OpinionFormResponseDTO>> Cancellation(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<StudentStatusCheckDTO>> CheckNegativeOpinions(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetCanceledListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<OpinionFormResponseDTO>> GetStartAndEndDates(CancellationToken cancellationToken, long studentId);
    }
}
