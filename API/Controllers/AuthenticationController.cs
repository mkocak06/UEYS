
using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthService authService;

        public AuthenticationController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.GetUserList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await authService.GetPaginateList(cancellationToken, filter));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.CreateUser)]
        //public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] UserForRegisterDTO userForRegisterDTO) => StatusCode(201, await authService.Create(cancellationToken, userForRegisterDTO));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.GetUserPermissionsModel)]
        public async Task<IActionResult> GetUserPermissionsModelAsync(CancellationToken cancellationToken) => Ok(await authService.GetUserPermissionsModelAsync(cancellationToken));

        [HttpGet]
        public async Task<IActionResult> SendVerificationMessage(CancellationToken cancellationToken, string phone) => Ok(await authService.SendVerificationMessage(cancellationToken, phone));

        [HttpGet]
        public async Task<IActionResult> SendVerificationMail(CancellationToken cancellationToken, string email) => Ok(await authService.SendVerificationMail(cancellationToken, email));

        [HttpGet]
        public async Task<IActionResult> VerifyPhone(CancellationToken cancellationToken, int verificationCode) => Ok(await authService.VerifyPhone(cancellationToken, verificationCode));

        [HttpGet]
        public async Task<IActionResult> VerifyMail(CancellationToken cancellationToken, int verificationCode) => Ok(await authService.VerifyMail(cancellationToken, verificationCode));

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(CancellationToken cancellationToken, [FromBody] UserForLoginDTO userForLoginDTO) => Ok(await authService.Login(cancellationToken, userForLoginDTO));
        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult ResourceExample([FromBody] UserForLoginDTO userForLoginDTO) => Ok(authService.ResourceExample(userForLoginDTO));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.ListRoles)]
        public async Task<IActionResult> Roles(CancellationToken cancellationToken) => Ok(await authService.Roles(cancellationToken));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRolesByUserIdAsync(CancellationToken cancellationToken, long userId) => Ok(await authService.GetRolesByUserId(cancellationToken, userId));

        //[HttpGet("{userId}")]
        //[HasPermission(Shared.Types.PermissionEnum.ListRoles)]
        //public async Task<IActionResult> GetRolesByUserId(CancellationToken cancellationToken, long userId) => Ok(await authService.GetRolesByUserId(cancellationToken, userId));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.ListPermissions)]
        public IActionResult Permissions() => Ok(authService.Permissions());

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AddRole)]
        public async Task<IActionResult> AddRole(CancellationToken cancellationToken, [FromBody] RoleDTO role) => StatusCode(201, await authService.AddRole(cancellationToken, role));

        //[HttpPut]
        //[HasPermission(Shared.Types.PermissionEnum.UpdateRole)]
        //public async Task<IActionResult> UpdateRolePermissions(CancellationToken cancellationToken, [FromBody] RolePermissionDTO rolePermissionDTO) => Ok(await authService.UpdateRolePermissions(cancellationToken, rolePermissionDTO));

        //[HttpPut]
        //[HasPermission(Shared.Types.PermissionEnum.UpdateUserRoles)]
        //public async Task<IActionResult> UpdateUserRoles(CancellationToken cancellationToken, [FromBody] UserRoleDTO userRolesDTO) => Ok(await authService.UpdateUserRoles(cancellationToken, userRolesDTO));

        //[HttpPut]
        //[HasPermission(Shared.Types.PermissionEnum.UpdateUserRoles)]
        //public async Task<IActionResult> CreateUserRoles(CancellationToken cancellationToken, [FromBody] UserRoleDTO userRolesDTO) => Ok(await authService.CreateUserRoles(cancellationToken, userRolesDTO));
        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.CreateUserAccount)]
        public async Task<ActionResult> CreateUserAccount(CancellationToken cancellationToken, [FromBody] UserForRegisterDTO createUserRequest) => Ok(await authService.CreateUserAccount(cancellationToken, createUserRequest));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.GetUserList)]
        public async Task<ActionResult> GetUserAccountList(CancellationToken cancellationToken) => Ok(await authService.GetUserAccountList(cancellationToken));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.GetUserById)]
        public async Task<IActionResult> GetByIdAsync(CancellationToken cancellationToken) => Ok(await authService.GetByIdAsync(cancellationToken));

        [HttpPut("{userId}")]
        //[HasPermission(Shared.Types.PermissionEnum.UpdateUserAccount)]
        public async Task<ActionResult> UpdateUserAccount(CancellationToken cancellationToken, long userId, [FromBody] UpdateUserAccountInfoDTO updateUserAccount) => Ok(await authService.UpdateUserAccount(cancellationToken, userId, updateUserAccount));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.DeleteUser)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await authService.Delete(cancellationToken, id));

        //[HttpPut]
        //[HasPermission(Shared.Types.PermissionEnum.UpdateUserOwnAccountPassword)]
        //public async Task<ActionResult> ChangeUserPassword(CancellationToken cancellationToken, [FromBody] ChangePasswordDTO changePassword) => Ok(await authService.ChangeUserPassword(cancellationToken, changePassword));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AddPermissionToRole)]
        public async Task<IActionResult> AddPermissionToRole(CancellationToken cancellationToken, [FromBody] RolePermission2DTO rolePermissionDTO) => Ok(await authService.AddPermissionToRole(cancellationToken, rolePermissionDTO));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.RemovePermissionToRole)]
        public async Task<IActionResult> RemovePermissionToRole(CancellationToken cancellationToken, [FromBody] RolePermission2DTO rolePermissionDTO) => Ok(await authService.RemovePermissionToRole(cancellationToken, rolePermissionDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.UpdateRole)]
        public async Task<IActionResult> UpdateRole(CancellationToken cancellationToken, long id, [FromBody] RoleDTO roleDto) => Ok(await authService.UpdateRole(cancellationToken, id, roleDto.RoleName, roleDto.Description));

        [AllowAnonymous]
        [HttpPut("{selectedRoleId}")]
        public async Task<IActionResult> UpdateSelectedRole(CancellationToken cancellationToken, long selectedRoleId) => Ok(await authService.UpdateSelectedRole(cancellationToken, selectedRoleId));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.DeleteRole)]
        public async Task<IActionResult> RemoveRole(CancellationToken cancellationToken, long id) => Ok(await authService.RemoveRole(cancellationToken, id));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationGetRoleById)]
        public async Task<IActionResult> GetRoleById(CancellationToken cancellationToken, long id) => Ok(await authService.GetRoleById(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.UserExcelExport)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await authService.ExcelExport(cancellationToken, filter));
        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationGetRolesByUserRole)]
        public async Task<IActionResult> GetRolesByUserRole(CancellationToken cancellationToken) => Ok(await authService.GetRolesByUserRole(cancellationToken));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.CreateUserAccount)]
        public async Task<IActionResult> GetZone(CancellationToken cancellationToken, long roleId) => Ok(await authService.GetZone(cancellationToken, roleId));

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> AssignSelectedRoles(CancellationToken cancellationToken) //assign default values, run only once
            => Ok(await authService.AssignSelectedRoles(cancellationToken));



        #region roleMenu

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationAddMenuToRole)]
        public async Task<IActionResult> AddMenuToRole(CancellationToken cancellationToken, RoleMenuDTO roleMenu) => Ok(await authService.AddMenuToRole(cancellationToken, roleMenu.RoleId, roleMenu.MenuId));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationRemoveMenuToRole)]
        public async Task<IActionResult> RemoveMenuToRole(CancellationToken cancellationToken, RoleMenuDTO roleMenu) => Ok(await authService.RemoveMenuToRole(cancellationToken, roleMenu.RoleId, roleMenu.MenuId));

        #endregion
        #region field
        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.AuthorizationGetFieldsByUser)]
        //public async Task<IActionResult> GetFieldsByUser(CancellationToken cancellationToken) => Ok(await authService.GetFieldByUser(cancellationToken));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.AuthorizationAddFieldToRole)]
        //public async Task<IActionResult> AddFieldToRole(CancellationToken cancellationToken, RoleFieldDTO roleField) => Ok(await authService.AddFieldToRole(cancellationToken, roleField));
        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.AuthorizationRemoveFieldFromRole)]
        //public async Task<IActionResult> RemoveFieldFromRole(CancellationToken cancellationToken, RoleFieldDTO roleFieldDTO) => Ok(await authService.RemoveFieldFromRole(cancellationToken, roleFieldDTO));
        #endregion
        #region OGN
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RedirectSSO() => Ok(authService.RedirectSSO());

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout() => Ok(authService.Logout());

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginOpenId(CancellationToken cancellationToken, SsoLoginDTO ssoLoginDTO) => Ok(await authService.LoginOpenId(cancellationToken, ssoLoginDTO));
        #endregion
    }
}
