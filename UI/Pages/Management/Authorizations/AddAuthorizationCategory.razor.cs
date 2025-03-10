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
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Management.Authorizations
{

    public partial class AddAuthorizationCategory
    {
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }

        [Inject] public IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private AuthorizationCategoryResponseDTO _authorizationCategory;
        private EditContext _ec;
        private bool _saving;
        private MyModal _authorizationCategoryListModal;
        private List<AuthorizationCategoryResponseDTO> _authorizationCategories;
        private List<BreadCrumbLink> _links;

        bool isOpened = false;
        string color = "#F1F7E9";

        private InputText _focusTarget;

        void OpenModal()
        {
            isOpened = true;
        }

        void ClosedEvent(string value)
        {
            color = value;
            isOpened = false;
            _authorizationCategory.ColorCode = value;
        }

        protected override void OnInitialized()
        {
            _authorizationCategory = new AuthorizationCategoryResponseDTO();
            _ec = new EditContext(_authorizationCategory);
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
                    Title = L["_List",L["Authorization Category"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Authorization Category"]]
                },
                };
        }
        private async Task Save()
        {
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();
            var dto = Mapper.Map<AuthorizationCategoryDTO>(_authorizationCategory);
            try
            {
                var response = await AuthorizationCategoryService.Add(dto);
                if (response.Result) 
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                    NavigationManager.NavigateTo($"./management/authorizationCategories");
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