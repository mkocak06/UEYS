using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.University;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.Hospitals.HospitalDetail.Store;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail
{
    public partial class ProgramDetail
    {
        [Parameter] public long? Id { get; set; }

        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] public IProvinceService ProvinceService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private EditContext _ec;
        private ProgramResponseDTO _program => ProgramDetailState.Value.Program.Program;
        private bool _loading => ProgramDetailState.Value.ProgramDetailLoading;
        private bool _loaded => ProgramDetailState.Value.ProgramDetailLoaded;
        private bool _saving => ProgramDetailState.Value.UpdateProgramDetailLoading;


        private UserForLoginResponseDTO _user => AuthenticationService.User;
        private string _selectedTab = "program";
        private List<BreadCrumbLink> _links;
        private bool forceRender;


        protected override async Task OnInitializedAsync()
        {
            //SubscribeToAction<ProgramDetailFailureAction>(async (action) =>
            //{
            //    await SweetAlert.ConfirmAlert(L["Page not found"], "", SweetAlertIcon.error, false, L["OK"], "");
            //    NavigationManager.NavigateTo("./institution-management/programs");
            //});
            _user.PrintJson("user");
            SubscribeToAction<ProgramDetailUpdateSuccessAction>((action) =>
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                NavigationManager.NavigateTo("./institution-management/programs");
            });
            SubscribeToAction<ProgramDetailUpdateFailureAction>(async (action) =>
            {
                await SweetAlert.ConfirmAlert(L["Could not be updated"], "", SweetAlertIcon.error, false, L["OK"], "");
            });
            SubscribeToAction<ProgramDetailSetAction>(async (action) =>
            {
                //List<ProgramExpertiseBreadcrumbResponseDTO> _universityPrograms = ProgramDetailState.Value.Program.RelatedPrograms?.ToList()??new();
                //List<UniversityBreadcrumbDTO> _expertiseUniversities = ProgramDetailState.Value.Program.Universities?.ToList()??new();
                //List<HospitalBreadcrumbDTO> _expertiseHospitals= ProgramDetailState.Value.Program.Hospitals?.ToList()??new();
                //StateHasChanged();

                _ec = new(_program);
                forceRender = true;
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
                    To = "/institution-management/programs",
                    OrderIndex = 1,
                    Title = L["Expertise Training Programs"]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 2,
                    Title = _program.Hospital?.Name,
                },new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 3,
                    Title = _program.ExpertiseBranch?.Name
                },
            };
                StateHasChanged();
            });
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (forceRender)
            {
                forceRender = false;
                JSRuntime.InvokeVoidAsync("initDropdown");
                JSRuntime.InvokeVoidAsync("initTooltip");
            }
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Id != null)
            {
                Dispatcher.Dispatch(new ProgramDetailLoadAction((long)Id));
            }
            await base.OnParametersSetAsync();
        }

        private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
        {
            var result = await ProvinceService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<ProvinceResponseDTO>();
        }


        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitutions(string searchQuery)
        {
            var result = await InstitutionService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<InstitutionResponseDTO>();
        }


        private async Task Save()
        {
            if (!_ec.Validate()) return;

            var dto = Mapper.Map<ProgramDTO>(_program);
            if (dto.FacultyId == null)
            {
                dto.Faculty = null;
            }
            Dispatcher.Dispatch(new ProgramDetailUpdateAction(_program.Id, dto));
        }

        private void SelectTab(string link)
        {
            if (_selectedTab != link)
            {
                _selectedTab = link;
                StateHasChanged();
            }
        }
        private bool IsSelected(string link)
        {
            return link == _selectedTab;
        }

        protected override ValueTask DisposeAsyncCore(bool disposing)
        {
            if (disposing)
                Dispatcher.Dispatch(new ProgramDetailClearStateAction());
            return base.DisposeAsyncCore(disposing);
        }
    }
}

