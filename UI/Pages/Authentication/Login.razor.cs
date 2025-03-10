using System;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Helper;
using UI.Services;

namespace UI.Pages.Authentication
{
    public partial class Login
    {
        [Inject] private IAuthenticationService AuthenticationService { get; set; }
        [Inject] private IDispatcher Dispatcher { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private ISweetAlert SweetAlert { get; set; }

        private UserForLoginDTO _model = new();
        private EditContext _ec;
        private bool _isLoading = false;

        private bool IsError = false;
        private string _error = String.Empty;
        private ColorType _errorColor = ColorType.Danger;

        private bool _showLogin;

        protected override async Task OnInitializedAsync()
        {
            if (AuthenticationService.User is not null)
            {
                NavigationManager.NavigateTo("/");
            }

            _ec = new EditContext(_model);
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async void HandleValidSubmit()
        {
            _isLoading = true;
            StateHasChanged();
            try
            {
                var result = await AuthenticationService.Login(_model);
                if (result.Result)
                {
                    string returnUrl = NavigationManager.QueryString("returnUrl") ?? "./";

                    NavigationManager.NavigateTo(returnUrl);
                }
                else
                {
                    _isLoading = false;

                    IsError = true;
                    _errorColor = ColorType.Danger;
                    _error = "Credentials mismatch";

                    StateHasChanged();
                }
            }
            catch (Exception)
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async void RedirectSSO()
        {
            try
            {
                string url = await AuthenticationService.SsoRedirect();
                if (!string.IsNullOrEmpty(url))
                    await JSRuntime.InvokeVoidAsync("open", url, "_self");
                else
                    throw new Exception("OGN Adresi alınamadı");
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "Hata", "OGN Sayfa alınamadı");
            }

        }
    }
}
