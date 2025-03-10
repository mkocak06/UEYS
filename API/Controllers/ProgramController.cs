using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService programService;
        private readonly IFileManagementService fileManagementService;

        public ProgramController(IProgramService programService, IFileManagementService fileManagementService)
        {
            this.programService = programService;
            this.fileManagementService = fileManagementService;
        }

        //[HttpGet]
        //[HasPermission(PermissionEnum.ProgramGetList)]
        //public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await programService.GetListAsync(cancellationToken));
        [HttpGet]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetProgramsCountForDashboard(CancellationToken cancellationToken) => Ok(await programService.GetProgramsForDashboard(cancellationToken));

        [HttpPost]
        //[HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetProgramsCountByUniversityType(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.CountByUniversityType(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetPaginateListForQuota(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetPaginateListForQuota(cancellationToken, filter));


        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetPaginateListForProtocolProgram(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetPaginateListForProtocolProgram(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetPaginateListOnly(cancellationToken, filter));

        [HttpPost]
        public async Task<IActionResult> GetPaginateListAll(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetPaginateListAll(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetListForSearch(CancellationToken cancellationToken, FilterDTO filter, bool getAll = false) => Ok(await programService.GetListForSearch(cancellationToken, filter, getAll));

        [HttpPost("{exBranchId}/{getAll}")]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetListForSearchByExpertiseBranchId(CancellationToken cancellationToken, FilterDTO filter, long exBranchId, bool getAll = false) => Ok(await programService.GetListForSearchByExpertiseBranchId(cancellationToken, filter, exBranchId, getAll));

        //[HttpGet("{uniId}")]
        //[HasPermission(PermissionEnum.ProgramGetListByUniversityId)]
        //public async Task<IActionResult> GetListByUniversityId(CancellationToken cancellationToken, long uniId) => Ok(await programService.GetListByUniversityId(cancellationToken, uniId));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await programService.GetByStudentIdAsync(cancellationToken, studentId));

        //[HttpGet("{expertiseBranchId}")]
        //[HasPermission(PermissionEnum.ProgramGetListByExpertiseBranchId)]
        //public async Task<IActionResult> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId) => Ok(await programService.GetListByExpertiseBranchIdAsync(cancellationToken, expertiseBranchId));

        //[HttpGet("{expertiseBranchId}")]
        //[HasPermission(PermissionEnum.ProgramGetListByExpertiseBranchId)]
        //public async Task<IActionResult> GetListByExpertiseBranchIdExceptOne(CancellationToken cancellationToken, long expertiseBranchId) => Ok(await programService.GetListByExpertiseBranchIdExceptOneAsync(cancellationToken, expertiseBranchId));

        [HttpGet]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetLocationsByExpertiseBranchId(CancellationToken cancellationToken, long? expBrId, long? authCategoryId) => Ok(await programService.GetLocationsByExpertiseBranchId(cancellationToken, expBrId, authCategoryId));

        [HttpGet("{hospitalId}")]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetProgramListByHospitalIdBreadCrumb(CancellationToken cancellationToken, long hospitalId) => Ok(await programService.GetProgramListByHospitalIdBreadCrumb(cancellationToken, hospitalId));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.ProgramGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await programService.GetByIdAsync(cancellationToken, id));

        [HttpGet]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetByHospitalAndBranchIdAsync(CancellationToken cancellationToken, long hospitalId, long expertiseBranchId) => Ok(await programService.GetByHospitalAndBranchIdAsync(cancellationToken, hospitalId, expertiseBranchId));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.ProgramGetById)]
        public async Task<IActionResult> GetWithBreadCrumbAsync(CancellationToken cancellationToken, long id) => Ok(await programService.GetByIdWithBreadcrumbAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ProgramDTO programDTO) => Ok(await programService.PostAsync(cancellationToken, programDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.ProgramUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ProgramDTO programDTO) => Ok(await programService.Put(cancellationToken, id, programDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.ProgramDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await programService.Delete(cancellationToken, id));

        // [HttpPost]
        // [HasPermission(PermissionEnum.ProgramImportFromExcel)]
        // public async Task<IActionResult> ImportFromExcel(CancellationToken cancellationToken, IFormFile formFile) => Ok(await programService.ImportFromExcel(cancellationToken, formFile));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramExportExcelList)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.ExcelExport(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetFieldNamesCountForDashboard(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetFieldNamesCountForDashboard(cancellationToken, filter));

        [HttpPost]
        //[HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> CountByAuthorizationCategory(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.CountByAuthorizationCategory(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetProgramCountByProvince(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetProgramCountByProvince(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetProgramCountByParentInstitution(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetProgramCountByParentInstitution(cancellationToken, filter));

        [HttpPost]
        //[HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.CountsByProfessionInstitution(cancellationToken, filter));


        [HttpPost("{hospitalId}")]
        [HasPermission(PermissionEnum.ProgramAdd)]
        public async Task<IActionResult> CreateAllBranchProgramsByHospitalId(CancellationToken cancellationToken, long hospitalId) => Ok(await programService.CreateAllBranchProgramsByHospitalId(cancellationToken, hospitalId));
    }
}
