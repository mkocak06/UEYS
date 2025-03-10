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
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Student.Students.StudentDetail.Tabs
{
    public partial class EducationTimeTracking
    {
        [Inject] public IState<StudentDetailState> StudentDetailState { get; set; }
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
        private Dropzone dropzone;
        private StudentResponseDTO SelectedStudent => StudentDetailState.Value.Student;

        private PaginationModel<EducationTrackingResponseDTO> _paginationModel;
        private bool _loading;
        private List<EducationTrackingResponseDTO> _educationTrackings;
        private List<DocumentResponseDTO> responseDocuments = new();
        private List<DocumentResponseDTO> Documents = new();
        private Flatpickr _fp1;
        private bool _saving;
        private CurriculumResponseDTO _latestCurriculum = new();
        private long? expertiseBranchId;
        private ReasonType? _reason;
        private bool _fileLoaded = true;
        private string _documentValidatorMessage;
        private DateTime? _assignmentDateLimit;
        private int _newAdditionalDayForLeavingInstitution;
        private EducationTrackingResponseDTO _leavingInstitutionRecord;
        private DateTime? _newDateOfRelatedRecord;
        private DateTime? _minDateLimit;
        private DateTime? _maxDateLimit;
        private bool _cannotBeTransferred = false;
        private string _transferFailedDescription = "Öðrenciye teblið edilmediði için nakil yapýlamadý.";

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
            _cannotBeTransferred = false;
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
                dropzone._selectedFileName.PrintJson("fileName");
                var response = await dropzone.SubmitFileAsync();
                if (response.Result)
                {

                    responseDocuments.Add(response.Item);

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
                }, true);
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
                }, SelectedStudent.Program.ExpertiseBranchId.Value);
            }

            return result.Items ?? new List<ProgramResponseDTO>();
        }
        private void OnChangeProgram(ProgramResponseDTO program)
        {
            _educationTracking.Program = program;
            _educationTracking.ProgramId = program.Id;

            StateHasChanged();
        }
        private string GetProgramClass()
        {
            return "form-control " + _educationTracking.Program is not null ? "invalid" : "";
        }


        private async Task OnChangeSelect(ProcessType? e)
        {
            _educationTracking.ProcessType = e;

            _educationTracking.ProcessDate = null;
            _educationTracking.ReasonType = null;
            _educationTracking.Program = null;
            _educationTracking.ProgramId = null;
            _educationTracking.AssignmentType = null;
            _educationTracking.AdditionalDays = null;
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

        private async Task OnChangeEndDate(DateTime? e)
        {
            _educationTracking.EndDate = e;
            if (_educationTracking.EndDate != null && _educationTracking.ProcessDate != null)
                _educationTracking.AdditionalDays = (_educationTracking.EndDate?.Date - _educationTracking.ProcessDate?.Date)?.Days + 1;
            StateHasChanged();
        }

        private async Task OnChangeProcessDate(DateTime? e)
        {
            if (_educationTracking.EndDate != null && _educationTracking.ProcessDate != null)
            {
                _educationTracking.AdditionalDays = (_educationTracking.EndDate?.Date - _educationTracking.ProcessDate?.Date)?.Days + 1;
                StateHasChanged();
            }

            _educationTracking.ProcessDate = e;
            StateHasChanged();
            //    if (_educationTracking.ProcessDate != null && _newProgram.Id > 0 && _newProgram.ExpertiseBranch.Id > 0)
            //    {
            //        try
            //        {

            //            var response = await CurriculumService.GetLatestByBeginningDateAndExpertiseBranchId(_newProgram.ExpertiseBranch.Id.Value, _educationTracking.ProcessDate.Value);
            //            if (response.Result)
            //            {
            //                _latestCurriculum = response.Item;
            //                if (_latestCurriculum == null)
            //                {
            //                    _curriculumMessage = "Bu branþa ait müfredat bulunamadý. Lütfen baþka bir branþ seçiniz.";
            //                }
            //                StateHasChanged();
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex);
            //        }
            //    }
            //}
        }

        private async Task OnChangeUpdateProcessDate(DateTime? e)
        {
            _educationTrackingForUpdate.EndDate.PrintJson("endda");
            if (_educationTrackingForUpdate.EndDate != null && _educationTrackingForUpdate.ProcessDate != null)
                _educationTrackingForUpdate.AdditionalDays = (_educationTrackingForUpdate.EndDate?.Date - _educationTrackingForUpdate.ProcessDate?.Date)?.Days + 1;
            _educationTrackingForUpdate.ProcessDate = e;
            StateHasChanged();
        }

        private async Task OnChangeUpdateEndDate(DateTime? e)
        {
            _educationTrackingForUpdate.EndDate = e;
            if (_educationTrackingForUpdate.EndDate != null && _educationTrackingForUpdate.ProcessDate != null)
                _educationTrackingForUpdate.AdditionalDays = (_educationTrackingForUpdate.EndDate?.Date - _educationTrackingForUpdate.ProcessDate?.Date)?.Days + 1;
            StateHasChanged();
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
                            Content = "<strong><u>" + L[item.ProcessType?.GetDescription()] + "</u></strong>" + (item.ProcessType != ProcessType.EstimatedFinish ? "<br>" + L[item.ReasonType?.GetDescription()] + "<br>" : "") + (item.AdditionalDays != null ? "(" + item.AdditionalDays + " " + L["Day"] + ")" : ""),
                            style = "background:" + item.ProcessType?.GetProcessColorForTimeline(),
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

                    //var program = await ProgramService.GetProgramByStudentId(SelectedStudent.Id);
                    //_selectedProgram = program.Item;
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

            if ((_educationTracking.ProcessType == ProcessType.TimeIncreasing || _educationTracking.ProcessType == ProcessType.TimeDecreasing || _educationTracking.ReasonType == ReasonType.TransferFailed || _educationTracking.ProcessType == ProcessType.Information || _educationTracking.ReasonType == ReasonType.TerminationSuspensionofCivilService || _educationTracking.ExcusedType == ExcusedType.NegativeOpinion) && string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = "Bu alan zorunludur!";
                StateHasChanged();
                return;
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

            if (_educationTracking.ProcessType == ProcessType.TimeIncreasing)
            {
                var confirm = await SweetAlert.ConfirmAlert($"{L["Warning"]}",
                   L["{0} days will be added to the estimated end date of the education. Do you confirm?", _educationTracking.AdditionalDays],
                   SweetAlertIcon.question, true, L["Yes"], L["No"]);
                if (!confirm)
                    return;
            }

            if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                var response = await CallDropzone();
                if (!response) return;
            }

            _saving = true;
            StateHasChanged();

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
                        Content = "<strong><u>" + L[_educationTracking.ProcessType.Value.GetDescription()] + "</u></strong><br>" + L[_educationTracking.ReasonType.Value.GetDescription()] + "<br>" + (_educationTracking.AdditionalDays != null ? "(" + _educationTracking.AdditionalDays + L["Day"] + ")" : ""),
                        style = "background:" + _educationTracking.ProcessType.Value.GetProcessColorForTimeline(),
                        Title = _educationTracking.ProcessDate.Value.ToString("dd MMMM yyyy"),
                        Start = _educationTracking.ProcessDate.Value.ToString("yyyy-MM-dd")
                    });
                    await JSRuntime.InvokeVoidAsync("redrawTimeline", WrapperId, (object)dataModel);
                    await GetEducatorTrackings();
                    _educationTrackingAddModal.CloseModal();
                    StateHasChanged();
                    if (_educationTracking.ProcessType == ProcessType.Transfer || _educationTracking.ReasonType == ReasonType.TransferFailed || _educationTracking.ProcessType == ProcessType.Assignment || _educationTracking.ProcessType == ProcessType.Finish)
                        NavigationManager.NavigateTo("/student/students");
                }
                else
                {
                    _educationTrackingAddModal.CloseModal();
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Error"], L[response.Message]);
                }
            }
            catch (Exception e)
            {

                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
                Console.WriteLine(e);
            }
            _saving = false;
            StateHasChanged();

            _educationTracking = new();
        }
        private void OnOpenUpdateModal(EducationTrackingResponseDTO educationTracking)
        {
            _minDateLimit = null;
            _maxDateLimit = null;
            _educationTrackingForUpdate = educationTracking;
            _eTOldForUpdate = educationTracking.Clone();
            if (educationTracking.RelatedEducationTrackingId != null)
            {
                var relatedTracking = _educationTrackings.FirstOrDefault(x => x.Id == educationTracking.RelatedEducationTrackingId);
                if (educationTracking.ProcessType == ProcessType.Start)
                    _minDateLimit = relatedTracking.ProcessDate;
                else
                    _maxDateLimit = relatedTracking?.ProcessDate;
                StateHasChanged();
            }
            //todo - min max date not working
            if (educationTracking.AdditionalDays != null)
                _educationTrackingForUpdate.EndDate = educationTracking.ProcessDate.Value.AddDays(educationTracking.AdditionalDays - 1 ?? 0);
            _ecUpdate = new EditContext(_educationTrackingForUpdate);
            _educationTrackingUpdateModal.OpenModal();


        }
        private async Task UpdateEducationTracking()
        {
            _documentValidatorMessage = string.Empty;
            if (!_ecUpdate.Validate()) return;

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
                        var diff = (_educationTrackingForUpdate?.ProcessDate?.Date - _educationTrackings.FirstOrDefault(x => x.FormerProgramId == _educationTrackingForUpdate.FormerProgramId && x.Id != _educationTrackingForUpdate.Id)?.ProcessDate?.Date)?.Days;
                        var diffDay = _educationTrackings.FirstOrDefault(x => x.ProcessType == ProcessType.EstimatedFinish).ProcessDate.Value.AddDays(diff ?? 0);
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
                e.StackTrace.PrintJson("Hta");
                _educationTrackingUpdateModal.CloseModal();
            }
            _saving = false;
            StateHasChanged();
        }
        private async Task OnRemoveEducationTracking(EducationTrackingResponseDTO educationTracking)
        {
            if (educationTracking.AssignmentType == AssignmentType.EducationAbroad && _educationTrackings.Any(x => x.ReasonType == ReasonType.CompletionOfAssignmentAbroad && x.Id == educationTracking.RelatedEducationTrackingId))
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, $"{L["Warning"]}", $"{L["You have to delete the related record first, to delete this record."]}");
                return;
            }
            //TO DO: modify this func
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                //bool changed = false;
                try
                {
                    await EducationTrackingService.Delete(educationTracking.Id.Value);
                    //SelectedStudent.EducationTrackings.RemoveAll(x => x.Id == educationTracking.Id.Value);
                    //if ((educationTracking.ReasonType == ReasonType.ExcusedTransfer || educationTracking.ReasonType == ReasonType.UnexcusedTransfer || educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment || educationTracking.ReasonType == ReasonType.BeginningToAssignedInstitution || educationTracking.ReasonType == ReasonType.CompletionOfAssignment || educationTracking.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment || educationTracking.ReasonType == ReasonType.KKTCHalfTimed) && educationTracking.FormerProgramId != null)
                    //{
                    //	if ((educationTracking.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment && educationTracking.AssignmentType != AssignmentType.EducationAbroad) || educationTracking.ReasonType == ReasonType.BeginningToAssignedInstitution)
                    //		SelectedStudent.Status = StudentStatus.EducationContinues;
                    //	else if (educationTracking.ReasonType == ReasonType.CompletionOfAssignment || educationTracking.ReasonType == ReasonType.BeginningToOwnInstitutionAfterAssignment)
                    //		SelectedStudent.Status = StudentStatus.Assigned;
                    //	else if (SelectedStudent.TransferredDueToOpinion == true && (educationTracking.ReasonType == ReasonType.ExcusedTransfer || educationTracking.ReasonType == ReasonType.UnexcusedTransfer))
                    //	{
                    //		SelectedStudent.Status = StudentStatus.TransferDueToNegativeOpinion;
                    //		SelectedStudent.TransferredDueToOpinion = false;
                    //		changed = true;
                    //	}
                    //	var relatedTracking = _educationTrackings.FirstOrDefault(x => x.RelatedEducationTrackingId == educationTracking.Id.Value);
                    //	if (relatedTracking != null)
                    //	{
                    //		//await EducationTrackingService.Delete((long)relatedTracking?.Id);
                    //		if (educationTracking.FormerProgramId != null)
                    //		{
                    //			SelectedStudent.ProgramId = educationTracking.FormerProgramId;
                    //			SelectedStudent.EducationTrackings.RemoveAll(x => x.Id == relatedTracking.Id);
                    //			await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                    //		}
                    //		dataModel.Remove(dataModel.FirstOrDefault(x => x.Id == relatedTracking.Id));
                    //	}
                    //}
                    //if (educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad)
                    //{
                    //	SelectedStudent.Status = StudentStatus.Abroad;
                    //	await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                    //}
                    //if (educationTracking.AssignmentType == AssignmentType.EducationAbroad)
                    //{
                    //	SelectedStudent.Status = StudentStatus.EducationContinues;
                    //	await StudentService.Update(SelectedStudent.Id, Mapper.Map<StudentDTO>(SelectedStudent));
                    //}
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
                    //if (educationTracking.StudentRotationId != null)
                    //{
                    //	await StudentRotationService.Delete((long)educationTracking.StudentRotationId);
                    //}

                    dataModel.Remove(dataModel.FirstOrDefault(x => x.Id == educationTracking.Id));
                    await JSRuntime.InvokeVoidAsync("redrawTimeline", WrapperId, (object)dataModel);
                    if (educationTracking.ReasonType == ReasonType.CompletionOfAssignmentAbroad || educationTracking.ReasonType == ReasonType.CompletionOfAssignment || educationTracking.AssignmentType == AssignmentType.EducationAbroad || educationTracking.ProcessType == ProcessType.Transfer || educationTracking.ProcessType == ProcessType.Assignment)
                    {
                        NavigationManager.NavigateTo("/student/students");
                    }
                    else
                    {
                        await GetEducatorTrackings();
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

            //if (_educationTracking.ReasonType == ReasonType.CompletionOfAssignment)
            //{
            //    if (_educationTrackings.LastOrDefault(x => x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment) != null)
            //    {
            //        var ownProgram = await ProgramService.GetById((long)_educationTrackings.LastOrDefault(x => x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment).FormerProgramId);
            //        _ownProgram = ownProgram?.Item;
            //        _newProgram = _ownProgram;
            //    }
            //}
            StateHasChanged();
        }

        private async Task OnUploadHandler(EducationTrackingResponseDTO educationTracking)
        {
            dropzone.ResetStatus();
            _educationTrackingForUpdate = educationTracking;
            StateHasChanged();
            UploaderModal.OpenModal();
        }
    }
}