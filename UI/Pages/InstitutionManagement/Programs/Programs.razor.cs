using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.DetailCards;
using UI.SharedComponents.Store;

namespace UI.Pages.InstitutionManagement.Programs;

public partial class Programs
{
    [Inject] private IProgramService ProgramService { get; set; }
    [Inject] private IAuthorizationDetailService AuthorizationDetailService { get; set; }
    [Inject] private IAuditFormService AuditFormService { get; set; }
    [Inject] private IStandardService StandardService { get; set; }
    [Inject] private IState<AppState> AppState { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    private List<ProgramPaginateResponseDTO> _programs;
    private FilterDTO _filter => AppState.Value.Filter;
    private PaginationModel<ProgramPaginateResponseDTO> _paginationModel;
    private FormResponseDTO _form = new();
    private bool _loading = false;
    private bool forceRender;
    private List<BreadCrumbLink> _links;
    private bool _loadingFile;
    private bool _getPassive = false;
    private MyModal _requestFormModal;
    private AuthorizationDetailsModal _authDetailsModal;
    private bool _standardsLoading;
    private bool _savingForm;
    private EditContext _ec;
    protected override async Task OnInitializedAsync()
    {
        _filter.Filter = new Filter()
        {
            Logic = "and",
            Filters = new()
            {
                    new Filter()
                    {
                        Field="IsDeleted",
                        Operator="eq",
                        Value=false
                    },
                    new Filter()
                    {
                        Field="AuthorizationCategoryIsActive",
                        Operator="eq",
                        Value=true
                    }
                },
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
                    To = "/institution-management/programs",
                    OrderIndex = 1,
                    Title = L["Expertise Training Programs"]
                }
        };
        Dispatcher.Dispatch(new ProgramDetailFilterClearAction());
        await GetPrograms();
        SubscribeToAction<ProgramsLoadAction>(async (action) =>
        {

            await GetPrograms();
        });

        await base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            //JsRuntime.InvokeVoidAsync("initQuickPanel");
            JsRuntime.InvokeVoidAsync("initPopOver");
        }
        if (forceRender)
        {
            forceRender = false;
            JsRuntime.InvokeVoidAsync("initTooltip");
            JsRuntime.InvokeVoidAsync("initPopOver");
        }
    }

    private async void GetAuthorizationDetails(long programId)
    {
        try
        {
            var response = await AuthorizationDetailService.GetListByProgramId(programId);
            if (response.Result)
            {
                _authDetailsModal.OpenModal(response.Item);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            SweetAlert.ErrorAlert();
        }
    }

    private async Task GetFormStandard(ProgramPaginateResponseDTO program)
    {
        _standardsLoading = true;
        StateHasChanged();
        _form = new() { FormStandards = new(), ApplicationDate = DateTime.UtcNow, VisitStatusType = VisitStatusType.RequestCreated };
        _ec = new EditContext(_form);
        try
        {
            if (program.ExpertiseBranchId.HasValue)
            {
                _form.ProgramId = program.Id;
                _requestFormModal.OpenModal();
                var response = await StandardService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Curriculum.ExpertiseBranchId", "eq", program.ExpertiseBranchId.Value, "and").Filter("IsDeleted","eq",false,"and"));
                if (response != null)
                {
                    response.Items.ForEach(x =>
                        _form.FormStandards.Add(new FormStandardResponseDTO() { Standard = x, StandardId = x.Id }));
                }
            }
            else
                SweetAlert.ErrorAlert();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            SweetAlert.ErrorAlert();
        }
        finally { _standardsLoading = false; StateHasChanged(); }
    }

    private async Task CreateRequestForm()
    {
        if (!_ec.Validate()) return;
        _savingForm = true;
        StateHasChanged();
        try
        {
            var response = await AuditFormService.AddForm(Mapper.Map<FormDTO>(_form));
            if (response.Result)
            {
                _programs.FirstOrDefault(x => x.Id == _form.ProgramId).IsAuditing = true;
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Talebiniz Başarıyla oluşturuldu");
            }
            else
                SweetAlert.ErrorAlert("Bir hata oluştu");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            SweetAlert.ErrorAlert("Bir hata oluştu");
        }
        finally { _savingForm = false; StateHasChanged(); _requestFormModal.CloseModal(); }
    }
    private string GetTextStyle(string authNo)
    {
        if (authNo == "3")

            return "color:white";
        else return "";

    }
    private async Task OnSortChange(Sort sort)
    {
        _filter.Sort = new[] { sort };
        await GetPrograms();
    }

    private async Task GetPrograms()
    {

        _paginationModel = await ProgramService.GetPaginateListOnly(_filter);
        if (_paginationModel.Items != null)
        {
            _programs = _paginationModel.Items;
            StateHasChanged();
            forceRender = true;
        }
        else
        {
            _loading = true;
            SweetAlert.ErrorAlert();
        }
    }

    private async Task OnDeleteHandler(ProgramPaginateResponseDTO program)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

        if (confirm)
        {
            try
            {
                await ProgramService.Delete(program.Id);
                _programs.Remove(program);
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

        await GetPrograms();
    }
    private void OnDetailHandler(ProgramResponseDTO program)
    {
        NavigationManager.NavigateTo($"/institution-management/programs/{program.Id}");
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
        await GetPrograms();
    }

    private async Task OnChangeBranchFilter(ChangeEventArgs args, string filterName)
    {
        bool value;
        if ((string)args.Value?.ToString().ToLower() == "a")
        {
            value = true;
        }
        else if ((string)args.Value?.ToString().ToLower() == "y")
        {
            value = false;
        }
        else
        {
            return;
        }

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
        await GetPrograms();
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
            await GetPrograms();
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

    private void OpenCanvas()
    {
        JsRuntime.InvokeVoidAsync("initQuickPanel");
        JsRuntime.InvokeVoidAsync("ShowAdvancedSearchAsync");
    }

    private string GetFilterCountString()
    {
        var count = AppState.Value.Filter?.Filter?.Filters?.Count(x => x.Field.Contains("Id"));
        if (count is null || count <= 0)
            return "";
        else
            return "<span class=\"badge badge-warning mr-1\">" + count + "</span>";
    }

    private async Task DownloadExcelFile()
    {
        if (_loadingFile)
        {
            return;
        }
        _loadingFile = true;
        StateHasChanged();
        var response = await ProgramService.GetExcelByteArray(_filter);

        if (response.Result)
        {
            await JsRuntime.InvokeVoidAsync("saveAsFile", $"YUEPList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
            _loadingFile = false;
        }
        else
        {
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();
    }

    private void OnChangeIsActive()
    {
        if (_getPassive)
        {
            _getPassive = false;
            _filter.Filter.Filters.Add(new Filter() { Field = "AuthorizationCategoryIsActive", Operator = "eq", Value = true });
        }
        else
        {
            _getPassive = true;
            _filter.Filter.Filters.Remove(_filter.Filter.Filters.FirstOrDefault(x => x.Field == "AuthorizationCategoryIsActive"));
        }
        GetPrograms();
    }
}