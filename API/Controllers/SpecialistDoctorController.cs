using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.Types;
using System.Threading.Tasks;
using System.Threading;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SpecialistDoctorController : ControllerBase
    {
        private readonly ISpecialistDoctorService specialistDoctorService;

        public SpecialistDoctorController(ISpecialistDoctorService specialistDoctorService)
        {
            this.specialistDoctorService = specialistDoctorService;
        }

        [HttpGet("{programId}")]
        public async Task<IActionResult> GetListByProgramId(CancellationToken cancellationToken, long programId) => Ok(await specialistDoctorService.GetListByProgramId(cancellationToken, programId));
    }
}
