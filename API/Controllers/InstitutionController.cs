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
    public class InstitutionController : BaseController
    {
        private readonly IInstitutionService institutionService;

        public InstitutionController(IInstitutionService institutionService)
        {
            this.institutionService = institutionService;
        }

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.InstitutionGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await institutionService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.InstitutionGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await institutionService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.InstitutionAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] InstitutionDTO institutionDTO) => Ok(await institutionService.PostAsync(cancellationToken, institutionDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.InstitutionUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] InstitutionDTO institutionDTO) => Ok(await institutionService.Put(cancellationToken, id, institutionDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.InstitutionDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await institutionService.Delete(cancellationToken, id));

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.InstitutionGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await institutionService.GetPaginateList(cancellationToken, filter));
        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.InstitutionGetList)]
        public async Task<IActionResult> GetCountsByParentInstitution(CancellationToken cancellationToken, FilterDTO filter) => Ok(await institutionService.CountsByParentInstitution(cancellationToken, filter));
        
        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.InstitutionGetList)]
        public async Task<IActionResult> GetUniversityHospitalCountsByParentInstitution(CancellationToken cancellationToken) => Ok(await institutionService.UniversityHospitalCountsByParentInstitution(cancellationToken));

    }
}
