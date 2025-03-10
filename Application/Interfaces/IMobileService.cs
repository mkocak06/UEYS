using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.Mobile;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IMobileService
    {
        Task<PaginationModel<MobileProgramPaginateResponseDTO>> ProgramGetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<MobileProgramResponseDTO>> ProgramGetById(CancellationToken cancellationToken, long id);
        Task<PaginationModel<MobileEducatorPaginateResponseDTO>> EducatorGetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<EducatorResponseDTO>> EducatorGetById(CancellationToken cancellationToken, long id);
        Task<PaginationModel<MobileStudentPaginateResponseDTO>> StudentGetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<StudentResponseDTO>> StudentGetById(CancellationToken cancellationToken, long id, bool isDeleted = false);
        Task<ResponseWrapper<MobileUserResponseDTO>> UserGetById(CancellationToken cancellationToken);

    }
}
