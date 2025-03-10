using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.DetailCards;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Components
{
    public partial class GridStudents
    {
        [Parameter] public bool AddStudent { get; set; } = false;
        [Parameter] public string FilterField { get; set; }
        [Parameter] public long Id { get; set; }
        [Parameter] public bool HideProgram { get; set; } = false;
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] IState<AppState> AppState { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] ICurriculumService CurriculumService { get; set; }
        private List<StudentResponseDTO> _students;
        private PaginationModel<StudentResponseDTO> _paginationModel;
        [Inject] IState<ProgramDetailState> ProgramDetailState { get; set; }
        private StudentViewer _programStudentDetail;
        private StudentResponseDTO _selectedStudent;
        private CurriculumResponseDTO _selectedCurriculum;
        private ProgramResponseDTO _program;
        private EducatorViewer _selectedAdvisorDetail;
        private EducatorResponseDTO _selectedAdvisor;
        private ProgramViewer _programDetail;
        private CurriculumViewer _programCurriculumDetail;
        private ProgramResponseDTO _selectedProgram;
        private bool forceRender = false;
        private MyModal _addStudentModal;

        private StudentResponseDTO _addUserWithStudentInfo;
        private ResponseWrapper<StudentResponseDTO> _studentInfoResponse;
        private bool _loading;
        private FilterDTO _filter = new();
        private string _identityNo = string.Empty;
        [Parameter] public bool ByProgramId { get; set; } = false;
        [Parameter] public bool ByUniversityId { get; set; } = false;
        [Parameter] public bool ByHospitalId { get; set; } = false;
        private bool _searching = false;
        private bool _adding = false;
        private bool isKPSResult = false;
        private EditContext _ec;
        private InputMask _inputMask;
        private UserWithStudentInfoResponseDTO _userNotKPS;
        private CurriculumResponseDTO _curriculum;
        private string _tcValidatorMessage;
        private AddUserWithStudentInfoDTO _user;

        protected override async Task OnParametersSetAsync()
        {
            _students = new List<StudentResponseDTO>();
            _curriculum = new CurriculumResponseDTO();
            _user = new AddUserWithStudentInfoDTO();
            _ec = new EditContext(_user);
            _filter.Sort = new[]
            {
                new Sort()
                {
                    Field = "User.Name",
                    Dir = Shared.Types.SortType.ASC
                },
            };
            _filter.Filter = new()
            {
                Logic = "and",
                Filters = new()
                {
                    new Filter()
                    {
                        Field = FilterField,
                        Operator = "eq",
                        Value = Id
                    },
                    new Filter
                    {
                        Field = "IsDeleted",
                        Operator = "eq",
                        Value = false
                    }
                }
            };
            await GetStudents();
            await base.OnParametersSetAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (forceRender)
            {
                forceRender = false;
                JSRuntime.InvokeVoidAsync("initTooltip");
            }
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetStudents();
        }

        private async Task GetStudents()
        {
            try
            {
                _loading = true;
                if (_filter?.Filter?.Filters?.Count == 0)
                {
                    _filter.Filter.Filters = null;
                    _filter.Filter.Logic = null;
                }
                else
                {
                    _paginationModel = await StudentService.GetPaginateList(_filter);
                }
                if (_paginationModel.Items != null)
                {
                    _students = _paginationModel.Items;
                    forceRender = true;
                }
                else
                {
                    _students = new();
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message);
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }

        }
        private async Task OnDeleteHandler(StudentResponseDTO student)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await StudentService.Delete((long)student.Id);
                    _students.Remove(student);
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

        private void NavigateToAddingPage()
        {
            NavigationManager.NavigateTo($"/student/students/add-student/{Id}");
        }

        private void OnStudentDetailHandler(StudentResponseDTO student)
        {
            _selectedStudent = student;
            _programStudentDetail.OpenModal();
        }
        private void OnCurriculumDetailHandler(CurriculumResponseDTO student)
        {
            _selectedCurriculum = student;
            _programCurriculumDetail.OpenModal();
        }

        private void OnProgramDetailHandler(ProgramResponseDTO program)
        {
            _selectedProgram = program;
            _programDetail.OpenModal();
        }

        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetStudents();
        }

        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
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
            await GetStudents();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
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
    }
}
