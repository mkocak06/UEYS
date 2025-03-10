using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentDependentProgramController : ControllerBase
    {
        private readonly IStudentDependentProgramService StudentDependentProgramService;

        public StudentDependentProgramController(IStudentDependentProgramService StudentDependentProgramService)
        {
            this.StudentDependentProgramService = StudentDependentProgramService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StudentDependentProgramDTO StudentDependentProgramDTO) => Ok(await StudentDependentProgramService.Put(cancellationToken, id, StudentDependentProgramDTO));

        [HttpGet]
        public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId)=> Ok(await StudentDependentProgramService.GetListByStudentId(cancellationToken, studentId));

    }
}
