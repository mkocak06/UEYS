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
    public partial class UniversityPrograms
    {

        [Inject] IState<UniversityDetailState> UniversityDetaiilState { get; set; }
        [Inject] IProgramService ProgramService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Inject] public IProgramService ProgramlService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IState<UniversityDetailState> UniversityDetailState { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }

        private UniversityResponseDTO University => UniversityDetailState.Value.University;
        private List<HospitalResponseDTO> Hospitals => UniversityDetailState.Value.Hospitals;
        private bool ProgramsLoaded => UniversityDetailState.Value.HospitalsLoaded;
        private bool ProgramsLoading => UniversityDetailState.Value.HospitalsLoading;
        private List<ProgramResponseDTO> _programs;
        private HospitalResponseDTO _program;
        private FilterDTO _filter;
        private PaginationModel<ProgramResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private EditContext _ec;
        private EditContext _programEc;
        private FacultyResponseDTO _faculty;

        protected override async Task OnInitializedAsync()
        {

             _program = new();
            _faculty = new();
            _ec = new EditContext(_program);

            _program = new HospitalResponseDTO();
            _programEc = new EditContext(_program);

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
                        Field = "Id"
                    }
                }
            };
            await GetPrograms();
            await base.OnInitializedAsync();
        }
        private async Task GetPrograms()
        {
            _paginationModel = await ProgramService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _programs = _paginationModel.Items;
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
            await GetPrograms();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetPrograms();
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
            await GetPrograms();
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
                await GetPrograms();
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