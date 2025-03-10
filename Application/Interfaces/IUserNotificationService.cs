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
    public interface IUserNotificationService
    {
        Task<PaginationModel<UserNotificationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<UserNotificationResponseDTO>> PostAsync(CancellationToken cancellationToken, UserNotificationDTO userNotificationDTO);
        Task<ResponseWrapper<UserNotificationResponseDTO>> Put(CancellationToken cancellationToken, long id, UserNotificationDTO userNotificationDTO);
        Task<ResponseWrapper<UserNotificationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
