using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            this.propertyService = propertyService;
        }

        [HttpGet]
        //[HasPermission(PermissionEnum.PropertyGetByType)]
        public async Task<IActionResult> GetByType(CancellationToken cancellationToken, PropertyType propertyType, PerfectionType? perfectionType, string queryText = null) => Ok(await propertyService.GetByType(cancellationToken, propertyType, perfectionType, queryText));

        [HttpPost]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await propertyService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        public async Task<IActionResult> UploadExcel(CancellationToken cancellationToken, IFormFile formFile) => Ok(await propertyService.ImportPropertyList(cancellationToken, formFile)); // TODO silinecek

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.PropertyGetByType)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await propertyService.GetByIdAsync(cancellationToken, id));


        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.PropertyDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await propertyService.Delete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.PropertyAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] PropertyDTO propertyDTO) => Ok(await propertyService.PostAsync(cancellationToken, propertyDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.PropertyUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] PropertyDTO propertyDTO) => Ok(await propertyService.Put(cancellationToken, id, propertyDTO));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.PropertyGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await propertyService.GetListAsync(cancellationToken));

    }
}
