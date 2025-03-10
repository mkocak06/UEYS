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
    public class EducatorCountContributionFormulaController : ControllerBase
    {
        private readonly IEducatorCountContributionFormulaService educatorCountContributionFormulaService;

        public EducatorCountContributionFormulaController(IEducatorCountContributionFormulaService educatorCountContributionFormulaService)
        {
            this.educatorCountContributionFormulaService = educatorCountContributionFormulaService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.EducatorCountContributionFormulaGetPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorCountContributionFormulaService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{id}")]
        //[HasPermission(PermissionEnum.EducatorCountContributionFormulaGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await educatorCountContributionFormulaService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.EducatorCountContributionFormulaAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EducatorCountContributionFormulaDTO educatorCountContributionFormulaDTO) => Ok(await educatorCountContributionFormulaService.PostAsync(cancellationToken, educatorCountContributionFormulaDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.EducatorCountContributionFormulaUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] EducatorCountContributionFormulaDTO educatorCountContributionFormulaDTO) => Ok(await educatorCountContributionFormulaService.Put(cancellationToken, id, educatorCountContributionFormulaDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.EducatorCountContributionFormulaDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await educatorCountContributionFormulaService.Delete(cancellationToken, id));
    }
}
