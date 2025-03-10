using Application.Services;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.MenuGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id)
            => Ok(await _menuService.GetByIdAsync(cancellationToken, id));

        [HttpGet]
        public async Task<IActionResult> GetList(CancellationToken cancellationToken)
            => Ok(await _menuService.GetListByUser(cancellationToken));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.MenuAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] MenuDTO menuDto)
            => Ok(await _menuService.PostAsync(cancellationToken, menuDto));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.MenuUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] MenuDTO menuDto)
            => StatusCode(201, await _menuService.Put(cancellationToken, id, menuDto));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.MenuDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id)
            => Ok(await _menuService.Delete(cancellationToken, id));
    }
}
