using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Authorization;
using Shared.ResponseModels.Wrapper;
using UI.Helper;

namespace UI.Services
{

    public interface IAuthService
    {

        Task<ResponseWrapper<AccountInfoResponseDTO>> CreateUserAccount(UserForRegisterDTO createUserRequest);
        Task<PaginationModel<UserPaginateResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<PaginationModel<UserPaginateResponseDTO>> GetArchiveList(FilterDTO filter);
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> UpdateUserAccount(long userId, UpdateUserAccountInfoDTO updateUserAccount, List<long> userRoleIds = null);
        //Task<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>> GetListByUserId(long UserId);
        Task<bool> Delete(long id);
        Task<List<RoleResponseDTO>> GetRolesByUserId( long userId);
        Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter);
        Task<ResponseWrapper<UpdateUserAccountInfoResponseDTO>> UnDeleteUser(long id);
        Task<ResponseWrapper<List<RoleResponseDTO>>> GetRolesByUserRole();
        Task<ResponseWrapper<ZoneModelDTO>> GetZone(long roleId);
    }

    public class AuthService : IAuthService
    {
        private readonly IHttpService _httpService;
        private readonly ILocalStorageService _localStorageService;

        public AuthService(IHttpService httpService, ILocalStorageService localStorageService)
        {
            _httpService = httpService;
            _localStorageService = localStorageService;
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> UpdateUserAccount(long userId, UpdateUserAccountInfoDTO updateUserAccount, List<long> userRoleIds = null)
        {
            var response =  await _httpService.Put<ResponseWrapper<UserAccountDetailInfoResponseDTO>>($"Authentication/UpdateUserAccount/{userId}", updateUserAccount);

            if (response.Result && userRoleIds != null)
            {
                var localUser = await _localStorageService.GetItem<UserForLoginResponseDTO>("user");
                localUser.UserRoleIds = userRoleIds;
                localUser.SelectedRoleId = response.Item.SelectedRoleId ?? 0;

                await _localStorageService.SetItem("user", localUser);
            }

            return response;
        }
        public async Task<ResponseWrapper<AccountInfoResponseDTO>> CreateUserAccount(UserForRegisterDTO createUserRequest)
        {
            return await _httpService.Post<ResponseWrapper<AccountInfoResponseDTO>>($"Authentication/CreateUserAccount", createUserRequest);
        }
        
        public async Task<List<RoleResponseDTO>> GetRolesByUserId(long userId)
        {
            return await _httpService.Get<List<RoleResponseDTO>>($"Authentication/GetRolesByUserId/{userId}");
        }
        
        public async Task<PaginationModel<UserPaginateResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<UserPaginateResponseDTO>>($"Authentication/GetPaginateList", filter);
        }

        public async Task<PaginationModel<UserPaginateResponseDTO>> GetArchiveList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<UserPaginateResponseDTO>>($"Archive/GetUserList", filter);
        }

        public async Task<bool> Delete(long id)
        {
            return await _httpService.Delete($"Authentication/Delete/{id}");
        }
        public async Task<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>> GetListByUserId(long UserId)
        {

            return await _httpService.Get<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>>($"Authentication/GetListByUserId/{UserId}");
        }
        
        public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter)
        {
            return await _httpService.ExcelByteArray("Authentication/ExcelExport", filter);
        }

        public async Task<ResponseWrapper<UpdateUserAccountInfoResponseDTO>> UnDeleteUser(long id)
        {
            return await _httpService.Put<ResponseWrapper<UpdateUserAccountInfoResponseDTO>>($"Archive/UnDeleteUser/{id}", null);
        }

        public async Task<ResponseWrapper<List<RoleResponseDTO>>> GetRolesByUserRole()
        {
            return await _httpService.Get<ResponseWrapper<List<RoleResponseDTO>>>("Authentication/GetRolesByUserRole");
        }

        public async Task<ResponseWrapper<ZoneModelDTO>> GetZone(long roleId)
        {
            return await _httpService.Get<ResponseWrapper<ZoneModelDTO>>($"Authentication/GetZone?roleId={roleId}");
        }
    }
}
