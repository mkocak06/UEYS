using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;
using Infrastructure.Services;
using Core.Interfaces;
using Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EReportController : BaseController
    {
        private readonly IEducationTrackingService educationTrackingService;
        private readonly IExitExamService exitExamService;
        private readonly IOpinionFormService opinionFormService;
        private readonly IPerformanceRatingService performanceRatingService;
        private readonly IScientificStudyService scientificStudyService;
        private readonly IStudentPerfectionService studentPerfectionService;
        private readonly IStudentRotationService studentRotationService;
        private readonly IThesisService thesisService;

        public EReportController(IEducationTrackingService educationTrackingService, IExitExamService exitExamService, IOpinionFormService opinionFormService, IPerformanceRatingService performanceRatingService, IScientificStudyService scientificStudyService, IStudentPerfectionService studentPerfectionService, IStudentRotationService studentRotationService, IThesisService thesisService)
        {
            this.educationTrackingService = educationTrackingService;
            this.exitExamService = exitExamService;
            this.opinionFormService = opinionFormService;
            this.performanceRatingService = performanceRatingService;
            this.scientificStudyService = scientificStudyService;
            this.studentPerfectionService = studentPerfectionService;
            this.studentRotationService = studentRotationService;
            this.thesisService = thesisService;
        }

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetEducationTracking(CancellationToken cancellationToken, long studentId) => Ok(await educationTrackingService.GetListByStudentIdAsync(cancellationToken, studentId));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetExitExam(CancellationToken cancellationToken, FilterDTO filter) => Ok(await exitExamService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetOpinionForm(CancellationToken cancellationToken, long studentId) => Ok(await opinionFormService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetPerformanceRating(CancellationToken cancellationToken, long studentId) => Ok(await performanceRatingService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetScientificStudy(CancellationToken cancellationToken, long studentId) => Ok(await scientificStudyService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetPerfection(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType) => Ok(await studentPerfectionService.GetByStudentIdAsync(cancellationToken, studentId, perfectionType));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetRotation(CancellationToken cancellationToken, long studentId) => Ok(await studentRotationService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet("{userId}")]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetFormerRotation(CancellationToken cancellationToken, long userId) => Ok(await studentRotationService.GetFormerStudentsListByUserId(cancellationToken, userId));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.EReportGet)]
        public async Task<IActionResult> GetThesis(CancellationToken cancellationToken, long studentId) => Ok(await thesisService.GetListForEreportByStudentId(cancellationToken, studentId));

    }
}
