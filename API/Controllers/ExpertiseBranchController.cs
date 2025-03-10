using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExpertiseBranchController : ControllerBase
    {
        private readonly IExpertiseBranchService expertiseBranchService;

        public ExpertiseBranchController(IExpertiseBranchService expertiseBranchService)
        {
            this.expertiseBranchService = expertiseBranchService;
        }

        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchGetList)]
        //public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await expertiseBranchService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await expertiseBranchService.GetByIdAsync(cancellationToken, id));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchGetListRelatedWithPrograms)]
        //public async Task<IActionResult> GetListRelatedWithProgramsByProfessionIdAsync(CancellationToken cancellationToken, long id) => Ok(await expertiseBranchService.GetListRelatedWithProgramsByProfessionIdAsync(cancellationToken, id));
        
        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchGetListByProfessionId)]
        //public async Task<IActionResult> GetListByProfessionIdAsync(CancellationToken cancellationToken, long id) => Ok(await expertiseBranchService.GetListByProfessionIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ExpertiseBranchDTO expertiseBranchDTO) => Ok(await expertiseBranchService.PostAsync(cancellationToken, expertiseBranchDTO));
       
        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await expertiseBranchService.GetPaginateList(cancellationToken, filter));
        
        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ExpertiseBranchDTO expertiseBranchDTO) => Ok(await expertiseBranchService.Put(cancellationToken, id, expertiseBranchDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await expertiseBranchService.Delete(cancellationToken, id));

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.ExpertiseBranchUploadFromExcel)]
        public async Task<ActionResult> UploadFromExcel(CancellationToken cancellationToken, IFormFile file) => Ok(await expertiseBranchService.ImportFromExcel(cancellationToken, file));
        
        [HttpGet("{hospitalId}")]
        public async Task<IActionResult> GetListForProtocolProgramByHospitalId(CancellationToken cancellationToken, long hospitalId) => Ok(await expertiseBranchService.GetListForProtocolProgramByHospitalId(cancellationToken, hospitalId));

        //[HttpPost]
        //public async Task<ActionResult> ImportFromExcelXX(CancellationToken cancellationToken, IFormFile file) => Ok(await expertiseBranchService.ImportFromExcelXX(cancellationToken, file));
    }
}
