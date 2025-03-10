using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Shared.Constants;
using Shared.RequestModels;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.Profile
{
    public partial class ProfileSettings
    {
        [Inject] IAuthenticationService AuthenticationService { get; set; }
        [Inject] IMapper Mapper { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] IState<AppState> AppState { get; set; }
        UserForLoginResponseDTO _user => AuthenticationService.User;
        private UserAccountDetailInfoResponseDTO UserInfo = new();
        private bool _loading = false;
        private List<BreadCrumbLink> _links;
        private ImageUpload _profileImage;
        protected override async Task OnInitializedAsync()
        {
            _links = new List<BreadCrumbLink>()
        {
            new BreadCrumbLink()
                {
                    IsActive = true,
                    To = "/",
                    OrderIndex = 0,
                    Title = L["Homepage"]
                },new BreadCrumbLink()
                {
                    IsActive = false,
                    To = "/settings",
                    OrderIndex = 1,
                    Title = L["My Profile"]
                }
        };
            _loading = true;
            StateHasChanged();
            try
            {
                var response = await AuthenticationService.GetUserById();
                if (response != null)
                {
                    UserInfo = response.Item;
                    StateHasChanged();
                }
                else
                {
                    SweetAlert.ErrorAlert();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally { _loading = false; StateHasChanged(); }
            await base.OnInitializedAsync();
        }
        public void ImageChanged(string imagePath)
        {
            UserInfo.ProfilePhoto = imagePath;
        }
        public async Task SaveProfileSettings()
        {
            _profileImage.SavePhoto(UserInfo.Id);
            var dto = Mapper.Map<UpdateUserAccountInfoDTO>(UserInfo);
            try
            {
                var response = await AuthenticationService.UpdateUserAccount(UserInfo.Id, dto);
                if (response.Result)
                {
                    UserInfo = response.Item;
                    Dispatcher.Dispatch(new ProfileSetAction());
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                    StateHasChanged();
                }
                else
                {
                    SweetAlert.ErrorAlert();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}