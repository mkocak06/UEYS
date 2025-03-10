using Application.Interfaces;
using Application.Services;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArchiveController : ControllerBase
    {
        private readonly IUniversityService universityService;
        private readonly IHospitalService hospitalService;
        private readonly IProgramService programService;
        private readonly IProtocolProgramService protocolProgramService;
        private readonly ICurriculumService curriculumService;
        private readonly ICurriculumRotationService curriculumRotationService;
        private readonly ICurriculumPerfectionService curriculumPerfectionService;
        private readonly IStudentService studentService;
        private readonly IEducatorService educatorService;
        private readonly IAuthService authService;
        private readonly IDocumentService documentService;

        public ArchiveController(IUniversityService universityService, IHospitalService hospitalService, IProgramService programService, IProtocolProgramService protocolProgramService, ICurriculumService curriculumService, ICurriculumRotationService curriculumRotationService, ICurriculumPerfectionService curriculumPerfectionService, IStudentService studentService, IEducatorService educatorService, IAuthService authService, IDocumentService documentService)
        {
            this.universityService = universityService;
            this.hospitalService = hospitalService;
            this.programService = programService;
            this.protocolProgramService = protocolProgramService;
            this.curriculumService = curriculumService;
            this.curriculumRotationService = curriculumRotationService;
            this.curriculumPerfectionService = curriculumPerfectionService;
            this.studentService = studentService;
            this.educatorService = educatorService;
            this.authService = authService;
            this.documentService = documentService;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetUniversityList)]
        public async Task<IActionResult> GetUniversityList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await universityService.GetPaginateList(cancellationToken,filter));
        
        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteUniversity)]
        public async Task<IActionResult> UnDeleteUniversity(CancellationToken cancellationToken, long id) => Ok(await universityService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetHospitalList)]
        public async Task<IActionResult> GetHospitalList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await hospitalService.GetPaginateList(cancellationToken, filter));
        
        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteHospital)]
        public async Task<IActionResult> UnDeleteHospital(CancellationToken cancellationToken, long id) => Ok(await hospitalService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetProgramList)]
        public async Task<IActionResult> GetProgramList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await programService.GetPaginateList(cancellationToken, filter));
        
        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteProgram)]
        public async Task<IActionResult> UnDeleteProgram(CancellationToken cancellationToken, long id) => Ok(await programService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetProtocolProgramList)]
        public async Task<IActionResult> GetProtocolProgramList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await protocolProgramService.GetPaginateList(cancellationToken, filter));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteProtocolProgram)]
        public async Task<IActionResult> UnDeleteProtocolProgram(CancellationToken cancellationToken, long id) => Ok(await protocolProgramService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetCurriculumList)]
        public async Task<IActionResult> GetCurriculumList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await curriculumService.GetPaginateList(cancellationToken, filter));
        
        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteCurriculum)]
        public async Task<IActionResult> UnDeleteCurriculum(CancellationToken cancellationToken, long id) => Ok(await curriculumService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetCurriculumList)]
        public async Task<IActionResult> GetCurriculumRotationList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await curriculumRotationService.GetPaginateList(cancellationToken, filter));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteCurriculum)]
        public async Task<IActionResult> UnDeleteCurriculumRotation(CancellationToken cancellationToken, long id) => Ok(await curriculumRotationService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetCurriculumList)]
        public async Task<IActionResult> GetCurriculumPerfectionList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await curriculumPerfectionService.GetPaginateList(cancellationToken, filter));
        
        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteCurriculum)]
        public async Task<IActionResult> UnDeleteCurriculumPerfection(CancellationToken cancellationToken, long id) => Ok(await curriculumPerfectionService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetStudentList)]
        public async Task<IActionResult> GetStudentList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.GetPaginateList(cancellationToken, filter));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteStudent)]
        public async Task<IActionResult> UnDeleteStudent(CancellationToken cancellationToken, long id) => Ok(await studentService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetEducatorList)]
        public async Task<IActionResult> GetEducatorList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.GetPaginateListOnly(cancellationToken, filter));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteEducator)]
        public async Task<IActionResult> UnDeleteEducator(CancellationToken cancellationToken, long id) => Ok(await educatorService.UnDelete(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetUserList)]
        public async Task<IActionResult> GetUserList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await authService.GetPaginateList(cancellationToken, filter));
        
        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteUser)]
        public async Task<IActionResult> UnDeleteUser(CancellationToken cancellationToken, int id) => Ok(await authService.UnDeleteUser(cancellationToken, id));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.ArchiveGetDocumentList)]
        public async Task<IActionResult> GetDocumentList(CancellationToken cancellationToken) => Ok(await documentService.GetDeletedList(cancellationToken));

        //[HttpPut("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.ArchiveUndeleteDocument)]
        //public async Task<IActionResult> UnDeleteDocument(CancellationToken cancellationToken, int id) => Ok(await documentService.UnDelete(cancellationToken, id));


    }
}
