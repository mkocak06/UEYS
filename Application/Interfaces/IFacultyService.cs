using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IFacultyService
    {
        Task<PaginationModel<FacultyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<FacultyResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<List<FacultyResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
    }
}
