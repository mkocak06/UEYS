using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ENabizPortfolioController : ControllerBase
    {
        private readonly IENabizPortfolioService _asistanHekimService;
        public ENabizPortfolioController(IENabizPortfolioService asistanHekimService)
        {
            _asistanHekimService = asistanHekimService;
        }

        [HttpGet("{userId}")]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetListPagination)]
        public async Task<IActionResult> GetByUserId(CancellationToken cancellationToken, long userId)
            => Ok(await _asistanHekimService.GetTotalOperationsByUserIdAsync(cancellationToken, userId));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetListPagination)]
        public async Task<IActionResult> GetByProgramId(CancellationToken cancellationToken, long programId, UserType? userType)
            => Ok(await _asistanHekimService.GetTotalOperationsByProgramIdAsync(cancellationToken, programId, userType));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetListPagination)]
        public async Task<IActionResult> GetList(CancellationToken cancellationToken)
            => Ok(await _asistanHekimService.GetAsync(cancellationToken));

        [HttpGet("{userId}")]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetListPagination)]
        public async Task<IActionResult> GetChartDataByUserId(CancellationToken cancellationToken, long userId)
            => Ok(await _asistanHekimService.GetUserOperationsChartData(cancellationToken, userId));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetListPagination)]
        public async Task<IActionResult> GetChartDataByProgramId(CancellationToken cancellationToken, long programId, UserType? userType)
            => Ok(await _asistanHekimService.GetUserOperationsChartDataByProgramId(cancellationToken, programId, userType));

        [HttpGet("{userId}")]
        [HasPermission(PermissionEnum.PerfectionExportExcelList)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, long userId) => Ok(await _asistanHekimService.ExcelExport(cancellationToken, userId));

        [HttpGet]
        [HasPermission(PermissionEnum.PerfectionExportExcelList)]
        public async Task<IActionResult> ExcelExportByProgramId(CancellationToken cancellationToken, long programId, UserType? userType) => Ok(await _asistanHekimService.ExcelExportByProgramId(cancellationToken, programId, userType));
    }
}
