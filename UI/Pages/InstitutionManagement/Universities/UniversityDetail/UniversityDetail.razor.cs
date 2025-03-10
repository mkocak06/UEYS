using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store;
using UI.Services;


namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail
{
    public partial class UniversityDetail
    {
        [Parameter] public long? Id { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<UniversityDetailState> UniversityDetailState { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private string _selectedTab = "university-overview";
        private bool Loaded => UniversityDetailState.Value.UniversitiesLoaded;
        private UniversityResponseDTO SelectedUniversity => UniversityDetailState.Value.University;

        private List<BreadCrumbLink> _links;
        private bool forceRender;

        protected override async Task OnInitializedAsync()
        {
            List<UniversityResponseDTO> universities = new ();
            try
            {
                var response = await UniversityService.GetAll();
                if (response.Result)
                    universities = response.Item;
            }
            catch (System.Exception)
            {
                
            }
            SubscribeToAction<UniversityDetailSetAction>((action) =>
            {
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
                    To = "/institution-management/universities",
                    OrderIndex = 1,
                    Title = L["Universities"]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 2,
                    Title = SelectedUniversity.Name,
                    DropdownList = universities.Select(x=> new DropdownElement()
                    {
                        Link = "/institution-management/universities/"+x.Id,
                        Name = x.Name
                    }).ToList()
                },
            };
            });
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (forceRender)
            {
                forceRender = false;
                JSRuntime.InvokeVoidAsync("initDropdown");
            }
            base.OnAfterRender(firstRender);
        }
        protected override void OnParametersSet()
        {
            if (Id != null)
            {
                Dispatcher.Dispatch(new UniversityDetailLoadAction((long)Id));
            }
            base.OnParametersSet();
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
                Dispatcher.Dispatch(new UniversityClearStateAction());
            return base.DisposeAsyncCore(disposing);
        }
    }
}
