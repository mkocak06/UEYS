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

namespace UI.Pages.InstitutionManagement.Universities
{
    public partial class Universities
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IUniversityService UniversityService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private bool ticked;
        private List<UniversityResponseDTO> _universities;
        private FilterDTO _filter;
        private PaginationModel<UniversityResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loadingFile;

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
                Sort = new[]{new Sort()
                {
                    Field = "Name",
                    Dir = SortType.ASC
                }}
            };
            await GetUniversities();
            await base.OnInitializedAsync();
        }

        private async Task GetUniversities()
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
                    IsActive = false,
                    To = "/institution-management/universities",
                    OrderIndex = 1,
                    Title = L["Ministries / Universities"]
                }
        };

            _paginationModel = await UniversityService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _universities = _paginationModel.Items;
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
            await GetUniversities();
        }

        private void OnDetailHandler(UniversityResponseDTO university)
        {
            NavigationManager.NavigateTo($"/institution-management/universities/{university.Id}");
        }
        private async Task OnDeleteHandler(UniversityResponseDTO university)
        {
            var confirm = await SweetAlert.ConfirmAlert("Emin Misiniz?",
                "Bu öğeyi silmek istediğinize emin misiniz? Bu işlem geri alınamaz.",
                SweetAlertIcon.question, true, "Sil", "İptal");

            if (confirm)
            {
                try
                {
                    await UniversityService.Delete(university.Id);
                    _universities.Remove(university);
                    StateHasChanged();
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

            await GetUniversities();
        }
        private async Task DownloadExcelFile()
        {
            if (_loadingFile)
            {
                return;
            }
            _loadingFile = true;
            StateHasChanged();
            var response = await UniversityService.GetExcelByteArray(_filter);

            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"UniversityList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
                _loadingFile = false;
            }
            else
            {
                SweetAlert.ErrorAlert();
            }
            StateHasChanged();
        }

        #region FilterChangeHandlers

        private async Task OnChangeFilterCheckBox(ChangeEventArgs args, string filterName)
        {
            var value = (bool)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
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
            await GetUniversities();
        }
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
            await GetUniversities();
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
                await GetUniversities();
            }

        }
        private async Task OnResetFilterCheckBox(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetUniversities();
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