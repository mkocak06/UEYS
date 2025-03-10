using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentsSpecificEducationController : BaseController
    {
        private readonly IStudentsSpecificEducationService studentsSpecificEducationService;

        public StudentsSpecificEducationController(IStudentsSpecificEducationService studentsSpecificEducationService)
        {
            this.studentsSpecificEducationService = studentsSpecificEducationService;
        }

        [HttpPost]
        //[HasPermission(PermissionEnum.StudentsSpecificEducationPagianateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentsSpecificEducationService.GetPaginateList(cancellationToken, filter));


        [HttpGet]
        //[HasPermission(PermissionEnum.StudentsSpecificEducationGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await studentsSpecificEducationService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.StudentsSpecificEducationGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await studentsSpecificEducationService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentsSpecificEducationAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudentSpecificEducationDTO studentsSpecificEducationDTO) => Ok(await studentsSpecificEducationService.PostAsync(cancellationToken, studentsSpecificEducationDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentsSpecificEducationUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StudentSpecificEducationDTO studentsSpecificEducationDTO) => Ok(await studentsSpecificEducationService.Put(cancellationToken, id, studentsSpecificEducationDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.StudentsSpecificEducationDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await studentsSpecificEducationService.Delete(cancellationToken, id));

    }
}
