using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentCountController : ControllerBase
    {
        private readonly IStudentCountService studentCountService;

        public StudentCountController(IStudentCountService studentCountService)
        {
            this.studentCountService = studentCountService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.StudentCountGetPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentCountService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.StudentCountGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await studentCountService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentCountAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudentCountDTO studentCountDTO) => Ok(await studentCountService.PostAsync(cancellationToken, studentCountDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentCountUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] StudentCountDTO studentCountDTO) => Ok(await studentCountService.Put(cancellationToken, id, studentCountDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.StudentCountDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await studentCountService.Delete(cancellationToken, id));
    }
}
