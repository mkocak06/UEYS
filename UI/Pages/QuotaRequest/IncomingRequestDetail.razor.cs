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

public partial class IncomingRequestDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IQuotaRequestService QuotaRequestService { get; set; }
    [Inject] public ISubQuotaRequestService SubQuotaRequestService { get; set; }
    [Inject] public IProgramService ProgramService { get; set; }
    [Inject] public ICurriculumService CurriculumService { get; set; }
    [Inject] public IEducatorCountContributionFormulaService EducatorCountContributionFormulaService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IStudentCountService StudentCountService { get; set; }
    [Inject] public ITitleService TitleService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private QuotaRequestResponseDTO _quotaRequest;
    private SubQuotaRequestPaginateResponseDTO _subQuotaRequest;
    //private EditContext _ec;
    //private EditContext _ecSubQuotaRequest;
    private bool _notFound;
    private bool _saving;
    private bool _loading;
    private bool _loaded;
    private MyModal _subQuotaRequestModal;
    private MyModal _annualGlobalQuotaModal;
    private MyModal _summaryTableModal;
    private List<QuotaType?> _quotaTypes;
    private List<BreadCrumbLink> _links;
    private ExpertiseBranchResponseDTO _selectedExpertiseBranch = new();
    private List<ExpertiseBranchResponseDTO> _expertiseBranchList = new List<ExpertiseBranchResponseDTO>();
    private List<SubQuotaRequestPaginateResponseDTO> _selectedSubQuotaRequests = new();
    private List<StudentCountResponseDTO> _studentCounts = new();
    private FilterDTO _filter;
    private List<EducatorCountContributionFormulaResponseDTO> _formulas = new List<EducatorCountContributionFormulaResponseDTO>();
    private int _curriculumDuration;
    private bool _loadingFile;
    private string _validationMessage;
    protected override void OnInitialized()
    {
        _quotaRequest = new QuotaRequestResponseDTO();
        _subQuotaRequest = new SubQuotaRequestPaginateResponseDTO();
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
                    To = "/quota-request/incoming-requests",
                    OrderIndex = 1,
                    Title = L["_List", L["Incoming Quota Request"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Incoming Quota Request"]]
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
                _expertiseBranchList.Add(new() { Id = -1, Name = "Toplam" });
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
        _loading = true;
        StateHasChanged();

        try
        {
            var response = await SubQuotaRequestService.GetPaginateList(new()
            {
                Filter = new()
                {
                    Logic = "and",
                    Filters =
                    new List<Filter>() { 
                        //new() { Field = "IsDeleted", Operator = "eq", Value = false },
                    new() { Field = "QuotaRequestId", Operator = "eq", Value = _quotaRequest.Id },
                    new() { Field = "ExpertiseBranchId", Operator = "eq", Value = _selectedExpertiseBranch.Id } }
                }
            });

            if (response.Items != null)
            {
                _selectedSubQuotaRequests = response.Items;

                var response_1 = await CurriculumService.GetLatestByBeginningDateAndExpertiseBranchId(_selectedExpertiseBranch.Id.Value, DateTime.UtcNow);
                _curriculumDuration = (int)response_1.Item.Duration;
            }

        }
        catch (Exception e)
        {
            SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
            e.PrintJson("hata");
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    private void OnChangeNewBranch(long? id)
    {
        _selectedExpertiseBranch = _expertiseBranchList.FirstOrDefault(x => x.Id == id);
        StateHasChanged();
    }

    private async void CalculateCapacity()
    {
        var globalQuota = _quotaRequest.GlobalQuota.FirstOrDefault(x => x.ExpertiseBranchId == _selectedExpertiseBranch.Id);
        if (globalQuota.AnnualGlobalQuota == null || globalQuota.AnnualGlobalQuota < 1)
        {
            SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L["You cannot calculate capacity without determining annual global quotas!"]);
            return;
        }

        _loading = true;
        StateHasChanged();
        foreach (var item in _selectedSubQuotaRequests)
        {
            var point_1 = GetPoint(item.ProfessorCount + item.AssociateProfessorCount, false);
            var point_2 = GetPoint(item.DoctorLecturerCount + item.ChiefAssistantCount, false);
            var point_3 = GetPoint(item.SpecialistDoctorCount, true);
            item.EducatorPoint = Convert.ToInt32(point_1 * 1.2 + point_2 * 0.6 + point_3 * 0.3);
        }

        foreach (var item in _selectedSubQuotaRequests)
        {
            item.EducatorPoint.PrintJson("eduPoint");
            item.EducatorIndex = item.EducatorPoint * 1000 / ((int)_selectedSubQuotaRequests.Select(x => x.EducatorPoint).Sum());
            _selectedSubQuotaRequests.Select(x => x.EducatorPoint).Sum().PrintJson("eduPointSum");
            if (item.Portfolios?.Count > 0)
                foreach (var item_1 in item.Portfolios)
                {
                    item.PortfolioIndex = +item_1.Answer * item_1.Ratio * 10 / (_selectedSubQuotaRequests.SelectMany(x => x.Portfolios.Where(x => x.PortfolioId == item_1.PortfolioId)).Sum(x => x.Answer));
                    var sum = _selectedSubQuotaRequests.SelectMany(x => x.Portfolios.Where(x => x.PortfolioId == item_1.PortfolioId)).Sum(x => x.Answer);
                    sum.PrintJson("PorfolioSum");
                }
            var qewqwe = item.EducatorIndex * _selectedExpertiseBranch.EducatorIndexRateToCapacityIndex;
            var asdasd = item.PortfolioIndex * _selectedExpertiseBranch.PortfolioIndexRateToCapacityIndex;
            item.PortfolioIndex.PrintJson("PortIndex");
            qewqwe.PrintJson("qewqwe");
            asdasd.PrintJson("asdad");
            item.CapacityIndex = (item.EducatorIndex * _selectedExpertiseBranch.EducatorIndexRateToCapacityIndex/100) + (item.PortfolioIndex * _selectedExpertiseBranch.PortfolioIndexRateToCapacityIndex/100);

            item.Capacity = _quotaRequest.GlobalQuota.FirstOrDefault(x => x.ExpertiseBranchId == item.ExpertiseBranchId).AnnualGlobalQuota * _curriculumDuration * item.CapacityIndex / 1000;

            item.CapacityIndex.PrintJson("CapacityIndex");
        }


        foreach (var item in _selectedSubQuotaRequests)
        {
            var dto = Mapper.Map<SubQuotaRequestDTO>(item);
            var response = await SubQuotaRequestService.Update((long)item.Id, dto);
        }
        _loading = false;
        StateHasChanged();
    }

    public int GetPoint(int? educatorCount, bool isExpert)
    {
        if (educatorCount == 0)
            return 0;
        var formula = _formulas.FirstOrDefault(x => x.IsExpert == (educatorCount < 6 ? true : isExpert) && x.MinEducatorCount <= educatorCount && x.MaxEducatorCount >= educatorCount);
        return Convert.ToInt32(formula.BaseScore + ((educatorCount - (formula.MinEducatorCount - 1)) * formula.Coefficient));
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

    private async Task OpenSummaryTableModal()
    {
        var response = await StudentCountService.GetPaginateList(new()
        { Filter = new() { Logic = "and", Filters = new List<Filter>() { new() { Field = "SubQuotaRequest.QuotaRequestId", Operator = "eq", Value = _quotaRequest.Id } } } });

        _studentCounts = response.Items;
        _quotaTypes = _studentCounts.Select(x => x.QuotaType).Distinct().ToList();
        StateHasChanged();
        _summaryTableModal.OpenModal();
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

    private void OnChangeFirstPeriod(int? value, long? expBrId)
    {
        var globalQuota = _quotaRequest.GlobalQuota.FirstOrDefault(x => x.ExpertiseBranchId == expBrId);
        globalQuota.FirstPeriodGlobalQuota = value;
        globalQuota.AnnualGlobalQuota = (globalQuota.FirstPeriodGlobalQuota ?? 0) + (globalQuota.SecondPeriodGlobalQuota ?? 0);
        StateHasChanged();
    }
    private void OnChangeSecondPeriod(int? value, long? expBrId)
    {
        var globalQuota = _quotaRequest.GlobalQuota.FirstOrDefault(x => x.ExpertiseBranchId == expBrId);
        globalQuota.SecondPeriodGlobalQuota = value;
        globalQuota.AnnualGlobalQuota = (globalQuota.FirstPeriodGlobalQuota ?? 0) + (globalQuota.SecondPeriodGlobalQuota ?? 0);
        StateHasChanged();
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
