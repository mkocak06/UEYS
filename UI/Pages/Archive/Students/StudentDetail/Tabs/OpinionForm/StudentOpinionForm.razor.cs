using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs.OpinionForm
{
    public partial class StudentOpinionForm
    {
        [Parameter] public OpinionFormResponseDTO OpinionForm { get; set; }
        [Parameter] public List<OpinionFormResponseDTO> OpinionForms { get; set; }
        [Parameter] public bool IsEditing { get; set; }
        [Parameter] public bool? IsExplanationRequired { get; set; }
        [Parameter] public EventCallback<bool> OnSaveHandler { get; set; }
        [Parameter] public EventCallback<bool> OnUpdateHandler { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] public IEducatorProgramService EducatorProgramService { get; set; }
        [Inject] public IEducationOfficerService EducationOfficerService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
        [Inject] public IOpinionService OpinionService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        private OpinionFormResponseDTO _opinion = new();
        private List<OpinionFormResponseDTO> _opinions = new();
        private List<DocumentResponseDTO> responseDocuments = new();
        private List<EducationTrackingResponseDTO> _educationTrackings;
        private EditContext _ec;
        private bool _loading = true;
        private bool[] Collapsed = { false, false, false, false, false };
        private DateTime? _lastDate = new();
        private bool _loaded = false;

        public Dropzone dropzoneForAdd;
        public Dropzone dropzoneForUpdate;
        public MyModal UploaderModalForAdd;
        public MyModal UploaderModalForUpdate;
        private bool _fileLoaded = true;
        public bool _saving;
        private int[] _scores;
        private string _explanationValidationMessage;
        private UserForLoginResponseDTO _user => AuthenticationService.User;

        protected override async Task OnInitializedAsync()
        {
            _opinions = OpinionForms;
            IsExplanationRequired.PrintJson("IsExplanationRequired");
            OpinionFormRequestDTO opinionForm = new();
            if (IsEditing)
            {
                _opinion = OpinionForm;
                opinionForm = new() { StartDate = _opinion.StartDate, EndDate = _opinion.EndDate, StudentId = _student.Id };
            }
            else
            {
                if (_opinions?.Count > 0)
                {
                    _lastDate = _opinions.OrderByDescending(x => x.EndDate)?.FirstOrDefault()?.EndDate?.AddDays(1);
                }
                else
                {
                    var educationStart = await EducationTrackingService.GetEducationStartByStudentId(_student.Id);
                    _lastDate = educationStart?.Item?.ProcessDate;
                }
                _opinion = new() { StartDate = _lastDate, EndDate = _lastDate?.AddMonths(6), StudentId = _student.Id, IsRepeating = IsExplanationRequired };
                opinionForm = new() { StartDate = _lastDate, EndDate = _lastDate?.AddMonths(6), StudentId = _student.Id };
            }
            var response = await EducationTrackingService.GetTimeIncreasingRecordsByDate(opinionForm);
            _educationTrackings = response.Item;
            if (_educationTrackings != null)
            {
                foreach (var item in _educationTrackings)
                    _opinion.EndDate?.AddDays((int)item.AdditionalDays);
                StateHasChanged();
            }
            if (_educationTrackings != null && _opinion.Id == null)
                _opinion.EndDate = _opinion.EndDate?.AddDays((int)_educationTrackings.Select(x => x.AdditionalDays).Sum());

            _ec = new EditContext(_opinion);
            _loaded = true;
            StateHasChanged();
            await base.OnInitializedAsync();
        }
        async Task ConfirmInternalNavigation(LocationChangingContext locationChanging)
        {
            if (_ec.IsModified())
            {
                var isConfirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Kaydedilmeyen alanlar silinsin mi?");
                if (!isConfirmed)
                {
                    locationChanging.PreventNavigation();
                }
            }
        }
        public async Task<bool> CallDropzone()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneForAdd.SubmitFileAsync();
                if (result.Result)
                {
                    responseDocuments.Add(result.Item);
                    _opinion.Documents = new();
                    _opinion.Documents.Add(result.Item);
                    _fileLoaded = true;
                    StateHasChanged();
                    UploaderModalForAdd.CloseModal();
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
                _fileLoaded = true;
                StateHasChanged();
                dropzoneForAdd.ResetStatus();
                StateHasChanged();
            }

        }
        public async Task<bool> CallDropzoneForUpdate()
        {
            try
            {
                var result = await dropzoneForUpdate.SubmitFileAsync();
                if (result.Result)
                {
                    responseDocuments.Add(result.Item);
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
                Console.WriteLine(e);
                return false;
            }
            finally { dropzoneForUpdate.ResetStatus(); }



        }

        private string GetAverageClass(string avg)
        {
            double avgS = Convert.ToDouble(avg);
            if (avgS >= 0 && avgS < 3)
                return "label label-xl label-danger label-pill label-inline font-size-h4";
            else if (avgS >= 3)
                return "label label-xl label-success label-pill label-inline font-size-h4";
            else return "label label-xl  label-pill label-inline font-size-h4";
        }
        private async Task GetOpinionDetail()
        {
            try
            {
                var response = await OpinionService.GetListByStudentId(_student.Id);
                if (response.Result)
                {
                    if (response.Item != null && response.Item.Any())
                    {
                        _opinions = response.Item;
                    }
                    _loading = false;
                    StateHasChanged();
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
        }
        public async Task Save()
        {
            _explanationValidationMessage = null;
            if (!_ec.Validate()) return;
            if (_opinion.AdditionalExplanation == null && _opinion.IsRepeating == true)
            {
                _explanationValidationMessage = L["Additional Explanation Field of New Opinion Form Added Instead of Canceled Opinion Form is Required"];
                StateHasChanged();
                return;
            }
            _saving = true;
            StateHasChanged();
            var dto = Mapper.Map<OpinionFormDTO>(_opinion);
            dto.StudentId = _student.Id;
            dto.SecretaryId = _user.Id;
            if (_opinion.Id == null || _opinion.Id == 0)
                dto.Period = (_opinions.Count == 0 ? PeriodType.Period_1 : _opinions.OrderByDescending(x => x.StartDate).FirstOrDefault().Period + 1);

            dto.Result = (_opinion.ComplianceToWorkingHours_DC + _opinion.DutyAccomplishment_DC + _opinion.DutyExecution_DC + _opinion.DutyResponsibility_DC) / (double)4 <= 3 || (_opinion.ProfessionalPracticeAbility + _opinion.Scientificness + _opinion.TeamworkAdaptation) / (double)3 <= 3 || (_opinion.ResearchDesire + _opinion.ResearchExecutionAndAccomplish + _opinion.UsingResourcesEfficiently + _opinion.BroadcastingAbility) / (double)4 <= 3 || (_opinion.ProblemAnalysisAndSolutionAbility + _opinion.OrganizationAndCoordinationAbility + _opinion.CommunicationSkills) / (double)3 <= 3 || (_opinion.RelationsWithOtherStudents + _opinion.RelationsWithEducators + _opinion.RelationsWithOtherEmployees + _opinion.RelationsWithPatients) / (double)4 <= 3 ? RatingResultType.Negative : RatingResultType.Positive;
            dto.ProgramId = _student.ProgramId;

            try
            {
                if (_opinion.Id != null && _opinion.Id > 0)
                {
                    var response = await OpinionService.Update(_opinion.Id.Value, dto);
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);

                        if (response.Item.Navigate == true)
                        {
                            NavigationManager.NavigateTo("/student/students");
                        }

                        await OnUpdateHandler.InvokeAsync(true);
                    }
                }
                else
                {
                    var response = await OpinionService.Add(dto);
                    if (response.Result)
                    {
                        foreach (var item in responseDocuments)
                        {
                            var documentDTO = Mapper.Map<DocumentDTO>(item);
                            documentDTO.EntityId = response.Item.Id.Value;
                            var result = await DocumentService.Update(item.Id, documentDTO);
                            if (!result.Result)
                            {
                                throw new Exception(result.Message);
                            }
                        }
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);

                        if (response.Item.Navigate == true)
                        {
                            NavigationManager.NavigateTo("/student/students");
                        }

                        await OnSaveHandler.InvokeAsync(true);
                    }
                    else
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], response.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            _saving = false;
            StateHasChanged();
        }

        private async Task<IEnumerable<EducatorResponseDTO>> SearchEducators(string searchQuery)
        {
            var result = await EducatorProgramService.GetListByProgramId(_student.ProgramId.Value);
            return result.Result ? result.Item.Select(x => x.Educator).Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<EducatorResponseDTO>();
        }

        private void OnChangeEducator(EducatorResponseDTO educator)
        {
            _opinion.Educator = educator;
            _opinion.EducatorId = educator.Id;
        }

        private void OnChangeBeginDate(DateTime? e)
        {
            _opinion.StartDate = e;
            _opinion.EndDate = _opinion.StartDate?.AddMonths(6);
        }

        //private void GetPenultimateDate()
        //{
        //    if (_opinions?.Count > 1)
        //    {
        //        _penultimateDate = _opinions[_opinions.Count - 2].EndDate;
        //    }
        //}
        public int Sum(List<int> ints)
        {
            int result = 0;

            for (int i = 0; i < ints.Count; i++)
            {
                result += ints[i];
            }

            return result;
        }
        public decimal GetOverallAverage()
        {
            var intArray = new int?[]{ _opinion.ComplianceToWorkingHours_DC,_opinion.DutyResponsibility_DC,_opinion.DutyExecution_DC,_opinion.DutyAccomplishment_DC,
                                    _opinion.ProblemAnalysisAndSolutionAbility,_opinion.OrganizationAndCoordinationAbility,_opinion.CommunicationSkills,
                                    _opinion.RelationsWithOtherStudents, _opinion.RelationsWithEducators, _opinion.RelationsWithOtherEmployees, _opinion.RelationsWithPatients,
                                    _opinion.ProfessionalPracticeAbility, _opinion.Scientificness, _opinion.TeamworkAdaptation, _opinion.ResearchDesire, _opinion.ResearchExecutionAndAccomplish, _opinion.UsingResourcesEfficiently,_opinion.BroadcastingAbility};
            List<int> intList = new List<int>();
            foreach (var item in intArray)
            {
                if (item != null)
                    intList.Add((int)item);
            }

            int sum = Sum(intList);
            decimal result = 0;

            if (intList.Count != 0)
                result = (decimal)sum / intList.Count;

            result = Convert.ToDecimal(String.Format("{0:0.00}", result));
            return result;
        }

        private async Task DownloadFile()
        {
            try
            {
                var x = await OpinionService.DownloadExampleForm("9c5255f4-0fe3-4759-b117-b686b5fc2d78.xlsx");
                await JSRuntime.InvokeVoidAsync("saveAsFile", "Örnek Kanaat Formu.xlsx", Convert.ToBase64String(x.Item.FileContent));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
                throw;
            }

        }
    }
}