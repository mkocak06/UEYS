using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.Extensions;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs
{
    public partial class EducationTimeTracking
    {
        [Inject] public IState<ArchiveStudentDetailState> ArchiveStudentDetailState { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public IStudentRotationService StudentRotationService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private Guid WrapperId { get; set; }
        private List<TimeLineModel> dataModel;
        private MyModal _educationTrackingAddModal;
        private MyModal _educationTrackingUpdateModal;
        private MyModal UploaderModal;
        private MyModal FileModal;

        private EditContext _ec;
        private EditContext _ecUpdate;

        private EducationTrackingResponseDTO _educationTracking;
        private EducationTrackingResponseDTO _educationTrackingForUpdate;
        private EducationTrackingResponseDTO _eTOldForUpdate;
        //private List<ReasonResponseDTO> _reasons = new();
        private Dropzone dropzone;
        private StudentResponseDTO SelectedStudent => ArchiveStudentDetailState.Value.Student;

        private PaginationModel<EducationTrackingResponseDTO> _paginationModel;
        private bool _loading;
        private List<EducationTrackingResponseDTO> _educationTrackings;
        private List<DocumentResponseDTO> responseDocuments = new();
        private List<DocumentResponseDTO> Documents = new();
        private BootstrapDatePicker _dp1;
        private Flatpickr _fp1;
        private bool _saving;
        private ProgramResponseDTO _newProgram;
        private ProgramResponseDTO _selectedProgram;
        private ProgramResponseDTO _ownProgram;
        private CurriculumResponseDTO _latestCurriculum = new();
        private long? expertiseBranchId;
        private string _validationMessage;
        private ReasonType? _reason;
        private DateTime? _minDate = DateTime.MinValue;
        private DateTime? _maxDate = DateTime.MaxValue;
        private string _curriculumMessage;
        private bool _fileLoaded = true;
        private string _documentValidatorMessage;
        private DateTime? _assignmentDateLimit;
        private int _newAdditionalDayForLeavingInstitution;
        private EducationTrackingResponseDTO _leavingInstitutionRecord;
        private DateTime? _newDateOfRelatedRecord;
        private DateTime? _minDateLimit;
        private DateTime? _maxDateLimit;

        protected override async Task OnInitializedAsync()
        {
            _educationTracking = new();
            _ec = new EditContext(_educationTracking);

            _educationTrackingForUpdate = new();
            _ecUpdate = new EditContext(_educationTrackingForUpdate);

            dataModel = new List<TimeLineModel>();

            WrapperId = Guid.NewGuid();

            await GetEducatorTrackings();
            await GetTimeLine();

            await base.OnInitializedAsync();
        }
        private void AddingList()
        {
            _educationTracking = new();
            _ec = new EditContext(_educationTracking);
            responseDocuments = new();
            StateHasChanged();
            _educationTrackingAddModal.OpenModal();
        }
        public async Task<bool> CallDropzone()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var response = await dropzone.SubmitFileAsync();
                if (response.Result)
                {

                    responseDocuments.Add(response.Item);

                    _fileLoaded = true;
                    StateHasChanged();
                    //UploaderModal.CloseModal();
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
        private async Task OnDownloadHandler(EducationTrackingResponseDTO educationTracking)
        {
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(educationTracking.Id.Value, DocumentTypes.EducationTimeTracking);
                if (response.Result)
                {
                    Documents = response.Item;
                    FileModal.OpenModal();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            StateHasChanged();
        }
        private async Task<IEnumerable<ProgramResponseDTO>> SearchPrograms(string searchQuery)
        {
            string[] searchParams = searchQuery.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            List<Filter> filterList = new List<Filter>();

            foreach (var item in searchParams)
            {
                var filter0 = new Filter()
                {
                    Field = "UniversityName",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter0);
                var filter1 = new Filter()
                {
                    Field = "ProvinceName",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter1);
                var filter2 = new Filter()
                {
                    Field = "ProfessionName",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter2);
                var filter3 = new Filter()
                {
                    Field = "HospitalName",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter3);
                var filter4 = new Filter()
                {
                    Field = "ExpertiseBranchName",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter4);
            }
            PaginationModel<ProgramResponseDTO> result = new();

            if (_educationTracking.ReasonType == ReasonType.BranchChange_End || _educationTracking.ProcessType == ProcessType.Assignment)
            {
                result = await ProgramService.GetListForSearch(new FilterDTO()
                {
                    Filter = new Filter()
                    {
                        Logic = "or",
                        Filters = filterList
                    },
                    page = 1,
                    pageSize = int.MaxValue
                });
            }
            else
            {
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
                }, _selectedProgram.ExpertiseBranchId.Value);
            }

            return result.Items ?? new List<ProgramResponseDTO>();
        }
        private void OnChangeProgram(ProgramResponseDTO program)
        {
            _newProgram = program;
            _newProgram.Id = program.Id;

            StateHasChanged();
        }
        private string GetProgramClass()
        {
            return "form-control " + _newProgram is not null ? "invalid" : "";
        }


        private async Task OnChangeSelect(ProcessType? e)
        {
            if (e == null)
            {
                _educationTracking.ProcessType = null;
            }
            _educationTracking.ProcessType = e;

            _educationTracking.ProcessDate = null;
            _educationTracking.ReasonType = null;
            _dp1.ClearDate();
            _newProgram = null;
            if (e == ProcessType.TimeIncreasing || e == ProcessType.TimeDecreasing)
            {
                _minDate = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.Start).ProcessDate;
                _maxDate = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish)?.ProcessDate
                    ?? _educationTrackings.OrderByDescending(x => x.ProcessDate).FirstOrDefault(x => x.ProcessType == ProcessType.Finish)?.ProcessDate;
            }
            else
            {
                _minDate = DateTime.MinValue;
                _maxDate = DateTime.MaxValue;
            }
            await _dp1.RunRender(_minDate, _maxDate);
            StateHasChanged();
        }

        private void OnSelectCompletionOfAssignment()
        {
            if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
            {
                var beginningToAssignedInstitution = _educationTrackings.OrderBy(x => x.ProcessDate).LastOrDefault(x => x.ReasonType == ReasonType.BeginningToAssignedInstitution);
                var relatedEducationTracking = _educationTrackings.FirstOrDefault(x => x.Id == beginningToAssignedInstitution.RelatedEducationTrackingId);
                _assignmentDateLimit = beginningToAssignedInstitution.ProcessDate?.AddDays((int)relatedEducationTracking.AdditionalDays);
            }
            else if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad)
            {
                var leavingEducationTracking = _educationTrackings.OrderBy(x => x.ProcessDate).LastOrDefault(x => x.AssignmentType == AssignmentType.EducationAbroad);
                _assignmentDateLimit = leavingEducationTracking.ProcessDate?.AddDays((int)leavingEducationTracking.AdditionalDays);
            }

        }
        private async Task OnChangeProcessDate(DateTime? e)
        {
            if (e == null)
            {
                _educationTracking.ProcessDate = null;
            }
            else
            {

                _educationTracking.ProcessDate = e;
                StateHasChanged();
                if (_educationTracking.ProcessDate != null && _newProgram.Id > 0 && _newProgram.ExpertiseBranch.Id > 0)
                {
                    try
                    {

                        var response = await CurriculumService.GetLatestByBeginningDateAndExpertiseBranchId(_newProgram.ExpertiseBranch.Id.Value, _educationTracking.ProcessDate.Value);
                        if (response.Result)
                        {
                            _latestCurriculum = response.Item;
                            if (_latestCurriculum == null)
                            {
                                _curriculumMessage = "Bu branþa ait müfredat bulunamadý. Lütfen baþka bir branþ seçiniz.";
                            }
                            StateHasChanged();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
        private async Task GetTimeLine()
        {
            if (_educationTrackings?.Count > 0)
            {
                foreach (var item in _educationTrackings)
                {
                    if (item.ProcessDate != null)
                    {
                        dataModel.Add(new TimeLineModel()
                        {
                            Id = item.Id.Value,
                            Content = "<strong><u>" + L[item.ProcessType?.GetDescription()] + "</u></strong>" + (item.ProcessType.Value != ProcessType.EstimatedFinish ? "<br>" + L[item.ReasonType?.GetDescription()] + "<br>" : "") + (item.AdditionalDays != null ? "(" + item.AdditionalDays + " " + L["Day"] + ")" : ""),
                            style = "background:" + item.ProcessType.Value.GetProcessColorForTimeline(),
                            Title = item.ProcessDate?.ToString("dd MMMM yyyy"),
                            Start = item.ProcessDate?.ToString("yyyy-MM-dd")

                        });
                    }
                }
            }
            await JSRuntime.InvokeVoidAsync("timeLine", WrapperId, (object)dataModel);
        }
        private async Task GetEducatorTrackings()
        {
            try
            {
                var response = await EducationTrackingService.GetListByStudentId(SelectedStudent.Id);
                if (response.Result && response.Item != null)
                {
                    _educationTrackings = response.Item.OrderBy(x => x.ProcessDate?.Date).ThenBy(x => x.Id).ToList();

                    var program = await ProgramService.GetProgramByStudentId(SelectedStudent.Id);
                    _selectedProgram = program.Item;
                    StateHasChanged();
                }
                else
                {
                    _loading = true;
                    SweetAlert.ErrorAlert("Hiçbir kayýt bulunamadý");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SweetAlert.ErrorAlert();
            }
        }
        private async Task AddEducationTracking()
        {
            _documentValidatorMessage = string.Empty;
            if (!_ec.Validate()) return;

            if ((_educationTracking.ProcessType == ProcessType.TimeIncreasing || _educationTracking.ProcessType == ProcessType.TimeDecreasing) && string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = "Bu alan zorunludur!";
                StateHasChanged();
                return;
            }
            else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                var response = await CallDropzone();
                if (!response) return;
            }

            if (_newProgram != null)
            {
                if (!(_newProgram.Id > 0) && (_educationTracking.ReasonType == ReasonType.UnexcusedTransfer || _educationTracking.ReasonType == ReasonType.ExcusedTransfer || _educationTracking.ReasonType == ReasonType.BranchChange_End || (_educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment && _educationTracking.AssignmentType != AssignmentType.EducationAbroad)))
                {
                    _validationMessage = "Bu alan zorunludur!";
                    StateHasChanged();
                    return;
                }
                if (_latestCurriculum == null)
                {
                    return;
                }
            }

            if (_educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment)
            {
                var pastDays = _educationTrackings.Where(x => x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment).Sum(x => x.AdditionalDays);
                if (pastDays + _educationTracking.AdditionalDays > 360)
                {
                    var confirm = await SweetAlert.ConfirmAlert($"{L["Warning"]}",
                   $"{L["Time of assignment cannot exceed 360 days!"]} \n {L["Time of Assignment So Far"]} : {pastDays} {L["Day"]}",
                   SweetAlertIcon.error, false, $"{L["Okey"]}", $"{L["Cancel"]}");
                    return;
                }
                var estimatedFinish = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish);
                if (estimatedFinish.ProcessDate < _educationTracking.ProcessDate?.AddDays((int)_educationTracking.AdditionalDays))
                {
                    var confirm = await SweetAlert.ConfirmAlert($"{L["Warning"]}",
                   $"{L["Time of assignment, cannot exceed the estimated finish!"]} {L["Estimated Finish of Education"]} : {estimatedFinish.ProcessDate?.ToShortDateString()}",
                   SweetAlertIcon.error, false, $"{L["Okey"]}", $"{L["Cancel"]}");
                    return;
                }
            }
            _saving = true;
            StateHasChanged();

            if (_educationTracking.ProcessType != ProcessType.TimeIncreasing && _educationTracking.ProcessType != ProcessType.TimeDecreasing && _educationTracking.ReasonType != ReasonType.LeavingTheInstitutionDueToAssignment)
            {
                _educationTracking.AdditionalDays = null;
            }

            // EÐÝTÝM BÝTÝYORSA TAHMÝNÝ BÝTÝÞ SÝLÝNÝR
            if (_educationTracking.ProcessType == ProcessType.Finish && _educationTracking.ReasonType != ReasonType.KKTCHalfTimed)
            {
                var estfinish = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish);
                if (estfinish != null)
                {
                    await EducationTrackingService.Delete(estfinish.Id.Value);
                    _educationTrackings.Remove(estfinish);
                    SelectedStudent.IsDeleted = true;
                    SelectedStudent.DeleteReason = L[_educationTracking.ReasonType?.GetDescription()];
                    await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                }
            }
            _educationTracking.ProgramId = SelectedStudent.ProgramId;
            // NAKÝL VEYA GÖREVLENDÝRME ÝÞLEMLERÝ
            if (_educationTracking.ReasonType == ReasonType.UnexcusedTransfer || _educationTracking.ReasonType == ReasonType.ExcusedTransfer || (_educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment && _educationTracking.AssignmentType != AssignmentType.EducationAbroad) || _educationTracking.ReasonType == ReasonType.CompletionOfAssignment || _educationTracking.ReasonType == ReasonType.KKTCHalfTimed)
            {
                if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                {
                    SelectedStudent.ProgramId = _newProgram.Id;
                    _educationTracking.ProgramId = _newProgram.Id;
                    SelectedStudent.Status = StudentStatus.EducationContinues;
                    var formerEduTr = _educationTrackings.LastOrDefault(x => x.ProgramId == _selectedProgram.Id && x.ProcessDate < _educationTracking.ProcessDate);
                    _leavingInstitutionRecord = _educationTrackings.FirstOrDefault(x => x.RelatedEducationTrackingId == formerEduTr.Id);
                    _leavingInstitutionRecord.AdditionalDays = (int)(_educationTracking.ProcessDate - formerEduTr.ProcessDate)?.Days;
                }
                else
                {
                    if (_educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment)
                        SelectedStudent.Status = StudentStatus.Assigned;
                    if (_educationTracking.ProcessType == ProcessType.Transfer && SelectedStudent.Status == StudentStatus.TransferDueToNegativeOpinion)
                    {
                        SelectedStudent.Status = StudentStatus.EducationContinues;
                        SelectedStudent.TransferredDueToOpinion = true;
                    }
                    if (_educationTracking.ProcessType == ProcessType.Transfer)
                    {
                        SelectedStudent.OriginalProgramId = _newProgram.Id;
                    }
                    SelectedStudent.ProgramId = _newProgram.Id;
                    _educationTracking.ProgramId = _newProgram.Id;
                }

                await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));

                _educationTracking.FormerProgramId = _selectedProgram.Id;

                EducationTrackingDTO newEtt = new()
                {
                    StudentId = SelectedStudent.Id,
                    ProcessType = ProcessType.Start,
                    ReasonType = _educationTracking.ReasonType,
                    ExcusedType = _educationTracking.ExcusedType,
                    FormerProgramId = _selectedProgram.Id,
                    AssignmentType = _educationTracking.AssignmentType,
                    ProgramId = _educationTracking.ReasonType == ReasonType.CompletionOfAssignment ? _ownProgram.Id : _newProgram.Id
                };

                if (_educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment)
                    newEtt.ReasonType = ReasonType.BeginningToAssignedInstitution;

                if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                    newEtt.ReasonType = ReasonType.BeginningToOwnInstitutionAfterAssignment;

                try
                {
                    newEtt.ProcessDate = newEtt?.ProcessDate?.Date;
                    var response = await EducationTrackingService.Add(newEtt);
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Nakil Bilgisi Eklendi"]}");
                        _educationTrackings.Add(response.Item);
                        _educationTracking.RelatedEducationTrackingId = response.Item.Id;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // YURT DIÞI
            if (_educationTracking.AssignmentType == AssignmentType.EducationAbroad)
            {

                SelectedStudent.Status = StudentStatus.Abroad;
                await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
            }
            if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad)
            {
                _educationTracking.RelatedEducationTrackingId = _educationTrackings.OrderByDescending(x => x.ProcessDate).FirstOrDefault(x => x.AssignmentType == AssignmentType.EducationAbroad).Id;
                SelectedStudent.Status = StudentStatus.EducationContinues;
                await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
            }

            //JS - TÝMELÝNE
            if (_educationTracking.ProcessType == ProcessType.TimeIncreasing && _educationTrackings.Any(x => x.ProcessType == ProcessType.EstimatedFinish))
            {
                var addedDay = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(_educationTracking.AdditionalDays.Value);
                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Start = addedDay.ToString("yyyy-MM-dd");
                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Title = addedDay.ToString("dd MMMM yyyy");
            }
            else if (_educationTracking.ProcessType == ProcessType.TimeDecreasing && _educationTrackings.Any(x => x.ProcessType == ProcessType.EstimatedFinish))
            {
                var diffDay = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(-_educationTracking.AdditionalDays.Value);
                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Start = diffDay.ToString("yyyy-MM-dd");
                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Title = diffDay.ToString("dd MMMM yyyy");
            }

            //BRANÞ DEÐÝÞÝKLÝÐÝ
            if (_educationTracking.ProcessType == ProcessType.Transfer && _educationTracking.ReasonType == ReasonType.BranchChange_End)
            {
                //TAHMÝNÝ BÝTÝÞ SÝLÝNÝR
                var estfinish = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish);
                if (estfinish != null && _educationTracking.ReasonType != ReasonType.BranchChange_End)
                {
                    await EducationTrackingService.Delete(estfinish.Id.Value);
                    _educationTrackings.Remove(estfinish);
                    dataModel.Remove(dataModel.FirstOrDefault(x => x.Id == estfinish.Id));
                    await JSRuntime.InvokeVoidAsync("redrawTimeline", WrapperId, (object)dataModel);
                }
                // ÖÐRENCÝNÝN PROGRAMI DEÐÝÞÝR
                StudentResponseDTO newStudent = SelectedStudent.Clone();
                SelectedStudent.IsDeleted = true;
                SelectedStudent.DeleteReason = L[ReasonType.BranchChange_End.GetDescription()];
                try
                {
                    await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                _educationTracking.StudentId = SelectedStudent.Id;
                var dto = Mapper.Map<EducationTrackingDTO>(_educationTracking);
                try
                {
                    dto.ProcessDate = dto.ProcessDate?.Date;
                    var response = await EducationTrackingService.Add(dto);
                    if (response.Result)
                    {
                        foreach (var item in responseDocuments)
                        {
                            var documentDTO = Mapper.Map<DocumentDTO>(item);
                            documentDTO.EntityId = (long)response.Item.Id;
                            var result = await DocumentService.Update(item.Id, documentDTO);
                            if (!result.Result)
                            {
                                throw new Exception(result.Message);
                            }
                        }
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                newStudent.EducationTrackings = new()
                {
                    new EducationTrackingResponseDTO
                    {
                        ProcessDate = _educationTracking.ProcessDate,
                        ProcessType = ProcessType.Start,
                        ReasonType = ReasonType.BranchChange,
                        FormerProgramId = _selectedProgram.Id,
                        Description = _educationTracking.Description,
                    }
                };

                var studentDTO = Mapper.Map<StudentDTO>(newStudent);

                studentDTO.CurriculumId = _latestCurriculum.Id;
                studentDTO.ProgramId = _newProgram.Id;
                studentDTO.OriginalProgramId = _newProgram.Id;

                try
                {
                    var response = await StudentService.Add(studentDTO);
                    if (response.Result)
                    {
                        _educationTrackingAddModal.CloseModal();
                        NavigationManager.NavigateTo($"./student/students/{response.Item.Id}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                {
                    var response = await CallDropzone();
                    if (!response) return;

                }
                _educationTracking.StudentId = SelectedStudent.Id;
                var dto = Mapper.Map<EducationTrackingDTO>(_educationTracking);
                try
                {
                    dto.ProcessDate = dto?.ProcessDate?.Date;
                    var response = await EducationTrackingService.Add(dto);
                    if (response.Result)
                    {
                        if (_educationTracking.ReasonType == ReasonType.EndOfEducationDuetoNegativeOpinion)
                        {
                            SelectedStudent.Status = StudentStatus.EducationEnded;
                            await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                        }
                        if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
                        {
                            var dto_1 = Mapper.Map<EducationTrackingDTO>(_leavingInstitutionRecord);
                            dto_1.ProcessDate = dto_1.ProcessDate?.Date;
                            await EducationTrackingService.Update((long)dto_1.Id, dto_1);
                        }
                        foreach (var item in responseDocuments)
                        {
                            var documentDTO = Mapper.Map<DocumentDTO>(item);
                            documentDTO.EntityId = (long)response.Item.Id;
                            var result = await DocumentService.Update(item.Id, documentDTO);
                            if (!result.Result)
                            {
                                throw new Exception(result.Message);
                            }
                        }
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");

                        dataModel.Add(new TimeLineModel()
                        {
                            Id = response.Item.Id.Value,
                            Content = "<strong><u>" + L[_educationTracking.ProcessType.Value.GetDescription()] + "</u></strong><br>" + L[_educationTracking.ReasonType.Value.GetDescription()] + "<br>" + (_educationTracking.AdditionalDays != null ? "(" + _educationTracking.AdditionalDays + L["Day"] : ""),
                            style = "background:" + _educationTracking.ProcessType.Value.GetProcessColorForTimeline(),
                            Title = _educationTracking.ProcessDate.Value.ToString("dd MMMM yyyy"),
                            Start = _educationTracking.ProcessDate.Value.ToString("yyyy-MM-dd")
                        });
                        await JSRuntime.InvokeVoidAsync("redrawTimeline", WrapperId, (object)dataModel);
                        await GetEducatorTrackings();
                        _educationTrackingAddModal.CloseModal();
                        StateHasChanged();
                        if (_educationTracking.ProcessType == ProcessType.Finish || _educationTracking.AssignmentType == AssignmentType.EducationAbroad || _educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad || _educationTracking.ReasonType == ReasonType.EndOfEducationDuetoNegativeOpinion || (SelectedStudent.TransferredDueToOpinion == true && (_educationTracking.ReasonType == ReasonType.UnexcusedTransfer || _educationTracking.ReasonType == ReasonType.ExcusedTransfer)))
                        {
                            NavigationManager.NavigateTo("/student/students");
                        }
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

                _dp1.ClearDate();
                _educationTracking = new();
            }
        }
        private void OnOpenUpdateModal(EducationTrackingResponseDTO educationTracking)
        {
            _minDateLimit = null;
            _maxDateLimit = null;
            _educationTrackingForUpdate = educationTracking;
            _eTOldForUpdate = educationTracking.Clone();
            _ecUpdate = new EditContext(_educationTrackingForUpdate);
            _educationTrackingUpdateModal.OpenModal();

            if (educationTracking.RelatedEducationTrackingId != null)
            {
                var relatedTracking = _educationTrackings.FirstOrDefault(x => x.Id == educationTracking.RelatedEducationTrackingId);

                if (educationTracking.ProcessType == ProcessType.Start)
                    _minDateLimit = relatedTracking.ProcessDate;
                else
                    _maxDateLimit = relatedTracking?.ProcessDate;
            }
            //todo - min max date not working
        }
        private async Task UpdateEducationTracking()
        {
            _documentValidatorMessage = string.Empty;
            if ((_educationTrackingForUpdate.ProcessType == ProcessType.TimeIncreasing || _educationTrackingForUpdate.ProcessType == ProcessType.TimeDecreasing) && _educationTrackingForUpdate.Documents?.Count == 0 && string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = "Bu alan zorunludur!";
                StateHasChanged();
                return;
            }
            else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                var response = await CallDropzone();
                if (!response) return;
            }
            _saving = true;
            StateHasChanged();
            _educationTrackingForUpdate.StudentId = SelectedStudent.Id;
            var dto = Mapper.Map<EducationTrackingDTO>(_educationTrackingForUpdate);
            try
            {
                dto.ProcessDate = dto.ProcessDate?.Date;
                var response = await EducationTrackingService.Update(_educationTrackingForUpdate.Id.Value, dto);
                if (response.Result)
                {
                    if (_educationTrackings.Any(x => x.ProcessType == ProcessType.EstimatedFinish) && _educationTrackingForUpdate.AdditionalDays != null && _eTOldForUpdate.AdditionalDays != null)
                    {
                        if (_educationTrackingForUpdate.AdditionalDays.Value - _eTOldForUpdate.AdditionalDays.Value != 0)
                        {
                            if (_educationTrackingForUpdate.ProcessType == ProcessType.TimeIncreasing)
                            {
                                var addedDay = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(_educationTrackingForUpdate.AdditionalDays.Value - _eTOldForUpdate.AdditionalDays.Value);
                                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Start = addedDay.ToString("yyyy-MM-dd");
                                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Title = addedDay.ToString("dd MMMM yyyy");
                            }
                            else if (_educationTrackingForUpdate.ProcessType == ProcessType.TimeDecreasing)
                            {
                                var diffDay = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(-(_educationTrackingForUpdate.AdditionalDays.Value - _eTOldForUpdate.AdditionalDays.Value));
                                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Start = diffDay.ToString("yyyy-MM-dd");
                                dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Title = diffDay.ToString("dd MMMM yyyy");
                            }
                        }
                    }

                    if (_educationTrackingForUpdate.ProcessType == ProcessType.Start && (_educationTrackingForUpdate.ReasonType == ReasonType.ExcusedTransfer || _educationTrackingForUpdate.ReasonType == ReasonType.UnexcusedTransfer || (_educationTrackingForUpdate.ReasonType == ReasonType.BeginningToAssignedInstitution && _educationTrackingForUpdate.AssignmentType != AssignmentType.EducationAbroad) || _educationTrackingForUpdate.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment || _educationTrackingForUpdate.ReasonType == ReasonType.KKTCHalfTimed))
                    {
                        if (dataModel.FirstOrDefault(x => x.Id == _educationTrackingForUpdate.Id) == null)
                        {
                            dataModel.Add(new TimeLineModel()
                            {
                                Id = _educationTrackingForUpdate.Id.Value,
                                Content = "<strong><u>" + L[_educationTrackingForUpdate.ProcessType?.GetDescription()] + "</u></strong><br>" + L[_educationTrackingForUpdate.ReasonType?.GetDescription()] + "<br>",
                                style = "background:" + _educationTrackingForUpdate.ProcessType.Value.GetProcessColorForTimeline(),
                                Title = _educationTrackingForUpdate.ProcessDate?.ToString("dd MMMM yyyy"),
                                Start = _educationTrackingForUpdate.ProcessDate?.ToString("yyyy-MM-dd")
                            });
                        }
                        var diff = (_educationTrackingForUpdate.ProcessDate - _educationTrackings.FirstOrDefault(x => x.FormerProgramId == _educationTrackingForUpdate.FormerProgramId && x.Id != _educationTrackingForUpdate.Id).ProcessDate).Value.Days;
                        var diffDay = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(diff);
                        dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Start = diffDay.ToString("yyyy-MM-dd");
                        dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Title = diffDay.ToString("dd MMMM yyyy");
                    }
                    dataModel.FirstOrDefault(x => x.Id == _educationTrackingForUpdate.Id.Value).Start = _educationTrackingForUpdate.ProcessDate.Value.ToString("yyyy-MM-dd");
                    dataModel.FirstOrDefault(x => x.Id == _educationTrackingForUpdate.Id.Value).Content = "<strong><u>" + L[_educationTrackingForUpdate.ProcessType.Value.GetDescription()] + "</u></strong><br>" + L[_educationTrackingForUpdate.ReasonType.Value.GetDescription()] + "<br>" + (_educationTrackingForUpdate.AdditionalDays != null ? "(" + _educationTrackingForUpdate.AdditionalDays + " " + L["Day"] + ")" : "");
                    dataModel.FirstOrDefault(x => x.Id == _educationTrackingForUpdate.Id.Value).Title = _educationTrackingForUpdate.ProcessDate.Value.ToString("dd MMMM yyyy");

                    await JSRuntime.InvokeVoidAsync("redrawTimeline", WrapperId, (object)dataModel);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                    await GetEducatorTrackings();

                    _educationTrackingUpdateModal.CloseModal();
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
        private async Task OnRemoveEducationTracking(EducationTrackingResponseDTO educationTracking)
        {
            if (educationTracking.AssignmentType == AssignmentType.EducationAbroad && _educationTrackings.Any(x => x.ReasonType == ReasonType.CompletionOfAssignmentAbroad && x.Id == educationTracking.RelatedEducationTrackingId))
            {
                var question = await SweetAlert.ConfirmAlert($"{L["Warning"]}",
                $"{L["You have to delete the related record first, to delete this record."]}",
                SweetAlertIcon.question, false, $"{L["Okey"]}", $"{L["Cancel"]}");
                if (question)
                {
                    return;
                }
            }
            //TO DO: modify this func
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                bool changed = false;
                try
                {
                    await EducationTrackingService.Delete(educationTracking.Id.Value);
                    if ((educationTracking.ReasonType == ReasonType.ExcusedTransfer || educationTracking.ReasonType == ReasonType.UnexcusedTransfer || educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment || educationTracking.ReasonType == ReasonType.BeginningToAssignedInstitution || educationTracking.ReasonType == ReasonType.CompletionOfAssignment || educationTracking.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment || educationTracking.ReasonType == ReasonType.KKTCHalfTimed) && educationTracking.FormerProgramId != null)
                    {
                        if ((educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment && educationTracking.AssignmentType != AssignmentType.EducationAbroad) || educationTracking.ReasonType == ReasonType.BeginningToAssignedInstitution)
                            SelectedStudent.Status = StudentStatus.EducationContinues;
                        else if (educationTracking.ReasonType == ReasonType.CompletionOfAssignment || educationTracking.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment)
                            SelectedStudent.Status = StudentStatus.Assigned;
                        else if (SelectedStudent.TransferredDueToOpinion == true && (educationTracking.ReasonType == ReasonType.ExcusedTransfer || educationTracking.ReasonType == ReasonType.UnexcusedTransfer))
                        {
                            SelectedStudent.Status = StudentStatus.TransferDueToNegativeOpinion;
                            SelectedStudent.TransferredDueToOpinion = false;
                            changed = true;
                        }
                        var relatedTracking = _educationTrackings.FirstOrDefault(x => x.RelatedEducationTrackingId == educationTracking.Id.Value);
                        if (relatedTracking != null)
                        {
                            await EducationTrackingService.Delete((long)relatedTracking?.Id);
                            if (educationTracking.FormerProgramId != null)
                            {
                                SelectedStudent.ProgramId = educationTracking.FormerProgramId;
                                StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                            }
                            dataModel.Remove(dataModel.FirstOrDefault(x => x.Id == relatedTracking.Id));
                        }
                    }
                    if (educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad)
                    {
                        SelectedStudent.Status = StudentStatus.Abroad;
                        await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                    }
                    if (educationTracking.AssignmentType == AssignmentType.EducationAbroad)
                    {
                        SelectedStudent.Status = StudentStatus.EducationContinues;
                        await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                    }
                    if (_educationTrackings.Any(x => x.ProcessType == ProcessType.EstimatedFinish) && educationTracking.AdditionalDays != null)
                    {
                        var days = educationTracking.AdditionalDays.Value;
                        DateTime updatedDate = new();
                        if (educationTracking.ProcessType == ProcessType.TimeIncreasing || educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment)
                        {
                            updatedDate = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(-days);
                        }
                        else
                        {
                            updatedDate = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(days);
                        }

                        dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Start = updatedDate.ToString("yyyy-MM-dd");
                        dataModel.FirstOrDefault(x => x.Id == _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).Id).Title = updatedDate.ToString("dd MMMM yyyy");
                    }
                    if (educationTracking.StudentRotationId != null)
                    {
                        await StudentRotationService.Delete((long)educationTracking.StudentRotationId);
                    }

                    dataModel.Remove(dataModel.FirstOrDefault(x => x.Id == educationTracking.Id));
                    await JSRuntime.InvokeVoidAsync("redrawTimeline", WrapperId, (object)dataModel);
                    await GetEducatorTrackings();
                    if (educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad || educationTracking.AssignmentType == AssignmentType.EducationAbroad || (changed == true && (educationTracking.ReasonType == ReasonType.ExcusedTransfer || educationTracking.ReasonType == ReasonType.UnexcusedTransfer)))
                    {
                        NavigationManager.NavigateTo("/student/students");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
            }
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }


        private async Task OnChangeReasonAsync(ReasonType? reason)
        {
            if (reason == null)
            {
                _educationTracking.ReasonType = null;
            }
            _educationTracking.ReasonType = reason;

            if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
            {
                if (_educationTrackings.LastOrDefault(x => x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment) != null)
                {
                    var ownProgram = await ProgramService.GetById((long)_educationTrackings.LastOrDefault(x => x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment).FormerProgramId);
                    _ownProgram = ownProgram?.Item;
                    _newProgram = _ownProgram;
                }
            }

            StateHasChanged();
        }
    }
}