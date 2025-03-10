using ApexCharts;
using AutoMapper;
using Blazored.Typeahead;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using Shared.Validations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UI;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents;
using UI.SharedComponents.BlazorLeaflet;
using UI.SharedComponents.Components;
using UI.Validation;

namespace UI.Pages.Student.Students.StudentDetail.Tabs
{
    public partial class ExitExams
    {
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IExitExamService ExitExamService { get; set; }
        [Inject] public IHospitalService HospitalService { get; set; }
        [Inject] public IEducatorService EducatorService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public IState<StudentDetailState> StudentDetailState { get; set; }
        private Dropzone dropzone;
        private List<DocumentResponseDTO> responseDocuments = new();
        private bool _fileLoaded = true;
        private bool _loading = false;
        private bool _saving;
        private string _documentValidatorMessage;

        private StudyResponseDTO _study;
        private List<ExitExamResponseDTO> _exitExams;
        private ExitExamResponseDTO _exitExam = new() { Juries = new() };
        private ExitExamRulesModel _rulesModel = new();

        private PaginationModel<ExitExamResponseDTO> _paginationModel;
        private PaginationModel<EducatorResponseDTO> _juryPaginationModel = new PaginationModel<EducatorResponseDTO>();
        private FilterDTO _filterJury;
        private FilterDTO _filter;


        private MyModal _exitExamModal;
        private MyModal _juryAddingModal;
        private MyModal UploaderModal;
        private MyModal FileModal;
        private MyModal FilePreviewModal;

        private List<DocumentResponseDTO> Documents = new();

        private EditContext _ec;
        private EditContext _ecUpdate;
        private JuryType _selectedJuryType;
        private bool _juryLoading;
        private string _practiseNote = string.Empty;
        private string _abilityNote = string.Empty;
        private int _intPractiseNote = 0;
        private bool forceRender;

        private StudentResponseDTO Student => StudentDetailState.Value.Student;
        private UserForLoginResponseDTO _user => AuthenticationService.User;
        protected override async Task OnInitializedAsync()
        {
            _ec = new EditContext(_exitExam);

            _filterJury = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "User.Name",
                Dir = SortType.ASC
            }}
            };
            _filterJury.Filter = new Filter()
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
                        Field="UserId",
                        Operator="neq",
                        Value=null
                    }
                },

            };
            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                {
                    new Filter()
                    {
                        Field="StudentId",
                        Operator="eq",
                        Value=Student.Id
                    },
                    new Filter()
                    {
                        Field="IsDeleted",
                        Operator="eq",
                        Value=false
                    }
                },
                    Logic = "and"
                },
            };

            await GetExitExams();

            await base.OnInitializedAsync();
        }

        private async Task AddingList()
        {
            try
            {
                var response = await ExitExamService.GetExitExamRules(Student.Id);
                if (response.Result)
                    _rulesModel = response.Item;
                else throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            if (!_rulesModel.AreRotationsCompleted)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Eksik Rotasyon", "Ýlgili öðrenciye bitirme sýnavý eklemek için zorunlu rotasyonlarýn tamamlanmasý gerekmektedir.");
                return;
            }
            else if (!_rulesModel.AreOpinionFormsCompleted)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Eksik Kanaat Formu", "Ýlgili öðrenciye bitirme sýnavý eklemek için gerekli sayýda kanaat formunun eklenmesi zorunludur.");
                return;
            }
            else if (!_rulesModel.IsThesisSuccess)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Baþarýlý Tez Bulunamadý", "Ýlgili öðrenciye bitirme sýnavý eklemek için baþarýlý bir tezi bulunmak zorundadýr.");
                return;
            }


            _exitExam = new() { EstimatedEndDate = _rulesModel.EstimatedEndDate, Description = _exitExams?.Count + 1 + ". Uzmanlýk Eðitimi Bitirme Sýnavý", Juries = new() };

            _ec = new EditContext(_exitExam);
            responseDocuments = new();
            await GetJuries();
            StateHasChanged();
            _exitExamModal.OpenModal();
        }

        public async Task<bool> CallDropzone()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    responseDocuments.Add(result.Item);
                    _fileLoaded = true;
                    StateHasChanged();
                    UploaderModal.CloseModal();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _fileLoaded = false;
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                dropzone.ResetStatus();
                StateHasChanged();
            }
        }
        private void OnChangeExamResultForAdd(ExitExamResultType? e)
        {
            if (e == null)
            {
                _exitExam.ExamStatus = null;
            }
            else
            {
                _exitExam.ExamStatus = e;
                if (_exitExam.ExamStatus == ExitExamResultType.NotConcluded)
                {
                    _exitExam.PracticeExamNote = null;
                    _exitExam.AbilityExamNote = null;
                }
            }
            StateHasChanged();
        }
        private async Task OnDownloadHandler(ExitExamResponseDTO exitExam)
        {
            responseDocuments = new();
            try
            {
                var response = await DocumentService.GetListByTypeByEntity((long)exitExam.EducationTrackingId, DocumentTypes.EducationTimeTracking);
                if (response.Result)
                {
                    responseDocuments = response.Item;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            StateHasChanged();
        }

        private async Task OnOpenUpdateModal(ExitExamResponseDTO ExitExam)
        {
            await OnDownloadHandler(ExitExam);
            _exitExam = ExitExam;
            _exitExam.EstimatedEndDate = Student.EducationTrackings.FirstOrDefault(x => x.ReasonType == ReasonType.EstimatedFinish)?.ProcessDate?.Date;
            if (responseDocuments.Count > 0)
                _exitExam.Documents = responseDocuments;
            _ec = new EditContext(_exitExam);
            await GetJuries();
            StateHasChanged();
            _exitExamModal.OpenModal();
        }


        private async Task OnRemoveExitExam(ExitExamResponseDTO exitExam)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    await ExitExamService.Delete(exitExam.Id.Value);
                    _exitExams.Remove(exitExam);
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

            await GetExitExams();
        }

        private async Task GetExitExams()
        {
            _paginationModel = await ExitExamService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                if (_filter.Sort == null)
                    _exitExams = _paginationModel.Items.OrderBy(x => x.ExamDate).ToList();
                else
                    _exitExams = _paginationModel.Items;

                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
        {
            var response = await HospitalService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Sort("Name", Shared.Types.SortType.ASC).OrFilter(new string[] { "Name", "Province.Name" }, "contains", searchQuery));
            return response.Items;
        }
        private void OnChangeHospital(HospitalResponseDTO hospital)
        {
            _exitExam.Hospital = hospital;
            _exitExam.HospitalId = hospital?.Id;
        }


        private async Task UpdateExitExam()
        {
            if (!_ec.Validate()) return;
            _documentValidatorMessage = string.Empty;
            if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                await CallDropzone();
            }


            _saving = true;
            StateHasChanged();
            _exitExam.StudentId = Student.Id;
            _exitExam.SecretaryId = _user.Id;


            if (_exitExam.ExamStatus == ExitExamResultType.Concluded && _exitExam.PracticeExamNote.HasValue && _exitExam.AbilityExamNote.HasValue)
            {

                bool examResult = (_exitExam.PracticeExamNote >= 60 && _exitExam.AbilityExamNote >= 60);
                examResult.PrintJson("examResult");
                _exitExam.EducationTracking.ReasonType.PrintJson("reasonType");
                _exitExam.EducationTracking.ReasonType = examResult ? ReasonType.SuccessfulInExitExam : ReasonType.FailureInExitExam;
                _exitExam.EducationTracking.Description = (((_exitExam.PracticeExamNote + _exitExam.AbilityExamNote) / 2) + " Ortalama ile Bitirme Sýnavýnda ") + (examResult ? L["Successful"] : L["Failed"]);
                _exitExam.EducationTracking.ProcessDate = _exitExam.ExamDate;
            }

            var dto = Mapper.Map<ExitExamDTO>(_exitExam);
            try
            {

                var response = await ExitExamService.Update(_exitExam.Id.Value, dto);
                if (response.Result)
                {

                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                    await GetExitExams();
                    _exitExamModal.CloseModal();
                    if (_exitExam.AbilityExamNote >= 60 && _exitExam.PracticeExamNote >= 60)
                        NavigationManager.NavigateTo("/student/students");
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

        private async Task AddExitExam()
        {
            if (!_ec.Validate()) return;

            if (string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = "Uzmanlýk Eðitimini Bitirme Sýnavý tutanaðýnýn doldurularak sisteme yüklenmesi gerekmektedir.";
                StateHasChanged();
                return;
            }
            else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                await CallDropzone();
            }

            _saving = true;
            StateHasChanged();

            _exitExam.StudentId = Student.Id;
            _exitExam.SecretaryId = _user.Id;

            if (_exitExam.ExamStatus == ExitExamResultType.Concluded && _exitExam.PracticeExamNote.HasValue && _exitExam.AbilityExamNote.HasValue)
            {

                bool examResult = (_exitExam.PracticeExamNote >= 60 && _exitExam.AbilityExamNote >= 60);
                _exitExam.EducationTracking = new EducationTrackingResponseDTO()
                {
                    ProcessType = ProcessType.Information,
                    ReasonType = examResult ? ReasonType.SuccessfulInExitExam : ReasonType.FailureInExitExam,
                    Description = (((_exitExam.PracticeExamNote + _exitExam.AbilityExamNote) / 2) + " Ortalama ile Bitirme Sýnavýnda ") + (examResult ? L["Successful"] : L["Failed"]),
                    ProcessDate = _exitExam.ExamDate,
                    StudentId = Student.Id,
                };
            }
            else
            {
                _exitExam.EducationTracking = new EducationTrackingResponseDTO()
                {
                    ProcessType = ProcessType.Information,
                    ReasonType = ReasonType.ExamNotConcluded,
                    Description = "Sýnav notlarýnýn girilmesi bekleniyor",
                    ProcessDate = _exitExam.ExamDate,
                    StudentId = Student.Id,
                };
            }

            var dto = Mapper.Map<ExitExamDTO>(_exitExam);
            try
            {
                var response = await ExitExamService.Add(dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                    foreach (var item in responseDocuments)
                    {
                        var documentDTO = Mapper.Map<DocumentDTO>(item);
                        documentDTO.EntityId = (long)response.Item.EducationTrackingId;
                        var resultDoc = await DocumentService.Update(item.Id, documentDTO);
                        if (!resultDoc.Result)
                        {
                            throw new Exception(resultDoc.Message);
                        }
                    }


                    await GetExitExams();
                    _exitExamModal.CloseModal();
                    if (_exitExam.AbilityExamNote >= 60 && _exitExam.PracticeExamNote >= 60)
                        NavigationManager.NavigateTo("/student/students");
                    StateHasChanged();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {

                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
                Console.WriteLine(e);
            }
            _saving = false;
            StateHasChanged();

        }
        private bool IsJuryChosen(EducatorResponseDTO educator)
        {
            _exitExam.Juries ??= new List<JuryResponseDTO>();
            return _exitExam.Juries.Any(x => x.EducatorId == educator.Id.Value);

        }
        private void OnAddJury(EducatorResponseDTO educator)
        {
            if (_selectedJuryType == JuryType.Core && _exitExam.Juries.Where(x => x.JuryType == JuryType.Core).ToList().Count == 5)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Asil Üye", "Asil Üye Sayýsý 5'ten fazla olamaz!");
                return;
            }
            else if (_selectedJuryType == JuryType.Alternate && _exitExam.Juries.Where(x => x.JuryType == JuryType.Alternate).ToList().Count == 2)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Yedek Üye", "Yedek Üye Sayýsý 2'den fazla olamaz!");
                return;

            }
            _exitExam.Juries ??= new List<JuryResponseDTO>();
            _exitExam.Juries.Add(new JuryResponseDTO() { Educator = educator, EducatorId = educator.Id.Value, JuryType = _selectedJuryType, ThesisDefenceId = null });
            _juryPaginationModel.Items.Remove(educator);
            _juryPaginationModel.TotalItemCount--;
            StateHasChanged();
        }
        private void OnRemoveAddingJury(JuryResponseDTO jury)
        {

            _exitExam.Juries.Remove(jury);
            _juryPaginationModel.Items.Add(jury.Educator);
            _juryPaginationModel.TotalItemCount++;
            StateHasChanged();
        }
        private async Task GetJuries()
        {
            _juryLoading = true;
            StateHasChanged();

            try
            {
                _juryPaginationModel = await EducatorService.GetPaginateList(_filterJury);
                if (_exitExam.Juries?.Count > 0)
                {
                    foreach (var item in _exitExam.Juries)
                    {

                        _juryPaginationModel.Items.Remove(_juryPaginationModel.Items.FirstOrDefault(x => x.Id == item.EducatorId));
                        _juryPaginationModel.TotalItemCount--;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Jüriler baþarýyla yüklendi.");
                _juryLoading = false;
                StateHasChanged();
            }
        }
        private async Task PaginationHandlerEducator(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterJury.page = item1;
            _filterJury.pageSize = item2;
            await GetJuries();
        }
        #region FilterJury
        private async Task OnSortChange(Sort sort)
        {
            _filterJury.Sort = new[] { sort };
            await GetJuries();
        }

        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filterJury.Filter ??= new Filter();
            _filterJury.Filter.Filters ??= new List<Filter>();
            _filterJury.Filter.Logic ??= "and";
            _filterJury.page = 1;
            var index = _filterJury.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filterJury.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value
                });
            }
            else
            {
                _filterJury.Filter.Filters[index].Value = value;
            }
            await GetJuries();
        }
        private async Task OnResetFilter(string filterName)
        {
            _filterJury.Filter ??= new Filter();
            _filterJury.Filter.Filters ??= new List<Filter>();
            _filterJury.Filter.Logic ??= "and";
            _filterJury.page = 1;
            var index = _filterJury.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filterJury.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetJuries();
            }
        }

        private bool IsFiltered(string filterName)
        {
            _filterJury.Filter ??= new Filter();
            _filterJury.Filter.Filters ??= new List<Filter>();
            _filterJury.Filter.Logic ??= "and";
            var index = _filterJury.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        #endregion
    }
}