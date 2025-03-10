using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;


namespace UI.Pages.QuotaRequest;

public partial class PastRequestDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IQuotaRequestService QuotaRequestService { get; set; }
    [Inject] public ISubQuotaRequestService SubQuotaRequestService { get; set; }
    [Inject] public IProgramService ProgramService { get; set; }
    [Inject] public ICurriculumService CurriculumService { get; set; }
    [Inject] public IEducatorCountContributionFormulaService EducatorCountContributionFormulaService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public ITitleService TitleService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private QuotaRequestResponseDTO _quotaRequest;
    private SubQuotaRequestPaginateResponseDTO _subQuotaRequest;
    private bool _notFound;
    private bool _saving;
    private bool _loaded;
    private MyModal _subQuotaRequestModal;
    private MyModal _annualGlobalQuotaModal;
    private List<BreadCrumbLink> _links;
    private ExpertiseBranchResponseDTO _selectedExpertiseBranch = new();
    private List<ExpertiseBranchResponseDTO> _expertiseBranchList = new List<ExpertiseBranchResponseDTO>();
    private List<SubQuotaRequestPaginateResponseDTO> _selectedSubQuotaRequests = new();
    private FilterDTO _filter;
    private List<EducatorCountContributionFormulaResponseDTO> _formulas = new List<EducatorCountContributionFormulaResponseDTO>();
    private int _curriculumDuration;
    private bool _calculating = false;
    private string _validationMessage;
    private bool _loadingFile;
    protected override void OnInitialized()
    {
        _quotaRequest = new QuotaRequestResponseDTO();
        _subQuotaRequest = new SubQuotaRequestPaginateResponseDTO();
        //_ec = new EditContext(_quotaRequest);
        //_ecSubQuotaRequest = new EditContext(_subQuotaRequest);
        _loaded = false;

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
                    IsActive = true,
                    To = "/quota-request/past-requests",
                    OrderIndex = 1,
                    Title = L["_List", L["Past Quota Request"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Past Quota Request"]]
                }
        };
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await QuotaRequestService.GetById((long)Id);
            var response_1 = await EducatorCountContributionFormulaService.GetPaginateList(new FilterDTO() { pageSize = 100 });
            _formulas = response_1.Items;
            if (response.Result && response.Item != null)
            {
                _quotaRequest = response.Item;

                _filter = new FilterDTO()
                {
                    Filter = new()
                    {
                        Filters = new()
                {
                            new Filter()
                            {
                                Field="Profession.Code",
                                Operator="eq",
                                Value= _quotaRequest.Type == PlacementExamType.EUS ? 3 : _quotaRequest.Type == PlacementExamType.DUS ? 2 : 1
                            },
                            new Filter()
                            {
                                Field="IsPrincipal",
                                Operator="eq",
                                Value=_quotaRequest.Type == PlacementExamType.YDUS ? false : true
                            },
                },
                        Logic = "and"
                    },
                    Sort = new[]{new Sort()
                    {
                        Field = "Name",
                        Dir = SortType.ASC
                    }},
                    pageSize = 150
                };

                var expBrResponse = await ExpertiseBranchService.GetPaginateList(_filter);
                _expertiseBranchList = expBrResponse.Items;
                //_ec = new EditContext(_quotaRequest);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();

                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/quotaRequests");
            }
        }
        await base.OnParametersSetAsync();
    }

    //private async Task OpenSubQuotaRequestModal(ProgramPaginateForQuotaResponseDTO program)
    //{
    //    _subQuotaRequest = _quotaRequest.SubQuotaRequests.FirstOrDefault(x => x.ProgramId == program.Id);

    //    StateHasChanged();
    //    _ecSubQuotaRequest = new EditContext(_subQuotaRequest);
    //    _subQuotaRequestModal.OpenModal();
    //}

    private async Task GetSubQuotaRequests()
    {
        _validationMessage = null;
        if (_selectedExpertiseBranch.Id == null || _selectedExpertiseBranch.Id == 0)
        {
            _validationMessage = "Bu alan zorunludur";
            StateHasChanged();
            return;
        }
        var response = await SubQuotaRequestService.GetPaginateList(new()
        { Filter = new() { Logic = "and", Filters = new List<Filter>() { new() { Field = "QuotaRequestId", Operator = "eq", Value = _quotaRequest.Id }, new() { Field = "ExpertiseBranchId", Operator = "eq", Value = _selectedExpertiseBranch.Id } } } });

        _selectedSubQuotaRequests = response.Items;
        var response_1 = await CurriculumService.GetLatestByBeginningDateAndExpertiseBranchId(_selectedExpertiseBranch.Id.Value, DateTime.UtcNow);
        _curriculumDuration = (int)response_1.Item.Duration;
        StateHasChanged();
    }

    private void OnChangeNewBranch(long? id)
    {
        _selectedExpertiseBranch = _expertiseBranchList.FirstOrDefault(x => x.Id == id);
        StateHasChanged();
    }

   
    private async Task Save()
    {
        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<QuotaRequestDTO>(_quotaRequest);
        try
        {
            var response = await QuotaRequestService.Update(_quotaRequest.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
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
        _saving = false;
        StateHasChanged();
        _annualGlobalQuotaModal.CloseModal();
    }

    private async Task OpenSubQuotaRequestModal(SubQuotaRequestPaginateResponseDTO subquotaRequest)
    {
        _subQuotaRequest = subquotaRequest;

        StateHasChanged();
        _subQuotaRequestModal.OpenModal();
    }

    private async Task UpdateStudentCount()
    {
        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<SubQuotaRequestDTO>(_subQuotaRequest);
        try
        {
            var response = await SubQuotaRequestService.Update((long)_subQuotaRequest.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
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
        _saving = false;
        StateHasChanged();
        _subQuotaRequestModal.CloseModal();
    }

    private async Task DownloadExcelFile()
    {
        if (_loadingFile)
        {
            return;
        }
        _loadingFile = true;
        StateHasChanged();
        var response = await SubQuotaRequestService.ExcelExport(new()
        { Filter = new() { Logic = "and", Filters = new List<Filter>() { new() { Field = "QuotaRequestId", Operator = "eq", Value = _quotaRequest.Id } } } });

        if (response.Result)
        {
            await JsRuntime.InvokeVoidAsync("saveAsFile", $"Talep_Listesi_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
            _loadingFile = false;
        }
        else
        {
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();
    }
}
