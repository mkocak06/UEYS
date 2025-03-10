using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.User.Educator;

public partial class Educator
{
    [Inject] IAuthenticationService AuthenticationService { get; set; }
    [Inject] private IEducatorService EducatorService { get; set; }
    [Inject] private ITitleService TitleService { get; set; }
    [Inject] private IState<AppState> AppState { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private List<string> _roles => AuthenticationService.Roles.Select(x => x.RoleName).ToList();
    private List<string> _adminTitles = new();

    private List<EducatorPaginateResponseDTO> _educators;
    private FilterDTO _filter = new();
    private PaginationModel<EducatorPaginateResponseDTO> _paginationModel;
    private bool _loading = false;
    private bool forceRender;
    private List<BreadCrumbLink> _links;
    private bool _loadingFile;
    private EducatorResponseDTO _educator = new();
    private MyModal _deleteModal;
    private bool _saving;
    private string _deleteReasonValidatorMessage;
    private string _explanationValidatorMessage;

    protected override async Task OnInitializedAsync()
    {

        _filter = new FilterDTO()
        {
            Filter = new()
            {
                Logic = "and",
                Filters = new()
            {
                new Filter()
                {
                    Field = "IsDeleted",
                    Operator = "eq",
                    Value = false
                },
                new Filter()
                {
                    Field = "UserIsDeleted",
                    Operator = "eq",
                    Value = false
                },
                new Filter()
                {
                    Field="EducatorType",
                    Operator="eq",
                    Value=EducatorType.Instructor
                }
            },

            },
            Sort = new[]{new Sort()
            {
                Field = "Name",
                Dir = SortType.ASC
            } }
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
                    To = "/educator/educators",
                    OrderIndex = 1,
                    Title =L["_List", L["Educator"]]
                }
        };
        _adminTitles = await GetAdminTitles();
        await GetEducators();

        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initializeSelectPicker");
        }
        if (forceRender)
        {
            forceRender = false;
            await JsRuntime.InvokeVoidAsync("initializeSelectPicker");

        }
    }
    private async Task<List<string>> GetAdminTitles()
    {
        try
        {
            var response = await TitleService.GetListByType(TitleType.Administrative);
            if (response.Result)
                return response.Item.Select(x=>x.Name).ToList();
            else
                throw new Exception();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private async Task OnSortChange(Sort sort)
    {
        _filter.Sort = new[] { sort };
        await GetEducators();
    }

    private async Task DownloadExcelFile()
    {
        if (_loadingFile)
        {
            return;
        }
        _loadingFile = true;
        StateHasChanged();
        if (_filter?.Filter?.Filters?.Count == 0)
        {
            _filter.Filter.Filters = null;
            _filter.Filter.Logic = null;
        }

        _filter.pageSize = int.MaxValue;
        _filter.page = 1;
        var response = await EducatorService.GetExcelByteArray(_filter);

        if (response.Result)
        {
            await JsRuntime.InvokeVoidAsync("saveAsFile", $"EducatorList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
            _loadingFile = false;
        }
        else
        {
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();
    }

    private async Task GetEducators()
    {
        if (_filter?.Filter?.Filters?.Count == 0)
        {
            _filter.Filter.Filters = null;
            _filter.Filter.Logic = null;
        }
        _paginationModel = await EducatorService.GetPaginateListOnly(_filter);
        if (_paginationModel.Items != null)
        {
            _educators = _paginationModel.Items.OrderBy(x => x.PrincipalBranchDutyPlace).ToList();
            StateHasChanged();
            forceRender = true;
            StateHasChanged();
        }
        else
        {
            _loading = true;
            SweetAlert.ErrorAlert();
        }
    }

    private async Task OnDeleteHandler(EducatorPaginateResponseDTO educator)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

        if (confirm)
        {
            _deleteModal.OpenModal();
            var educatorToDelete = await EducatorService.GetById((int)educator.Id);
            _educator = educatorToDelete.Item;
        }
    }

    private async Task DeleteEducator()
    {
        _deleteReasonValidatorMessage = null;
        _explanationValidatorMessage = null;

        if (_educator.DeleteReason == 0) { _deleteReasonValidatorMessage = L["Deleting Reason cannot be empty!"]; return; }
        if (string.IsNullOrEmpty(_educator.DeleteReasonExplanation) && _educator.DeleteReason == EducatorDeleteReasonType.Other) { _explanationValidatorMessage = L["Explanation cannot be empty!"]; return; }

        _saving = true;
        StateHasChanged();
        _educator.IsDeleted = true;
        var dto = Mapper.Map<EducatorDTO>(_educator);
        try
        {
            var response = await EducatorService.Update((long)_educator.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Item Deleted!"]);
                _educators.Remove(_educators.FirstOrDefault(x => x.Id == _educator.Id));
            }
            else
            {
                throw new Exception(response.Message);
            }
        }
        catch (Exception e)
        {
            SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
            Console.WriteLine(e.Message);
        }
        finally
        {
            _saving = false;
            StateHasChanged();
            _deleteModal.CloseModal();
        }
    }

    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetEducators();
    }
    private void OnDetailHandler(EducatorResponseDTO educator)
    {
        NavigationManager.NavigateTo($"/educator/educators/{educator.Id}");
    }

    #region FilterChangeHandlers
    private async Task OnChangeSelectFilter(ChangeEventArgs args, string filterName)
    {
        var values = args.Value as string[];
        if (values != null)
        {
            var value = string.Join(",", values);
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;
            foreach (var item in values)
            {


                if (_filter.Filter.Filters.Where(x => x.Field == filterName).All(x => x.Value.ToString() != item))
                {
                    _filter.Filter.Filters.Add(new Filter()
                    {
                        Field = filterName,
                        Operator = "eq",
                        Value = item
                    });
                }

            }
            foreach (var item in _filter.Filter.Filters.Where(x => x.Field == filterName).ToList())
            {

                if (!values.Any(x => x == item.Value.ToString()))
                {
                    _filter.Filter.Filters.Remove(item);
                }
            }
            await GetEducators();
        }
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
                Value = value?.Trim()
            });
        }
        else
        {
            _filter.Filter.Filters[index].Value = value;
        }
        await GetEducators();
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
            await GetEducators();
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