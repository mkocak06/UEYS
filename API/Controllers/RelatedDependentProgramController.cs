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
    public class RelatedDependentProgramController : BaseController
    {
        private readonly IRelatedDependentProgramService relatedDependentProgramService;

        public RelatedDependentProgramController(IRelatedDependentProgramService relatedDependentProgramService)
        {
            this.relatedDependentProgramService = relatedDependentProgramService;
        }

        
        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] RelatedDependentProgramDTO relatedDependentProgramDTO) => Ok(await relatedDependentProgramService.PostAsync(cancellationToken, relatedDependentProgramDTO));

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] RelatedDependentProgramDTO relatedDependentProgramDTO) => Ok(await relatedDependentProgramService.Put(cancellationToken, id, relatedDependentProgramDTO));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await relatedDependentProgramService.Delete(cancellationToken, id));

    }
}
