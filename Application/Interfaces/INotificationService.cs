using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<PaginationModel<NotificationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<NotificationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<NotificationResponseDTO>> PostAsync(CancellationToken cancellationToken, NotificationDTO notificationDTO);
        Task<ResponseWrapper<NotificationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
