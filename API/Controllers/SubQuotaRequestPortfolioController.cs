using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubQuotaRequestPortfolioController : ControllerBase
    {
        private readonly ISubQuotaRequestPortfolioService subQuotaRequestPortfolioService;

        public SubQuotaRequestPortfolioController(ISubQuotaRequestPortfolioService subQuotaRequestPortfolioService)
        {
            this.subQuotaRequestPortfolioService = subQuotaRequestPortfolioService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] SubQuotaRequestPortfolioDTO subQuotaRequestPortfolioDTO) => Ok(await subQuotaRequestPortfolioService.PostAsync(cancellationToken, subQuotaRequestPortfolioDTO));

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] SubQuotaRequestPortfolioDTO subQuotaRequestPortfolioDTO) => Ok(await subQuotaRequestPortfolioService.Put(cancellationToken, id, subQuotaRequestPortfolioDTO));

        [HttpDelete]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await subQuotaRequestPortfolioService.Delete(cancellationToken, id));
    }
}
