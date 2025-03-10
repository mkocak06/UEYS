using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs

{
    public partial class ProtocolProgram
    {
        [Parameter] public long Id { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }

        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IStudentDependentProgramService StudentDependentProgramService { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private ProgramResponseDTO _mainProgram;
        private bool _loading = false;
        private EditContext _ec = new EditContext(new StudentResponseDTO());
        private List<StudentDependentProgramPaginateDTO> _studentDependentPrograms = new();
        private MyModal _modal;
        private DateTime? _date;
        private string _dateValidatorMessage;
        private string _remainingDateMessage;
        private StudentDependentProgramPaginateDTO _studentDependentProgram = new();
        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        private BootstrapDatePicker _dp;
        private Dropzone dropzone;
        private string _documentValidatorMessage;
        private MyModal _fileModal;
        private MyModal _uploaderModal;
        private List<DocumentResponseDTO> _documents = new();

        protected override async Task OnInitializedAsync()
        {
            _ec = new EditContext(_student);

            try
            {
                _loading = true;
                var response = await StudentDependentProgramService.GetListByStudentId((long)_student.Id);
                var response_2 = await ProgramService.GetById((long)_student.OriginalProgramId);
                if (response.Result && response_2.Result)
                {
                    _studentDependentPrograms = response.Item;
                    _mainProgram = response_2.Item;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _loading = false;
            }

            await base.OnInitializedAsync();
        }

        private async Task SendToDependentProgramHandler(StudentDependentProgramPaginateDTO studentDependentProgram)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to send student to this dependent program?"]}",
                SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                _studentDependentProgram = studentDependentProgram;
                dropzone.ResetStatus();
                StateHasChanged();
                _modal.OpenModal();
            }
        }

        private async void SendToDependentProgram()
        {
            _dateValidatorMessage = string.Empty;
            _documentValidatorMessage = string.Empty;
            if (_date == null)
            {
                _dateValidatorMessage = L["Lütfen bir tarih seçiniz."];
                StateHasChanged();
                return;
            }
            dropzone.PrintJson("0");

            if (string.IsNullOrEmpty(dropzone._selectedFileName) && _studentDependentProgram.Documents?.Count < 1)
            {
                _documentValidatorMessage = "Bu alan zorunludur!";
                StateHasChanged();
                return;
            }

            _studentDependentProgram.StartDate = _date;
            try
            {
                var response = await EducationTrackingService.SendStudentToDependentProgram((long)_studentDependentProgram.Id, _studentDependentProgram);
                if (response.Result)
                {
                    if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                        await CallDropzone();
                    _modal.CloseModal();
                    NavigationManager.NavigateTo($"./student/students");
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], response.Message);
            }
            catch (Exception)
            {
                SweetAlert.ErrorAlert();
            }
        }

        private async void FinishDependentProgramEducationHandler(StudentDependentProgramPaginateDTO studentDependentProgram)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to send student to main program in the protocol?"]}",
                SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                _studentDependentProgram = studentDependentProgram;
                _dp.ClearDate();
                _date = null;
                _remainingDateMessage = null;
                StateHasChanged();
                _modal.OpenModal();
            }
        }

        private async void FinishDependentProgramEducation(bool uncompleted)
        {
            _dateValidatorMessage = string.Empty;
            _documentValidatorMessage = string.Empty;
            if (_date == null)
            {
                _dateValidatorMessage = L["Lütfen bir tarih seçiniz."];
                StateHasChanged();
                return;
            }
            if (_studentDependentProgram.Documents?.Count < 1)
            {
                dropzone.PrintJson("0");
                if (string.IsNullOrEmpty(dropzone._selectedFileName))
                {
                    _documentValidatorMessage = "Bu alan zorunludur!";
                    StateHasChanged();
                    return;
                }
            }

            _studentDependentProgram.EndDate = _date;
            _studentDependentProgram.IsUnCompleted = uncompleted;
            try
            {
                var response = await EducationTrackingService.ReturnToMainProgramInProtocol((long)_studentDependentProgram.Id, _studentDependentProgram);
                if (response.Result)
                {
                    if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                        await CallDropzone();
                    _modal.CloseModal();
                    NavigationManager.NavigateTo($"./student/students");
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], response.Message);
            }
            catch (Exception e)
            {
                SweetAlert.ErrorAlert();
                Console.WriteLine(e);
            }
        }

        private async Task OnChangeDate(DateTime? e)
        {
            _remainingDateMessage = string.Empty;
            if (e == null)
            {
                _date = null;
            }
            else
            {
                _date = e;
                if (_student.ProtocolProgramId != null)
                {
                    _studentDependentProgram.EndDate = _date;
                    StateHasChanged();
                    try
                    {
                        var response = await EducationTrackingService.GetRemainingDaysForDependentProgram(_studentDependentProgram);
                        if (response.Result)
                        {
                            if (response.Item > 0)
                            {
                                _remainingDateMessage = L["The remaining time to complete the education in this program is {0} days.", response.Item];
                                StateHasChanged();
                            }
                        }
                        else
                            SweetAlert.ErrorAlert();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        public async Task<bool> CallDropzone()
        {
            StateHasChanged();
            try
            {
                var response = await dropzone.SubmitFileAsync();
                if (response.Result)
                {
                    _studentDependentProgram.Documents.Add(response.Item);
                    StateHasChanged();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                dropzone.ResetStatus();
                StateHasChanged();
            }
        }

        private async Task OnDownloadHandler(StudentDependentProgramPaginateDTO studentDependentProgram)
        {
            _studentDependentProgram = studentDependentProgram;
            StateHasChanged();
            _fileModal.OpenModal();
        }

        private async Task OnUploadHandler(StudentDependentProgramPaginateDTO studentDependentProgram)
        {
            dropzone.ResetStatus();
            _studentDependentProgram = studentDependentProgram;
            StateHasChanged();
            _uploaderModal.OpenModal();
        }
    }
}