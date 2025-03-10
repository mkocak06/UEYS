using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.Extensions;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Archive.Curriculums.Tabs

{

    public partial class ArchiveCurriculumPerfections
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IPerfectionService PerfectionService { get; set; }
        [Inject] public ICurriculumPerfectionService CurriculumPerfectionService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IPropertyService PropertyService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<CurriculumDetailState> CurriculumDetailState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private List<CurriculumPerfectionResponseDTO> _curriculumPerfections;
        private FilterDTO _filter;
        private PaginationModel<CurriculumPerfectionResponseDTO> _paginationModel = new();
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool forceRender;
        [Parameter] public long Id { get; set; }

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
                pageSize = 10,

                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "Curriculum.ExpertiseBranch.Name"
                    }
                }
            };
            SubscribeToAction<CurriculumDetailSetAction>((action) =>
            {
                forceRender = true;
            });

            await GetCurriculumPerfections();
            base.OnInitialized();
        }


        private async Task GetCurriculumPerfections()
        {
            _paginationModel = await CurriculumPerfectionService.GetArchiveList(_filter);
            if (_paginationModel.Items != null)
            {
                if (_filter.Sort == null)
                    _curriculumPerfections = _paginationModel.Items.OrderBy(x => x.Curriculum.Version).ToList();
                else
                    _curriculumPerfections = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task OnUndeleteHandler(CurriculumPerfectionResponseDTO curriculumPerfection)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to take back this item?"]}",
                SweetAlertIcon.question, true, $"{L["Take Back"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    var response = await CurriculumPerfectionService.Undelete((long)curriculumPerfection.Id);

                    if (response.Result)
                    {
                        //NavigationManager.NavigateTo($"./archive/archivecurriculumperfections");
                        _curriculumPerfections.Remove(curriculumPerfection);
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
            await GetCurriculumPerfections();
        }

        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetCurriculumPerfections();
        }

        #region FilterChangeHandlers

        private async Task OnChangePerfectionTypeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            var filterValue = new PerfectionType?();

            if (PerfectionType.Clinical.GetDescription().ToLower().Contains(value.ToLower()))
            {
                Console.WriteLine(PerfectionType.Clinical.GetDescription());
                filterValue = PerfectionType.Clinical;
            }
            else if (PerfectionType.Interventional.GetDescription().ToLower().Contains(value.ToLower()))
            {
                filterValue = PerfectionType.Interventional;
            }
            else
            {
                filterValue = null;
            }

            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filter.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "eq",
                    Value = filterValue
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = filterValue;
            }
            await GetCurriculumPerfections();
        }

        private async Task OnChangeFilter(ChangeEventArgs args, string propertyType)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            _filter.Filter.Filters.Add(new Filter()
            {
                Field = "PropertyType",
                Operator = propertyType,
                Value = value
            });

            await GetCurriculumPerfections();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            Console.WriteLine(filterName);

            int index = 0;
            if (filterName == "Perfection.PerfectionType")
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            }
            else
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Operator == filterName);
            }

            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetCurriculumPerfections();
            }
        }

        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";

            int index = 0;

            if (filterName == "Perfection.PerfectionType")
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            }
            else
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Operator == filterName);
            }
            return index >= 0;
        }
        #endregion
    }
}