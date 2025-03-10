using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService _announcementService;
        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.AnnouncementGetListPagination)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, [FromBody] FilterDTO filter)
            => Ok(await _announcementService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AnnouncementGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id)
            => Ok(await _announcementService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AnnouncementAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] AnnouncementDTO announcementDto)
            => Ok(await _announcementService.PostAsync(cancellationToken, announcementDto));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AnnouncementUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] AnnouncementDTO announcementDto)
            => StatusCode(201, await _announcementService.Put(cancellationToken, id, announcementDto));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AnnouncementDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id)
            => Ok(await _announcementService.Delete(cancellationToken, id));
    }
}
