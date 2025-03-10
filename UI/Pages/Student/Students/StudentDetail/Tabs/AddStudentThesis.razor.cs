using ApexCharts;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.Extensions;
using Shared.FilterModels.Base;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.Management.ExpertiseBranches;
using UI.Pages.RolesAndPermissions.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Student.Students.StudentDetail.Tabs
{
    public partial class AddStudentThesis
    {
        [Parameter] public StudentResponseDTO SelectedStudent { get; set; }
        [Parameter] public ThesisResponseDTO ThesisForUpdate { get; set; }
        [Parameter] public bool? OnlyRead { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IMapper Mapper { get; set; }
        [Inject] IEducatorService EducatorService { get; set; }
        [Inject] ITitleService TitleService { get; set; }
        [Inject] IThesisService ThesisService { get; set; }
        [Inject] IHospitalService HospitalService { get; set; }
        [Inject] IDocumentService DocumentService { get; set; }
        [Inject] IOfficialLetterService OfficialLetterService { get; set; }
        [Inject] IEthicCommitteeService EthicCommitteeService { get; set; }
        [Inject] IProgressReportService ProgressReportService { get; set; }
        [Inject] IAdvisorThesisService AdvisorThesisService { get; set; }
        [Inject] IThesisDefenceService ThesisDefenceService { get; set; }
        [Inject] IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        private ThesisResponseDTO _thesisForAdd;
        private PaginationModel<EducatorResponseDTO> _juryPaginationModel = new PaginationModel<EducatorResponseDTO>();
        private PaginationModel<AdvisorPaginateResponseDTO> _educatorPaginationModel = new PaginationModel<AdvisorPaginateResponseDTO>();
        private List<EducatorResponseDTO> _juryList = new();

        private FilterDTO _filterJury;
        private FilterDTO _filterEducator;
        private MyModal _advisorAddingModal;
        private MyModal _juryNonEducatorAddingModal;
        private MyModal _subjectSavingModal;
        private MyModal _titleAddingModal;
        private MyModal _ethicAddingModal;
        private MyModal _progressAddingModal;
        private MyModal _juryAddingModal;
        private MyModal _defenseAddingModal;
        private MyModal _letterModal;
        private MyModal _thesisFileAddingModal;
        private MyModal _fileModalForEthic;
        private MyModal _fileModal;
        private MyModal _educatorListModal;
        private MyModal _advisorThesisDetailModal;

        private StudentThesis StudentThesis { get; set; }
        private string _identityNo = string.Empty;
        private bool _searching = false;
        private InputMask _inputMask;
        private bool isKPSResult = false;
        private UserResponseDTO _user = new();
        private AdvisorThesisResponseDTO _advisorThesis;
        private ChangeCoordinatorAdvisorThesisDTO _changeCoordinatorAdvisorThesis;
        private ThesisDefenceResponseDTO _thesisDefence = new();
        private EthicCommitteeDecisionResponseDTO _ethicCommitteeDecision = new();
        private OfficialLetterResponseDTO _officialLetter = new();
        private ProgressReportResponseDTO _progressReport = new();
        private TitleResponseDTO _academicTitle = new();
        private TitleResponseDTO _staffTitle = new();
        private EducatorResponseDTO _existEducator = new();
        private JuryResponseDTO _jury = new();

        private List<JuryResponseDTO> _juries = new();
        private List<ExpertiseBranchResponseDTO> _expertiseBranches = new();
        private List<AdvisorThesisResponseDTO> _deletedAdvisors = new();
        List<EducatorResponseDTO> _coordinatorEducatorListView = new();

        private string _tcValidatorMessage;
        private bool _adding = false;
        private bool forceRender;
        private JuryType? _selectedJuryType;
        private EducatorType? _selectedEducatorType;
        private NonInstructorType? _selectedNonInstructorType;

        private EditContext _ec;
        private EditContext _ecForSubject;
        private EditContext _ecForTitle;
        private EditContext _ecForEthic;
        private EditContext _ecForProgress;
        private EditContext _ecForDefense;
        private EditContext _ecForLetter = new(new OfficialLetterResponseDTO());
        private EditContext _ecForAdvisorThesis;
        private EditContext _ecForJury;
        private EditContext _ecForAdvisorThesisDetail;

        private InputDate<DateTime?> _advisorAssignDatePicker;
        private Dropzone dropzoneThesisFile;
        private Dropzone dropzoneThesisDefence;
        private Dropzone dropzoneLetter;
        private Dropzone dropzoneProgressReport;
        private Dropzone dropzoneEthic;

        public DocumentResponseDTO document1;
        public DocumentResponseDTO document2;
        public DocumentResponseDTO document3;
        public DocumentResponseDTO document4;


        private ThesisStatusType? ThesisStatus = ThesisStatusType.Continues;
        private bool _juryLoading;
        private JuryType JuryType;
        private int orderNumber = 1;
        private bool _fileLoaded = true;
        private List<DocumentResponseDTO> responseDocuments = new();
        private string _thesisTitle;
        private DateTime? _thesisDate;
        private string thesisTitleValidation;
        private string thesisDateValidation;
        private bool _juryCoreLoading;
        private bool _forAdvisor;
        private bool thesisFileLoading;
        private bool _educatorLoading;
        private string _phoneValidatorMessage = string.Empty;
        private string _emailValidatorMessage = string.Empty;
        private string _officialLetterValidationMessage = string.Empty;
        private string _ethicCommitteeDecisionValidationMessage = string.Empty;
        private string _proggressReportValidationMessage = string.Empty;
        private string _thesisDefenceValidationMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _ec = new EditContext(_user);
            _ecForLetter = new EditContext(_officialLetter);
            _ecForEthic = new EditContext(_ethicCommitteeDecision);
            _ecForProgress = new EditContext(_progressReport);
            _ecForDefense = new EditContext(_thesisDefence);

            _filterJury = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "User.Name",
                Dir = SortType.ASC
            }}
            };
            _filterJury.pageSize = 5;
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
                    }

                },

            };

            _filterEducator = new FilterDTO()
            {
                Sort = new[]
                {
                    new Sort()
                        {
                               Field = "DutyPlaceHospital",
                               Dir = SortType.ASC
                        }
                }
            };
            _filterEducator.pageSize = 5;
            _filterEducator.Filter = new Filter()
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
                        Field="UserIsDeleted",
                        Operator="eq",
                        Value=false
                    },
                    //  new Filter()
                    //{
                    //    Field="Educator.DeleteReason",
                    //    Operator="neq",
                    //    Value=EducatorDeleteReasonType.Death
                    //},
                    //new Filter()
                    //{
                    //    Field="DutyPlaceHospitalId",
                    //    Operator="eq",
                    //    Value=SelectedStudent.OriginalProgram?.HospitalId
                    //},
                    //new Filter()
                    //{
                    //    Field="DutyPlaceExpertiseBranchId",
                    //    Operator="eq",
                    //    Value=SelectedStudent.OriginalProgram?.ExpertiseBranchId
                    //},
                    new Filter()
                    {
                        Field="Type",
                        Operator="eq",
                        Value=EducatorType.Instructor
                    },
                },

            };

            _jury.JuryType = JuryType.Core;
            _advisorThesis = new()
            {
                Educator = new() { User = new() },
                Thesis = new(),
            };
            _ecForAdvisorThesis = new EditContext(_advisorThesis);
            _thesisForAdd = new()
            {
                AdvisorTheses = new List<AdvisorThesisResponseDTO>(),
                EthicCommitteeDecisions = new List<EthicCommitteeDecisionResponseDTO>(),
                ProgressReports = new List<ProgressReportResponseDTO>(),
                OfficialLetters = new List<OfficialLetterResponseDTO>(),
            };
            if (ThesisForUpdate != null)
            {
                _thesisForAdd = ThesisForUpdate;
                await DocumentView(_thesisForAdd.Id.Value, DocumentTypes.Thesis);
                _thesisForAdd.Documents = responseDocuments;
                _thesisTitle = _thesisForAdd.ThesisTitle;
                _thesisDate = _thesisForAdd.ThesisTitleDetermineDate;
            }
            else
            {
                try
                {
                    var response = await ThesisService.PostAsync(new ThesisDTO() { Status = ThesisStatusType.Continues, StudentId = SelectedStudent.Id });
                    if (response.Result)
                    {
                        _thesisForAdd = response.Item;
                        _thesisForAdd.ThesisSubjectType_1 = null;
                        _thesisForAdd.ThesisSubjectType_2 = null;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            _ecForSubject = new EditContext(_thesisForAdd);
            _ecForTitle = new EditContext(_thesisForAdd);

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                JSRuntime.InvokeVoidAsync("initTooltip");
                JSRuntime.InvokeVoidAsync("initDropdown");
                JSRuntime.InvokeVoidAsync("initPopOver");
            }
            else
            {
                JSRuntime.InvokeVoidAsync("initPopOver");
            }

            if (forceRender)
            {
                JSRuntime.InvokeVoidAsync("initTooltip");
                forceRender = false;
            }

        }
        public async void DeleteThesis()
        {
            try
            {
                if (_thesisForAdd.Id.HasValue)
                    await ThesisService.Delete(_thesisForAdd.Id.Value);
                else SweetAlert.ToastAlert(SweetAlertIcon.error, "Bu tez zaten silinmiş");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task SearchByIdentityNo()
        {
            try
            {
                _tcValidatorMessage = string.Empty;
                if (string.IsNullOrEmpty(_identityNo))
                {
                    _tcValidatorMessage = "Bu alan zorunludur!";
                    return;
                }
                else if (!_identityNo.TcValidator())
                {
                    _tcValidatorMessage = L["Invalid {0}", L["Identity Number"]];
                    Console.WriteLine("#" + _identityNo + "#");
                    return;
                }

                if (_forAdvisor)
                {
                    if (_thesisForAdd.AdvisorTheses.Any(x => x.User?.IdentityNo == _identityNo || x.Educator?.User?.IdentityNo == _identityNo))
                    {
                        _tcValidatorMessage = "Girilen T.C. Kimlik numarası mevcut bir danışmana aittir. Lütfen farklı bir T.C. Kimlik numarası giriniz.";
                        return;
                    }
                }
                else
                {
                    if (_thesisForAdd.ThesisDefences.SelectMany(x => x.Juries).Any(x => x.User?.IdentityNo == _identityNo || x.Educator?.User?.IdentityNo == _identityNo))
                    {
                        _tcValidatorMessage = "Girilen T.C. Kimlik numarası mevcut bir jüri üyesine aittir. Lütfen farklı bir T.C. Kimlik numarası giriniz.";
                        return;
                    }
                }

                _searching = true;
                StateHasChanged();

                var response = await UserService.GetUserByIdentityNoForThesis(_identityNo, _forAdvisor);
                if (response.Result)
                {
                    _user = response.Item;

                    if (_forAdvisor)
                    {
                        _advisorThesis = new()
                        {
                            Educator = new(),
                            Thesis = new(),
                            User = response.Item,
                            UserId = response.Item.Id,
                        };
                        _ecForAdvisorThesis = new(_advisorThesis);
                    }
                    else
                    {

                    }

                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Fetched"]);
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", response.Message ?? L["An Error Occured."]);
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message ?? L["An Error Occured."]);
                Console.WriteLine(e);
                CancelEducator();
            }
            finally
            {
                _searching = false;
                StateHasChanged();
            }
        }
        private List<EducatorExpertiseBranchResponseDTO> GetEducatorExpertiseBranches(List<DoctorExpertiseBranch> doctorExpertiseBranches)
        {
            List<EducatorExpertiseBranchResponseDTO> eebs = new List<EducatorExpertiseBranchResponseDTO>();

            if (doctorExpertiseBranches?.Count > 0)
            {
                foreach (var item in doctorExpertiseBranches)
                {
                    eebs.Add(new EducatorExpertiseBranchResponseDTO()
                    {
                        ExpertiseBranchName = item.ExpertiseBranchName,
                        RegistrationBranchName = item.RegistrationBranchName,
                        RegistrationDate = item.RegistrationDate != "" ? DateTime.ParseExact(item.RegistrationDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) : null,
                        RegistrationGraduationSchool = item.RegistrationGraduationSchool,
                        ExpertiseBranchId = item.ExpertiseBranchId,
                        //IsPrincipal = item.IsPrincipal,
                        ExpertiseBranch = new() { IsPrincipal = item.IsPrincipal },
                        RegistrationNo = item.RegistrationNo,
                        SubBranchIds = item.SubBrIds
                    });
                }
            }
            return eebs;
        }
        public async Task AddAdvisorFromEducatorList()
        {
            if (!_ecForAdvisorThesis.Validate()) return;

            _advisorThesis.ThesisId = _thesisForAdd.Id.Value;

            _adding = true;
            StateHasChanged();

            try
            {
                var response = await AdvisorThesisService.Add(Mapper.Map<AdvisorThesisDTO>(_advisorThesis));
                if (response.Result)
                {
                    //_advisorThesis.Id = response.Item.Id;
                    //_advisorThesis.ExpertiseBranch ??= new();
                    //_advisorThesis.ExpertiseBranch = _educatorPaginationModel.Items.FirstOrDefault(x => x.Id == _advisorThesis.EducatorId).ExpertiseBranches.FirstOrDefault(x => x.ExpertiseBranchId == _advisorThesis.ExpertiseBranch.Id).ExpertiseBranch;
                    _thesisForAdd.AdvisorTheses.Add(response.Item);

                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Tez Danışmanı başarı ile listeye eklendi");

                    _advisorThesisDetailModal.CloseModal();
                    _educatorListModal.CloseModal();
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e);
            }
            finally { _adding = false; StateHasChanged(); }
        }

        private void GetPhoneValidatorMessage()
        {
            if (string.IsNullOrEmpty(_user.Phone))
            {
                _phoneValidatorMessage = "Bu alan zorunludur!";
            }
            else if (!_user.Phone.StartsWith("5"))
            {
                _phoneValidatorMessage = "Lütfen numaranızı başında 5 olacak şekilde (5__) giriniz";
            }
            else if (_user.Phone.Length != 10)
            {
                _phoneValidatorMessage = "Lütfen numaranızı eksiksiz giriniz";
            }
            else
            {
                _phoneValidatorMessage = string.Empty;
            }
            StateHasChanged();
        }
        private void GetEmailValidatorMessage()
        {
            if (string.IsNullOrEmpty(_user.Email))
            {
                _emailValidatorMessage = "Bu alan zorunludur!";
            }
            else if (!IsValidEmail(_user.Email))
            {
                _emailValidatorMessage = "Lütfen geçerli bir e-posta adresi giriniz";
            }
            else
            {
                _emailValidatorMessage = string.Empty;
            }
            StateHasChanged();
        }
        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private async Task AddNonEducatorAdvisor()
        {
            if (!string.IsNullOrEmpty(_emailValidatorMessage) || !string.IsNullOrEmpty(_phoneValidatorMessage) || !_ecForAdvisorThesis.Validate())
                return;
            _advisorThesis.ThesisId = _thesisForAdd.Id.Value;
            _advisorThesis.StudentId = SelectedStudent.Id;
            try
            {
                _adding = true;
                var response = await AdvisorThesisService.AddNotEducatorAdvisor(Mapper.Map<AdvisorThesisDTO>(_advisorThesis));
                if (response.Result)
                {
                    _thesisForAdd.AdvisorTheses.Add(response.Item);
                    SweetAlert.IconAlert(SweetAlertIcon.success, L["Successful"], L["Successfully Added"]);
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], response.Message);

            }
            catch (Exception e)
            {
                SweetAlert.ErrorAlert();
            }
            finally
            {
                _adding = false;
                StateHasChanged();
                _advisorAddingModal.CloseModal();
            }
        }

        //public async Task AddEducatorAdvisor()
        //{
        //    if (!string.IsNullOrEmpty(_emailValidatorMessage) || !string.IsNullOrEmpty(_phoneValidatorMessage) || !_ecForAdvisorThesis.Validate())
        //        return;

        //    _advisorThesis.ThesisId = _thesisForAdd.Id.Value;

        //    _adding = true;
        //    StateHasChanged();

        //    UserDTO userDTO = Mapper.Map<UserDTO>(_user);

        //    try
        //    {
        //        if (_user.EducatorId != null)
        //            await AddAdvisor(_user);
        //        else
        //        {
        //            var responseUser = await UserService.AddUserWithEducatorInfo(userDTO);
        //            if (responseUser.Result)
        //                await AddAdvisor(responseUser.Item);
        //            else
        //                throw new Exception(responseUser.Message);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
        //        Console.WriteLine(e.Message);
        //    }
        //    finally
        //    {
        //        _adding = false;
        //        CancelEducator();
        //    }
        //}

        private async Task AddAdvisor(UserResponseDTO user)
        {
            _advisorThesis.EducatorId = (long)user.Educator.Id;
            _advisorThesis.Educator = user.Educator;
            var response = await AdvisorThesisService.Add(Mapper.Map<AdvisorThesisDTO>(_advisorThesis));
            if (response.Result)
            {
                _advisorThesis.Id = response.Item.Id;
                _thesisForAdd.AdvisorTheses.Add(_advisorThesis);

                SweetAlert.ToastAlert(SweetAlertIcon.success, "Tez Danışmanı başarı ile listeye eklendi");
                _advisorAddingModal.CloseModal();
            }
            else
                throw new Exception(response.Message);
        }

        private async Task OnOpenDefenceAddingModal()
        {
            if (_thesisForAdd.ThesisDefences.Any(x => x.Result == DefenceResultType.InProgress))
                SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["If there is a defense with the status 'Defense Date Determined', you cannot add a new defense. You must update the status of the thesis defense."]);
            else if (_thesisForAdd.ThesisDefences.Any(x => x.Result is DefenceResultType.Successful or DefenceResultType.SuccessfulWithRevision))
                SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["If there is a successful defense, you cannot add a new defense."]);
            else
            {
                //var response = await ThesisDefenceService.IsThesisDefenceAddable(_thesisForAdd.Id ?? 0, null);
                //if (response.Result)
                //{
                    _thesisDefence = new() { DefenceOrder = _thesisForAdd.ThesisDefences?.Count + 1 };
                    _ecForDefense = new EditContext(_thesisDefence);
                    dropzoneThesisDefence.ResetStatus();
                    _defenseAddingModal.OpenModal();
                    GetEducatorByIdForJuryList();
                    await GetJuries();
                //}
                //else
                //    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
            }
        }
        private async Task GetEducators()
        {
            _educatorLoading = true;
            StateHasChanged();

            try
            {
                _educatorPaginationModel = await EducatorService.GetPaginateListForAdvisor(_filterEducator);
                forceRender = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Eğiticiler başarıyla yüklendi.");
                _educatorLoading = false;
                StateHasChanged();
            }
        }
        private async Task PaginationHandlerEducator(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterEducator.page = item1;
            _filterEducator.pageSize = item2;
            await GetEducators();
        }
        private async Task GetJuries()
        {
            //SelectedJuryType = JuryType.Alternate;
            _juryLoading = true;
            StateHasChanged();

            try
            {
                _juryPaginationModel = await EducatorService.GetPaginateList(_filterJury);
                if (_thesisDefence.Juries?.Count > 0)
                {
                    foreach (var item in _thesisDefence.Juries)
                    {

                        _juryPaginationModel.Items.Remove(item.Educator);
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
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Jüriler başarıyla yüklendi.");
                _juryLoading = false;
                StateHasChanged();
            }
        }
        private async Task PaginationHandlerJury(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterJury.page = item1;
            _filterJury.pageSize = item2;
            await GetJuries();
        }
        private async Task<IEnumerable<EducatorResponseDTO>> SearchEducators(string searchQuery)
        {
            return (_thesisForAdd.AdvisorTheses.Count > 0 ? _thesisForAdd.AdvisorTheses.Select(x => x.Educator).Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).OrderBy(x => x.User?.Name) :
                new List<EducatorResponseDTO>());
        }
        private void OnChangeAdvisor(EducatorResponseDTO educator)
        {
            _progressReport.Educator = educator;
            _progressReport.EducatorId = educator?.Id;
        }
        private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
        {
            var response = await HospitalService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Sort("Name", Shared.Types.SortType.ASC).OrFilter(new string[] { "Name", "Province.Name" }, "contains", searchQuery));
            return response.Items;
        }
        private void OnChangeHospital(HospitalResponseDTO hospital)
        {
            _thesisDefence.Hospital = hospital;
            _thesisDefence.HospitalId = hospital?.Id;
        }
        private async void OpenUpdateProgressModal(ProgressReportResponseDTO progressReport)
        {
            await DocumentView(progressReport.Id, DocumentTypes.ProgressReport);
            _progressReport = progressReport;
            _progressReport.Documents = responseDocuments;
            _ecForProgress = new EditContext(progressReport);
            StateHasChanged();

            _progressAddingModal.OpenModal();
        }

        public async void UpdateProgressFunc()
        {
            if (!_ecForProgress.Validate()) return;
            if (!string.IsNullOrEmpty(dropzoneProgressReport._selectedFileName))
                CallDropzone(dropzoneProgressReport);

            try
            {
                var response = await ProgressReportService.Update(_progressReport.Id, Mapper.Map<ProgressReportDTO>(_progressReport));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else { throw new Exception(response.Message); }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally
            {
                _progressAddingModal.CloseModal();
                _progressReport = new ProgressReportResponseDTO();

                StateHasChanged();

            }
        }
        public async void RemoveProgressFunc(ProgressReportResponseDTO progressReport)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    await ProgressReportService.Delete(progressReport.Id);
                    _thesisForAdd.ProgressReports.Remove(progressReport);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                }
                finally { StateHasChanged(); }
            }
        }
        public async void AddProgressFunc()
        {
            _proggressReportValidationMessage = string.Empty;

            if (!_ecForProgress.Validate()) return;
            if (string.IsNullOrWhiteSpace(dropzoneProgressReport._selectedFileName))
            {
                _proggressReportValidationMessage = "File upload is required";
                StateHasChanged();
                return;
            }
            else
            {
                await CallDropzone(dropzoneProgressReport);
                _progressReport.Documents = responseDocuments;
            }
            _progressReport.ThesisId = _thesisForAdd.Id;
            try
            {
                var response = await ProgressReportService.Add(Mapper.Map<ProgressReportDTO>(_progressReport));
                if (response.Result)
                {
                    _progressReport.Id = response.Item.Id;
                    _thesisForAdd.ProgressReports ??= new List<ProgressReportResponseDTO>();
                    _thesisForAdd.ProgressReports.Add(_progressReport);
                    if (_progressReport.Documents?.Count > 0)
                    {
                        foreach (var item in _progressReport.Documents)
                        {
                            var documentDTO = Mapper.Map<DocumentDTO>(item);
                            documentDTO.EntityId = response.Item.Id;
                            var result = await DocumentService.Update(item.Id, documentDTO);
                            if (!result.Result)
                            {
                                throw new Exception(result.Message);
                            }
                        }
                    }

                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else { throw new Exception(response.Message); }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally
            {
                _progressAddingModal.CloseModal();
                _progressReport = new ProgressReportResponseDTO();
                StateHasChanged();
            }

            StateHasChanged();
            _progressAddingModal.CloseModal();
        }
        private async void GetEducatorByIdForJuryList()
        {
            _coordinatorEducatorListView = new();
            if (_thesisForAdd.AdvisorTheses?.Any(x => x.IsCoordinator == true) == false) return;

            try
            {
                var response = await EducatorService.GetByIdForJuryList(_thesisForAdd.AdvisorTheses.FirstOrDefault(x => x.IsCoordinator == true).EducatorId ?? 0);
                if (response.Result)
                    _coordinatorEducatorListView = new() { response.Item };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async void OpenUpdateThesisDefenceModal(long id)
        {
            await DocumentView(id, DocumentTypes.ThesisDefence);
            var thesisDefence = await ThesisDefenceService.GetById(id);


            _thesisDefence = thesisDefence.Item;
            _thesisDefence.IsConcluded = _thesisDefence.Result != DefenceResultType.InProgress;
            _thesisDefence.IsConcluded.PrintJson("sonuç");
            _thesisDefence.Documents = responseDocuments;
            _ecForDefense = new EditContext(_thesisDefence);
            StateHasChanged();
            _defenseAddingModal.OpenModal();
            if (ThesisForUpdate != null)
            {
                GetEducatorByIdForJuryList();
                GetJuries();
            }
        }
        public async void RemoveThesisDefenceFunc(ThesisDefenceResponseDTO thesisDefence)
        {
            if (thesisDefence.DefenceOrder != _thesisForAdd.ThesisDefences.Count && _thesisForAdd.ThesisDefences.Count > 1)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Tez Savunması Silmeden Önce", "Tez savunmalarını silerken sondan başlamak zorundasınız..");
                return;
            }
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    var response = await ThesisDefenceService.Delete(thesisDefence.Id.Value, SelectedStudent.Id);
                    _thesisForAdd.ThesisDefences.Remove(thesisDefence);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                    Console.WriteLine(e);
                }
                finally { StateHasChanged(); }
            }
        }

        public async void UpdateThesisDefenceFunc()
        {
            if (!_ecForDefense.Validate()) return;

            if (_thesisDefence.Result != DefenceResultType.InProgress)
            {
                if (string.IsNullOrWhiteSpace(dropzoneThesisDefence._selectedFileName) && !(_thesisDefence.Documents?.Count > 0))
                {
                    _thesisDefenceValidationMessage = "File upload is required";
                    StateHasChanged();
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(dropzoneThesisDefence._selectedFileName))
                {
                    var result = await dropzoneThesisDefence.SubmitFileAsync();
                    _thesisDefence.Documents ??= new List<DocumentResponseDTO>();
                    _thesisDefence.Documents.Add(result.Item);
                    StateHasChanged();
                }
            }

            try
            {
                var response = await ThesisDefenceService.Update(_thesisDefence.Id.Value, Mapper.Map<ThesisDefenceDTO>(_thesisDefence));
                if (response.Result)
                {
                    _thesisForAdd.ThesisDefences.FirstOrDefault(x => x.Id == _thesisDefence.Id).Result = _thesisDefence.Result;
                    StateHasChanged();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                    _defenseAddingModal.CloseModal();
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
            }
        }
        public async Task AddThesisDefenceFunc()
        {
            if (!_ecForDefense.Validate()) return;

            _thesisDefence.ThesisId = _thesisForAdd.Id;
            try
            {
                var response = await ThesisDefenceService.Add(Mapper.Map<ThesisDefenceDTO>(_thesisDefence));
                if (response.Result)
                {
                    _thesisDefence.Id = response.Item.Id;

                    _thesisForAdd.ThesisDefences ??= new List<ThesisDefenceResponseDTO>();
                    _thesisForAdd.ThesisDefences.Add(_thesisDefence);

                    if (!string.IsNullOrWhiteSpace(dropzoneThesisDefence._selectedFileName))
                    {
                        var result = await dropzoneThesisDefence.SubmitFileAsync(response.Item.Id);
                    }

                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);

                    _defenseAddingModal.CloseModal();
                    _thesisDefence = new ThesisDefenceResponseDTO();
                    StateHasChanged();
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
        }
        public async Task AddThesisFileFunc()
        {
            if (!string.IsNullOrEmpty(dropzoneThesisFile._selectedFileName))
            {
                thesisFileLoading = true;
                StateHasChanged();
                await CallDropzone(dropzoneThesisFile);
                await DocumentView(_thesisForAdd.Id.Value, DocumentTypes.Thesis);
                _thesisForAdd.Documents = responseDocuments;
                thesisFileLoading = false;
                StateHasChanged();
            }
            _thesisFileAddingModal.CloseModal();
        }
        public async void AddLetterFunc()
        {
            _officialLetterValidationMessage = string.Empty;
            if (!_ecForLetter.Validate()) return;

            if (string.IsNullOrWhiteSpace(dropzoneLetter._selectedFileName))
            {
                _officialLetterValidationMessage = "File upload is required";
                StateHasChanged();
                return;
            }
            else
            {
                await CallDropzone(dropzoneLetter);
                _officialLetter.Documents = responseDocuments;
            }

            _officialLetter.ThesisId = _thesisForAdd.Id;
            try
            {
                var response = await OfficialLetterService.Add(Mapper.Map<OfficialLetterDTO>(_officialLetter));
                if (response.Result)
                {
                    _officialLetter.Id = response.Item.Id;
                    _thesisForAdd.OfficialLetters ??= new List<OfficialLetterResponseDTO>();
                    _thesisForAdd.OfficialLetters.Add(_officialLetter);
                    if (_officialLetter.Documents?.Count > 0)
                    {

                        foreach (var item in _officialLetter.Documents)
                        {
                            var documentDTO = Mapper.Map<DocumentDTO>(item);
                            documentDTO.EntityId = response.Item.Id.Value;
                            var result = await DocumentService.Update(item.Id, documentDTO);
                            if (!result.Result)
                            {
                                throw new Exception(result.Message);
                            }
                        }
                    }

                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else { throw new Exception(response.Message); }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally
            {
                _letterModal.CloseModal();
                _officialLetter = new OfficialLetterResponseDTO();
                StateHasChanged();
            }
        }
        public async void DeleteLetterFunc(OfficialLetterResponseDTO officialLetter)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    await OfficialLetterService.Delete(officialLetter.Id.Value);
                    _thesisForAdd.OfficialLetters.Remove(officialLetter);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                }
                finally { StateHasChanged(); }
            }
        }

        public async void DeleteSubjectFunc()
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    _thesisForAdd.Subject = null;
                    _thesisForAdd.SubjectDetermineDate = null;
                    _thesisForAdd.ThesisSubjectType_1 = null;
                    _thesisForAdd.ThesisSubjectType_2 = null;
                    var response = await ThesisService.Put(_thesisForAdd.Id.Value, Mapper.Map<ThesisDTO>(_thesisForAdd));
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
                    }
                    else throw new Exception(response.Message);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                }
                finally { StateHasChanged(); }
            }
        }

        public async void DeleteTitleFunc()
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    _thesisForAdd.ThesisTitle = null;
                    _thesisForAdd.ThesisTitleDetermineDate = null;
                    var response = await ThesisService.Put(_thesisForAdd.Id.Value, Mapper.Map<ThesisDTO>(_thesisForAdd));
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
                    }
                    else throw new Exception(response.Message);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                }
                finally { StateHasChanged(); }
            }
        }

        public async void DeleteAdvisorFunc(AdvisorThesisResponseDTO advisorThesis)
        {
            bool confirm = false;

            if (advisorThesis.IsCoordinator == true)
            {
                var confirm_1 = await SweetAlert.ConfirmAlert($"{L["Warning"]}",
                       $"{L["If you want to delete the coordinator advisor, you have to select a new coordinator advisor. Do you want to continue?"]}",
                        SweetAlertIcon.question, true, L["Yes"], L["No"]);

                if (confirm_1)
                    OnOpenChangeCoordinatorModal();
            }
            else
            {
                confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                       $"{L["Do you want to delete this advisor? This action is irreversible."]}",
                        SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["No"]}");
            }
            if (confirm)
            {
                try
                {
                    await AdvisorThesisService.Delete(advisorThesis.Id);
                    _thesisForAdd.AdvisorTheses.Remove(advisorThesis);
                    _thesisForAdd.DeletedAdvisorTheses.Add(advisorThesis);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                    Console.WriteLine(e);
                }
                finally { StateHasChanged(); }
            }
        }

        public async void UpdateLetterFunc()
        {
            _officialLetterValidationMessage = string.Empty;
            if (!_ecForLetter.Validate()) return;

            if (string.IsNullOrEmpty(dropzoneLetter._selectedFileName) && _officialLetter.Documents?.Count < 1)
            {
                _officialLetterValidationMessage = "File upload is required";
                StateHasChanged();
                return;
            }
            else if (!string.IsNullOrEmpty(dropzoneLetter._selectedFileName))
                CallDropzone(dropzoneLetter);
            try
            {
                var response = await OfficialLetterService.Update(_officialLetter.Id.Value, Mapper.Map<OfficialLetterDTO>(_officialLetter));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else { throw new Exception(response.Message); }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally
            {
                _letterModal.CloseModal();
                _officialLetter = new OfficialLetterResponseDTO();
                StateHasChanged();
            }

        }
        private async Task DocumentView(long id, DocumentTypes docType)
        {
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(id, docType);
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
        private async Task OpenUpdateLetterModal(OfficialLetterResponseDTO officialLetter)
        {
            await DocumentView(officialLetter.Id.Value, DocumentTypes.OfficialLetter);
            _officialLetter = officialLetter;
            _officialLetter.Documents = responseDocuments;
            _ecForLetter = new EditContext(_officialLetter);
            StateHasChanged();

            _letterModal.OpenModal();
        }
        public async void RemoveEthicFunc(EthicCommitteeDecisionResponseDTO ethicCommitteeDecision)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
           $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
           SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    await EthicCommitteeService.Delete(ethicCommitteeDecision.Id);
                    _thesisForAdd.EthicCommitteeDecisions.Remove(ethicCommitteeDecision);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
                }
                catch (Exception e)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                    Console.WriteLine(e);
                }
                finally { StateHasChanged(); }
            }
        }

        public async void AddEthicFunc()
        {
            _ethicCommitteeDecisionValidationMessage = string.Empty;
            if (!_ecForEthic.Validate()) return;

            if (string.IsNullOrWhiteSpace(dropzoneEthic._selectedFileName))
            {
                _ethicCommitteeDecisionValidationMessage = "File upload is required";
                StateHasChanged();
                return;
            }
            else
            {
                await CallDropzone(dropzoneEthic);
                _ethicCommitteeDecision.Documents = responseDocuments;
            }
            _ethicCommitteeDecision.ThesisId = _thesisForAdd.Id;
            try
            {
                var response = await EthicCommitteeService.Add(Mapper.Map<EthicCommitteeDecisionDTO>(_ethicCommitteeDecision));
                if (response.Result)
                {
                    _ethicCommitteeDecision.Id = response.Item.Id;
                    _thesisForAdd.EthicCommitteeDecisions ??= new List<EthicCommitteeDecisionResponseDTO>();
                    _thesisForAdd.EthicCommitteeDecisions.Add(_ethicCommitteeDecision);
                    if (_ethicCommitteeDecision.Documents?.Count > 0)
                    {

                        foreach (var item in _ethicCommitteeDecision.Documents)
                        {
                            var documentDTO = Mapper.Map<DocumentDTO>(item);
                            documentDTO.EntityId = response.Item.Id;
                            var result = await DocumentService.Update(item.Id, documentDTO);
                            if (!result.Result)
                            {
                                throw new Exception(result.Message);
                            }
                        }
                    }

                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else { throw new Exception(response.Message); }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally
            {
                _ethicAddingModal.CloseModal();
                _ethicCommitteeDecision = new EthicCommitteeDecisionResponseDTO();
                StateHasChanged();
            }
        }
        private async void OpenUpdateEthicModal(EthicCommitteeDecisionResponseDTO ethicCommitteeDecision)
        {
            await DocumentView(ethicCommitteeDecision.Id, DocumentTypes.EthicCommitteeDecision);
            _ethicCommitteeDecision = ethicCommitteeDecision;
            _ethicCommitteeDecision.Documents = responseDocuments;
            _ecForEthic = new EditContext(_ethicCommitteeDecision);
            StateHasChanged();

            _ethicAddingModal.OpenModal();
        }
        public async void UpdateEthicFunc()
        {
            if (!_ecForEthic.Validate()) return;

            if (!string.IsNullOrEmpty(dropzoneEthic._selectedFileName))
                CallDropzone(dropzoneEthic);

            try
            {
                var response = await EthicCommitteeService.Update(_ethicCommitteeDecision.Id, Mapper.Map<EthicCommitteeDecisionDTO>(_ethicCommitteeDecision));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else { throw new Exception(response.Message); }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally
            {
                _ethicAddingModal.CloseModal();
                _ethicCommitteeDecision = new EthicCommitteeDecisionResponseDTO();
                StateHasChanged();
            }
        }
        private void OnChangeSubjectSelect(ThesisSubjectType_1? e)
        {
            if (e == null)
            {
                _thesisForAdd.ThesisSubjectType_1 = null;
            }
            else
            {
                _thesisForAdd.ThesisSubjectType_1 = e;
                _thesisForAdd.ThesisSubjectType_2 = null;
            }
        }

        public async void SaveSubjectFunc()
        {
            if (!_ecForSubject.Validate()) return;

            try
            {
                var response = await ThesisService.Put(_thesisForAdd.Id.Value, Mapper.Map<ThesisDTO>(_thesisForAdd));
                if (response.Result)
                {
                    StateHasChanged();
                    _subjectSavingModal.CloseModal();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async void SaveTitleFunc()
        {
            thesisTitleValidation = string.Empty;
            thesisDateValidation = string.Empty;
            if (string.IsNullOrEmpty(_thesisTitle))
            {
                thesisTitleValidation = "Bu alan zorunludur.";
                StateHasChanged();
                return;
            }
            if (!_thesisDate.HasValue)
            {
                thesisDateValidation = "Bu alan zorunludur.";
                StateHasChanged();
                return;
            }

            _thesisForAdd.ThesisTitle = _thesisTitle;
            _thesisForAdd.ThesisTitleDetermineDate = _thesisDate;
            try
            {
                var response = await ThesisService.Put(_thesisForAdd.Id.Value, Mapper.Map<ThesisDTO>(_thesisForAdd));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                    _titleAddingModal.CloseModal();
                    StateHasChanged();
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<bool> CallDropzone(Dropzone dropzone)
        {
            responseDocuments.Clear();
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
        public async Task Save()
        {
            _thesisForAdd.PrintJson("Tez.....:");
            var dto = Mapper.Map<ThesisDTO>(_thesisForAdd);
            dto.StudentId = SelectedStudent.Id;
            dto.PrintJson("Tez DTO.....:");
            try
            {
                var response = await ThesisService.PostAsync(dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Saved"]);
                }
                else
                {
                    SweetAlert.ErrorAlert(response.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task UpdateThesis()
        {
            try
            {
                var response = await ThesisService.Put(_thesisForAdd.Id.Value, Mapper.Map<ThesisDTO>(_thesisForAdd));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                }
                else throw new Exception(response.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async void UpdateAdvisorFunc()
        {
            if (!_ecForAdvisorThesis.Validate()) return;

            try
            {
                ResponseWrapper<AdvisorThesisResponseDTO> response;

                if (_advisorThesis.ChangeCoordinator == true)
                {
                    var mappedDTO = Mapper.Map<ChangeCoordinatorAdvisorThesisDTO>(_advisorThesis);
                    mappedDTO.OldAdvisorId = _advisorThesis.Id;
                    mappedDTO.StudentId = SelectedStudent.Id;
                    response = await AdvisorThesisService.ChangeCoordinatorAdvisor(_advisorThesis.Id, mappedDTO);
                }
                else
                    response = await AdvisorThesisService.Update(_advisorThesis.Id, Mapper.Map<AdvisorThesisDTO>(_advisorThesis));

                if (response.Result)
                {
                    var advisorThesis = _thesisForAdd.AdvisorTheses.FirstOrDefault(x => x.Id == _advisorThesis.Id);
                    advisorThesis.DeleteExplanation = _advisorThesis.DeleteExplanation;
                    advisorThesis.DeleteReason = _advisorThesis.DeleteReason;
                    _thesisForAdd.AdvisorTheses.Remove(advisorThesis);

                    if (_advisorThesis.ChangeCoordinator == true)
                        _thesisForAdd.DeletedAdvisorTheses.Add(advisorThesis);

                    _thesisForAdd.AdvisorTheses.Add(response.Item);
                    StateHasChanged();
                    SweetAlert.IconAlert(SweetAlertIcon.success, L["Successfull"], L["Successfully Updated!"]);
                }
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                e.PrintJson("hataaaa");
            }
            finally
            {
                _advisorThesisDetailModal.CloseModal();
                _advisorThesis = new AdvisorThesisResponseDTO();
                _advisorAssignDatePicker.Value = null;
                _educatorListModal.CloseModal();
                StateHasChanged();
            }
        }

        private async Task OpenDeleteAdvisorModal()
        {

        }

        private async Task<IEnumerable<TitleResponseDTO>> SearchAcademicTitles(string searchQuery)
        {
            var result = await TitleService.GetListByType(TitleType.Academic);

            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<TitleResponseDTO>();

        }
        private void OnChangeAcademicTitle(TitleResponseDTO title)
        {
            _academicTitle = title;
            _academicTitle.Id = title.Id;
        }
        private async Task<IEnumerable<TitleResponseDTO>> SearchStaffTitles(string searchQuery)
        {
            var result = await TitleService.GetListByType(TitleType.Staff);

            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<TitleResponseDTO>();

        }
        private void OnChangeStaffTitle(TitleResponseDTO title)
        {
            _staffTitle = title;
            _staffTitle.Id = title.Id;
        }
        private string GetIdentiyClass()
        {
            return "form-control " + (!string.IsNullOrEmpty(_tcValidatorMessage) ? "invalid" : "");
        }

        private void IdentyNoChanged(string id)
        {
            _identityNo = id;
        }

        private void OnOpenChangeCoordinatorModal()
        {
            OnOpenCoordinatorEducatorsModal();
            _advisorThesis.ChangeCoordinator = true;
            _advisorThesis.Id = _thesisForAdd.AdvisorTheses.FirstOrDefault(x => !x.IsDeleted && x.IsCoordinator == true).Id;
            _advisorThesis.ThesisId = _thesisForAdd.Id ?? 0;
        }

        private void OnOpenCoordinatorEducatorsModal()
        {
            _educatorPaginationModel = new PaginationModel<AdvisorPaginateResponseDTO>();

            var filter = _filterEducator.Filter.Filters.FirstOrDefault(x => x.Field == "DutyPlaceHospitalId");
            if (filter == null)
            {
                _filterEducator.Filter.Filters.AddRange([new Filter()
                {
                    Field = "DutyPlaceHospitalId",
                    Operator = "eq",
                    Value = SelectedStudent.OriginalProgram?.HospitalId
                },new Filter()
                {
                    Field = "ExpertiseBranchId",
                    Operator = "contains",
                    Value = SelectedStudent.OriginalProgram?.ExpertiseBranchId
                }]);
            }

            _advisorThesis = new()
            {
                Educator = new(),
                Thesis = new(),
                IsCoordinator = true,
                StudentId = SelectedStudent.Id,
                Type = AdvisorType.Educator,
            };
            _ecForAdvisorThesis = new EditContext(_advisorThesis);
            GetEducators();
            StateHasChanged();
            _educatorListModal.OpenModal();
        }

        private void OnOpenEducatorsModal()
        {
            _educatorPaginationModel = new();
            var dutyPlaceFilter = _filterEducator.Filter.Filters.FirstOrDefault(x => x.Field == "DutyPlaceHospitalId");
            var expBrFilter = _filterEducator.Filter.Filters.FirstOrDefault(x => x.Field == "ExpertiseBranchId");
            if (dutyPlaceFilter != null)
                _filterEducator.Filter.Filters.Remove(dutyPlaceFilter);
            if (expBrFilter != null)
                _filterEducator.Filter.Filters.Remove(expBrFilter);

            _advisorThesis = new()
            {
                Educator = new(),
                Thesis = new(),
                IsCoordinator = false,
                StudentId = SelectedStudent.Id,
                Type = AdvisorType.Educator,
            };
            _ecForAdvisorThesis = new EditContext(_advisorThesis);
            GetEducators();
            StateHasChanged();
            _educatorListModal.OpenModal();
        }

        private void OnOpenNonEducatorModal()
        {
            //aranacak kişinin danışman için mi yoksa jüri için mi arandığını belirlemek amacıyla
            _forAdvisor = true;
            _identityNo = String.Empty;
            _user = null;
            _existEducator = null;
            _advisorThesis = new()
            {
                Educator = new(),
                Thesis = new(),
            };
            _selectedNonInstructorType = NonInstructorType.ThesisAdvisor;
            _ecForAdvisorThesis = new EditContext(_advisorThesis);
            StateHasChanged();
            _advisorAddingModal.OpenModal();
        }
        private async void OnOpenProgressReportModal()
        {
            try
            {
                var response = await ProgressReportService.CalculateStartEndDates(_thesisForAdd.Id ?? 0, SelectedStudent.Id);
                if (response.Result)
                {
                    _progressReport = new() { Description = (_thesisForAdd.ProgressReports.Count + 1) + ". İlerleme Raporu", Educator = new(), BeginDate = response.Item.BeginDate, EndDate = response.Item.EndDate };
                    _ecForProgress = new EditContext(_progressReport);
                    StateHasChanged();
                    _progressAddingModal.OpenModal();
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ErrorAlert();
            }
        }

        private async void OpenUpdateAdvisorModal(AdvisorThesisResponseDTO advisorThesis)
        {
            try
            {
                var response = await AdvisorThesisService.GetByIdAsync(advisorThesis.Id);
                if (response.Result)
                    _advisorThesis = response.Item;
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", response.Message ?? L["An Error Occured."]);
                _expertiseBranches = response.Item?.Educator?.EducatorExpertiseBranches?.Select(x => x.ExpertiseBranch)?.ToList();
                //_advisorThesis = advisorThesis;
                //_expertiseBranches = new();
                //_expertiseBranches = _advisorThesis.Educator.EducatorExpertiseBranches.Select(x => x.ExpertiseBranch).ToList();
                //IsCoordinator = advisorThesis.IsCoordinator;
                _ecForAdvisorThesis = new EditContext(_advisorThesis);
                StateHasChanged();
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message ?? L["An Error Occured."]);
            }

            _advisorThesisDetailModal.OpenModal();
        }
        private void CancelEducator()
        {
            _user = null;
            _tcValidatorMessage = null;
            _identityNo = String.Empty;
            StateHasChanged();
        }
        private string GetAdminStyle(AdvisorThesisResponseDTO item)
        {
            if (item.IsCoordinator == true)
                return "- <span class=\"label label-inline label-light-danger font-weight-bold\">" + L["Coordinator Thesis Advisor"] + "</span>";
            else
                return "";
        }
        private void JuryTypeChangeHandler(ChangeEventArgs args)
        {

            var value = (string)args.Value;
            if (string.IsNullOrEmpty(value)) return;

            _jury.JuryType = (JuryType)Enum.Parse(typeof(JuryType), value);

            // _jury.JuryType = JuryType

            //_jury.JuryType = value;
            //if (_jury.JuryType == JuryType.Alternate)
            //    _jury.JuryType = JuryType.Core;
            //else
            //    _jury.JuryType = JuryType.Alternate;

            StateHasChanged();
        }
        private bool IsJuryChosen(EducatorResponseDTO educator)
        {
            return _thesisDefence.Juries?.Any(x => x.EducatorId == educator.Id.Value) == true;

        }

        //private bool IsOnlyOneExpertiseBranchOfAdvisor(AdvisorPaginateResponseDTO educator)
        //{
        //    var selectedEducatorExpBrs = educator.EducatorExpertiseBranches?.Select(x => x.ExpertiseBranchId)?.ToList();
        //    if (selectedEducatorExpBrs?.Where(x => x != null && x != 0).ToList().Count == 1)
        //    {
        //        foreach (var item in _thesisForAdd.AdvisorTheses?.Select(x => x.Educator))
        //        {
        //            if (item.EducatorExpertiseBranches?.Where(x => x.ExpertiseBranchId != null && x.ExpertiseBranchId != 0)?.ToList()?.Count == 1 &&
        //                item.EducatorExpertiseBranches?.FirstOrDefault(x => x.ExpertiseBranchId != null && x.ExpertiseBranchId != 0)?.ExpertiseBranchId == selectedEducatorExpBrs?.FirstOrDefault(x => x != null && x != 0))
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        private bool IsEducatorChosen(AdvisorPaginateResponseDTO educator)
        {
            //var selectedEducatorExpBrs = educator.EducatorExpertiseBranches.Select(x => x.ExpertiseBranchId).ToList();

            //var thesisEducatorsExpBrIds = _thesisForAdd.AdvisorTheses.SelectMany(x => x.Educator.EducatorExpertiseBranches.Select(x => x.ExpertiseBranchId)).ToList();

            //var result = selectedEducatorExpBrs.Intersect(thesisEducatorsExpBrIds).Any();

            return _thesisForAdd.AdvisorTheses.Any(x => x.EducatorId == educator.Id) /*|| result*/;

        }
        private void OnAddAdvisorFromEducatorList(AdvisorPaginateResponseDTO educator)
        {
            _advisorThesis.Educator = new() { User = new() { Name = educator.Name } };
            _advisorThesis.EducatorId = (long)educator.Id;
            _advisorThesis.ExpertiseBranchId = 0;
            _advisorThesis.Description = null;
            _advisorThesis.AdvisorAssignDate = null;
            _advisorThesis.UserId = educator.UserId;
            _expertiseBranches = new();
            if (_advisorThesis.IsCoordinator == true)
            {
                var coordinatorBranch = educator.ExpertiseBranches.Select(x => x.ExpertiseBranch).FirstOrDefault(x => x.Id == SelectedStudent.OriginalProgram.ExpertiseBranchId);
                _expertiseBranches.Add(coordinatorBranch);
                _advisorThesis.ExpertiseBranchId = coordinatorBranch.Id ?? 0;
            }
            _expertiseBranches = educator.ExpertiseBranches.Select(x => x.ExpertiseBranch).ToList();

            //_advisorThesis.Zone = new Shared.ResponseModels.Authorization.ZoneStudentModel()
            //{
            //    StudentId = SelectedStudent.Id,
            //    UserId = educator.UserId.Value,
            //};
            _ecForAdvisorThesis = new(_advisorThesis);
            StateHasChanged();
            _advisorThesisDetailModal.OpenModal();

        }
        public async Task AddNonEducatorForJury()
        {
            if (!string.IsNullOrEmpty(_emailValidatorMessage) || !string.IsNullOrEmpty(_phoneValidatorMessage))
            {
                return;
            }

            _adding = true;
            StateHasChanged();

            _thesisDefence.Juries ??= new List<JuryResponseDTO>();
            _thesisDefence.Juries.Add(new JuryResponseDTO()
            {
                JuryType = _selectedJuryType.Value,
                User = _user,
                UserId = _user?.Id,
                ThesisDefenceId = _thesisDefence.Id
            });

            _adding = false;
            CancelEducator();
            _juryNonEducatorAddingModal.CloseModal();

        }
        private void OnAddJury(EducatorResponseDTO educator)
        {
            _thesisDefence.Juries ??= new List<JuryResponseDTO>();
            var juryResponse = new JuryResponseDTO()
            {
                Educator = educator,
                EducatorId = educator.Id.Value,
                UserId = educator.UserId,
                JuryType = _selectedJuryType.Value,
                ThesisDefenceId = _thesisDefence.Id,
            };
            _thesisDefence.Juries.Add(juryResponse);

            StateHasChanged();
        }
        private void OnRemoveJury(long? educatorId, long? userId)
        {
            _thesisDefence.Juries.Remove(_thesisDefence.Juries.FirstOrDefault(x => educatorId != null ?  x.EducatorId == educatorId : x.UserId == userId));
            StateHasChanged();
        }
        #region FilterThesis
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
        #region FilterEducators
        private async Task OnSortChangeEducator(Sort sort)
        {
            if (sort.Field == "EducatorExpertiseBranches" || sort.Field == "EducatorPrograms") return;
            _filterEducator.Sort = new[] { sort };
            await GetEducators();
        }

        private async Task OnChangeFilterEducator(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filterEducator.Filter ??= new Filter();
            _filterEducator.Filter.Filters ??= new List<Filter>();
            _filterEducator.Filter.Logic ??= "and";
            var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filterEducator.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value
                });
            }
            else
            {
                _filterEducator.Filter.Filters[index].Value = value;
            }
            await GetEducators();
        }

        private async Task OnResetFilterEducator(string filterName)
        {
            _filterEducator.Filter ??= new Filter();
            _filterEducator.Filter.Filters ??= new List<Filter>();
            _filterEducator.Filter.Logic ??= "and";
            var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filterEducator.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetEducators();
            }
        }

        private bool IsFilteredEducator(string filterName)
        {
            _filterEducator.Filter ??= new Filter();
            _filterEducator.Filter.Filters ??= new List<Filter>();
            _filterEducator.Filter.Logic ??= "and";
            var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        #endregion
    }
}