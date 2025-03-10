using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalService hospitalService;

        public HospitalController(IHospitalService hospitalService)
        {
            this.hospitalService = hospitalService;
        }



        [HttpGet]
        //[HasPermission(PermissionEnum.HospitalGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await hospitalService.GetListAsync(cancellationToken));

        [HttpPost]
        //[HasPermission(PermissionEnum.HospitalGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await hospitalService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{uniId}")]
        //[HasPermission(PermissionEnum.HospitalGetList)]
        public async Task<IActionResult> GetListByUniversityId(CancellationToken cancellationToken, long uniId) => Ok(await hospitalService.GetListByUniversityId(cancellationToken, uniId));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.HospitalGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await hospitalService.GetByIdAsync(cancellationToken, id));

        [HttpGet("{expId}")]
        public async Task<IActionResult> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expId) => Ok(await hospitalService.GetListByExpertiseBranchId(cancellationToken, expId));

        [HttpPost]
        [HasPermission(PermissionEnum.HospitalAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] HospitalDTO hospitalDTO) => Ok(await hospitalService.PostAsync(cancellationToken, hospitalDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.HospitalUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] HospitalDTO hospitalDTO) => Ok(await hospitalService.Put(cancellationToken, id, hospitalDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.HospitalDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await hospitalService.Delete(cancellationToken, id));

        [HttpGet]
        //[HasPermission(PermissionEnum.HospitalGetList)]
        public async Task<IActionResult> GetListForMap(CancellationToken cancellationToken, long? universityId) => Ok(await hospitalService.GetListForMap(cancellationToken, universityId));

        [HttpGet]
        [HasPermission(PermissionEnum.HospitalGetById)]
        public async Task<IActionResult> GetUserHospitalDetail(CancellationToken cancellationToken) => Ok(await hospitalService.GetUserHospitalDetail(cancellationToken));

        [HttpGet]
        public async Task<IActionResult> GetLatLongDetails(CancellationToken cancellationToken) => Ok(await hospitalService.GetLatLongDetailsFromGoogle(cancellationToken));

		[HttpPost]
		[HasPermission(PermissionEnum.HospitalExcelExport)]
		public async Task<IActionResult> DetailedExcelExport(CancellationToken cancellationToken, ProgramFilter filter) => Ok(await hospitalService.DetailedExcelExport(cancellationToken, filter));
        //{
        //    var byteArray = await hospitalService.DetailedExcelExport(cancellationToken, filter);
        //
        //    return File(byteArray.Item, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Rapor.xlsx");
        //}
        
        [HttpPost]
        [HasPermission(PermissionEnum.HospitalExcelExport)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await hospitalService.ExcelExport(cancellationToken, filter));

        [HttpPost]
        public async Task<IActionResult> ExcelImport(CancellationToken cancellationToken, IFormFile formFile) => Ok(await hospitalService.ImportFromExcel(cancellationToken, formFile));

        [HttpPost]
        //[HasPermission(PermissionEnum.HospitalGetList)]
        public async Task<IActionResult> GetHospitalCountByParentInstitution(CancellationToken cancellationToken, FilterDTO filter) => Ok(await hospitalService.GetHospitalCountByParentInstitution(cancellationToken, filter));

    }
}
