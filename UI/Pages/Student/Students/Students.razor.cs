using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Shared.Extensions;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Pages.InstitutionManagement.ProtocolPrograms;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;


namespace UI.Pages.Student.Students;

public partial class Students
{
    [Inject] private IStudentService StudentService { get; set; }
    [Inject] private IState<AppState> AppState { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
    [Inject] public IAuthenticationService AuthenticationService { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    private List<StudentPaginateResponseDTO> _students;
    private List<StudentPaginateResponseDTO> _expiredStudents;
    private StudentResponseDTO _student;
    private FilterDTO _filter;
    private PaginationModel<StudentPaginateResponseDTO> _paginationModel;
    private bool _loading = true;
    private bool forceRender;
    private List<BreadCrumbLink> _links;
    private MyModal _cancelModal;
    private EditContext _cancelEc;
    private bool _saving;
    private string _validationMessage;
    private bool _loadingFile;
    private string _deleteReasonValidatorMessage;
    private string _explanationValidatorMessage;
    private StudentStatus? _studentStatus = null;
    private EducationTrackingResponseDTO _educationTracking = new() { ProcessType = ProcessType.Finish, };
    private EditContext _ec;
    private MyModal _educationTrackingAddModal;
    private MyModal _expiredStudentModal;
    private List<StudentStatus?> StudentStatuses = new() { StudentStatus.Abroad, StudentStatus.Assigned, StudentStatus.TransferDueToNegativeOpinion, StudentStatus.EndOfEducationDueToNegativeOpinion };
    private UserForLoginResponseDTO User => AuthenticationService.User;
    private List<RoleCategoryType?> RoleCategoryTypes = new() { RoleCategoryType.University, RoleCategoryType.Faculty, RoleCategoryType.Hospital, RoleCategoryType.Program, RoleCategoryType.Province };

    protected override async Task OnInitializedAsync()
    {
        _student = new StudentResponseDTO();
        _cancelEc = new EditContext(_student);
        _ec = new EditContext(_educationTracking);

        _filter = new FilterDTO()
        {
            Filter = new()
            {
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
                        Field="UserIsDeleted",
                        Operator="eq",
                        Value=false
                    },
                    new Filter()
                    {
                        Field="MainList"
                    }
                },
                Logic = "and"
            },
            Sort = new[]{new Sort()
                {
                    Field = "Name",
                    Dir = SortType.ASC
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
                    To = "/student/students",
                    OrderIndex = 1,
                    Title = L["Specialization Student Informations"]
                }
        };

        RoleCategoryTypes.PrintJson("types");
        User.RoleCategoryType.PrintJson("type");
        if (RoleCategoryTypes.Contains(User?.RoleCategoryType))
            await GetExpiredStudents();
        await GetStudents();
        await base.OnInitializedAsync();
    }
    private async Task OnSortChange(Sort sort)
    {
        _filter.Sort = new[] { sort };
        await GetStudents();
    }

    private async Task GetStudents()
    {
        _loading = true;
        StateHasChanged();
        try
        {
            _paginationModel = await StudentService.GetPaginateListOnly(_filter);
            if (_paginationModel.Items != null)
            {
                _students = _paginationModel.Items.OrderBy(x => x.HospitalName).ToList();
                _loading = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();

    }

    private async Task GetExpiredStudents()
    {
        _loading = true;
        StateHasChanged();
        try
        {
            var response = await StudentService.GetExpiredStudents();
            if (response.Items?.Count > 0)
            {
                _expiredStudents = response.Items.OrderBy(x => x.Name).ToList();
                _expiredStudentModal.OpenModal();
                _loading = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();

    }

    private async Task OnDeleteHandler(StudentPaginateResponseDTO studentPaginate)
    {
        var student = await StudentService.GetById((int)studentPaginate.Id);
        _student = student.Item;
        _educationTrackingAddModal.OpenModal();
        _ec = new EditContext(_educationTracking);

        //_student = new StudentResponseDTO()
        //{
        //    Id = (int)student.Id,
        //    DeleteReason = student.DeleteReason,
        //    AdvisorId = student.AdvisorId,
        //    CurriculumId = student.CurriculumId,
        //    ProgramId = student.ProgramId,
        //    UserId = student.UserId,
        //    //User = student.User
        //};

        //await DeleteStudent();

        //var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
        //    $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
        //    SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

        //if (confirm)
        //{
        //    try
        //    {
        //        await StudentService.Delete((long)student.Id);
        //        _students.Remove(student);
        //        StateHasChanged();
        //        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        SweetAlert.ErrorAlert();
        //        throw;
        //    }
        //}
    }

    private async Task DeleteStudent()
    {
        //_deleteReasonValidatorMessage = null;
        //_explanationValidatorMessage = null;

        //if (_student.DeleteReasonType == null) { _deleteReasonValidatorMessage = L["Deleting Reason cannot be empty!"]; return; }
        //if (string.IsNullOrEmpty(_student.DeleteReason) && _student.DeleteReasonType == StudentDeleteReasonType.Other) { _explanationValidatorMessage = L["Explanation cannot be empty!"]; return; }
        //var confirm = await SweetAlert.ConfirmAlert(L["Warning"], L[_student.DeleteReasonType == StudentDeleteReasonType.RegistrationByMistake ? "Are you sure you want to delete this item? This action cannot be undone." : "If you delete the student, you cannot add them to the same program again! Are you sure you want to delete it?"], SweetAlertIcon.question, true, L["Yes"], L["No"]);


        //if (confirm)
        //{
        //_saving = true;
        //StateHasChanged();
        _student.IsDeleted = true;
        _student.DeleteDate = DateTime.UtcNow;
        var dto = Mapper.Map<StudentDTO>(_student);
        try
        {
            ResponseWrapper<StudentResponseDTO> response;
            if (_student.DeleteReasonType == ReasonType.RegistrationByMistake)
                response = await StudentService.CompletelyDelete(_student.Id);
            else
                response = await StudentService.Update(_student.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                await GetStudents();
            }
            else
                throw new Exception(response.Message);
        }
        catch (Exception e)
        {
            SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
            Console.WriteLine(e.Message);
        }
        //_saving = false;
        StateHasChanged();
        //_cancelModal.CloseModal();
        //}
    }

    private async Task AddGraduateFilter()
    {
        _filter.Filter.Filters.Add(
        new Filter()
        {
            Field = "Graduate"
        });
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

        var response = await StudentService.GetExcelByteArray(_filter);

        if (response.Result)
        {
            await JsRuntime.InvokeVoidAsync("saveAsFile", $"StudentList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
            _loadingFile = false;
        }
        else
        {
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();
    }

    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetStudents();
    }
    private void OnDetailHandler(long? id)
    {
        _expiredStudentModal.CloseModal();
        NavigationManager.NavigateTo($"/student/students/{id}");
    }

    #region FilterChangeHandlers

    private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
    {
        var value = (string)args.Value;
        if (filterName == "Status") _studentStatus = (StudentStatus)Enum.Parse(typeof(StudentStatus), value);
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
        await GetStudents();
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
            if (filterName == "Status") _studentStatus = null;
            await GetStudents();
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

    private async Task AddEducationTracking()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();

        _educationTracking.StudentId = _student.Id;
        var dto = Mapper.Map<EducationTrackingDTO>(_educationTracking);
        try
        {
            dto.ProcessDate = dto?.ProcessDate?.Date;
            var response = await EducationTrackingService.Add(dto);
            if (!response.Result)
                SweetAlert.IconAlert(SweetAlertIcon.error, L["Error"], L[response.Message]);
        }
        catch (Exception e)
        {
            SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
            Console.WriteLine(e);
        }
        finally
        {
            _educationTrackingAddModal.CloseModal();
            _saving = false;
            StateHasChanged();
            _educationTracking = new() { ProcessType = ProcessType.Finish, };
        }

        await GetStudents();
    }
}