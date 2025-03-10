using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Models;
using UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail.Tabs
{
    public partial class AffiliationDetail
    {
        [Inject] IState<UniversityDetailState> UniversityDetailState { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IAffiliationService AffiliationService { get; set; }

        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        private List<AffiliationResponseDTO> _affiliations;
        private AffiliationResponseDTO _affiliation;
        private FilterDTO _filter;
        private PaginationModel<AffiliationResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private EditContext _ec;
        private EditContext _affiliationEc;
        private FacultyResponseDTO _faculty;

        private UniversityResponseDTO University => UniversityDetailState.Value.University;
        protected override async Task OnInitializedAsync()
        {
            _affiliation = new();
            _faculty = new();
            _ec = new EditContext(_affiliation);
            Dispatcher.Dispatch(new HospitalsLoadAction(University.Id));

            _affiliation = new AffiliationResponseDTO();
            _affiliationEc = new EditContext(_affiliation);

            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                {

                    new Filter()
                    {
                        Field="Faculty.UniversityId",
                        Operator="eq",
                        Value=University.Id
                    }

                },
                    Logic = "and"

                },
                page = 1,
                pageSize = 20,

                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "ProtocolNo"
                    }
                }


            };
            await GetAffiliations();
            base.OnInitialized();
        }
        private async Task GetAffiliations()
        {
            
            _paginationModel = await AffiliationService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _affiliations = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetAffiliations();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetAffiliations();
        }
        #region FilterChangeHandlers

        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filter.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            await GetAffiliations();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetAffiliations();
            }
        }

        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }

        #endregion
    }
}