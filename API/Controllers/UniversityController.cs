using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UniversityController : BaseController
    {
        private readonly IUniversityService universityService;

        public UniversityController(IUniversityService universityService)
        {
            this.universityService = universityService;
        }

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.UniversityGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await universityService.GetListAsync(cancellationToken));

        [HttpGet("{expertiseBranchId}")]
        public async Task<IActionResult> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId) => Ok(await universityService.GetListByExpertiseBranchId(cancellationToken, expertiseBranchId));

        //[HttpGet("{provinceId}")]
        //public async Task<IActionResult> GetListByProvinceId(CancellationToken cancellationToken, long provinceId) => Ok(await universityService.GetListByProvinceId(cancellationToken, provinceId));

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.UniversityGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await universityService.GetPaginateList(cancellationToken, filter));
       
        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.UniversityGetList)]
        public async Task<IActionResult> GetAffiliationPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await universityService.GetAffiliationPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.UniversityGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await universityService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.UniversityAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] UniversityDTO universityDTO) => Ok(await universityService.PostAsync(cancellationToken, universityDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.UniversityUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] UniversityDTO universityDTO) => Ok(await universityService.Put(cancellationToken, id, universityDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.UniversityDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await universityService.Delete(cancellationToken, id));

		[HttpPost]
		[HasPermission(Shared.Types.PermissionEnum.UniversityExcelExport)]
		public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await universityService.ExcelExport(cancellationToken, filter));

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.UniversityGetList)]
        public async Task<IActionResult> GetFilteredReport(CancellationToken cancellationToken, [FromBody] List<FilterDTO> filter)
          => Ok(await universityService.GetFilteredReport(cancellationToken, filter));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.UniversityGetList)]
        public async Task<IActionResult> UniversityCountByParentInstitution() => Ok(await universityService.UniversityCountByParentInstitution());
    }
}
