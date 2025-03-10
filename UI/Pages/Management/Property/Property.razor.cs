using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;

namespace UI.Pages.Management.Property
{

    public partial class Property
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IPropertyService PropertyService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<PropertyResponseDTO> _propertys;
        private FilterDTO _filter;
        private PaginationModel<PropertyResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;

        protected override async Task OnInitializedAsync()
        {
            _filter = new FilterDTO()
            {
                Sort = new[] { new Sort()
                {
                    Dir = SortType.ASC, Field = "PerfectionType" //???Burası önemli
                } }
              
            };
            _links = new List<BreadCrumbLink>()
            {
                new BreadCrumbLink()
                {
                    IsActive = true,
                    To = "/",
                    OrderIndex = 0,
                    Title = L["Homepage"],
                }, new BreadCrumbLink()
                {
                    IsActive= false, To = "/management/properties",
                    OrderIndex = 1,
                    Title= L["_List", L["Properties"]]
                }
            };
            await GetProperty();
            await base.OnInitializedAsync();


        }
        private async Task GetProperty()
        {
            if(_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _paginationModel = await PropertyService.GetPaginateList(_filter);
            if(_paginationModel.Items != null)
            {
                _propertys = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }
        
        async Task UpdatePropertyFilter(ChangeEventArgs args, string filterName) 
        {
            var value = (string)args.Value; //(string)args.value
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;
            _filter.PrintJson("filt");
            filterName.PrintJson("filtername");
            var index = _filter.Filter.Filters.FindIndex(x=>x.Field == filterName);
            if(index < 0)
            {
                _filter.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,  Operator = "contains", Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            _filter.PrintJson("filt1");

            await GetProperty();
        }
        

        async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic = "and";
            _filter.page = 1;
            var index = _filter.Filter.Filters.FindIndex(x=> x.Field == filterName);
            if(index >= 0) 
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetProperty();
            }
        }
        async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetProperty();
        }
        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(_ => _.Field == filterName);
            return index >= 0;
        }

        async Task PaginationHandler(PaginationInfo paginationInfo)
        {
            var (item1, item2) = (paginationInfo.Page, paginationInfo.PageSize);
            _filter.page = item1;
            _filter.pageSize = item2;
            await GetProperty();
        }
        async Task OnDeleteHandler(PropertyResponseDTO property)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    await PropertyService.Delete((long)property.Id);
                    _propertys.Remove(property);
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
    }
}