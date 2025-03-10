using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.ResponseModels;
using UI.Services;
using System.Linq;
using Shared.FilterModels.Base;
using Shared.Types;
using Microsoft.JSInterop;
using UI.Models;
using UI.SharedComponents.Components;
using AutoMapper;
using Shared.RequestModels;
using UI.SharedComponents.Store;
using System.Globalization;
using UI.Helper;

namespace UI.Pages.Archive.Curriculums.Tabs
{
    public partial class ArchiveCurriculumRotations
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public ICurriculumRotationService CurriculumRotationService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<CurriculumRotationResponseDTO> _curriculumRotations = new();
        private PaginationModel<CurriculumRotationResponseDTO> _paginationModel = new();
        private FilterDTO _filter;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;

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
                        Value=true
                    }
                },
                    Logic = "and"

                },
                page = 1,
                pageSize = int.MaxValue,
                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "Curriculum.ExpertiseBranch.Name"
                    }
                }
            };

            await GetCurriculumRotations();
            base.OnInitialized();
        }
        private async Task GetCurriculumRotations()
        {
            var _paginationModel = await CurriculumRotationService.GetArchiveList(_filter);
            if (_paginationModel.Items != null)
            {
                if(_filter.Sort == null)
                    _curriculumRotations = _paginationModel.Items.OrderBy(x => x.Curriculum.Version).ToList();
                else
                    _curriculumRotations = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task OnUndeleteHandler(CurriculumRotationResponseDTO curriculumRotation)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to take back this item?"]}",
                SweetAlertIcon.question, true, $"{L["Take Back"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    var response = await CurriculumRotationService.UnDelete((long)curriculumRotation.Id);

                    if (response.Result)
                    {
                        _curriculumRotations.Remove(curriculumRotation);
                        _paginationModel.TotalItemCount = _paginationModel.TotalItemCount - 1;
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
            }
        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetCurriculumRotations();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetCurriculumRotations();
        }

        #region FilterChangeHandlers
        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;
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
            await GetCurriculumRotations();
        }
        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetCurriculumRotations();
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