using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using UI;
using UI.SharedComponents;
using Microsoft.AspNetCore.Authorization;
using UI.Services;
using UI.Models;
using UI.Helper;
using UI.Validation;
using Shared.Validations;
using UI.SharedComponents.Components;
using UI.SharedComponents.BlazorLeaflet;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Blazored.Typeahead;
using Microsoft.Extensions.Localization;
using AutoMapper;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Pages.Student.Students.StudentDetail.Store;

namespace UI.Pages.Student.Students.StudentDetail.Tabs.PerformanceRating
{
    public partial class PerformanceRating
    {
        [Inject] public IState<StudentDetailState> StudentDetailState { get; set; }
        [Inject] private IPerformanceRatingService PerformanceRatingService { get; set; }
        [Inject] private IDocumentService DocumentService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        public PerformanceRatingDetail performanceRatingForAdd { get; set; }
        public PerformanceRatingDetail performanceRatingForUpdate { get; set; }
        private List<PerformanceRatingResponseDTO> _pRatings;
        private FilterDTO _filter;
        private PaginationModel<PerformanceRatingResponseDTO> _paginationModel;
        private bool _loading = false;
        private bool _isEditing = false;
        private bool _isAdding = false;
        private PerformanceRatingResponseDTO _performanceRating = new();
        protected override async Task OnInitializedAsync()
        {
            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                {

                    new Filter()
                    {
                        Field="IsDeleted",
                        Operator="eq",
                        Value=false
                    },
                    new Filter()
                    {
                        Field="Student.Id",
                        Operator = "eq",
                        Value = _student.Id
                    }

                },
                    Logic = "and"

                },

                Sort = new[]{new Sort()
            {
                Field = "Student.User.Name",
                Dir = SortType.ASC
            }}
            };

            await GetPerformanceRatings();

            await base.OnInitializedAsync();
        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetPerformanceRatings();
        }

        private async Task GetPerformanceRatings()
        {
            _paginationModel = await PerformanceRatingService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _pRatings = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }
        private void OnDetailHandler(PerformanceRatingResponseDTO pRating)
        {
            _performanceRating = pRating;
            _isEditing = true;
            StateHasChanged();
        }
        private async Task OnDeleteHandler(PerformanceRatingResponseDTO pRating)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to take back this item?."]}",
                SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await PerformanceRatingService.Delete(pRating.Id.Value);
                    _pRatings.Remove(pRating);

                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
            }
        }

        private async void OnSavePerformanceRating()
        {
            await GetPerformanceRatings();
            _isAdding = false;
            StateHasChanged();
        }
        private string GetAverageClass(string avg)
        {
            double avgS = Convert.ToDouble(avg);
            if (avgS > 0 && avgS < 3)
                return "table-danger";
            else if (avgS == 3)
                return "table-warning";
            else if (avgS > 3)
                return "table-success";
            else return "";
        }
        private async void OnUpdatePerformanceRating()
        {
            await GetPerformanceRatings();
            _isEditing = false;
            StateHasChanged();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetPerformanceRatings();
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
            await GetPerformanceRatings();
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
                await GetPerformanceRatings();
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