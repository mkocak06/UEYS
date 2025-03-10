using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SpecificEducationPlaceController : BaseController
    {
        private readonly ISpecificEducationPlaceService specificEducationPlaceService;

        public SpecificEducationPlaceController(ISpecificEducationPlaceService specificEducationPlaceService)
        {
            this.specificEducationPlaceService = specificEducationPlaceService;
        }

        [HttpPost]
        //[HasPermission(PermissionEnum.SpecificEducationPlacePagianateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await specificEducationPlaceService.GetPaginateList(cancellationToken, filter));


        [HttpGet]
        //[HasPermission(PermissionEnum.SpecificEducationPlaceGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await specificEducationPlaceService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.SpecificEducationPlaceGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await specificEducationPlaceService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.SpecificEducationPlaceAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] SpecificEducationPlaceDTO specificEducationDTO) => Ok(await specificEducationPlaceService.PostAsync(cancellationToken, specificEducationDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.SpecificEducationPlaceUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] SpecificEducationPlaceDTO specificEducationDTO) => Ok(await specificEducationPlaceService.Put(cancellationToken, id, specificEducationDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.SpecificEducationPlaceDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await specificEducationPlaceService.Delete(cancellationToken, id));

    }
}
