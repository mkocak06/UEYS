using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.NotificationGetPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await notificationService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.NotificationGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await notificationService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.NotificationAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] NotificationDTO notificationDTO) => Ok(await notificationService.PostAsync(cancellationToken, notificationDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.NotificationDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await notificationService.Delete(cancellationToken, id));
    }
}
