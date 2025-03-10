using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.ResponseModels;
using UI.Services;
using System.Linq;
using Shared.FilterModels.Base;
using Shared.Types;
using Microsoft.JSInterop;
using UI.Models;
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.SharedComponents.Components;
using AutoMapper;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using UI.Helper;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;
using Radzen.Blazor;
using ApexCharts;
using Majorsoft.Blazor.Components.Common.JsInterop.Scroll;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs

{

    public partial class StudentRotation
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStudentRotationService StudentRotationService { get; set; }
        [Inject] public IRotationService RotationService { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IHospitalService HospitalService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IEducatorProgramService EducatorProgramService { get; set; }
        [Inject] public IEducationOfficerService EducationOfficerService { get; set; }
        [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] public IStudentRotationPerfectionService StudentRotationPerfectionService { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private List<CurriculumRotationResponseDTO> _rotations = new();
        private List<StudentRotationResponseDTO> _activeStudentRotations = new();
        private List<StudentRotationResponseDTO> _uncompletedStudentRotations = new();
        private List<RotationResponseDTO> _curriculumRotations;
        private RotationResponseDTO _rotation;
        private StudentRotationResponseDTO _studentRotationUpdateModel = new();
        private StudentRotationPerfectionResponseDTO _studentRotationPerfection = new();
        private bool _loadingRotations;
        private bool _loadingStudentRotations;
        private bool _saving;
        private List<BreadCrumbLink> _links;
        private bool Loaded => StudentDetailState.Value.StudentLoaded;
        private bool _loaded;
        private bool _srpDeleteSaving;
        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        private bool forceRender;
        private string _endDateMinLimitValidatorMessage;

        private MyModal _sendStudentRotationModal;
        private MyModal _uncompletedRotationSendModal;
        private MyModal _exemptStudentRotationModal;
        private MyModal _finishStudentRotationModal;
        private MyModal _studentRotationDetailModal;
        private MyModal _addPastRotationModal;
        private MyModal _studentRotationPerfectionDetailModal;
        private MyModal UploaderModal;
        private MyModal _fileModal;
        private StudentRotationResponseDTO _studentRotation = new();
        private BootstrapDatePicker _dp1;

        private DateTime? _beginDate;
        private DateTime? _endDate;
        private ProgramResponseDTO _updatedProgram;
        private EducatorResponseDTO _updatedEducator;
        private EducatorResponseDTO _srpEducator;
        private ProgramResponseDTO _newProgram;
        private ProgramResponseDTO _newProgramUpdated;
        private bool _isSuccessful;
        private EditContext _ec;
        private EditContext _ecStudentRotation;
        private EditContext _ecPastStudentRotation;
        private EditContext _ecStudentRotationPerfection;

        private List<StudentRotationResponseDTO> _formerStudentRotations = new();
        private List<StudentRotationResponseDTO> _completedStudentRotations = new();
        private Dropzone dropzone;
        private Dropzone dropzoneExemptRotation;
        private bool _fileLoaded = true;
        private bool _studentRotationLoaded = true;
        private List<DocumentResponseDTO> responseDocuments = new();
        RadzenDataGrid<StudentRotationResponseDTO> activeRotationGrid;
        RadzenDataGrid<StudentRotationResponseDTO> uncompletedRotationGrid;
        RadzenDataGrid<PerfectionResponseDTO> activestudentRotationPerfectionGrid;
        RadzenDataGrid<PerfectionResponseDTO> uncompletedStudentRotationPerfectionGrid;
        RadzenDataGrid<StudentRotationPerfectionResponseDTO> rotationPerfectionGrid;
        private string _processDateValidatorMessage = string.Empty;
        private string _documentValidatorMessage = string.Empty;
        private bool _loadingformerRotations;

        [Parameter] public long Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _studentRotation = new StudentRotationResponseDTO()
            {
                Rotation = new RotationResponseDTO() { ExpertiseBranch = new ExpertiseBranchResponseDTO() { } }
            };
            _studentRotationUpdateModel = new StudentRotationResponseDTO();
            _updatedEducator = new EducatorResponseDTO();
            _ec = new EditContext(_studentRotation);
            _ecStudentRotation = new EditContext(_studentRotationUpdateModel);
            _ecStudentRotation = new EditContext(_studentRotationPerfection);
            _ecPastStudentRotation = new EditContext(new StudentRotationResponseDTO());
            await LoadRotations();
            await LoadFormerRotations();
        }

        private async Task LoadRotations()
        {
            if (_student.CurriculumId != null)
            {
                _activeStudentRotations.Clear();
                _completedStudentRotations.Clear();
                try
                {
                    _loadingRotations = true;
                    StateHasChanged();
                    var response = await RotationService.GetListByStudentId((long)_student.Id);
                    var response_2 = await StudentRotationService.GetListByStudentId((long)_student.Id);
                    if (response.Result && response_2.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Rotation Information has been successfully loaded."]);
                        _rotations = response.Item;

                        foreach (var item in response_2.Item)
                        {
                            if (item.IsUncompleted == true)
                            {
                                _uncompletedStudentRotations.Add(item);
                            }
                            else if (item.IsSuccessful == null && item.IsUncompleted != true && item.ProcessDateForExemption == null)
                            {
                                _activeStudentRotations.Add(item);
                            }
                            else
                            {
                                _completedStudentRotations.Add(item);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during the loading of Rotation Information."]);
                }
                finally
                {
                    _loadingRotations = false;
                    StateHasChanged();
                }
            }
            else
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during the loading of Rotation Information."]);
                _rotations = new();
            }
            StateHasChanged();

        }
        private async Task LoadFormerRotations()
        {
            _formerStudentRotations.Clear();
            _loadingformerRotations = true;
            StateHasChanged();
            try
            {
                var response = await RotationService.GetFormerStudentsListByUserId((long)_student.UserId);
                if (response.Result && response.Item != null)
                {
                    foreach (var item in response.Item)
                    {
                        _formerStudentRotations.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error has occurred."]);
            }
            finally
            {
                _loadingformerRotations = false;
                StateHasChanged();

            }
        }

        private async Task OnDownloadHandler(StudentRotationResponseDTO studentRotation)
        {
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(studentRotation.Id.Value, DocumentTypes.StudentRotation);
                if (response.Result)
                {
                    responseDocuments = response.Item;
                    _fileModal.OpenModal();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            StateHasChanged();
        }

        private async Task OnUploadHandler(StudentRotationResponseDTO studentRotation)
        {
            dropzone.ResetStatus();
            _studentRotation = studentRotation;
            StateHasChanged();
            UploaderModal.OpenModal();
        }

        private void OnAddStudentRotationExemptModal(RotationResponseDTO rotation)
        {
            _studentRotation = new StudentRotationResponseDTO()
            {
                Rotation = rotation,
                RotationId = rotation.Id.Value,
                StudentId = _student.Id,
                Student = _student,
            };
            StateHasChanged();
            _exemptStudentRotationModal.OpenModal();
        }

        private void OnAddPastStudentRotationModal(RotationResponseDTO rotation)
        {
            _studentRotation = new StudentRotationResponseDTO()
            {
                Rotation = rotation,
                RotationId = rotation.Id.Value,
                StudentId = _student.Id,
                Student = _student,
            };
            _ecPastStudentRotation = new EditContext(_studentRotation);
            StateHasChanged();
            _addPastRotationModal.OpenModal();
        }
        private void OnAddStudentRotationModal(RotationResponseDTO rotation)
        {
            if (_uncompletedStudentRotations.Any(x => x.RotationId == rotation.Id))
            {
                _studentRotation = _uncompletedStudentRotations.FirstOrDefault(x => x.RotationId == rotation.Id);
                _studentRotation.BeginDate = null;
                _newProgram = _studentRotation.Program;
                _studentRotation.Program.Name.PrintJson("studentRotation.Program");
            }
            else
            {
                _studentRotation = new StudentRotationResponseDTO()
                {
                    Rotation = rotation,
                    RotationId = rotation.Id.Value,
                    StudentId = _student.Id,
                    Student = _student,
                };
                _newProgram = null;
            }

            _ec = new EditContext(_studentRotation);
            StateHasChanged();
            _sendStudentRotationModal.OpenModal();
        }

        private async Task AddExemptStudentRotation()
        {
            _processDateValidatorMessage = string.Empty;
            if (_studentRotation.ProcessDateForExemption == null) return;
            if (dropzoneExemptRotation == null || string.IsNullOrEmpty(dropzoneExemptRotation._selectedFileName))
            {
                _processDateValidatorMessage = "Bu alan zorunludur!";
                StateHasChanged();
                return;
            }
            responseDocuments = new();
            await CallDropzone(dropzoneExemptRotation);


            var dto = Mapper.Map<StudentRotationDTO>(_studentRotation);
            try
            {
                _saving = true;
                StateHasChanged();
                var response = await StudentRotationService.PostAsync(dto);
                if (response.Result)
                {
                    var educationTracking = new EducationTrackingDTO
                    {
                        StudentId = _studentRotation.StudentId,
                        ProcessDate = _studentRotation.ProcessDateForExemption,
                        StudentRotationId = response.Item.Id,
                        Description = "Önceki eğitimi çerçevesinde aldığı rotasyondan başarılı olmuştur. (" + _studentRotation.Rotation?.ExpertiseBranch?.Name + ")",
                        AdditionalDays = Convert.ToInt32(_studentRotation.Rotation.Duration),
                        ProcessType = ProcessType.TimeDecreasing,
                        ReasonType = ReasonType.ExemptionOfRotation

                    };
                    var ettResponse = await EducationTrackingService.Add(educationTracking);

                    foreach (var item in responseDocuments)
                    {
                        var documentDTO = Mapper.Map<DocumentDTO>(item);
                        documentDTO.EntityId = response.Item.Id.Value;
                        var result = await DocumentService.Update(item.Id, documentDTO);
                        if (!result.Result)
                            throw new Exception(result.Message);

                        item.Id = 0;
                        var ettdocumentDTO = Mapper.Map<DocumentDTO>(item);
                        ettdocumentDTO.EntityId = ettResponse.Item.Id.Value;
                        ettdocumentDTO.DocumentType = DocumentTypes.EducationTimeTracking;
                        var ettDocumentResponse = await DocumentService.Add(ettdocumentDTO);
                        if (!ettDocumentResponse.Result)
                            throw new Exception(ettDocumentResponse.Message);
                    }
                    _exemptStudentRotationModal.CloseModal();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An Error Occured."]);
            }
            finally
            {
                _saving = false;
                await LoadRotations();
                await LoadFormerRotations();
                dropzoneExemptRotation.DisposeAsync();
                responseDocuments.Clear();
            }
        }
        private async Task AddStudentRotation()
        {
            if (!_ec.Validate())
            {
                return;
            }
            if (dropzone != null && !String.IsNullOrEmpty(dropzone._selectedFileName))
            {
                responseDocuments = new();
                await CallDropzone(dropzone);
            }
            var dto = Mapper.Map<StudentRotationDTO>(_studentRotation);
            try
            {
                _saving = true;
                StateHasChanged();
                var response = await StudentRotationService.SendStudentToRotation(dto);
                if (response.Result)
                {
                    _newProgram = null;
                    foreach (var item in responseDocuments)
                    {
                        var documentDTO = Mapper.Map<DocumentDTO>(item);
                        documentDTO.EntityId = response.Item.Id.Value;
                        var result = await DocumentService.Update(item.Id, documentDTO);
                        if (!result.Result)
                            throw new Exception(result.Message);
                    }
                    _sendStudentRotationModal.CloseModal();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                    NavigationManager.NavigateTo($"/student/students");
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
                dropzone.DisposeAsync();
                responseDocuments.Clear();
            }
        }

        private async Task AddPastStudentRotation()
        {
            _ecPastStudentRotation.GetValidationMessages().PrintJson("");
            _documentValidatorMessage = string.Empty;
            if (!_ecPastStudentRotation.Validate())
            {
                return;
            }
            if (string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = L["This field is required"];
                StateHasChanged();
                return;
            }
            else
            {
                responseDocuments = new();
                await CallDropzone(dropzone);
            }
            var dto = Mapper.Map<StudentRotationDTO>(_studentRotation);
            try
            {
                var response = await StudentRotationService.AddPastRotation(dto);
                if (response.Result)
                {
                    _newProgram = null;
                    foreach (var item in responseDocuments)
                    {
                        var documentDTO = Mapper.Map<DocumentDTO>(item);
                        documentDTO.EntityId = response.Item.Id.Value;
                        var result = await DocumentService.Update(item.Id, documentDTO);
                        if (!result.Result)
                            throw new Exception(result.Message);
                    }
                    //_completedStudentRotations.Add(response.Item);
                    _addPastRotationModal.CloseModal();
                    LoadRotations();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
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
                dropzone.DisposeAsync();
                responseDocuments.Clear();
            }
        }

        //private async Task OnRemoveRotationHandler(RotationResponseDTO rotation)
        //{

        //    if (IsRotationChosen(rotation))
        //    {
        //        _saving = true;
        //        try
        //        {
        //            var studentRotation = await StudentRotationService.GetByStudentAndRotationIdAsync(_student.Id, rotation.Id.Value);
        //            await StudentRotationService.Delete(_student.Id, rotation.Id.Value);
        //            var educationTrackings = await EducationTrackingService.GetListByStudentId(_student.Id);
        //            var educationTracking = educationTrackings.Item.FirstOrDefault(x => x.StudentRotationId == studentRotation.Item.Id);
        //            if (educationTracking != null)
        //                await EducationTrackingService.Delete((long)educationTracking.Id);

        //            await LoadRotations();
        //        }
        //        catch (Exception)
        //        {
        //            SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during Rotation deletion."]);
        //        }
        //        finally
        //        {
        //            _saving = false;
        //            StateHasChanged();
        //        }
        //    }
        //}

        private async Task OnRemoveActiveStudentRotationHandler(StudentRotationResponseDTO studentRotation)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}", $"{L["Are you sure you want to delete this item? This action cannot be undone."]}", SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                _saving = true;
                try
                {
                    await StudentRotationService.DeleteActiveRotation((long)studentRotation.Id);
                    NavigationManager.NavigateTo("/student/students");
                }
                catch (Exception)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during Rotation deletion."]);
                }
                finally
                {
                    _saving = false;
                    StateHasChanged();
                }
            }
        }

        private async Task OnRemoveCompletedStudentRotationHandler(StudentRotationResponseDTO studentRotation)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}", $"{L["Are you sure you want to delete this item? This action cannot be undone."]}", SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                _saving = true;
                try
                {
                    await StudentRotationService.Delete((long)studentRotation.Id);
                    var educationTrackings = await EducationTrackingService.GetListByStudentId(_student.Id);
                    var educationTracking = educationTrackings.Item.FirstOrDefault(x => x.StudentRotationId == studentRotation.Id);
                    await EducationTrackingService.Delete((long)educationTracking.Id);

                }
                catch (Exception)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during Rotation deletion."]);
                }
                finally
                {
                    _saving = false;
                    await LoadRotations();
                    await LoadFormerRotations();
                    StateHasChanged();
                }
            }
        }

        private bool IsRotationChosen(RotationResponseDTO rotation)
        {
            return rotation.StudentRotations.Count > 0;
        }

        private bool IsSuccessful(RotationResponseDTO rotation)
        {
            return rotation.StudentRotations.Any(x => x.IsSuccessful == true);
        }

        private async Task<IEnumerable<EducatorResponseDTO>> SearchEducators(string searchQuery)
        {
            var result = await EducationOfficerService.GetListByProgramId((long)_studentRotation.ProgramId);
            return result.Result ? result.Item.Select(x => x.Educator).Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<EducatorResponseDTO>();
        }

        private async Task<IEnumerable<EducatorResponseDTO>> SearchUpdatedEducators(string searchQuery)
        {
            var result = await EducationOfficerService.GetListByProgramId(_updatedProgram.Id);
            if (result.Result && result.Item != null)
                _updatedEducator = result.Item.FirstOrDefault()?.Educator;
            return result.Result ? result.Item.Select(x => x.Educator).Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<EducatorResponseDTO>();
        }
        private async Task<IEnumerable<EducatorResponseDTO>> SearchSRPEducators(string searchQuery)
        {
            var result = await EducatorProgramService.GetListByProgramId((long)_activeStudentRotations.FirstOrDefault().ProgramId);
            return result.Result ? result.Item.Select(x => x.Educator).Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<EducatorResponseDTO>();
        }

        private void OnChangeEducator(EducatorResponseDTO educator)
        {
            _studentRotation.Educator = educator;
            _studentRotation.EducatorId = educator.Id;
        }

        private void OnChangeSRPEducator(EducatorResponseDTO educator)
        {
            _studentRotationPerfection.Educator = educator;
            _studentRotationPerfection.EducatorId = educator.Id;
        }

        private void OnChangeUpdatedEducator(EducatorResponseDTO educator)
        {
            _updatedEducator = educator;
            _studentRotationUpdateModel.EducatorId = _updatedEducator?.Id;
        }

        //private async Task OnStudentRotationDetail(RotationResponseDTO rotation)
        //{

        //    if (rotation.Id != 0)
        //    {
        //        var response = await StudentRotationService.GetByStudentAndRotationIdAsync(_student.Id, rotation.Id.Value);

        //        if (response.Result && response.Item != null)
        //        {
        //            _studentRotationUpdateModel = response.Item;
        //            _updatedEducator = _studentRotationUpdateModel?.Educator;
        //            _updatedProgram = _studentRotationUpdateModel?.Program;
        //            _ecStudentRotation = new EditContext(_studentRotationUpdateModel);
        //            _loaded = true;
        //            StateHasChanged();
        //            _studentRotationDetailModal.OpenModal();
        //        }

        //    }
        //}

        private async Task OnStudentRotationDetail(StudentRotationResponseDTO studentRotation)
        {
            var response = await StudentRotationService.GetByIdAsync((long)studentRotation.Id);

            if (response.Result && response.Item != null)
            {
                _studentRotationUpdateModel = response.Item;

                if (_studentRotationUpdateModel.EducatorId != null)
                {
                    _updatedEducator = _studentRotationUpdateModel?.Educator;
                }
                else
                {
                    try
                    {
                        var result = await EducationOfficerService.GetListByProgramId(studentRotation.ProgramId.Value);
                        if (result.Result && result.Item != null)
                        {
                            _updatedEducator = result.Item.FirstOrDefault().Educator;
                            _studentRotationUpdateModel.EducatorId = _updatedEducator.Id;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        SweetAlert.ErrorAlert(L["An error occured"]);
                    }
                }
                _updatedProgram = _studentRotationUpdateModel?.Program;
                _ecStudentRotation = new EditContext(_studentRotationUpdateModel);
                _loaded = true;
                StateHasChanged();
                _studentRotationDetailModal.OpenModal();
            }
        }

        private void OnStudentRotationPerfectionAdd(PerfectionResponseDTO perfection, StudentRotationResponseDTO studentRotation)
        {
            _studentRotationPerfection = new()
            {
                StudentRotation = studentRotation,
                StudentRotationId = studentRotation.Id,
                Perfection = perfection,
                PerfectionId = perfection.Id
            };

            _ecStudentRotationPerfection = new EditContext(_studentRotationPerfection);
            _studentRotationPerfectionDetailModal.OpenModal();
        }
        private async Task OnStudentRotationPerfectionDetail(StudentRotationPerfectionResponseDTO studentRotationPerfection)
        {
            _studentRotationPerfection = studentRotationPerfection;
            _ecStudentRotationPerfection = new EditContext(_studentRotationPerfection);
            _studentRotationPerfectionDetailModal.OpenModal();

        }
        private async Task OnRemoveActiveStudentRotationPerfectionHandler(StudentRotationPerfectionResponseDTO studentRotationPerfection)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}", $"{L["Are you sure you want to delete this item? This action cannot be undone."]}", SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                _srpDeleteSaving = true;
                StateHasChanged();
                try
                {
                    await StudentRotationPerfectionService.Delete(studentRotationPerfection.Id.Value);
                    _activeStudentRotations.FirstOrDefault(x => x.Id == studentRotationPerfection.StudentRotationId)?.StudentRotationPerfections?.Remove(studentRotationPerfection);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L[e.Message]);
                }
                finally { _srpDeleteSaving = false; StateHasChanged(); }
            }
        }
        private async Task UpdateStudentRotationPerfection()
        {

            var dto = Mapper.Map<StudentRotationPerfectionDTO>(_studentRotationPerfection);
            dto.EducatorId = _studentRotationPerfection.Educator.Id;
            try
            {
                var response = await StudentRotationPerfectionService.Update(_studentRotationPerfection.Id.Value, dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                    _studentRotationPerfectionDetailModal.CloseModal();
                }
                else
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An error occured"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ToastAlert(SweetAlertIcon.error, L[e.Message]);
            }
        }
        private async Task SaveStudentRotationPerfection()
        {
            _studentRotationPerfection.EducatorId = _studentRotationPerfection.Educator?.Id;
            if (!_ecStudentRotationPerfection.Validate()) return;
            var dto = Mapper.Map<StudentRotationPerfectionDTO>(_studentRotationPerfection);
            try
            {
                var response = await StudentRotationPerfectionService.Add(dto);
                if (response.Result)
                {
                    _studentRotationPerfection.Id = response.Item.Id;
                    _activeStudentRotations.FirstOrDefault(x => x.Id == _studentRotationPerfection.StudentRotationId)?.StudentRotationPerfections?.Add(_studentRotationPerfection);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An error occured"]);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L[e.Message]);
            }
            finally
            {
                _studentRotationPerfectionDetailModal.CloseModal();
            }
        }
        private async Task FinishStudentRotation(bool isUncompleted)
        {
            _endDateMinLimitValidatorMessage = null;
            if (!_ecStudentRotation.Validate()) return;

            //var minDate = _studentRotationUpdateModel.BeginDate?.AddDays(Convert.ToInt32(_studentRotationUpdateModel.Rotation.Duration) - 1);

            //if (_studentRotationUpdateModel.EndDate < minDate)
            //{
            //    _endDateMinLimitValidatorMessage = L["End Date Must Be Greater Than {0}", minDate?.ToShortDateString()];
            //    return;
            //}
            _saving = true;
            StateHasChanged();
            if (dropzone != null && !String.IsNullOrEmpty(dropzone._selectedFileName))
            {
                await CallDropzone(dropzone);
            }
            _studentRotationUpdateModel.ProgramId = _updatedProgram.Id;

            if (isUncompleted)
                _studentRotationUpdateModel.IsUncompleted = true;
            else
            {
                _studentRotationUpdateModel.IsSuccessful = _studentRotationUpdateModel?.StudentRotationPerfections?.Any(x => x.IsSuccessful == false) == true ? false : true;
            }
            var dto = Mapper.Map<StudentRotationDTO>(_studentRotationUpdateModel);
            try
            {
                var response = await StudentRotationService.FinishStudentsRotation((long)_studentRotationUpdateModel.Id, dto);
                if (response.Result)
                {
                    //var educationTrackings = await EducationTrackingService.GetListByStudentId(_student.Id);
                    //var educationTracking = educationTrackings.Item.FirstOrDefault(x => x.StudentRotationId == _studentRotationUpdateModel.Id);

                    //var newEducationTracking = new EducationTrackingDTO
                    //{
                    //    StudentId = _studentRotationUpdateModel.StudentId,
                    //    ProgramId = educationTracking.FormerProgramId,
                    //    FormerProgramId = _studentRotationUpdateModel.ProgramId,
                    //    ProcessDate = _studentRotationUpdateModel.EndDate,
                    //    Description = _studentRotationUpdateModel.Rotation.ExpertiseBranch.Name,
                    //    AdditionalDays = Convert.ToInt32(_studentRotationUpdateModel.Rotation.Duration),
                    //    StudentRotationId = _studentRotationUpdateModel.Id,
                    //    RelatedEducationTrackingId = educationTracking.Id
                    //};

                    //if (dto.IsSuccessful == true)
                    //{
                    //    newEducationTracking.ProcessType = ProcessType.Information;
                    //    newEducationTracking.ReasonType = ReasonType.CompletionOfRotation;
                    //}
                    //else
                    //{
                    //    newEducationTracking.ProcessType = ProcessType.TimeIncreasing;
                    //    newEducationTracking.ReasonType = ReasonType.TimeExtensionDueToFailureInRotation;
                    //}
                    //var addedEducationTracking = await EducationTrackingService.Add(newEducationTracking);
                    //educationTracking.RelatedEducationTrackingId = addedEducationTracking.Item.Id;
                    //await EducationTrackingService.Update((long)educationTracking.Id, Mapper.Map<EducationTrackingDTO>(educationTracking));

                    //_student.ProgramId = educationTracking.FormerProgramId;
                    //_student.Status = StudentStatus.EducationContinues;
                    //await StudentService.Update(_student.Id, Mapper.Map<StudentDTO>(_student));

                    _studentRotationDetailModal.CloseModal();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
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

        private void OnChangeBeginDate(DateTime? e)
        {
            _studentRotation.BeginDate = e;
        }

        private void OnChangeUpdateBeginDate(DateTime? e)
        {
            _studentRotationUpdateModel.BeginDate = e;
            _studentRotationUpdateModel.EndDate = _studentRotationUpdateModel.BeginDate?.AddDays(Convert.ToInt32(_studentRotationUpdateModel.Rotation.Duration));
        }
        public async Task<bool> CallDropzone(Dropzone dropzone)
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

        private async Task<IEnumerable<ProgramResponseDTO>> SearchPrograms(string searchQuery)
        {
            string[] searchParams = searchQuery.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            List<Filter> filterList = new List<Filter>();

            foreach (var item in searchParams)
            {
                var filter0 = new Filter()
                {
                    Field = "Faculty.University.Name",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter0);
                var filter1 = new Filter()
                {
                    Field = "Hospital.Province.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter1);
                var filter2 = new Filter()
                {
                    Field = "ExpertiseBranch.Profession.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter2);
                var filter3 = new Filter()
                {
                    Field = "Hospital.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter3);
                var filter4 = new Filter()
                {
                    Field = "ExpertiseBranch.Name",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter4);
            }
            PaginationModel<ProgramResponseDTO> result = new();

            filterList.Add(new() { Field = "GetActives" });

            result = await ProgramService.GetListForSearchByExpertiseBranchId(new FilterDTO()
            {
                Filter = new Filter()
                {
                    Logic = "or",
                    Filters = filterList
                },
                page = 1,
                pageSize = int.MaxValue
            }, _studentRotation.Rotation.ExpertiseBranchId.Value, true);

            return result.Items ?? new List<ProgramResponseDTO>();
        }
        private string GetProgramClass()
        {
            return "form-control " + _newProgram is not null ? "invalid" : "";
        }
        private void OnChangeProgram(ProgramResponseDTO program)
        {
            _newProgram = program;
            _studentRotation.ProgramId = program.Id;
            _studentRotation.Educator = null;
            _studentRotation.EducatorId = null;

            StateHasChanged();
        }

        private void OnChangePastRotationProgram(ProgramResponseDTO program)
        {
            _studentRotation.Program = program;
            _studentRotation.ProgramId = program.Id;

            StateHasChanged();
        }

        private void OnChangeProgramUpdated(ProgramResponseDTO program)
        {
            _updatedProgram = program;
            _newProgramUpdated = program;
            _studentRotationUpdateModel.ProgramId = program.Id;
            _studentRotationUpdateModel.Educator = null;
            _studentRotationUpdateModel.EducatorId = null;
            _updatedEducator = null;

            StateHasChanged();
        }

        private async Task<IEnumerable<ProgramResponseDTO>> SearchProgramsUpdated(string searchQuery)
        {
            string[] searchParams = searchQuery.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            List<Filter> filterList = new List<Filter>();

            foreach (var item in searchParams)
            {
                var filter0 = new Filter()
                {
                    Field = "Faculty.University.Name",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter0);
                var filter1 = new Filter()
                {
                    Field = "Hospital.Province.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter1);
                var filter2 = new Filter()
                {
                    Field = "ExpertiseBranch.Profession.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter2);
                var filter3 = new Filter()
                {
                    Field = "Hospital.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter3);
                var filter4 = new Filter()
                {
                    Field = "ExpertiseBranch.Name",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter4);
            }
            PaginationModel<ProgramResponseDTO> result = new();

            filterList.Add(new() { Field = "GetActives" });

            result = await ProgramService.GetListForSearchByExpertiseBranchId(new FilterDTO()
            {
                Filter = new Filter()
                {
                    Logic = "or",
                    Filters = filterList
                },
                page = 1,
                pageSize = int.MaxValue
            }, _studentRotationUpdateModel.Rotation.ExpertiseBranchId.Value);

            return result.Items ?? new List<ProgramResponseDTO>();
        }
    }
}