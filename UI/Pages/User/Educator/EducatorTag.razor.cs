using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using UI.Helper;
using UI.Models;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Pages.User.Educator.Tabs;
using UI.Services;

namespace UI.Pages.User.Educator
{
    public partial class EducatorTag
    {
        [Parameter] public long? Id { get; set; }
        [Inject] public IEducatorService EducatorService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private EducatorResponseDTO Educator = new EducatorResponseDTO();
        private List<BreadCrumbLink> _links;
        private bool _loading = true;
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
        }
        protected override async void OnParametersSet()
        {
            if (Id != null)
            {
                var response = await EducatorService.GetById((long)Id);

                Educator = response.Item;

                Educator.AdministrativeTitlesForEducatorTag = string.Join(", ", Educator.EducatorAdministrativeTitles.Select(x => x.AdministrativeTitle.Name));
                _loading = false;
            }

            await GetBreadCrumbLinksForEditing();
            base.OnParametersSet();
            StateHasChanged();
        }

        private async Task GetBreadCrumbLinksForEditing()
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
                    IsActive = true,
                    To = "/educator/educators",
                    OrderIndex = 1,
                    Title = L["_List", L["Educator"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 2,
                    Title = Educator?.User?.Name,
                },
            };
        }
    }
}