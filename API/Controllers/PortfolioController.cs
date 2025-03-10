using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            this.portfolioService = portfolioService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.PortfolioGetPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await portfolioService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.PortfolioGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await portfolioService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.PortfolioAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] PortfolioDTO portfolioDTO) => Ok(await portfolioService.PostAsync(cancellationToken, portfolioDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.PortfolioUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] PortfolioDTO portfolioDTO) => Ok(await portfolioService.Put(cancellationToken, id, portfolioDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.PortfolioDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await portfolioService.Delete(cancellationToken, id));
    }
}
