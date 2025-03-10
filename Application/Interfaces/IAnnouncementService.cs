using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IAnnouncementService
    {
        Task<PaginationModel<AnnouncementResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<AnnouncementResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<AnnouncementResponseDTO>> PostAsync(CancellationToken cancellationToken, AnnouncementDTO announcementDto);
        Task<ResponseWrapper<AnnouncementResponseDTO>> Put(CancellationToken cancellationToken, long id, AnnouncementDTO announcementDto);
        Task<ResponseWrapper<AnnouncementResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
