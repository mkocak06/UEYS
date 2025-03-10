using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.Services;
namespace UI.Pages.Archive.Curriculums
{
    public partial class ArchiveCurriculumDetail
    {
        [Parameter] public long? Id { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private bool _editing = false;

        private List<BreadCrumbLink> _links;
        private bool forceRender;

        protected override async Task OnInitializedAsync()
        {
            if (Id is not null)
            {
                _editing = true;
                StateHasChanged();
                await GetBreadCrumbLinksForEditing();
            }
            else
            {
                _editing = false;
                StateHasChanged();
                GetBreadCrumbLinksForAdding();
            }
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
        protected override async void OnParametersSet()
        {
            if (Id != null)
            {
                _editing = true;
                StateHasChanged();
                await GetBreadCrumbLinksForEditing();
                StateHasChanged();
                Dispatcher.Dispatch(new CurriculumDetailLoadAction((long)Id));
            }
            base.OnParametersSet();
        }
        private void GetBreadCrumbLinksForAdding()
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
                    To = "/institution-management/curriculums",
                    OrderIndex = 1,
                    Title = L["Curriculums"]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Curriculum"]]
                },
                };
        }
        private async Task GetBreadCrumbLinksForEditing()
        {
            List<CurriculumResponseDTO> curriculums = new();
            try
            {
                var response = await CurriculumService.GetAll();
                if (response.Result)
                    curriculums = response.Item;
            }
            catch (System.Exception)
            {

            }
            SubscribeToAction<CurriculumDetailSetAction>((action) =>
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
                    To = "/institution-management/curriculums",
                    OrderIndex = 1,
                    Title = L["Curriculums"]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 2,
                    Title = action.Curriculum.ExpertiseBranch.Name+" (v"+action.Curriculum.Version+")",
                    DropdownList = curriculums.Select(x=> new DropdownElement()
                    {
                        Link = "/institution-management/curriculums/"+x.Id,
                        Name = x.ExpertiseBranch.Name+" (v"+x.Version+")"
                    }).ToList()
                },
                };
            });
        }
    }
}
