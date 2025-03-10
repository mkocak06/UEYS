using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EducatorProgramController : ControllerBase
    {
        private readonly IEducatorProgramService educatorProgramService;

        public EducatorProgramController(IEducatorProgramService educatorProgramService)
        {
            this.educatorProgramService = educatorProgramService;
        }

        [HttpGet("{educatorId}")]
        public async Task<IActionResult> GetListByEducatorId(CancellationToken cancellationToken, long educatorId) => Ok(await educatorProgramService.GetListByEducatorIdAsync(cancellationToken, educatorId));

        [HttpGet("{hospitalId}")]
        public async Task<IActionResult> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId) => Ok(await educatorProgramService.GetListByHospitalId(cancellationToken, hospitalId));

        [HttpGet("{programId}")]
        public async Task<IActionResult> GetListByProgramId(CancellationToken cancellationToken, long programId) => Ok(await educatorProgramService.GetListByProgramId(cancellationToken, programId));

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EducatorProgramDTO educatorProgramDTO) => Ok(await educatorProgramService.PostAsync(cancellationToken, educatorProgramDTO));

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] EducatorProgramDTO educatorProgramDTO) => Ok(await educatorProgramService.Put(cancellationToken, id, educatorProgramDTO));

        [HttpGet]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await educatorProgramService.Delete(cancellationToken, id));

        [HttpGet]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken, long id) => Ok(await educatorProgramService.GetById(cancellationToken, id));
    }
}
