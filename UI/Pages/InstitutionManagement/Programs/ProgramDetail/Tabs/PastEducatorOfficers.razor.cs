using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.DetailCards;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Tabs
{
    public partial class PastEducatorOfficers
    {
        [Inject] public IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IEducationOfficerService EducationOfficerService{ get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private ProgramResponseDTO ProgramDetail => ProgramDetailState.Value.Program.Program;
        private List<EducationOfficerResponseDTO> _educationOfficers = new List<EducationOfficerResponseDTO>() { new EducationOfficerResponseDTO() { Educator = new() { User = new(), AcademicTitle = new(), StaffTitle = new() }, Program = new() } };
        private FilterDTO _filter = new();
        private PaginationModel<EducationOfficerResponseDTO> _paginationModel;
        private EducatorViewer _selectedEducatorDetail;
        private EducatorResponseDTO _selectedEducator;
        protected override async void OnInitialized()
        {
            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                    {
                         new Filter()
                        {
                            Field="ProgramId",
                            Operator="eq",
                            Value=ProgramDetail.Id
                        },
                    },
                    Logic = "and"

                },

                Sort = new[] { new Sort()
                {
                    Field = "StartDate",
                    Dir = SortType.ASC
                } }
            };

            await GetEducationOfficers();
            base.OnInitialized();
        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetEducationOfficers();
        }

        private async Task GetEducationOfficers()
        {
            try
            {
                _paginationModel = await EducationOfficerService.GetPaginateListForProgramDetail(_filter);

                if (_paginationModel.Items != null)
                {
                    _educationOfficers = _paginationModel.Items;
                }
                else
                {
                    _educationOfficers = new();
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message);
            }
            finally
            {
                StateHasChanged();
            }
        }

        private void OnEducatorDetailHandler(EducatorResponseDTO educator)
        {
            _selectedEducator = educator;
            _selectedEducator.Id = educator.Id;
            _selectedEducatorDetail.OpenModal();
        }

        #region EducatorFilter
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;
            await GetEducationOfficers();
        }

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
            await GetEducationOfficers();
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
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetEducationOfficers();
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

        private string GetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
                return (string)_filter.Filter.Filters[index].Value;
            return string.Empty;
        }

        #endregion
    }
}
