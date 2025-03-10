using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.Types;
using UI.Models;
using UI.Services;

namespace UI.Pages.Management.FacultyDetails;

public partial class Faculties
{
    [Inject] private IProfessionService ProfessionService { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    private PaginationModel<ProfessionResponseDTO> _paginationModel;

    private List<ProfessionResponseDTO> _faculties;

    private FilterDTO _filter;

    private List<BreadCrumbLink> _links;

    private bool _loading;


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
                    To = "/management/faculties/",
                    OrderIndex = 1,
                    Title = L["_List", L["Expertise Branch"]]
                }
        };
        await GetFaculties();
        await base.OnInitializedAsync();
    }
    private async Task GetFaculties()
    {
        if (_filter?.Filter?.Filters?.Count == 0)
        {
            _filter.Filter.Filters = null;
            _filter.Filter.Logic = null;
        }
        _paginationModel = await ProfessionService.GetPaginateList(_filter);
        if (_paginationModel.Items != null)
        {
            _faculties = _paginationModel.Items;
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
        await GetFaculties();
    }
    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetFaculties();
    }
    private async Task OnDeleteHandler(ProfessionResponseDTO faculty)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

        if (confirm)
        {
            try
            {
                await ProfessionService.Delete(faculty.Id);
                _faculties.Remove(faculty);
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
		_filter.page= 1;
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
        await GetFaculties();
    }

    private async Task OnResetFilter(string filterName)
    {
        _filter.Filter ??= new Filter();
        _filter.Filter.Filters ??= new List<Filter>();
        _filter.Filter.Logic ??= "and";
		_filter.page= 1;
        var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
        if (index >= 0)
        {
            _filter.Filter.Filters.RemoveAt(index);
            await JsRuntime.InvokeVoidAsync("clearInput", filterName);
            await GetFaculties();
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