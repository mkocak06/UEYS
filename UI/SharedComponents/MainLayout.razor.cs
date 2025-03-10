using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using UI.Services;
using System;
using Majorsoft.Blazor.Components.GdprConsent;
using System.Globalization;
using UI.SharedComponents.Components;
using Shared.ResponseModels;
using AutoMapper;
using Shared.RequestModels;
using Microsoft.AspNetCore.Components.Forms;
using UI.Helper;
using ApexCharts;

namespace UI.SharedComponents;

public partial class MainLayout
{
    [Inject] private IJSRuntime JsRuntime { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IAuthenticationService AuthenticationService { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] private IMapper Mapper { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    private MyModal _clarificationTextModal;
    private MyModal _profileSettings;
    private GdprBanner _gdprBanner;
    private UserAccountDetailInfoResponseDTO _user = new();
    private EditContext _ecProfile;
    private int? _phoneVerificationCode;
    private int? _mailVerificationCode;
    private bool _loading;
    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationService.User is null)
        {
            if (!(NavigationManager.Uri.ToLower().Contains("resetpassword") || NavigationManager.Uri.ToLower().Contains("activation") || NavigationManager.Uri.ToLower().Contains("forgot") || NavigationManager.Uri.ToLower().Contains("ssologin")))
            {
                NavigationManager.NavigateTo("/login");
            }
        }
        _ecProfile = new EditContext(_user);
        try
        {
            var response = await AuthenticationService.GetUserById();
            if (response.Result)
            {
                _user = response.Item;
                _ecProfile = new EditContext(_user);
                if (!_user.IsReadClarification)
                {
                    _clarificationTextModal.OpenModal();
                    IsReadClarification();
                }
                if (_user.IsPhoneVerified != true || _user.IsMailVerified != true)
                {
                    _profileSettings.OpenModal();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        StateHasChanged();
        await base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            JsRuntime.InvokeAsync<bool>("CallJQueryFunctions");
        return base.OnAfterRenderAsync(firstRender);
    }

    private async void IsReadClarification()
    {
        _user.IsReadClarification = true;
        try
        {
            var response = await AuthenticationService.UpdateUserAccount(_user.Id, Mapper.Map<UpdateUserAccountInfoDTO>(_user));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    private async Task<bool> UpdateProfile()
    {
        if (!_ecProfile.Validate()) return false;
        try
        {
            var response = await AuthenticationService.UpdateUserAccount(_user.Id, Mapper.Map<UpdateUserAccountInfoDTO>(_user));

            return response.Result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private async Task SendVerificationMessage()
    {
        _loading = true;
        StateHasChanged();
        var response = await AuthenticationService.SendVerificationMessage(_user.Phone);

        if (response.Result)
            SweetAlert.IconAlert(SweetAlertIcon.success, L["Successfull"], L["Code Sent Successfully"]);
        else
            SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L["An error occurred while sending the code. Please check the contact information!"]);
        _loading = false;
        StateHasChanged();
    }

    private async Task SendVerificationMail()
    {
        _loading = true;
        StateHasChanged();
        var response = await AuthenticationService.SendVerificationMail(_user.Email.Trim());
        response.Result.PrintJson("response");
        if (response.Result)
            SweetAlert.IconAlert(SweetAlertIcon.success, L["Successfull"], L["Code Sent Successfully"]);
        else
            SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L["An error occurred while sending the code. Please check the contact information!"]);
        _loading = false;
        StateHasChanged();
    }

    private async Task VerifyPhone()
    {
        _loading = true;
        StateHasChanged();
        var response = await AuthenticationService.VerifyPhone(_phoneVerificationCode ?? 0);
        response.PrintJson("response2");
        if (response.Result)
        {
            _user.IsPhoneVerified = true;
            SweetAlert.IconAlert(SweetAlertIcon.success, "Başarılı", "Telefon Numarası Başarıyla Doğrulandı.");
            StateHasChanged();
            if (_user.IsMailVerified == true && _user.IsPhoneVerified == true)
                _profileSettings.CloseModal();
        }
        else
            SweetAlert.IconAlert(SweetAlertIcon.error, "Başarısız", "Telefon Numarası Doğrulanamadı.");

        _loading = false;
        StateHasChanged();
    }

    private async Task VerifyMail()
    {
        _loading = true;
        StateHasChanged();
        var response = await AuthenticationService.VerifyMail(_mailVerificationCode ?? 0);
        if (response.Result)
        {
            _user.IsMailVerified = true;
            SweetAlert.IconAlert(SweetAlertIcon.success, "Başarılı", "Mail Adresi Başarıyla Doğrulandı.");
            StateHasChanged();
            if (_user.IsMailVerified == true && _user.IsPhoneVerified == true)
                _profileSettings.CloseModal();
        }
        else
            SweetAlert.IconAlert(SweetAlertIcon.error, "Başarısız", "E-posta Adresi Doğrulanamadı.");

        _user.IsPhoneVerified.PrintJson("phone");
        _loading = false;
        StateHasChanged();
    }

    private async void NavigateToCookie()
    {
        var culture = CultureInfo.CurrentCulture;

        if (culture.Name == "tr-TR")
        {
            await JsRuntime.InvokeAsync<object>("open", "/CookiePolicy_tr", "_blank");
        }
        else if (culture.Name == "en-US")
        {
            await JsRuntime.InvokeAsync<object>("open", "/CookiePolicy_en", "_blank");
        }
    }
}