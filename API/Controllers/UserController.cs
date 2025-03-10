using Application.Interfaces;
using Core.Interfaces;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public UserController(IUserService userService, IS3Service s3Service, IFileManagementService fileManagementService)
        {
            this.userService = userService;
            this.s3Service = s3Service;
            this.fileManagementService = fileManagementService;
        }

        [HttpGet("{identityNo}")]
        [HasPermission(PermissionEnum.EducatorAdd)]
        public async Task<IActionResult> GetUserByIdentityNoWithEducationalInfo(CancellationToken cancellationToken, string identityNo) => Ok(await userService.GetUserByIdentityNoWithEducationalInfo(cancellationToken, identityNo));

        [HttpGet]
        [HasPermission(PermissionEnum.AdvisorThesisAdd)]
        public async Task<IActionResult> GetUserByIdentityNoForThesis(CancellationToken cancellationToken, string identityNo, bool forAdvisor) => Ok(await userService.GetUserByIdentityNoForThesis(cancellationToken, identityNo, forAdvisor));

        [HttpPost]
        [HasPermission(PermissionEnum.EducatorAdd)]
        public async Task<IActionResult> AddUserWithEducatorInfo(CancellationToken cancellationToken, [FromBody] UserDTO userDTO) => Ok(await userService.PostWithEdu(cancellationToken, userDTO));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentAdd)]
        public async Task<IActionResult> AddUserWithStudentInfo(CancellationToken cancellationToken, [FromBody] UserDTO userDTO) => Ok(await userService.PostWithStu(cancellationToken, userDTO));

        [HttpGet("{identityNo}")]
        [HasPermission(PermissionEnum.UserGetByIdentityNo)]
        public async Task<IActionResult> GetUserByIdentityNoAsync(CancellationToken cancellationToken, string identityNo) => Ok(await userService.GetUserByIdentityNoAsync(cancellationToken, identityNo));

        [HttpGet]
        [HasPermission(PermissionEnum.UpdateUserAccount)]
        public async Task<IActionResult> GetUserForChangeIdentityNoAndName(CancellationToken cancellationToken, string identityNo) => Ok(await userService.GetUserForChangeIdentityNoAndName(cancellationToken, identityNo));

        [HttpGet]
        [HasPermission(PermissionEnum.UpdateUserAccount)]
        public async Task<IActionResult> ChangeIdentityNoAndName(CancellationToken cancellationToken, string identityNo, long id) => Ok(await userService.ChangeIdentityNoAndName(cancellationToken, identityNo, id));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken, long id) => Ok(await userService.GetById(cancellationToken, id));

        [HttpGet]
        public ActionResult GetActivePassiveForChart() => Ok(userService.GetActivePassiveList());

        [HttpPut("{userId}")]
        [HasPermission(PermissionEnum.UpdateUserAccount)]
        public async Task<ActionResult> UpdateActivePassiveUser(CancellationToken cancellationToken, long userId) => Ok(await userService.UpdateActivePassiveUser(cancellationToken, userId));

        //[HttpGet("{tc}")]
        //[HasPermission(PermissionEnum.UserGetById)]
        //public async Task<IActionResult> KPS(CancellationToken cancellationToken, string tc) => Ok(await userService.KPS(tc));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> GetUserInfoById(CancellationToken cancellationToken, long id) => Ok(await userService.GetUserInfoById(cancellationToken, id));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> GetAcademicInfoById(CancellationToken cancellationToken, long id) => Ok(await userService.GetAcademicInfoById(cancellationToken, id));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> GetCKYSInfoById(CancellationToken cancellationToken, long id) => Ok(await userService.GetCKYSInfoById(cancellationToken, id));

        [HttpGet("{identityNo}")]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> GetCKYSInfoByIdentityNo(string identityNo) => Ok(await userService.GetCKYSInfoByIdentityNo(identityNo));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> GetGraduationInfoById(CancellationToken cancellationToken, long id) => Ok(await userService.GetGraduationInfoById(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.UserDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.UserDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.UserDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

        [HttpPost]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> PostImageByte(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.PostImageByte(cancellationToken, fileModel));

        [HttpGet("{identityNo}")]
        [HasPermission(PermissionEnum.UserGetById)]
        public async Task<IActionResult> CheckEmailExist(CancellationToken cancellationToken, string email) => Ok(await userService.CheckEmailExist(cancellationToken, email));
    }
}
