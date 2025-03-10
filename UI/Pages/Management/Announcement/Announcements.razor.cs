using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.ResponseModels;
using UI.Services;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.Types;
using UI.Models;

namespace UI.Pages.Management.Announcement
{

    public partial class Announcements
    {
        [Inject] private IAnnouncementService AnnouncementService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<AnnouncementResponseDTO> _announcements;
        private PaginationModel<AnnouncementResponseDTO> _paginationModel;
        private FilterDTO _filter;
        private bool _loading;
        private List<BreadCrumbLink> _links;

        protected override async Task OnInitializedAsync()
        {
            _filter = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Dir = SortType.ASC,
                Field = "Title"
            }},
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
                    To = "/management/announcements",
                    OrderIndex = 1,
                    Title = L["_List", L["Announcement"]]
                }
        };
            await GetAnnouncements();
            await base.OnInitializedAsync();
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
            await GetAnnouncements();
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
                await GetAnnouncements();
            }
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetAnnouncements();
        }
        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetAnnouncements();
        }
        private async Task GetAnnouncements()
        {
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _paginationModel = await AnnouncementService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _announcements = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }
        private async Task OnDeleteHandler(AnnouncementResponseDTO announcement)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await AnnouncementService.Delete((long)announcement.Id);
                    _announcements.Remove(announcement);
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