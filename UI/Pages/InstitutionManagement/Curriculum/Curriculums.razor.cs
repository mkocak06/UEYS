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

namespace UI.Pages.InstitutionManagement.Curriculum
{
    public partial class Curriculums
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private ICurriculumService CurriculumService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<CurriculumResponseDTO> _curriculums;
        private FilterDTO _filter;
        private PaginationModel<CurriculumResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;

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

                    }

                },
                    Logic = "and"

                },
                page = 1,
                pageSize = 10,
                Sort = new[]{
                    new Sort()
                {
                    Field = "ExpertiseBranch.Name",
                    Dir = SortType.ASC
                },
                 //   new Sort()
                 //{
                 // Field = "Version",
                 // Dir = SortType.ASC
                 //}
                }

            };

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
                    IsActive = false,
                    To = "/institution-management/curriculums",
                    OrderIndex = 1,
                    Title = L["Curriculums"]
                }
        };
            await GetCurriculums();
            await base.OnInitializedAsync();
        }

        private async Task GetCurriculums()
        {
            _loading = true;
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }

            _paginationModel = await CurriculumService.GetPaginateList(_filter);

            if (_paginationModel.Items != null)
            {
                _curriculums = _paginationModel.Items;
                _loading = false;
                StateHasChanged();
            }
            else
            {
                _loading = false;
                SweetAlert.ErrorAlert();
            }

        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetCurriculums();
        }

        private void OnDetailHandler(CurriculumResponseDTO curriculum)
        {
            NavigationManager.NavigateTo($"/institution-management/curriculums/{curriculum.Id}");
        }
        private async Task OnDeleteHandler(CurriculumResponseDTO curriculum)
        {
            var confirm = await SweetAlert.ConfirmAlert("Emin Misiniz?",
                "Bu öðeyi silmek istediðinize emin misiniz? Bu iþlem geri alýnamaz.",
                SweetAlertIcon.question, true, "Sil", "Ýptal");

            if (confirm)
            {
                try
                {
                    await CurriculumService.Delete((long)curriculum.Id);
                    _curriculums.Remove(curriculum);
                    StateHasChanged();
                    await GetCurriculums();
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
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetCurriculums();
        }

        #region FilterChangeHandlers

        private async Task OnChangeFilterCheckBox(ChangeEventArgs args, string filterName)
        {
            var value = (bool)args.Value;
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
                    Operator = "eq",
                    Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            await GetCurriculums();
        }
        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (object)args.Value;
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
            await GetCurriculums();
        }
        private async Task OnChangeFilterForNumber(ChangeEventArgs args, string filterName)
        {
            var value = Convert.ToInt32(args.Value);
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
                    Operator = "eq",
                    Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            await GetCurriculums();
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
                await GetCurriculums();
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