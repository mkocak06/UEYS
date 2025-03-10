using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Captcha;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.Student.Students
{

    public partial class AddStudent
    {
        [Parameter] public long? programId { get; set; }
        [Inject] public IState<AppState> AppState { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IUserService UserService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public ICountryService CountryService { get; set; }
        [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public ICaptchaService CaptchaService { get; set; }
        [Inject] public IConfiguration Configuration { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private StudentResponseDTO _student;
        private bool _saving;
        private List<BreadCrumbLink> _links;
        private BootstrapDatePicker _dp1;

        private UserResponseDTO _user;
        private ProgramResponseDTO _program;
        private ProgramResponseDTO _programFromYUEP;
        private CurriculumResponseDTO _curriculum;
        private PlacementExamType? _beginningExamType = null;
        private PeriodType? _periodType = null;
        private ForeignType? _foreignType;

        string validationMessage = string.Empty;
        private string _identityNo = String.Empty;
        private string _tcValidatorMessage;
        private string _osymDocumentValidatorMessage;
        private string _transferDocumentValidatorMessage;
        private string _programValidatorMessage;
        private string _curriculumValidatorMessage;
        private string _beginningExamValidatorMessage;
        private string _placementScoreValidatorMessage;
        private string _periodTypeValidatorMessage;
        private string _placementYearValidatorMessage;
        private bool _searchingIdentity;
        private bool _isKpsResult = false;
        private bool _isNative = true;
        private DateTime? _processDate;
        private int? _beginningYear;
        private double? _placementScore;
        private List<ExpertiseBranchResponseDTO> allExpBranches = new();
        private List<ExpertiseBranchResponseDTO> principalBranches = new();
        private List<ExpertiseBranchResponseDTO> subBranches = new();
        private List<CountryResponseDTO> _countries = new();
        private PaginationModel<CountryResponseDTO> countryResponse = new();
        private CountryResponseDTO _country = new();
        private InputMask PhoneInput;
        private EditContext _ec;
        private ImageUpload _addStudentProfile;
        private MyModal _osymFileAddingModal;
        private MyModal _transferFileAddingModal;
        private Dropzone dropzoneOsymFile;
        private Dropzone dropzoneTransferFile;
        private bool _fileLoaded = true;
        private List<DocumentResponseDTO> responseDocuments = new();
        private CaptchaResponseDTO captcha;
        private string inputCaptcha = string.Empty;
        private string _captchaValidatorMessage;
        private bool _showCaptcha = true;
        private bool isDomainAllowed;
        protected override async Task OnInitializedAsync()
        {
            if (programId is not null)
            {
                var response = await ProgramService.GetById(programId.Value);
                if (response.Result)
                    _programFromYUEP = response.Item;

                StateHasChanged();
            }

            countryResponse = await CountryService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", "türkiye", "and"));
            _country = countryResponse.Items?.FirstOrDefault();

            _curriculum = new CurriculumResponseDTO();
            _student = new StudentResponseDTO();
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
                    IsActive = true,
                    To = "/student/students",
                    OrderIndex = 1,
                    Title = L["Specialization Student Informations"]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = @L["add_new", L["Specialization Student"]]
                }
        };
            isDomainAllowed = IsDomainAllowed();
            if (isDomainAllowed)
                captcha = (await CaptchaService.Get()).Item;
            await base.OnInitializedAsync();
        }
        private bool IsDomainAllowed()
        {
            var allowedDomains = Configuration["AllowedDomain"];
            var currentDomain = new Uri(NavigationManager.Uri).Host;
            return allowedDomains.Contains(currentDomain);
        }
        public void ImageChanged(string imagePath)
        {
            _user.ProfilePhoto = imagePath;
        }
        public async Task<bool> CallDropzoneOsym()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneOsymFile.SubmitFileAsync();

                if (result.Result)
                {
                    responseDocuments.Add(result.Item);
                    _osymDocumentValidatorMessage = null;
                    _osymFileAddingModal.CloseModal();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                _fileLoaded = true;
                StateHasChanged();
                dropzoneOsymFile.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneTransfer()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneTransferFile.SubmitFileAsync();

                if (result.Result)
                {
                    responseDocuments.Add(result.Item);
                    _transferDocumentValidatorMessage = null;
                    _transferFileAddingModal.CloseModal();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                _fileLoaded = true;
                StateHasChanged();
                dropzoneTransferFile.ResetStatus();
                StateHasChanged();
            }
        }

        private async Task Save()
        {
            _tcValidatorMessage = null;
            _curriculumValidatorMessage = null;
            _osymDocumentValidatorMessage = null;
            _transferDocumentValidatorMessage = null;
            if (_user is null)
            {
                _tcValidatorMessage = L["Please enter your T.C ID identification."];
                StateHasChanged();
                return;
            }
            if (!_ec.Validate())
            {
                JSRuntime.InvokeVoidAsync("scrollToValidationSummary");
                return;
            }

            if ((_user.Student.ProgramId is not null) && _processDate is null)
            {
                _curriculumValidatorMessage = L["Lütfen bir tarih seçiniz."];
                StateHasChanged();
                return;
            }

            if (responseDocuments?.Any(x => x.DocumentType == DocumentTypes.OsymResultDocument) == false)
            {
                _osymDocumentValidatorMessage = L["Lütfen bir ÖSYM Sonuç Belgesi yükleyin."];
                StateHasChanged();
                return;
            }

            _saving = true;
            StateHasChanged();
            _user.Student.OriginalProgramId = _user.Student.ProgramId;
            _user.Student.Status = StudentStatus.EducationContinues;

            _user.Student.EducationTrackings = new();
            var start = new EducationTrackingResponseDTO()
            {
                ProcessType = ProcessType.Start,
                ProcessDate =  _processDate,
                ReasonType =  _user.Student.StartReason,
                ProgramId =_user.Student.ProgramId
            };
            _user.Student.EducationTrackings.Add(start);
                       
            if (_user.Id != null && _user.Id > 0 && !string.IsNullOrEmpty(_user.ProfilePhoto))
            {
                _addStudentProfile.SavePhoto(_user.Id);
            }
            UserDTO userDTO = Mapper.Map<UserDTO>(_user);
            try
            {
                var result = await UserService.AddUserWithStudentInfo(userDTO);
                if (result.Result)
                {
                    foreach (var item in responseDocuments)
                    {
                        var documentDTO = Mapper.Map<DocumentDTO>(item);
                        documentDTO.EntityId = result.Item.Student.Id;
                        var resultDoc = await DocumentService.Update(item.Id, documentDTO);
                        if (!resultDoc.Result)
                        {
                            throw new Exception(result.Message);
                        }
                    }
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                    NavigationManager.NavigateTo(programId == null ? $"./student/students/{result.Item.Student.Id}"
                        : $"./institution-management/programs/{programId}");
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], result.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("1");
                Console.WriteLine(e.Message);
            }

            _saving = false;
            StateHasChanged();
        }

        private async Task SearchByIdentityNo()
        {
            _tcValidatorMessage = string.Empty;
            _captchaValidatorMessage = string.Empty;
            if (string.IsNullOrEmpty(_identityNo))
            {
                _tcValidatorMessage = L["This field must be filled!"];
                return;
            }
            else if (!_identityNo.TcValidator())
            {
                _tcValidatorMessage = L["Invalid {0}", L["Identity Number"]];
                Console.WriteLine("#" + _identityNo + "#");
                return;
            }
            if (isDomainAllowed && string.IsNullOrEmpty(inputCaptcha))
            {
                _captchaValidatorMessage = L["Lütfen güvenlik kodunu giriniz."];
                return;
            }
            if (isDomainAllowed && _showCaptcha)
            {
                try
                {
                    var response = await CaptchaService.VerifyCaptcha(captcha.Chiper, inputCaptcha);
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, "Doğrulama Başarılı");
                        _showCaptcha = false;
                        StateHasChanged();
                    }
                    else
                    {
                        _captchaValidatorMessage = L[response.Message];
                        StateHasChanged();
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            _searchingIdentity = true;
            StateHasChanged();



            try
            {
                var response = await UserService.GetUserByIdentityNoWithEducationalInfo(_identityNo);
                if (response.Result)
                {
                //    if (response.Item.IsDeleted)
                //    {
                //        var confirm = await SweetAlert.ConfirmAlert($"{L["Pasif Kullanıcı Bulundu"]}",
                //$"{L["Girmiş olduğunuz TCKN'a ait kullanıcı sistemde pasif olarak bulunmaktadır. Aktifleştirmek ister misiniz?"]}",
                //SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["No"]}");

                //        if (confirm)
                //        {
                //            var result = await UserService.UpdateActivePassiveUser(response.Item.Id);
                //            if (result)
                //                SweetAlert.ToastAlert(SweetAlertIcon.success, "Kullanıcı Aktifleştirildi!");
                //            else
                //                SweetAlert.ToastAlert(SweetAlertIcon.error, "Bir hata oluştu");
                //        }
                //        else
                //        {
                //            CancelUser();
                //            return;
                //        }
                //    }

                    var expResponse = await ExpertiseBranchService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue)
                                                                                    .Sort("Name", Shared.Types.SortType.ASC));
                    if (expResponse.Items != null)
                    {
                        allExpBranches = expResponse.Items;
                        principalBranches = expResponse.Items.Where(x => !x.PrincipalBranches.Any()).ToList();
                        subBranches = expResponse.Items.Where(x => x.PrincipalBranches.Any()).ToList();
                    }
                    _user = response.Item;

                    _user.Student = new()
                    {
                        OriginalProgramId = _user.Student?.ProgramId,//check this row, is it true?
                        ProgramId = programId,
                        Program = _programFromYUEP ?? new(),
                        GraduatedDate = _user.CKYSStudentResult?.GraduatedDate,
                        GraduatedSchool = _user.CKYSStudentResult?.GraduatedSchool,
                        MedicineRegistrationDate = _user.CKYSStudentResult?.MedicineRegistrationDate,
                        MedicineRegistrationNo = _user.CKYSStudentResult?.MedicineRegistrationNo,
                        StudentExpertiseBranches = GetStudentExpertiseBranches(_user.CKYSStudentResult?.DoctorExpertiseBranches)
                    };


                    _user.Phone ??= _user.CKYSStudentResult?.Phone ?? "";
                    _user.Email ??= _user.CKYSStudentResult?.Email ?? "";

                    if (_user.CountryId == null || _user.CountryId == 0)
                    {
                        countryResponse = await CountryService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", _user.Nationality, "and"));
                        if (countryResponse.Items?.Count == 0)
                        {
                            countryResponse = await CountryService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", "türkiye", "and"));
                        }
                        _user.CountryId ??= countryResponse.Items?.FirstOrDefault().Id;
                        _user.Country ??= countryResponse.Items?.FirstOrDefault();
                    }

                    if (_identityNo.StartsWith("99"))
                    {
                        _isNative = false;
                        _user.ForeignType = ForeignType.Foreign;
                        _country = null;
                        StateHasChanged();
                    }
                    _ec = new(_user);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Fetched"]);
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message ?? L["An Error Occured."]);
                Console.WriteLine(e);
                CancelUser();
            }
            finally
            {
                _searchingIdentity = false;
                StateHasChanged();
            }
        }

        private string GetIdentiyClass()
        {
            return "form-control " + (!string.IsNullOrEmpty(_tcValidatorMessage) ? "invalid" : "");
        }

        private async Task<IEnumerable<ProgramResponseDTO>> SearchPrograms(string searchQuery, bool getAll)
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

            var result = new PaginationModel<ProgramResponseDTO>();

            var filter = new FilterDTO()
            {
                Filter = new Filter()
                {
                    Logic = "or",
                    Filters = filterList
                },
                page = 1,
                pageSize = int.MaxValue
            };

            result = _user.Student.TransferProgram == null ? await ProgramService.GetListForSearch(filter, getAll) : await ProgramService.GetListForSearchByExpertiseBranchId(filter, (long)_user.Student.TransferProgram.ExpertiseBranch.Id, getAll);

            return result.Items ?? new List<ProgramResponseDTO>();

        }
        private async Task OnChangeProcessDate(DateTime? e)
        {

            if (e == null)
            {
                _user.Student.Curriculum = new();
            }
            else
            {
                _processDate = e;
                if (_user?.Student?.FirstInsStartDate == null)
                {
                    try
                    {
                        var response = await CurriculumService
                            .GetLatestByBeginningDateAndExpertiseBranchId((long)(programId != null ? _programFromYUEP.ExpertiseBranch.Id : _user.Student.Program.ExpertiseBranch.Id), _processDate.Value);
                        if (response.Result)
                        {
                            _user.Student.Curriculum = response.Item;
                            _user.Student.CurriculumId = response.Item.Id;
                        }
                    }
                    catch (Exception a)
                    {
                        Console.WriteLine(a);
                    }
                    StateHasChanged();
                }
            }
        }

        private async Task OnChangeTransferProcessDate(DateTime? e)
        {
            if (e == null)
            {
                _user.Student.Curriculum = new();
            }
            else
            {
                _user.Student.FirstInsStartDate = e;
                try
                {
                    var response = await CurriculumService
                        .GetLatestByBeginningDateAndExpertiseBranchId((long)(programId != null ? _programFromYUEP.ExpertiseBranch.Id : _user.Student.TransferProgram.ExpertiseBranch.Id), _user.Student.FirstInsStartDate.Value);
                    if (response.Result)
                    {
                        _user.Student.Curriculum = response.Item;
                        _user.Student.CurriculumId = response.Item.Id;
                    }
                }
                catch (Exception a)
                {
                    Console.WriteLine(a);
                }
                StateHasChanged();
            }
        }

        private void OnChangeProgram(ProgramResponseDTO program)
        {
            if (_user.Student.ProgramId != null && _user.Student.ProgramId == program.Id)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "", "Bu öğrenci zaten bu programa kayıtlı görünüyor.");
                return;
            }
            _user.Student.Program = program;
            _user.Student.ProgramId = program.Id;
            _curriculum = null;
            StateHasChanged();
        }

        private void OnChangeTransferProgram(ProgramResponseDTO program)
        {
            _user.Student.TransferProgram = program;
            _user.Student.TransferProgramId = program.Id;
            _user.Student.Program = null;
            _user.Student.ProgramId = null;
            StateHasChanged();
        }

        private void OnChangeCountry(CountryResponseDTO country)
        {
            _user.Country = country;
            _user.CountryId = country.Id;
            StateHasChanged();
        }

        //private async Task<CurriculumResponseDTO> SearchCurriculum()
        //{
        //    var result = await CurriculumService.GetLatestByBeginningDateAndExpertiseBranchId((long)_program.ExpertiseBranchId, _processDate.Value);
        //    return result.Result ? result.Item : new CurriculumResponseDTO();
        //}

        //private void OnChangeCurriculum(CurriculumResponseDTO curriculum)
        //{
        //    _curriculum = curriculum;
        //    _curriculum.Id = curriculum.Id;
        //    StateHasChanged();
        //}

        private void IdentyNoChanged(string id)
        {
            _identityNo = id;
        }

        private void CancelUser()
        {
            _user = null;
            _searchingIdentity = false;
            _program = null;
            _curriculum = null;
            _tcValidatorMessage = null;
            _programValidatorMessage = null;
            _curriculumValidatorMessage = null;
            _beginningExamValidatorMessage = null;
            _periodTypeValidatorMessage = null;
            _placementScoreValidatorMessage = null;
            _placementYearValidatorMessage = null;
            _identityNo = String.Empty;
            _showCaptcha = true;
            _captchaValidatorMessage = string.Empty;
            inputCaptcha = string.Empty;
            StateHasChanged();
        }

        private string GetProgramClass()
        {
            return "form-control " + _user.Student.ProgramId is null ? "invalid" : "";
        }

        private List<StudentExpertiseBranchResponseDTO> GetStudentExpertiseBranches(List<DoctorExpertiseBranch> doctorExpertiseBranches)
        {
            List<StudentExpertiseBranchResponseDTO> eebs = new List<StudentExpertiseBranchResponseDTO>();

            if (doctorExpertiseBranches?.Count > 0)
            {
                foreach (var item in doctorExpertiseBranches)
                {
                    eebs.Add(new StudentExpertiseBranchResponseDTO()
                    {
                        ExpertiseBranchName = item.ExpertiseBranchName,
                        RegistrationDate = DateTime.ParseExact(item.RegistrationDate, "dd.MM.yyyy",
                 System.Globalization.CultureInfo.InvariantCulture),
                        RegistrationNo = item.RegistrationNo,
                        ExpertiseBranchId = item.ExpertiseBranchId,
                        IsPrincipal = item.IsPrincipal
                    });
                }
            }
            return eebs;
        }

        private void OnChangeNewBranch(long? id, StudentExpertiseBranchResponseDTO edx)
        {
            if (principalBranches.Any(x => x.Id == id))
            {
                edx.IsPrincipal = true;
            }
            else
                edx.IsPrincipal = false;
            edx.ExpertiseBranchId = id;
            StateHasChanged();

        }
        private async Task<IEnumerable<CountryResponseDTO>> SearchCountries(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", searchQuery, "and").Sort("Name");

            var result = await CountryService.GetPaginateList(filter);

            return result.Items ?? new List<CountryResponseDTO>();
        }
        private async Task OnChangeNative()
        {
            _isNative = !_isNative;
            _foreignType = null;

            if (!_isNative)
            {
                _user.Country = null;
                _user.CountryId = null;
            }
            else
            {
                countryResponse = await CountryService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", "türkiye", "and"));
                _user.Country = countryResponse.Items?.FirstOrDefault();
                _user.CountryId = countryResponse.Items?.FirstOrDefault().Id;
            }

            StateHasChanged();
        }
        private async void ReloadCaptcha()
        {
            captcha = null;
            captcha = (await CaptchaService.Get()).Item;
            _captchaValidatorMessage = string.Empty;
            //_model.CaptchaChiper = captcha.Chiper;
            StateHasChanged();
        }
    }
}