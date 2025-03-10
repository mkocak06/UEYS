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


namespace UI.Pages.InstitutionManagement.Institutions
{

    public partial class InstitutionDetail
    {
        [Parameter] public long? Id { get; set; }

        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private InstitutionResponseDTO _institution;
        private EditContext _ec;
        private bool _saving;
        private bool _notFound;
        private bool _loaded;
        private List<BreadCrumbLink> _links;

        protected override void OnInitialized()
        {
            _institution = new InstitutionResponseDTO();
            _ec = new EditContext(_institution);
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
                To = "/institution-management/institutions",
                OrderIndex = 1,
                Title =  L["_List",L["Parent Institution"]]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 1,
                Title = L["_Detail",L["Parent Institution"]]
            },
            };
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Id != null)
            {
                var response = await InstitutionService.GetById((long)Id);
                if (response.Result && response.Item != null)
                {
                    _institution = response.Item;
                    _ec = new EditContext(_institution);
                    _loaded = true;
                    StateHasChanged();
                }
                else
                {
                    _loaded = true;
                    _notFound = true;
                    StateHasChanged();
                    //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                    //NavigationManager.NavigateTo("./management/institutions");
                }
            }
            await base.OnParametersSetAsync();
        }

        private async Task Save()
        {
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();
            var dto = Mapper.Map<InstitutionDTO>(_institution);
            try
            {
                var response = await InstitutionService.Update(_institution.Id, dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                    NavigationManager.NavigateTo("./management/institutions");
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
            return _institution is { Id: > 0 } ? $"{L["_Detail", L["Institutions"]]}: {_institution.Name}" : "-";
        }


    }
}