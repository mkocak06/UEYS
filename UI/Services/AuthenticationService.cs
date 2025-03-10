using System;
using Shared.RequestModels;
using Shared.ResponseModels;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Linq;
using UI.Helper;

namespace UI.Services
{
    public interface IAuthenticationService
    {
        UserForLoginResponseDTO User { get; }
        List<RoleResponseDTO> Roles { get; }
        List<PermissionDTO> UserPermissions { get; }
        Task Initialize();
        Task InitializeRoles();
        Task<ResponseWrapper<UserForLoginResponseDTO>> Login(UserForLoginDTO user);
        Task Logout();
        Task<IEnumerable<RoleResponseDTO>> GetRoles();
        Task<ResponseWrapper<bool>> UpdateRolePermissions(RolePermissionDTO rolePermissionDto);
        Task<ResponseWrapper<List<PermissionDTO>>> GetUserPermissionsModel();
        Task<IEnumerable<PermissionResponseDTO>> GetPermissions();
        Task<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>> GetUserAccountList();
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetUserById();
        Task<ResponseWrapper<bool>> SendVerificationMessage(string phone);
        Task<ResponseWrapper<bool>> SendVerificationMail(string email);
        Task<ResponseWrapper<bool>> VerifyPhone(int verificationCode);
        Task<ResponseWrapper<bool>> VerifyMail(int verificationCode);

        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> UpdateUserAccount(long userId, UpdateUserAccountInfoDTO updateUserAccount);
        Task<UserForLoginResponseDTO> UpdateUserAccount(ChangePasswordDTO changePassword);
        Task<string> SsoRedirect();
        Task<ResponseWrapper<UserForLoginResponseDTO>> SsoLogin(SsoLoginDTO ssoLoginDto);
        Task<string> SsoLogout();
        Task<RoleResponseDTO> UpdateRole(long id, RoleDTO role);
        Task<RoleResponseDTO> AddRole(RoleDTO role);
        Task<RolePermission2DTO> AddPermissionToRole(RolePermission2DTO rolePermission);
        Task<RolePermission2DTO> RemovePermissionToRole(RolePermission2DTO rolePermission);
        Task DeleteRole(long id);
        Task<ResponseWrapper<RoleResponseDTO>> GetRoleById(long id);
        Task<ResponseWrapper<bool>> AddMenuToRole(RoleMenuDTO roleMenu);
        Task<ResponseWrapper<bool>> RemoveMenuToRole(RoleMenuDTO roleMenu);
        bool IsPermitted(List<PermissionEnum> permissions);
        Task<bool> UpdateSelectedRole(long selectedRoleId);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;

        public UserForLoginResponseDTO User { get; private set; }
        public List<RoleResponseDTO> Roles { get; private set; }
        public List<PermissionDTO> UserPermissions { get; private set; }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService)
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<UserForLoginResponseDTO>("user");
            if (User != null)
            {
                await InitializeRoles();
            }
            else if (!(_navigationManager.Uri.ToLower().Contains("reset-password") || _navigationManager.Uri.ToLower().Contains("activation") || _navigationManager.Uri.ToLower().Contains("forgot") || _navigationManager.Uri.ToLower().Contains("ssologin")))
            {
                _navigationManager.NavigateTo("/login");
            }
        }
        public async Task InitializeRoles()
        {
            if (User != null)
            {
                try
                {
                    var userPermissionsResponse = await _httpService.Get<ResponseWrapper<List<PermissionDTO>>>("Authentication/GetUserPermissionsModel");
                    if (userPermissionsResponse.Result)
                        UserPermissions = userPermissionsResponse.Item;
                    Roles = await _httpService.Get<List<RoleResponseDTO>>("Authentication/Roles");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    await Logout();
                }
            }
        }
        public bool IsPermitted(List<PermissionEnum> permissions)
        {
            if (UserPermissions != null && UserPermissions.Any(x => x.Name == "SuperAdmin"))
            {
                return true;
            }
            if (UserPermissions != null && permissions != null && permissions.Any())
            {
                return UserPermissions.Any(x => permissions.Any(y => y.ToString() == x.Name));
            }
            return false;
        }
        public async Task<ResponseWrapper<UserForLoginResponseDTO>> Login(UserForLoginDTO user)
        {
            var result = await _httpService.Post<ResponseWrapper<UserForLoginResponseDTO>>("Authentication/Login", user);
            if (result.Result)
            {
                User = result.Item;
                await _localStorageService.SetItem("user", User);
                await InitializeRoles();
            }
            return result;
        }
        public async Task<string> SsoLogout()
        {
            return await _httpService.Get<string>($"Authentication/Logout");
        }
        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
        }
        public async Task<ResponseWrapper<RoleResponseDTO>> GetRoleById(long id)
        {
            return await _httpService.Get<ResponseWrapper<RoleResponseDTO>>($"Authentication/GetRoleById?id={id}");
        }

        public async Task<IEnumerable<RoleResponseDTO>> GetRoles()
        {
            return await _httpService.Get<IEnumerable<RoleResponseDTO>>("Authentication/Roles");
        }

        public async Task<IEnumerable<PermissionResponseDTO>> GetPermissions()
        {
            return await _httpService.Get<IEnumerable<PermissionResponseDTO>>("Authentication/Permissions");
        }

        public async Task<ResponseWrapper<bool>> UpdateRolePermissions(RolePermissionDTO rolePermissionDto)
        {
            return await _httpService.Put<ResponseWrapper<bool>>($"Authentication/UpdateRolePermissions", rolePermissionDto);
        }

        public async Task<ResponseWrapper<List<PermissionDTO>>> GetUserPermissionsModel()
        {
            return await _httpService.Get<ResponseWrapper<List<PermissionDTO>>>($"Authentication/GetUserPermissionsModel");
        }

        public async Task<ResponseWrapper<AddUserDTO>> AddUser(AddUserDTO user)
        {
            return await _httpService.Post<ResponseWrapper<AddUserDTO>>($"Authentication/AddUser", user);
        }


        public async Task<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>> GetUserAccountList()
        {
            return await _httpService.Get<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>>($"Authentication/GetUserAccountList");
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetUserById()
        {
            return await _httpService.Get<ResponseWrapper<UserAccountDetailInfoResponseDTO>>($"Authentication/GetById");
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> UpdateUserAccount(long userId, UpdateUserAccountInfoDTO updateUserAccount)
        {
            var response = await _httpService.Put<ResponseWrapper<UserAccountDetailInfoResponseDTO>>($"Authentication/UpdateUserAccount/{userId}", updateUserAccount);
            if (response.Result)
            {
                User.ProfilePhoto = response.Item.ProfilePhoto;
                User.Name = response.Item.Name;
                await _localStorageService.SetItem("user", User);
            }
            return response;
        }

        public async Task<UserForLoginResponseDTO> UpdateUserAccount(ChangePasswordDTO changePassword)
        {
            return await _httpService.Put<UserForLoginResponseDTO>($"Authentication/UpdateUserAccount", changePassword);
        }


        public async Task<string> SsoRedirect()
        {
            try
            {
                var result = await _httpService.Get<string>("Authentication/RedirectSSO");
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ResponseWrapper<UserForLoginResponseDTO>> SsoLogin(SsoLoginDTO ssoLoginDto)
        {
            ResponseWrapper<UserForLoginResponseDTO> result = await _httpService.Post<ResponseWrapper<UserForLoginResponseDTO>>($"Authentication/LoginOpenId", ssoLoginDto);
            if (result.Result)
            {
                User = result.Item;
                await _localStorageService.SetItem("user", result.Item);
                await InitializeRoles();
            }
            return result;
        }

        public async Task<RolePermission2DTO> AddPermissionToRole(RolePermission2DTO rolePermission)
        {
            return await _httpService.Post<RolePermission2DTO>(
                "Authentication/AddPermissionToRole", rolePermission);
        }

        public async Task<RolePermission2DTO> RemovePermissionToRole(RolePermission2DTO rolePermission)
        {
            return await _httpService.Post<RolePermission2DTO>(
                "Authentication/RemovePermissionToRole", rolePermission);
        }
        public async Task<ResponseWrapper<bool>> AddMenuToRole(RoleMenuDTO roleMenu)
        {
            return await _httpService.Post<ResponseWrapper<bool>>($"Authentication/AddMenuToRole", roleMenu);
        }

        public async Task<ResponseWrapper<bool>> RemoveMenuToRole(RoleMenuDTO roleMenu)
        {
            return await _httpService.Post<ResponseWrapper<bool>>(
                $"Authentication/RemoveMenuToRole", roleMenu);
        }
        public async Task<RoleResponseDTO> UpdateRole(long id, RoleDTO role)
        {
            return await _httpService.Put<RoleResponseDTO>(
                $"Authentication/UpdateRole/{id}", role);
        }

        public async Task<RoleResponseDTO> AddRole(RoleDTO role)
        {
            return await _httpService.Post<RoleResponseDTO>(
                "Authentication/AddRole", role);
        }

        public async Task DeleteRole(long id)
        {
            await _httpService.Delete(
                $"Authentication/RemoveRole/{id}");
        }
        public async Task<bool> UpdateSelectedRole(long selectedRoleId)
        {
            var response = await _httpService.Put<bool>(
                $"Authentication/UpdateSelectedRole/{selectedRoleId}", null);
            if (response == true)
            {
                User.SelectedRoleId = selectedRoleId;
                await _localStorageService.SetItem("user", User);
                return true;
            }
            else
                return false;
        }

        public async Task<ResponseWrapper<bool>> SendVerificationMessage(string phone)
        {
            return await _httpService.Get<ResponseWrapper<bool>>($"Authentication/SendVerificationMessage?phone={phone}");
        }

        public async Task<ResponseWrapper<bool>> SendVerificationMail(string email)
        {
            return await _httpService.Get<ResponseWrapper<bool>>($"Authentication/SendVerificationMail?email={email}");
        }

        public async Task<ResponseWrapper<bool>> VerifyPhone(int verificationCode)
        {
            return await _httpService.Get<ResponseWrapper<bool>>($"Authentication/VerifyPhone?verificationCode={verificationCode}");
        }

        public async Task<ResponseWrapper<bool>> VerifyMail(int verificationCode)
        {
            return await _httpService.Get<ResponseWrapper<bool>>($"Authentication/VerifyMail?verificationCode={verificationCode}");
        }
    }
}
