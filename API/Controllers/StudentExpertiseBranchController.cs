using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentExpertiseBranchController : ControllerBase
    {
        private readonly IStudentExpertiseBranchService studentExpertiseBranchService;

        public StudentExpertiseBranchController(IStudentExpertiseBranchService studentExpertiseBranchService)
        {
            this.studentExpertiseBranchService = studentExpertiseBranchService;
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long studentId) => Ok(await studentExpertiseBranchService.GetListByStudentIdAsync(cancellationToken, studentId));

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudentExpertiseBranchDTO studentExpertiseBranchDTO) => Ok(await studentExpertiseBranchService.PostAsync(cancellationToken, studentExpertiseBranchDTO));

        [HttpDelete]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long studentId, long expBranchId) => Ok(await studentExpertiseBranchService.Delete(cancellationToken, studentId, expBranchId));
    }
}
