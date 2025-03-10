using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.RequestModels.Authorization;
using Shared.ResponseModels;
using Shared.ResponseModels.Authorization;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseWrapper<UserForLoginResponseDTO>> Login(CancellationToken cancellationToken, UserForLoginDTO userForLoginDTO);
        Task<ResponseWrapper<UserForLoginResponseDTO>> OGNLogin(CancellationToken cancellationToken, string identityNo);
        Task<ResponseWrapper<UserForRegisterDTO>> Create(CancellationToken cancellationToken, UserForRegisterDTO userForRegisterDTO);
        Task<List<RoleResponseDTO>> Roles(CancellationToken cancellationToken);
        Task<List<RoleResponseDTO>> GetRolesByUserId(CancellationToken cancellationToken, long userId);
        ICollection<PermissionDTO> Permissions();
        Task<RoleResponseDTO> AddRole(CancellationToken cancellationToken, RoleDTO role);
        Task<ResponseWrapper<bool>> UpdateUserRoles(CancellationToken cancellationToken, UserRoleDTO userRoleDTO);
        Task<ResponseWrapper<bool>> UpdateRolePermissions(CancellationToken cancellationToken, RolePermissionDTO rolePermissionDTO);
        Task<ResponseWrapper<AccountInfoResponseDTO>> CreateUserAccount(CancellationToken cancellationToken, UserForRegisterDTO createUserRequest);
        Task<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>> GetUserAccountList(CancellationToken cancellationToken);
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> UpdateUserAccount(CancellationToken cancellationToken, long userId, UpdateUserAccountInfoDTO updateUserAccount);
        Task<ResponseWrapper<UserForLoginResponseDTO>> ChangeUserPassword(CancellationToken cancellationToken, ChangePasswordDTO changePassword);
        Task<ResponseWrapper<List<PermissionDTO>>> GetUserPermissionsModelAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<UserRoleResponseDTO>> CreateUserRoles(CancellationToken cancellationToken,UserRoleDTO userRoleDto);
        string ResourceExample(UserForLoginDTO userForLoginDTO);
        Task<PaginationModel<UserPaginateResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<bool>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetByIdAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<UpdateUserAccountInfoResponseDTO>> UnDeleteUser(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<PermissionDTO>>> AddPermissionToRole(CancellationToken cancellationToken, RolePermission2DTO rolePermissionDTO);
        Task<ResponseWrapper<List<PermissionDTO>>> RemovePermissionToRole(CancellationToken cancellationToken, RolePermission2DTO rolePermissionDTO);
        Task<ResponseWrapper<RoleResponseDTO>> UpdateRole(CancellationToken cancellationToken, long id, string roleName, string description);
        Task<ResponseWrapper<RoleResponseDTO>> RemoveRole(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<RoleResponseDTO>> GetRoleById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<bool>> AddMenuToRole(CancellationToken cancellationToken, long roleId, long menuId);
        Task<ResponseWrapper<bool>> RemoveMenuToRole(CancellationToken cancellationToken, long roleId, long menuId);
        Task<ResponseWrapper<List<RoleResponseDTO>>> GetRolesByUserRole(CancellationToken cancellationToken);
        Task<ResponseWrapper<Shared.ResponseModels.Authorization.ZoneModelDTO>> GetZone(CancellationToken cancellationToken, long roleId);
        Task<bool> UpdateSelectedRole(CancellationToken cancellationToken, long selectedRoleId);
        Task<bool> AssignSelectedRoles(CancellationToken cancellationToken);
        Task<ResponseWrapper<bool>> SendVerificationMessage(CancellationToken cancellationToken, string phone);
        Task<ResponseWrapper<bool>> SendVerificationMail(CancellationToken cancellationToken, string mail);
        Task<ResponseWrapper<bool>> VerifyPhone(CancellationToken cancellationToken, int verificationCode);
        Task<ResponseWrapper<bool>> VerifyMail(CancellationToken cancellationToken, int verificationCode);
        //Task<ResponseWrapper<List<FieldResponseDTO>>> GetFieldByUser(CancellationToken cancellationToken);
        //Task<ResponseWrapper<bool>> AddFieldToRole(CancellationToken cancellationToken, RoleFieldDTO roleFieldDTO);
        //Task<ResponseWrapper<bool>> RemoveFieldFromRole(CancellationToken cancellationToken, RoleFieldDTO roleFieldDTO);

        #region OGN
        string RedirectSSO();
        string Logout();
        Task<ResponseWrapper<UserForLoginResponseDTO>> LoginOpenId(CancellationToken cancellationToken, SsoLoginDTO ssoLoginDTO);
        #endregion
    }
}
