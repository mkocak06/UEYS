using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen.Blazor;
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

public partial class ActiveRequestDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IQuotaRequestService QuotaRequestService { get; set; }
    [Inject] public ISubQuotaRequestService SubQuotaRequestService { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IProgramService ProgramService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private QuotaRequestResponseDTO _quotaRequest;
    private SubQuotaRequestResponseDTO _subQuotaRequest;
    private List<ProgramPaginateForQuotaResponseDTO> _programs;
    private EditContext _ec;
    private EditContext _ecSubQuotaRequest;
    private bool _notFound;
    private bool _saving;
    private bool _loaded;
    private PaginationModel<ProgramPaginateForQuotaResponseDTO> _paginationModel;
    private MyModal _subQuotaRequestModal;
    private FilterDTO _filter;
    private List<BreadCrumbLink> _links;
    private List<QuotaType> _quotaTypes;

    protected override void OnInitialized()
    {
        _quotaRequest = new QuotaRequestResponseDTO();
        _subQuotaRequest = new SubQuotaRequestResponseDTO();
        _ec = new EditContext(_quotaRequest);
        _ecSubQuotaRequest = new EditContext(_subQuotaRequest);
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
                    To = "/quota-request/active-requests",
                    OrderIndex = 1,
                    Title = L["_List", L["Active Quota Request"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Active Quota Request"]]
                }
        };
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await QuotaRequestService.GetById((long)Id);
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
                                Field="ProfessionCode",
                                Operator="eq",
                                Value= _quotaRequest.Type == PlacementExamType.DUS ? 3 : _quotaRequest.Type == PlacementExamType.EUS ? 2 : 1
                            },
                            new Filter()
                            {
                                Field="IsQuotaRequestable",
                                Operator="eq",
                                Value=true
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
                        Field = "ProgramName",
                        Dir = SortType.ASC
                    }}
                };

                GetPrograms();
                _ec = new EditContext(_quotaRequest);
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

    private async Task Save()
    {
        if (!_ec.Validate()) return;

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
    }

    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetPrograms();
    }

    private async Task GetPrograms()
    {

        _paginationModel = await ProgramService.GetPaginateListForQuotaRequest(_filter);
        if (_paginationModel.Items != null)
        {
            _programs = _paginationModel.Items;
            StateHasChanged();
        }
        else
        {
            SweetAlert.ErrorAlert();
        }
    }

    private async Task OpenSubQuotaRequestModal(ProgramPaginateForQuotaResponseDTO program)
    {
        _quotaTypes = Enum.GetValues(typeof(QuotaType)).Cast<QuotaType>().ToList();

        var subQuotaRequest = _quotaRequest.SubQuotaRequests.FirstOrDefault(x => x.ProgramId == program.Id);

        if (subQuotaRequest != null)
            _subQuotaRequest = subQuotaRequest;
        else
        {
            _subQuotaRequest = new()
            {
                Program = new() { },
                ProgramId = program.Id,
                QuotaRequestId = _quotaRequest.Id,
                SubQuotaRequestPortfolios = new List<SubQuotaRequestPortfolioResponseDTO>(),
                StudentCounts = new List<StudentCountResponseDTO>(),
                AssociateProfessorCount = _programs.FirstOrDefault(x => x.Id == program.Id).AssociateProfessorCount,
                ChiefAssistantCount = _programs.FirstOrDefault(x => x.Id == program.Id).ChiefAssistantCount,
                DoctorLecturerCount = _programs.FirstOrDefault(x => x.Id == program.Id).DoctorLecturerCount,
                ProfessorCount = _programs.FirstOrDefault(x => x.Id == program.Id).ProfessorCount,
                TotalEducatorCount = _programs.FirstOrDefault(x => x.Id == program.Id).TotalEducatorCount,
                CurrentStudentCount = _programs.FirstOrDefault(x => x.Id == program.Id).CurrentStudentCount,
                StudentWhoLast6MonthToFinishCount = _programs.FirstOrDefault(x => x.Id == program.Id).StudentWhoLast6MonthToFinishCount
            };
            foreach (var item in _programs.FirstOrDefault(x => x.Id == _subQuotaRequest.ProgramId).Portfolios)
                _subQuotaRequest.SubQuotaRequestPortfolios.Add(new() { PortfolioId = item.Id, Portfolio = item });
        }
        StateHasChanged();
        _ecSubQuotaRequest = new EditContext(_subQuotaRequest);
        _subQuotaRequestModal.OpenModal();
    }

    private async Task AddSubQuotaRequest()
    {
        if (!_ecSubQuotaRequest.Validate())
        {
            return;
        }
        var dto = Mapper.Map<SubQuotaRequestDTO>(_subQuotaRequest);
        dto.TotalEducatorCount += dto.SpecialistDoctorCount;
        try
        {
            _saving = true;
            StateHasChanged();
            var response = await SubQuotaRequestService.Add(dto);
            if (response.Result)
            {
                _quotaRequest.SubQuotaRequests.Add(_subQuotaRequest);
                _subQuotaRequestModal.CloseModal();
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                GetPrograms();
            }
            else
            {
                throw new Exception();
            }
        }
        catch
        {
            SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An Error Occured."]);
        }
        finally
        {
            _saving = false;
        }
    }

    private async Task UpdateSubQuotaRequest()
    {
        if (!_ecSubQuotaRequest.Validate())
        {
            return;
        }
        var dto = Mapper.Map<SubQuotaRequestDTO>(_subQuotaRequest);
        try
        {
            _saving = true;
            StateHasChanged();
            var response = await SubQuotaRequestService.Update((long)_subQuotaRequest.Id, dto);
            if (response.Result)
            {
                _subQuotaRequestModal.CloseModal();
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated"]);
                GetPrograms();
            }
            else
            {
                throw new Exception();
            }
        }
        catch
        {
            SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An Error Occured."]);
        }
        finally
        {
            _saving = false;
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

    private void OnAddStudentCount()
    {
        _subQuotaRequest.StudentCounts ??= new List<StudentCountResponseDTO>();
        _subQuotaRequest.StudentCounts.Add(new StudentCountResponseDTO());
        StateHasChanged();
    }
}
