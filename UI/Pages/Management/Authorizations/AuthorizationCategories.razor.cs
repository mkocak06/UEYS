using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.Types;
using UI.Models;
using UI.Services;

namespace UI.Pages.Management.Authorizations
{

    public partial class AuthorizationCategories
    {
        [Inject] private IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }

        private List<AuthorizationCategoryResponseDTO> _authorizationCategories;
        [Inject] public IJSRuntime JsRuntime { get; set; }
        private PaginationModel<AuthorizationCategoryResponseDTO> _paginationModel;

        private FilterDTO _filter;

        private List<BreadCrumbLink> _links;

        private bool _loading, _changingOrder;
        private long _downId, _upId;

        private async Task GetAuthorizationCategory()
        {
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _paginationModel = await AuthorizationCategoryService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _authorizationCategories = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetAuthorizationCategory();
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetAuthorizationCategory();
        }
        protected override async Task OnInitializedAsync()
        {
            _filter = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Dir = SortType.ASC,
                Field = "Name"
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
                    To = "/management/authorizationCategories",
                    OrderIndex = 1,
                    Title = L["_List", L["Authorization Category"]]
                }
        };
            await GetAuthorizationCategory();
            await base.OnInitializedAsync();
        }

        private async Task OnDeleteHandler(AuthorizationCategoryResponseDTO authorizationCategory)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await AuthorizationCategoryService.Delete(authorizationCategory.Id);
                    _authorizationCategories.Remove(authorizationCategory);
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
            await GetAuthorizationCategory();
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
                await GetAuthorizationCategory();
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
        //private async Task ChangeOrder(AuthorizationCategoryResponseDTO acr, bool isToUp)
        //{
        //    if (_changingOrder)
        //        return;
        //    _upId = 0;
        //    _downId= 0;
        //    _changingOrder = true;
        //    StateHasChanged();
        //    try
        //    {
        //        var response = await AuthorizationCategoryService.ChangeOrder(acr.Id, isToUp);
        //        if (response.Result)
        //        {
        //            if (isToUp)
        //            {
        //                var replacedItem = _authorizationCategories.FirstOrDefault(x => x.Order == acr.Order - 1);
        //                if (replacedItem.Order != null)
        //                {
        //                    replacedItem.Order++;
        //                    acr.Order--;
        //                    _upId = replacedItem.Id;
        //                    _downId = acr.Id;
        //                }
        //            }
        //            else
        //            {
        //                var replacedItem = _authorizationCategories.FirstOrDefault(x => x.Order == acr.Order + 1);
        //                if (replacedItem.Order != null)
        //                {
        //                    replacedItem.Order--;
        //                    acr.Order++;
        //                    _upId = acr.Id;
        //                    _downId = replacedItem.Id;
        //                }
        //            }
        //            _authorizationCategories = _authorizationCategories.OrderBy(x => x.Order).ToList();
        //            StateHasChanged();
        //        }
        //        else
        //        {
        //            SweetAlert.IconAlert(SweetAlertIcon.error, "", L[response.Message]);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SweetAlert.ErrorAlert();
        //        Console.WriteLine(ex.Message);
        //    }
        //    _changingOrder = false;
        //    StateHasChanged();
        //}

        private string GetColor(AuthorizationCategoryResponseDTO item, bool isForUp)
        {
            if ((_upId == item.Id && isForUp == false) || (_downId == item.Id && isForUp))
                return "btn-light-warning btn-icon btn-sm ml-1";
            else
                return "btn-light-primary btn-icon btn-sm ml-1";
        }
    }

}
