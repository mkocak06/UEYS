using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Shared.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("ExternalIntegrationPolicy")]
    public class ENabizController : ControllerBase
    {
        private readonly IENabizService eNabizService;

        public ENabizController(IENabizService eNabizService)
        {
            this.eNabizService = eNabizService;
        }

        [HttpGet]
        [HasPermission(PermissionEnum.ENabizGetStudentList)]
        public async Task<IActionResult> GetStudentList(CancellationToken cancellationToken, DateTime? createDate) => Ok(await eNabizService.StudentList(cancellationToken, createDate));

        [HttpGet]
        [HasPermission(PermissionEnum.ENabizGetExpertiseBranchList)]
        public async Task<IActionResult> GetExpertiseBranchList(CancellationToken cancellationToken) => Ok(await eNabizService.ExpertiseBranchList(cancellationToken));
    }
}