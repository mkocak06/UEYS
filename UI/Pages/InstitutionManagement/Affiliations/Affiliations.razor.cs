using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Models;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Affiliations
{

    public partial class Affiliations
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IAffiliationService AffiliationService { get; set; }
        [Inject] private ISweetAlert SweetAlert { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<AffiliationResponseDTO> _Affiliations;
        private AffiliationResponseDTO _interactedAffiliation = new();
        private FilterDTO _filter;
        private PaginationModel<AffiliationResponseDTO> _paginationModel;
        private bool _loading = false;
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
                Field = "ProtocolDate",
                Dir = SortType.ASC
            }}
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
                    To = "/institution-management/affiliations",
                    OrderIndex = 1,
                    Title = L["Affiliations"]
                }
        };
            await GetAffiliations();

            await base.OnInitializedAsync();
        }


        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetAffiliations();
        }

        private async Task GetAffiliations()
        {
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _paginationModel = await AffiliationService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _Affiliations = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task OnDeleteHandler(AffiliationResponseDTO Affiliation)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await AffiliationService.Delete(Affiliation.Id);
                    _Affiliations.Remove(Affiliation);
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

            await GetAffiliations();
        }
        private void OnDetailHandler(AffiliationResponseDTO Affiliation)
        {
            NavigationManager.NavigateTo($"/institution-management/affiliations/{Affiliation.Id}");
        }
        private async Task DownloadExcelFile()
        {
            if (_loadingFile)
            {
                return;
            }
            _loadingFile = true;
            StateHasChanged();
            var response = await AffiliationService.GetExcelByteArray(_filter);

            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"AffiliationList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
                _loadingFile = false;
            }
            else
            {
                SweetAlert.ErrorAlert();
            }
            StateHasChanged();
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
            await GetAffiliations();
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