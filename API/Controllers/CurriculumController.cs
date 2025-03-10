using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurriculumController : BaseController
    {
        private readonly ICurriculumService curriculumService;

        public CurriculumController(ICurriculumService curriculumService)
        {
            this.curriculumService = curriculumService;
        }

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.CurriculumGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await curriculumService.GetListAsync(cancellationToken));

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.CurriculumGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await curriculumService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await curriculumService.GetByIdAsync(cancellationToken, id));

        [HttpGet("{expertiseBranchId}")]
        //[HasPermission(Shared.Types.PermissionEnum.CurriculumGetList)]
        public async Task<IActionResult> GetByExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId) => Ok(await curriculumService.GetByExpertiseBranchIdAsync(cancellationToken, expertiseBranchId));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.CurriculumGetList)]
        public async Task<IActionResult> GetLatestByBeginningDateAndExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId, DateTime beginningDate) => Ok(await curriculumService.GetLatestByBeginningDateAndExpertiseBranchIdAsync(cancellationToken, expertiseBranchId, beginningDate));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] CurriculumDTO curriculumDTO) => Ok(await curriculumService.PostAsync(cancellationToken, curriculumDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] CurriculumDTO curriculumDTO) => Ok(await curriculumService.Put(cancellationToken, id, curriculumDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await curriculumService.Delete(cancellationToken, id));

        [HttpPost("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumAdd)]
        public async Task<IActionResult> CreateCopy(CancellationToken cancellationToken, int id, [FromBody] CurriculumDTO curriculumDTO) => Ok(await curriculumService.CreateCopy(cancellationToken, id, curriculumDTO));

        [HttpPost] // TODO Silinecek
        public async Task<IActionResult> UploadExcel(CancellationToken cancellationToken, IFormFile formFile) => Ok(await curriculumService.UploadExcel(cancellationToken, formFile));

    }
}
