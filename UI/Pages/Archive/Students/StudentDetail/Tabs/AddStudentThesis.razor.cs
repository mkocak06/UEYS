using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.Extensions;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.RolesAndPermissions.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs
{
    public partial class AddStudentThesis
    {
        [Parameter] public StudentResponseDTO SelectedStudent { get; set; }
        [Parameter] public ThesisResponseDTO ThesisForUpdate { get; set; }
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
        private PaginationModel<EducatorResponseDTO> _educatorPaginationModel = new PaginationModel<EducatorResponseDTO>();
        private List<EducatorResponseDTO> _juryList = new();

        private FilterDTO _filterJury;
        private FilterDTO _filterEducator;
        private MyModal _advisorAddingModal;
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
        private ThesisDefenceResponseDTO _thesisDefence = new();
        private EthicCommitteeDecisionResponseDTO _ethicCommitteeDecision = new();
        private OfficialLetterResponseDTO _officialLetter = new();
        private ProgressReportResponseDTO _progressReport = new();
        private TitleResponseDTO _academicTitle = new();
        private TitleResponseDTO _staffTitle = new();
        private EducatorResponseDTO _existEducator = new();
        private JuryResponseDTO _jury = new();

        private List<JuryResponseDTO> _juries = new();

        private string _tcValidatorMessage;
        private bool _adding = false;
        private bool forceRender;
        private JuryType? _selectedJuryType;
        private EducatorType? _selectedEducatorType;

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
        private bool? IsCoordinator;
        private bool thesisFileLoading;
        private bool _educatorLoading;
        private string _phoneValidatorMessage = string.Empty;
        private string _emailValidatorMessage = string.Empty;

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
                               Field = "IsExistEducationPlace",
                               Dir = SortType.DESC
                        },
                    new Sort()
                        {
                               Field = "IsExistExpertiseBranch",
                               Dir = SortType.DESC
                        },


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
                        Field="Educator.IsDeleted",
                        Operator="eq",
                        Value=false
                    },
                    new Filter()
                    {
                        Field="StudentExp",
                        Operator="eq",
                        Value=SelectedStudent.StudentExpertiseBranches.Select(x=>x.ExpertiseBranchId).ToList()
                    },
                    new Filter()
                    {
                        Field="StudentEducationPlace",
                        Operator="eq",
                        Value=SelectedStudent.Program?.HospitalId
                    },
                    new Filter()
                    {
                        Field="Educator.EducatorType",
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

            base.OnAfterRender(firstRender);
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
            else if (_thesisForAdd.AdvisorTheses.Any(x => x.Educator.User.IdentityNo == _identityNo))
            {
                _tcValidatorMessage = "Bu kimlik bilgisine sahip danışman zaten eklendi.";
                return;
            }
            _searching = true;
            StateHasChanged();
            try
            {
                var response = await UserService.GetUserByIdentityNoWithEducationalInfo(_identityNo);
                if (response.Result)
                {
                    if (response.Item != null)
                    {
                        _user = response.Item;
                        _user.Educator ??= new() { EducatorType = EducatorType.NotInstructor };
                        _user.Phone ??= _user.CKYSDoctorResult?.Phone ?? "";
                        _user.Email ??= _user.CKYSDoctorResult?.Email ?? "";

                        if (_user.CKYSDoctorResult?.DoctorExpertiseBranches?.Count > 0)
                        {
                            var confirm = await SweetAlert.ConfirmAlert($"{L["Eğitici Eklemek Mi İstiyorsunuz?"]}",
                    $"{L["Bu T.C.'ye sahip kişi sağlık çalışanı olarak gözüküyor. Eğitici olarak sisteme eklemek ister misiniz?"]}",
                    SweetAlertIcon.question, true, $"{L["Evet"]}", $"{L["Cancel"]}");

                            if (confirm)
                            {
                                _advisorAddingModal.CloseModal();
                                NavigationManager.NavigateTo($"./educator/educators/add-educator");
                                return;
                            }
                            else
                            {
                                CancelEducator();
                                _searching = false;
                                StateHasChanged();
                                return;
                            }
                        }
                        if (_user.Educator != null)
                        {
                            if (_user.Educator?.IsDeleted == false && _user.Educator?.EducatorType == EducatorType.Instructor)
                            {
                                var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                                        $"{L["Bu eğitici sisteme kayıtlı. Eğiticileri görüntülemek ister misiniz?"]}",
                                         SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["Cancel"]}");

                                if (confirm)
                                {
                                    _advisorAddingModal.CloseModal();
                                    OnOpenEducatorsModal();
                                }
                                else
                                {
                                    CancelEducator();
                                }
                            }
                            else if (_user.Educator?.IsDeleted == false && _user.Educator?.EducatorType == EducatorType.NotInstructor)
                            {
                                _ec = new(_user);
                                _advisorThesis = new()
                                {
                                    Educator = new(),
                                    Thesis = new(),
                                };
                                IsCoordinator = false;
                                _advisorThesis.Educator = _user.Educator;
                                _advisorThesis.EducatorId = (long)_user.Educator.Id;

                                _ecForAdvisorThesis = new(_advisorThesis);


                            }
                        }
                        else
                        {
                            _ec = new(_user);
                            _advisorThesis = new()
                            {
                                Educator = new(),
                                Thesis = new(),
                            };
                            IsCoordinator = false;
                            _ecForAdvisorThesis = new(_advisorThesis);
                        }
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Fetched"]);

                    }
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message ?? L["An Error Occured."]);
                Console.WriteLine(e.Message);
                CancelEducator();
            }
            finally
            {
                _searching = false;
                StateHasChanged();
            }
        }

        public async Task AddAdvisorFromEducatorList()
        {
            if (!_ecForAdvisorThesis.Validate()) return;

            if (_thesisForAdd.AdvisorTheses.Any(x => x.IsCoordinator == true) && IsCoordinator == true)
            {
                var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                    $"{L["Zaten bir koordinatör var. Bu danışmanı yeni koordinatör olarak değiştirmek mi istiyorsunuz?"]}",
                    SweetAlertIcon.question, true, $"{L["Evet"]}", $"{L["Cancel"]}");
                if (confirm)
                {
                    foreach (var item in _thesisForAdd.AdvisorTheses.Where(x => x.IsCoordinator == true))
                    {
                        item.IsCoordinator = false;
                    }
                }
                else
                {
                    return;
                }
            }
            _advisorThesis.IsCoordinator = IsCoordinator;
            _advisorThesis.ThesisId = _thesisForAdd.Id.Value;

            _adding = true;
            StateHasChanged();

            try
            {
                var response = await AdvisorThesisService.Add(Mapper.Map<AdvisorThesisDTO>(_advisorThesis));
                if (response.Result)
                {
                    _advisorThesis.Id = response.Item.Id;
                    _thesisForAdd.AdvisorTheses.Add(_advisorThesis);

                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Tez Danışmanı başarı ile listeye eklendi");

                    _advisorThesisDetailModal.CloseModal();
                    _educatorListModal.CloseModal();
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
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
        public async Task AddEducatorFunc()
        {
            if (!string.IsNullOrEmpty(_emailValidatorMessage) || !string.IsNullOrEmpty(_phoneValidatorMessage) || !_ecForAdvisorThesis.Validate())
            {
                return;
            }

            if (_thesisForAdd.AdvisorTheses.Any(x => x.IsCoordinator == true) && IsCoordinator == true)
            {
                var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                    $"{L["Zaten bir koordinatör var. Bu danışmanı yeni koordinatör olarak değiştirmek mi istiyorsunuz?"]}",
                    SweetAlertIcon.question, true, $"{L["Evet"]}", $"{L["Cancel"]}");
                if (confirm)
                {
                    foreach (var item in _thesisForAdd.AdvisorTheses.Where(x => x.IsCoordinator == true))
                    {
                        item.IsCoordinator = false;
                    }
                    await UpdateThesis();
                }
                else
                {
                    return;
                }
            }
            _advisorThesis.IsCoordinator = IsCoordinator;
            _advisorThesis.ThesisId = _thesisForAdd.Id.Value;

            _adding = true;
            StateHasChanged();

            UserDTO userDTO = Mapper.Map<UserDTO>(_user);

            try
            {
                var responseUser = await UserService.AddUserWithEducatorInfo(userDTO);
                if (responseUser.Result)
                {

                    _advisorThesis.EducatorId = (long)responseUser.Item.Educator.Id;
                    _advisorThesis.Educator = responseUser.Item.Educator;
                    var response = await AdvisorThesisService.Add(Mapper.Map<AdvisorThesisDTO>(_advisorThesis));
                    if (response.Result)
                    {
                        _advisorThesis.Id = response.Item.Id;
                        _thesisForAdd.AdvisorTheses.Add(_advisorThesis);

                        SweetAlert.ToastAlert(SweetAlertIcon.success, "Tez Danışmanı başarı ile listeye eklendi");
                        _advisorAddingModal.CloseModal();
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                else
                {
                    throw new Exception(responseUser.Message);
                }


            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
                Console.WriteLine(e.Message);
            }
            finally
            {
                _adding = false;
                CancelEducator();
                StateHasChanged();
            }
        }

        private async Task OnOpenDefenceAddingModal()
        {
            Console.WriteLine(_thesisForAdd.ThesisDefences?.Count);
            _thesisDefence = new() { DefenceOrder = _thesisForAdd.ThesisDefences?.Count + 1 };
            _ecForDefense = new EditContext(_thesisDefence);
            dropzoneThesisDefence.ResetStatus();
            _defenseAddingModal.OpenModal();
            await GetJuries();
        }
        private async Task GetEducators()
        {
            _educatorLoading = true;
            StateHasChanged();

            try
            {
                //_educatorPaginationModel = await EducatorService.GetPaginateListForAdvisor(_filterEducator);

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
            _progressReport.Educator.PrintJson("Educator..:");
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
            try
            {
                await ProgressReportService.Delete(progressReport.Id);
                _thesisForAdd.ProgressReports.Remove(progressReport);
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally { StateHasChanged(); }
        }
        public async void AddProgressFunc()
        {
            if (!_ecForProgress.Validate()) return;
            if (!string.IsNullOrEmpty(dropzoneProgressReport._selectedFileName))
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
        private async void OpenUpdateThesisDefenceModal(ThesisDefenceResponseDTO thesisDefence)
        {
            await DocumentView(thesisDefence.Id.Value, DocumentTypes.ThesisDefence);
            _thesisDefence = thesisDefence;
            _thesisDefence.Documents = responseDocuments;
            _ecForDefense = new EditContext(_thesisDefence);
            StateHasChanged();
            if (ThesisForUpdate != null) GetJuries();
            _defenseAddingModal.OpenModal();
        }
        public async void RemoveThesisDefenceFunc(ThesisDefenceResponseDTO thesisDefence)
        {
            //if (thesisDefence.DefenceOrder == 1 && _thesisForAdd.ThesisDefences.Count > 1)
            //{
            //    SweetAlert.IconAlert(SweetAlertIcon.warning, "Tez Savunması Silmeden Önce", "Tez savunmalarını silerken sondan başlamak zorundasınız..");
            //    return;
            //}
            //var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            //$"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            //SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            //if (confirm)
            //{
            //    try
            //    {
            //        var response = await EducationTrackingService.GetListByStudentId(SelectedStudent.Id);
            //        if (response.Result)
            //        {
            //            var ett = response.Item.FirstOrDefault(x => x.ReasonType == (thesisDefence.DefenceOrder == 1 ? ReasonType.FirstThesisDefence : ReasonType.SecondThesisDefence));


            //            await EducationTrackingService.Delete(ett.Id.Value);

            //            SweetAlert.ToastAlert(SweetAlertIcon.success, "Eğitim Süre Takibi Başarıyla Güncellendi");

            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
            //        Console.WriteLine(e);
            //    }

            //    try
            //    {
            //        await ThesisDefenceService.Delete(thesisDefence.Id.Value);
            //        _thesisForAdd.ThesisDefences.Remove(thesisDefence);
            //        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
            //    }
            //    catch (Exception e)
            //    {
            //        SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
            //        Console.WriteLine(e);
            //    }
            //    finally { StateHasChanged(); }
            //}
            //else
            //{
            //    return;
            //}
        }
        public async void UpdateThesisDefenceFunc()
        {
            if (!_ecForDefense.Validate()) return;
            if (!string.IsNullOrEmpty(dropzoneThesisDefence._selectedFileName))
                CallDropzone(dropzoneThesisDefence);

            try
            {
                var response = await EducationTrackingService.GetListByStudentId(SelectedStudent.Id);
                if (response.Result)
                {
                    var ett = response.Item.FirstOrDefault(x => x.ReasonType == (_thesisDefence.DefenceOrder == 1 ? ReasonType.FirstThesisDefence : ReasonType.SecondThesisDefence));
                    ett.Description = L[_thesisDefence.Result?.GetDescription()];
                    ett.ProcessDate = _thesisDefence.ExamDate;

                    var result = await EducationTrackingService.Update(ett.Id.Value, Mapper.Map<EducationTrackingDTO>(ett));
                    if (result.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, "Eğitim Süre Takibi Başarıyla Güncellendi");
                    }
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }

            if (_thesisDefence.Result is DefenceResultType.Successful or DefenceResultType.SuccessfulWithRevision)
                _thesisForAdd.Status = ThesisStatusType.Successful;
            else if (_thesisDefence.Result == DefenceResultType.Failed)
                _thesisForAdd.Status = ThesisStatusType.Unsuccessful;
            else if (_thesisDefence.Result == DefenceResultType.InProgress)
                _thesisForAdd.Status = ThesisStatusType.Continues;

            await UpdateThesis();
            try
            {
                var response = await ThesisDefenceService.Update(_thesisDefence.Id.Value, Mapper.Map<ThesisDefenceDTO>(_thesisDefence));
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
                _defenseAddingModal.CloseModal();
                _thesisDefence = new ThesisDefenceResponseDTO();

                StateHasChanged();

            }
        }
        public async Task AddThesisDefenceFunc()
        {
            if (!_ecForDefense.Validate()) return;
            if (!string.IsNullOrEmpty(dropzoneThesisDefence._selectedFileName))
            {
                await CallDropzone(dropzoneThesisDefence);
                _thesisDefence.Documents = responseDocuments;
            }
            _thesisDefence.ThesisId = _thesisForAdd.Id;

            if (_thesisDefence.Result is DefenceResultType.Successful or DefenceResultType.SuccessfulWithRevision)
                _thesisForAdd.Status = ThesisStatusType.Successful;
            else if (_thesisDefence.Result == DefenceResultType.Failed)
                _thesisForAdd.Status = ThesisStatusType.Unsuccessful;
            else if (_thesisDefence.Result == DefenceResultType.InProgress)
                _thesisForAdd.Status = ThesisStatusType.Continues;

            await UpdateThesis();

            try
            {
                var response = await ThesisDefenceService.Add(Mapper.Map<ThesisDefenceDTO>(_thesisDefence));
                if (response.Result)
                {
                    _thesisDefence.Id = response.Item.Id;
                    _thesisForAdd.ThesisDefences ??= new List<ThesisDefenceResponseDTO>();
                    _thesisForAdd.ThesisDefences.Add(_thesisDefence);
                    if (_thesisDefence.Documents?.Count > 0)
                    {
                        foreach (var item in _thesisDefence.Documents)
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

                    var timeExtensionETT = new EducationTrackingDTO()
                    {
                        StudentId = SelectedStudent.Id,
                        ProcessDate = _thesisDefence.ExamDate,
                        ReasonType = _thesisDefence.DefenceOrder == 1 ? ReasonType.FirstThesisDefence : _thesisDefence.DefenceOrder == 2 ? ReasonType.SecondThesisDefence : null,
                        ProcessType = ProcessType.Information,
                        Description = L[_thesisDefence.Result?.GetDescription()],
                    };

                    var resultEts = await EducationTrackingService.Add(timeExtensionETT);
                    if (resultEts.Result)
                        SweetAlert.ToastAlert(SweetAlertIcon.info, "Eğitim Süre Takibi başarıyla güncellendi.");
                    else
                        SweetAlert.ErrorAlert();

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
                _defenseAddingModal.CloseModal();
                _thesisDefence = new ThesisDefenceResponseDTO();
                StateHasChanged();
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
            if (!_ecForLetter.Validate()) return;

            if (!string.IsNullOrEmpty(dropzoneLetter._selectedFileName))
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
            try
            {
                await OfficialLetterService.Delete(officialLetter.Id.Value);
                _thesisForAdd.OfficialLetters.Remove(officialLetter);
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally { StateHasChanged(); }
        }
        public async void DeleteAdvisorFunc(AdvisorThesisResponseDTO advisorThesis)
        {
            try
            {
                await AdvisorThesisService.Delete(advisorThesis.Id);
                _thesisForAdd.AdvisorTheses.Remove(advisorThesis);
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Deleted"]);
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally { StateHasChanged(); }
        }

        public async void UpdateLetterFunc()
        {
            if (!_ecForLetter.Validate()) return;

            if (!string.IsNullOrEmpty(dropzoneLetter._selectedFileName))
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
        public async void AddEthicFunc()
        {
            if (!_ecForEthic.Validate()) return;

            if (!string.IsNullOrEmpty(dropzoneEthic._selectedFileName))
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
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                }
                else throw new Exception(response.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                StateHasChanged();
                _subjectSavingModal.CloseModal();
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
                }
                else throw new Exception(response.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _titleAddingModal.CloseModal();
                StateHasChanged();
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

            if (_thesisForAdd.AdvisorTheses.Cast<AdvisorThesisResponseDTO>().Except(new AdvisorThesisResponseDTO[] { _advisorThesis }).Any(x => x.IsCoordinator == true) && IsCoordinator == true)
            {
                var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                    $"{L["Zaten bir kordinatör var. Bu danışmanı yeni kordinatör olarak değiştirmek mi istiyorsunuz?"]}",
                    SweetAlertIcon.question, true, $"{L["Evet"]}", $"{L["Cancel"]}");
                if (confirm)
                {
                    foreach (var item in _thesisForAdd.AdvisorTheses.Cast<AdvisorThesisResponseDTO>().Except(new AdvisorThesisResponseDTO[] { _advisorThesis }).Where(x => x.IsCoordinator == true))
                    {
                        item.IsCoordinator = false;

                    }
                }
                else
                {
                    return;
                }
            }
            _advisorThesis.IsCoordinator = IsCoordinator;
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
            finally
            {
                _advisorThesisDetailModal.CloseModal();
                _advisorThesis = new AdvisorThesisResponseDTO();
                _advisorAssignDatePicker.Value = null;

                StateHasChanged();
            }
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
        private void OnOpenEducatorsModal()
        {
            _advisorThesis = new()
            {
                Educator = new(),
                Thesis = new(),
            };
            IsCoordinator = false;
            _ecForAdvisorThesis = new EditContext(_advisorThesis);
            GetEducators();
            StateHasChanged();
            _educatorListModal.OpenModal();
        }
        private void OnOpenNonEducatorModal()
        {
            _identityNo = String.Empty;
            _user = null;
            _existEducator = null;
            _advisorThesis = new()
            {
                Educator = new(),
                Thesis = new(),
            };
            IsCoordinator = false;
            _ecForAdvisorThesis = new EditContext(_advisorThesis);
            StateHasChanged();
            _advisorAddingModal.OpenModal();
        }
        private void OpenUpdateAdvisorModal(AdvisorThesisResponseDTO advisorThesis)
        {
            _advisorThesis = advisorThesis;
            IsCoordinator = advisorThesis.IsCoordinator;
            _ecForAdvisorThesis = new EditContext(_advisorThesis);
            StateHasChanged();

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

            Console.WriteLine(_jury.JuryType);
            StateHasChanged();
        }
        private bool IsJuryChosen(EducatorResponseDTO educator)
        {
            _thesisDefence.Juries ??= new List<JuryResponseDTO>();
            return _thesisDefence.Juries.Any(x => x.EducatorId == educator.Id.Value);

        }
        private bool IsEducatorChosen(EducatorResponseDTO educator)
        {
            return _thesisForAdd.AdvisorTheses.Any(x => x.EducatorId == educator.Id);

        }
        private void OnAddAdvisorFromEducatorList(EducatorResponseDTO educator)
        {
            _advisorThesis ??= new();
            _advisorThesis.Educator = educator;
            _advisorThesis.EducatorId = (long)educator.Id;

            //_advisorThesis.Zone = new Shared.ResponseModels.Authorization.ZoneStudentModel()
            //{
            //    StudentId = SelectedStudent.Id,
            //    UserId = educator.UserId.Value,
            //};
            _ecForAdvisorThesis = new(_advisorThesis);
            StateHasChanged();
            _advisorThesisDetailModal.OpenModal();

        }

        private void OnAddJury(EducatorResponseDTO educator)
        {
            if (_selectedJuryType == JuryType.Core && _thesisDefence.Juries.Where(x => x.JuryType == JuryType.Core).ToList().Count == 3)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Asil Üye", "Asil Üye Sayısı 3'ten fazla olamaz!");
                return;
            }
            else if (_selectedJuryType == JuryType.Alternate && _thesisDefence.Juries.Where(x => x.JuryType == JuryType.Alternate).ToList().Count == 2)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Yedek Üye", "Yedek Üye Sayısı 2'den fazla olamaz!");
                return;

            }
            _thesisDefence.Juries ??= new List<JuryResponseDTO>();
            var juryResponse = new JuryResponseDTO()
            {
                Educator = educator,
                EducatorId = educator.Id.Value,
                JuryType = _selectedJuryType.Value,
                Zone = new()
                {
                    StudentId = SelectedStudent.Id,
                    UserId = educator.UserId.Value
                }

            };
            _thesisDefence.Juries.Add(juryResponse);

            _juryPaginationModel.Items.Remove(educator);
            _juryPaginationModel.TotalItemCount--;
            StateHasChanged();
        }
        private void OnRemoveJury(EducatorResponseDTO educator)
        {
            _thesisDefence.Juries.Remove(_thesisDefence.Juries.FirstOrDefault(x => x.EducatorId == educator.Id));
            _juryPaginationModel.Items.Add(educator);
            _juryPaginationModel.TotalItemCount++;
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