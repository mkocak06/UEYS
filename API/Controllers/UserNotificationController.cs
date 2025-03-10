using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserNotificationController : ControllerBase
    {
        private readonly IUserNotificationService userNotificationService;

        public UserNotificationController(IUserNotificationService userNotificationService)
        {
            this.userNotificationService = userNotificationService;
        }

        [HttpPost]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await userNotificationService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] UserNotificationDTO userNotificationDTO) => Ok(await userNotificationService.PostAsync(cancellationToken, userNotificationDTO));

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] UserNotificationDTO userNotificationDTO) => Ok(await userNotificationService.Put(cancellationToken, id, userNotificationDTO));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await userNotificationService.Delete(cancellationToken, id));
    }
}
