using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Models;
using UI.Services;

namespace UI.Pages.Management.ExpertiseBranches;

public partial class ExpertiseBranches
{
    [Inject] private IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IFacultyService FacultyService { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; }
    private List<BreadCrumbLink> _links;
    private List<ExpertiseBranchResponseDTO> _branches;
    private FilterDTO _filter;
    private PaginationModel<ExpertiseBranchResponseDTO> _paginationModel;
    private bool _loading;
    private int altLimit;
    private int ustLimit;
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
                    To = "/management/expertisebranches",
                    OrderIndex = 1,
                    Title = L["_List", L["Expertise Training Program"]]
                }
        };
        await GetExpertiseBranches();
        await base.OnInitializedAsync();
    }
    private async Task GetExpertiseBranches()
    {

        if (_filter?.Filter?.Filters?.Count == 0)
        {
            _filter.Filter.Filters = null;
            _filter.Filter.Logic = null;
        }
        _paginationModel = await ExpertiseBranchService.GetPaginateList(_filter);
        if (_paginationModel.Items != null)
        {
            _branches = _paginationModel.Items;
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
        await GetExpertiseBranches();
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
            await GetExpertiseBranches();
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
        await GetExpertiseBranches();
    }
    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetExpertiseBranches();
    }
    private async Task OnChangeCheckBoxFilter(ChangeEventArgs args, string filterName)
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
        await GetExpertiseBranches();
    }

    private async Task OnDeleteHandler(ExpertiseBranchResponseDTO branch)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
        $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
        SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

        if (confirm)
        {
            try
            {
                await ExpertiseBranchService.Delete((long)branch.Id);
                _branches.Remove(branch);
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

    private async Task SendStudent()
    {
        if (_filter.Filter.Filters.Any(x => x.Field == "AltLimit"))
        {
            var altLimitFilter = _filter.Filter.Filters.FirstOrDefault(x => x.Field == "AltLimit");
            var ustLimitFilter = _filter.Filter.Filters.FirstOrDefault(x => x.Field == "UstLimit");

            altLimitFilter.Value = altLimit;
            ustLimitFilter.Value = ustLimit;
        }
        else
        {
            _filter.Filter.Filters.Add(new Filter()
            {
                Field = "AltLimit",
                Operator = "eq",
                Value = altLimit
            });

            _filter.Filter.Filters.Add(new Filter()
            {
                Field = "UstLimit",
                Operator = "eq",
                Value = ustLimit
            });
        }
        await GetExpertiseBranches();
    }
}