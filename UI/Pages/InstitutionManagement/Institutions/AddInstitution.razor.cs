using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Models;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Institutions
{

    public partial class AddInstitution
    {
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private InstitutionResponseDTO _institution;
        private EditContext _ec;
        private bool _saving;
        private List<BreadCrumbLink> _links;

        private InputText _focusTarget;
        protected override void OnInitialized()
        {
            _institution = new InstitutionResponseDTO();
            _ec = new EditContext(_institution);
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
                    Title = L["_List",L["Parent Institution"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Parent Institution"]]
                },
                };
        }
       
        private async Task Save()
        {
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();
            var dto = Mapper.Map<InstitutionDTO>(_institution);
            try
            {
                var response = await InstitutionService.Add(dto);

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                    NavigationManager.NavigateTo($"./institution-management/institutions/");
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
    }

















}