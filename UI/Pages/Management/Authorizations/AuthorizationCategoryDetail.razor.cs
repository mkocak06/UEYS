using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using UI.Models;
using UI.Services;


namespace UI.Pages.Management.Authorizations
{

    public partial class AuthorizationCategoryDetail
    {
        [Parameter] public long? Id { get; set; }

        [Inject] public IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private AuthorizationCategoryResponseDTO _authorizationCategory;
        private EditContext _ec;
        private bool _saving;
        private bool _loaded;
        private bool _notFound;
        private List<BreadCrumbLink> _links;




        protected override void OnInitialized()
        {
            _authorizationCategory = new AuthorizationCategoryResponseDTO();
            _ec = new EditContext(_authorizationCategory);
            _loaded = false;
            base.OnInitialized();
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
                    IsActive = true,
                    To = "/management/authorizationCategories",
                    OrderIndex = 1,
                    Title = L["_List", L["Authorization Category"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Authorization Category"]]
                }
        };
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Id != null)
            {
                var response = await AuthorizationCategoryService.GetById((long)Id);
                if (response.Result && response.Item != null)
                {
                    _authorizationCategory = response.Item;
                    _ec = new EditContext(_authorizationCategory);
                    _loaded = true;
                    StateHasChanged();
                }
                else
                {
                    _loaded = true;
                    _notFound = true;
                    StateHasChanged();
                    //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                    //NavigationManager.NavigateTo("./management/authorizationCategories");
                }
            }
            await base.OnParametersSetAsync();
        }

        private async Task Save()
        {
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();
            var dto = Mapper.Map<AuthorizationCategoryDTO>(_authorizationCategory);
            try
            {
                var response = await AuthorizationCategoryService.Update(_authorizationCategory.Id, dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                    NavigationManager.NavigateTo("./management/authorizationcategories");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
                Console.WriteLine(e.Message);
            }
            _saving = false;
            StateHasChanged();
        }

        private string GetTitle()
        {
            return _authorizationCategory is { Id: > 0 } ? $"{L["_Detail", L["Authorization Category"]]}: {_authorizationCategory.Name}" : "-";
        }

        private void OnChangeIsActiveUpdate1()
        {
            _authorizationCategory.IsActive = true;

            StateHasChanged();
        }

        private void OnChangeIsActiveUpdate2()
        {
            _authorizationCategory.IsActive = false;

            StateHasChanged();
        }
    }
}