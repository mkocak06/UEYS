using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.ResponseModels.Captcha;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using UI.Helper;
using Shared.FilterModels.Base;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Fluxor;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.Pages.InstitutionManagement.Universities;
using UI.Pages.User.Educator.Tabs;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;
using Humanizer;

namespace UI.Pages.User.Educator
{

    public partial class AddEducator
    {
        [Parameter] public long? programId { get; set; }
        [Inject] public IState<AppState> AppState { get; set; }
        [Inject] public IEducatorService EducatorService { get; set; }
        [Inject] ITitleService TitleService { get; set; }
        [Inject] public IUserService UserService { get; set; }
        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IEducatorProgramService EducatorProgramService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IHospitalService HospitalService { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public ICaptchaService CaptchaService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IConfiguration Configuration { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        public EducatorOverview EducatorOverviewComponent { get; set; }
        private Dropzone dropzone;
        private Dropzone dropzoneDeclaration;
        private Dropzone dropzoneProgram;
        private Dropzone dropzoneOfficer;
        private Dropzone dropzoneChairman;
        private Dropzone dropzoneMember;
        private MyModal UploaderModal;
        private List<DocumentResponseDTO> responseDocuments = new();
        private bool _fileLoaded = true;

        private EducatorResponseDTO _educator = new();
        private bool _saving;
        private List<BreadCrumbLink> _links;
        private EducatorProgramResponseDTO _educatorProgram = new();
        private EducatorProgramResponseDTO _selectedEducatorProgram = new();
        private EducationOfficerResponseDTO _educationOfficer = new();
        private UserResponseDTO _user;
        private ProgramResponseDTO _program;
        private ProgramResponseDTO _newProgram;
        private CurriculumResponseDTO _curriculum;
        private TitleResponseDTO _academicTitle = new();
        private TitleResponseDTO _staffTitle = new();
        private List<TitleResponseDTO> staffTitles = new();
        private TitleResponseDTO _selectedStaffTitle = new();
        private List<TitleResponseDTO> academicTitles = new();
        private List<TitleResponseDTO> administrativeTitles = new();
        private List<ExpertiseBranchResponseDTO> allExpBranches = new();
        private List<ExpertiseBranchResponseDTO> expertiseBranches = new();
        private ExpertiseBranchResponseDTO _selectedBranch = new();
        private TitleResponseDTO _selectedAcademicTitle = new();
        private List<EducatorProgramResponseDTO> _selectedEducatorPrograms = new() { new EducatorProgramResponseDTO() { DutyType = DutyType.PermanentDuty } };
        private List<EducationOfficerResponseDTO> _educationOfficers = new();
        private List<DocumentResponseDTO> _educatorDocuments = new();
        private EditContext _ec;
        private bool _isConditionalEducator;
        private bool _isForensicMedicineInstitutionEducator;
        private DateTime? _titleDate;
        private DateTime? _membershipStartDate;
        private DateTime? _membershipEndDate;
        private bool isDisabled = false;
        private MyModal _educatorProgramModal;
        private EditContext _ecForProgram;
        private string _documentValidatorMessage;
        private string _declarationDocumentValidatorMessage;
        private string _chairmanDocumentValidatorMessage;
        private string _memberDocumentValidatorMessage;
        private string _dutyPlaceValidatorMessage;
        private string _educationOfficerDocumentValidatorMessage;

        private string _identityNo = String.Empty;
        private string _tcValidatorMessage;
        private string _staffTitleValidatorMessage;
        private bool _searchingIdentity;
        private bool _isKpsResult = false;
        private bool _isAdmin = false;
        private InputMask PhoneInput;
        private List<TitleResponseDTO> _selectedAdministrativeTitles = new();
        private string _programStartDateValidatorMessage;
        private List<HospitalResponseDTO> _staffInstitutions;
        private List<UniversityResponseDTO> _staffParentInstitutions;
        private CaptchaResponseDTO captcha;
        private string inputCaptcha = string.Empty;
        private string _captchaValidatorMessage;
        private bool _showCaptcha = true;
        private bool isDomainAllowed;
        private bool _existRegistrationNo = false;
        private long? existEducatorId = null;
        private bool _loadingCKYSInfo = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _ecForProgram = new EditContext(_educatorProgram);
            var universityResponse = await UniversityService.GetAll();
            _staffParentInstitutions = universityResponse.Item;
            var hospitalResponse = await HospitalService.GetAll();
            _staffInstitutions = hospitalResponse.Item;
            if (programId is not null)
            {
                var response = await ProgramService.GetById(programId.Value);
                if (response.Result)
                    _program = response.Item;

                StateHasChanged();
            }
            _curriculum = new CurriculumResponseDTO();
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
                    To = "/educator/educators",
                    OrderIndex = 1,
                    Title = L["_List", L["Educator"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = @L["add_new", L["Educator"]]
                }
        };
            isDomainAllowed = IsDomainAllowed();
            if (isDomainAllowed)
                captcha = (await CaptchaService.Get()).Item;
        }
        private bool IsDomainAllowed()
        {
            var allowedDomains = Configuration["AllowedDomain"];
            var currentDomain = new Uri(NavigationManager.Uri).Host;
            return allowedDomains.Contains(currentDomain);
        }
        private void OnRadioChange(object sender)
        {
            _isAdmin = (bool)sender;
            StateHasChanged();
            if (_isAdmin)
                SweetAlert.IconAlert(SweetAlertIcon.info, "Emin Misiniz?", "Eklediğiniz Eğitici \"Birim Eğitim Sorumlusu\" Mu? Bu değişiklik programdaki diğer sorumluyu da etkileyecek!");
        }
        public async Task<bool> CallDropzone()
        {
            _educatorDocuments ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorDocuments.Add(result.Item);
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
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                dropzone.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneProgram()
        {
            _educatorProgram.Documents ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneProgram.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorProgram.Documents.Add(result.Item);
                    _educatorProgram.DocumentOrder = result.Item.BucketKey;
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
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                dropzoneProgram.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneOfficer()
        {
            _educationOfficer.Documents ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneOfficer.SubmitFileAsync();
                if (result.Result)
                {
                    _educationOfficer.Documents.Add(result.Item);
                    _educationOfficer.DocumentOrder = result.Item.BucketKey;
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
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                dropzoneOfficer.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneDeclaration()
        {
            _educatorDocuments ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneDeclaration.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorDocuments.Add(result.Item);
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
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                dropzoneDeclaration.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneChairman()
        {
            _educatorDocuments ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneChairman.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorDocuments.Add(result.Item);
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
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                dropzoneChairman.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneMember()
        {
            _educatorDocuments ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneMember.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorDocuments.Add(result.Item);
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
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                dropzoneMember.ResetStatus();
                StateHasChanged();
            }
        }

        private async void OnUpdateDropzone(bool invoke)
        {
            if (invoke)
            {

                if (_user.Educator.IsConditionalEducator == true)
                {
                    if (string.IsNullOrEmpty(dropzone._selectedFileName))
                        _documentValidatorMessage = "Bu alan zorunludur!";
                    else
                        _documentValidatorMessage = string.Empty;
                }
            }
            else
                _documentValidatorMessage = "Bu alan zorunludur!";
            StateHasChanged();
        }
        private async void OnUpdateDropzoneDeclaration(bool invoke)
        {
            if (invoke)
            {
                if (_user.Educator.IsConditionalEducator == true)
                {
                    if (string.IsNullOrEmpty(dropzoneDeclaration._selectedFileName))
                        _declarationDocumentValidatorMessage = "Bu alan zorunludur!";
                    else
                        _declarationDocumentValidatorMessage = string.Empty;
                }
            }
            else
                _declarationDocumentValidatorMessage = "Bu alan zorunludur!";
            StateHasChanged();
        }

        private async void OnUpdateDropzoneChairman(bool invoke)
        {
            if (invoke)
            {
                if (_user.Educator.IsChairman == true)
                {
                    if (string.IsNullOrEmpty(dropzoneChairman._selectedFileName))
                        _chairmanDocumentValidatorMessage = "Bu alan zorunludur!";
                    else
                        _chairmanDocumentValidatorMessage = string.Empty;
                }
            }
            else
                _chairmanDocumentValidatorMessage = "Bu alan zorunludur!";
            StateHasChanged();
        }

        private async void OnUpdateDropzoneMember(bool invoke)
        {
            if (invoke)
            {
                if (_user.Educator.IsChairman == false)
                {
                    if (string.IsNullOrEmpty(dropzoneMember._selectedFileName))
                        _memberDocumentValidatorMessage = "Bu alan zorunludur!";
                    else
                        _memberDocumentValidatorMessage = string.Empty;
                }
            }
            else
                _memberDocumentValidatorMessage = "Bu alan zorunludur!";
            StateHasChanged();
        }
        private void CheckConditionalEducator()
        {
            _documentValidatorMessage = string.Empty;
            _declarationDocumentValidatorMessage = string.Empty;
            StateHasChanged();

            if (_user.Educator.IsConditionalEducator == true)
            {
                _documentValidatorMessage = "Bu alan zorunludur!";
                _declarationDocumentValidatorMessage = "Bu alan zorunludur!";

            }
            else
            {
                _user.Educator.TitleDate = null;
                dropzone.ResetStatus();
                dropzoneDeclaration.ResetStatus();

            }
            StateHasChanged();
        }
        private async Task Save()
        {
            _tcValidatorMessage = null;
            _staffTitleValidatorMessage = null;
            _documentValidatorMessage = string.Empty;
            _declarationDocumentValidatorMessage = string.Empty;
            _chairmanDocumentValidatorMessage = string.Empty;
            _memberDocumentValidatorMessage = string.Empty;
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



            if (_user.Educator.IsConditionalEducator == true)
            {

                if (string.IsNullOrEmpty(dropzone._selectedFileName) && _educatorDocuments?.Any(x => x.DocumentType == DocumentTypes.AssociateProfessorship) == false)
                {
                    _documentValidatorMessage = "Bu alan zorunludur!";
                    StateHasChanged();
                    return;
                }
                else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                {
                    var result = await CallDropzone();
                    if (!result)
                        return;
                }

                if (string.IsNullOrEmpty(dropzoneDeclaration._selectedFileName) && _educatorDocuments?.Any(x => x.DocumentType == DocumentTypes.DeclarationDocument) == false)
                {
                    _declarationDocumentValidatorMessage = "Bu alan zorunludur!";
                    StateHasChanged();
                    return;
                }
                else if (!string.IsNullOrEmpty(dropzoneDeclaration._selectedFileName))
                {
                    var result = await CallDropzoneDeclaration();
                    if (!result)
                        return;
                }
            }

            if (_isForensicMedicineInstitutionEducator == true)
            {
                if (_user.Educator.IsChairman == true)
                {
                    if (string.IsNullOrEmpty(dropzoneChairman._selectedFileName) && _educatorDocuments?.Any(x => x.DocumentType == DocumentTypes.SpecializationBoardChairman) == false)
                    {
                        _chairmanDocumentValidatorMessage = "Bu alan zorunludur!";
                        StateHasChanged();
                        return;
                    }
                    else if (!string.IsNullOrEmpty(dropzoneChairman._selectedFileName))
                        await CallDropzoneChairman();
                }
                else
                {
                    if (string.IsNullOrEmpty(dropzoneMember._selectedFileName) && _educatorDocuments?.Any(x => x.DocumentType == DocumentTypes.SpecializationBoardMember) == false)
                    {
                        _memberDocumentValidatorMessage = "Bu alan zorunludur!";
                        StateHasChanged();
                        return;
                    }
                    else if (!string.IsNullOrEmpty(dropzoneMember._selectedFileName))
                        await CallDropzoneMember();
                }
            }

            _saving = true;
            StateHasChanged();

            _user.Educator.Documents = _educatorDocuments;
            _user.Educator.EducationOfficers = _educationOfficers;

            UserDTO userDTO = Mapper.Map<UserDTO>(_user);

            try
            {
                var response = await UserService.AddUserWithEducatorInfo(userDTO);
                if (response.Result)
                {
                    if (programId != null)
                        NavigationManager.NavigateTo($"./institution-management/programs/{programId}");
                    else
                        NavigationManager.NavigateTo($"./educator/educators/{response.Item.Educator.Id}");
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured."]);
                Console.WriteLine(e);
            }
            finally
            {
                _saving = false;
                CancelUser();
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

            var finalResult = new PaginationModel<ProgramResponseDTO>() { Items = new List<ProgramResponseDTO>() };
            var expBrIdList = _user.Educator?.EducatorExpertiseBranches.Select(x => x.ExpertiseBranchId).ToList();
            var hospitalId = _user.Educator?.EducatorPrograms?.FirstOrDefault(x => x.DutyEndDate == null || x.DutyEndDate <= DateTime.UtcNow)?.Program?.Hospital?.Id;

            if (_user.Educator?.EducatorPrograms?.Any() == true)
            {
                foreach (var item in _user.Educator.EducatorPrograms)
                {
                    if (_user.Educator?.EducatorExpertiseBranches?.Any() == true)
                    {
                        foreach (var eduExpBr in _user.Educator.EducatorExpertiseBranches.Where(x => x.ExpertiseBranchId != null))
                        {
                            if (eduExpBr.ExpertiseBranch?.IsPrincipal == item.Program.ExpertiseBranch.IsPrincipal && item.DutyEndDate == null)
                            {
                                expBrIdList.Remove(eduExpBr.ExpertiseBranchId);
                            }
                        }
                    }
                }
            }

            if (programId == null)
            {
                foreach (var item in expBrIdList)
                {
                    if (hospitalId != null)
                    {
                        var result = await ProgramService.GetByHospitalAndExpertiseBranchId((long)hospitalId, (long)item);
                        if (result != null)
                        {
                            var result_1 = result.Item.Name.ToLower().Contains(searchQuery.ToLower());
                            if (result_1)
                                finalResult.Items.Add(result.Item);
                        }
                    }
                    else
                    {
                        var result = await ProgramService.GetListForSearchByExpertiseBranchId(new FilterDTO()
                        {
                            Filter = new Filter()
                            {
                                Logic = "or",
                                Filters = filterList
                            },
                            page = 1,
                            pageSize = int.MaxValue
                        }, (long)item, true);

                        if (result.Items != null)
                            foreach (var item_1 in result.Items)
                                finalResult.Items.Add(item_1);
                    }
                }
            }
            else
            {
                var result = await ProgramService.GetByHospitalAndExpertiseBranchId((long)_program.HospitalId, (long)_program.ExpertiseBranchId);
                if (result != null)
                {
                    var result_1 = result.Item.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture));
                    if (result_1)
                        finalResult.Items.Add(result.Item);
                }
            }
            return finalResult.Items.Where(x => _user.Educator?.EducatorPrograms?.Select(x => x.ProgramId)?.Contains(x.Id) == false).ToList()
                ?? new List<ProgramResponseDTO>();
        }
        private string GetProgramClass()
        {
            return "form-control " + _newProgram is not null ? "invalid" : "";
        }
        private void OnChangeProgram(ProgramResponseDTO program)
        {
            _newProgram = program;
            _newProgram.Id = program.Id;
            StateHasChanged();
        }

        private void OnChangeEducatorProgram(ProgramResponseDTO program)
        {
            if (_user.Educator?.EducatorPrograms?.Any(a => a.ProgramId != null && a.ProgramId == program.Id) == true)
            {
                if (_user.Educator?.IsDeleted == true)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.warning, "", "Bu programa kayıtlı pasif bir eğitici kaydı bulunmaktadır. Lütfen başka bir program seçiniz.");
                    _program = new();
                    return;
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.warning, "", "Bu programa kayıtlı aktif bir eğitici kaydı bulunmaktadır.");
                    return;
                }
            }
            _educatorProgram.Program = program;
            _educatorProgram.ProgramId = program.Id;
            StateHasChanged();
        }
        private async Task<IEnumerable<TitleResponseDTO>> SearchAcademicTitles(string searchQuery)
        {
            var result = await TitleService.GetListByType(TitleType.Academic);

            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<TitleResponseDTO>();

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
        private void OnChangeNewBranch(long? id, EducatorExpertiseBranchResponseDTO edx)
        {
            edx.ExpertiseBranchId = id;
            //edx.IsPrincipal = allExpBranches.FirstOrDefault(x => x.Id == id).IsPrincipal;

            edx.SubBranchIds = allExpBranches.FirstOrDefault(x => x.Id == edx.ExpertiseBranchId).SubBranches?.Select(x => (long)x.SubBranchId)?.ToList();

            if (edx.SubBranchIds?.Count > 0)
            {
                foreach (var subId in edx.SubBranchIds)
                {
                    if (!expertiseBranches.Select(x => x.Id).Contains(subId))
                        expertiseBranches.Add(allExpBranches.FirstOrDefault(x => x.Id == subId));
                }
            }
            //expertiseBranches = expertiseBranches.OrderBy(x => x.Name).ToList();

            StateHasChanged();
        }
        private void PhoneNoChanged(string tel)
        {
            _user.Phone = tel;
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

            var titleResponse = await TitleService.GetAll();
            if (titleResponse.Result)
            {
                academicTitles = titleResponse.Item.Where(x => x.TitleType == TitleType.Academic).ToList();
                staffTitles = titleResponse.Item.Where(x => x.TitleType == TitleType.Staff).ToList();
                administrativeTitles = titleResponse.Item.Where(x => x.TitleType == TitleType.Administrative).ToList();
            }

            var expResponse = await ExpertiseBranchService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue)
                                                                                    .Sort("Name", Shared.Types.SortType.ASC));
            if (expResponse.Items != null)
            {
                allExpBranches = expResponse.Items;
                expertiseBranches = allExpBranches.Where(x => x.IsPrincipal == true).ToList();
            }

            try
            {
                var response = await UserService.GetUserByIdentityNoWithEducationalInfo(_identityNo);
                if (response.Result)
                {
                    if (response.Item.IsDeleted)
                    {
                        //var confirm = await SweetAlert.ConfirmAlert($"{L["Pasif Kullanıcı Bulundu"]}",
                        //$"{L["Girmiş olduğunuz kimlik numarasına sahip kullanıcı sistemde pasif olarak bulunmaktadır. Bu kullanıcıyı aktifleştirerek devam etmek istiyor musunuz?"]}", SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["No"]}");

                        //if (confirm)
                        //{
                        //    var result = await UserService.UpdateActivePassiveUser(response.Item.Id);
                        //    if (result)
                        //        SweetAlert.ToastAlert(SweetAlertIcon.success, "Kullanıcı Aktifleştirildi!");
                        //    else
                        //        SweetAlert.ToastAlert(SweetAlertIcon.error, "Bir hata oluştu");
                        //}
                        //else
                        //{
                        //    CancelUser();
                        //    return;
                        //}
                    }
                    if (response.Item.Educator?.EducatorType == EducatorType.NotInstructor)
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.warning, "", "Bu TCKN'ye sahip kullanıcının tez danışmanı olarak zaten aktif bir kaydı bulunmaktadır.");
                        CancelUser();
                        return;
                    }

                    if (programId != null && programId > 0 && response.Item.Educator?.EducatorPrograms?.Any(a => a.ProgramId == programId) == true)
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.warning, "", "Eklemek istediğiniz eğitici bu programa pasif olarak zaten kayıtlı görünüyor. Lütfen başka bir program seçiniz.");
                        CancelUser();
                        return;
                    }
                    _user = response.Item;
                    if (response.Item.IsDeleted)
                        _user.IsDeleted = false;
                    _user.Educator ??= new() { EducatorType = EducatorType.Instructor };

                    _user.Phone ??= _user.CKYSDoctorResult?.Phone ?? "";
                    _user.Email ??= _user.CKYSDoctorResult?.Email ?? "";

                    _user.Educator.EducatorExpertiseBranches = GetEducatorExpertiseBranches(_user.CKYSDoctorResult?.DoctorExpertiseBranches) ?? new();

                    if (_user.Educator.EducatorExpertiseBranches?.Any(x => string.IsNullOrEmpty(x.RegistrationNo)) == true)
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.warning, "Tescil Bilgisi Bulunamadı!", "Kişinin tescil bilgisi ÇKYS'de kayıtlı değildir. \r\nÇKYS ile iletişime geçilmesi ve tescil bilgisinin tamamlanması gerekmektedir. Sonrasında kişiyi eğitici olarak ekleyebilirsiniz.");
                        CancelUser();
                        return;
                    }
                    else
                    {
                        _existRegistrationNo = true;
                        _user.Educator.IsConditionalEducator = false;//nedir bu
                        StateHasChanged();
                    }

                    _user.Educator.EducatorPrograms ??= new();
                    _user.Educator.StaffParentInstitutions = new();
                    _user.Educator.StaffInstitutions = new();
                    _user.Educator.StaffTitleId = _user.CKYSDoctorResult?.StaffTitleId;
                    _user.Educator.AcademicTitleId = _user.YOKResult?.AcademicTitleId;
                    _user.Educator.GraduationDetails = _user.EgitimBilgisiResult;
                    existEducatorId = _user.EducatorId;

                    SetExpertiseBranchList();

                    //if (_user.EgitimBilgisiResult?.Where(x => x.IsPhd == true).ToList() != null)
                    //    foreach (var item in _user.EgitimBilgisiResult?.Where(x => x.IsPhd == true).ToList())
                    //        if (item.GraduationDate != null && (item.GraduationDate != "" ? DateTime.ParseExact(item.GraduationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null) < tueyDate)
                    //            isDisabled = false; StateHasChanged();

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

        private void SetExpertiseBranchList()
        {
            if (_user.Educator.EducatorExpertiseBranches?.Any() == true)
            {
                foreach (var eduExpBr in _user.Educator.EducatorExpertiseBranches)
                    if (eduExpBr.ExpertiseBranchId != null && eduExpBr.SubBranchIds?.Count > 0)
                        foreach (var subId in eduExpBr.SubBranchIds)
                            if (!expertiseBranches.Any(x => x.Id == subId))
                                expertiseBranches.Add(allExpBranches.FirstOrDefault(x => x.Id == subId));
            }
        }

        private string GetIdentiyClass()
        {
            return "form-control " + (!string.IsNullOrEmpty(_tcValidatorMessage) ? "invalid" : "");
        }
        private void IdentyNoChanged(string id)
        {
            _identityNo = id;
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
        private void OnChangeStaffTitle(ChangeEventArgs args)
        {
            if (args.Value == null) return;
            var value = (string)args.Value;

            _selectedStaffTitle = staffTitles.Where(x => x.Name == value).FirstOrDefault();
        }
        private void OnChangeAcademicTitle(ChangeEventArgs args)
        {
            if (args.Value == null) return;
            var value = (string)args.Value;

            _selectedAcademicTitle = academicTitles.Where(x => x.Name == value).FirstOrDefault();
        }

        private async Task<IEnumerable<EducatorAdministrativeTitleResponseDTO>> SearchAdministrativeTitles(string searchQuery)
        {
            var result = await TitleService.GetListByType(TitleType.Administrative);

            List<EducatorAdministrativeTitleResponseDTO> adminTitles = new();
            if (result.Result)
            {
                result.Item.ForEach(x => adminTitles.Add(new EducatorAdministrativeTitleResponseDTO() { AdministrativeTitle = x, AdministrativeTitleId = x.Id }));

                return result.Result ? adminTitles.Where(x => x.AdministrativeTitle.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) : new List<EducatorAdministrativeTitleResponseDTO>();
            }
            return new List<EducatorAdministrativeTitleResponseDTO>();

        }

        private void OnChangeAdministrativeTitle(IList<TitleResponseDTO> values)
        {
            _selectedAdministrativeTitles = values.ToList();
        }

        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitutions(string searchQuery)
        {
            var result = await InstitutionService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<InstitutionResponseDTO>();
        }
        private void OnChangeInstitution(InstitutionResponseDTO institution)
        {
            _user.Institution = institution;
            _user.InstitutionId = institution?.Id ?? 0;
        }
        private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculties(string searchQuery)
        {
            var result = await FacultyService.GetAll();
            return result.Result ? result.Item.OrderBy(x => x.Name).Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(100) : new List<FacultyResponseDTO>();
        }

        //private void OnChangeFaculty(FacultyResponseDTO faculty)
        //{
        //    _user.Faculty = faculty;
        //    _user.FacultyId = faculty?.Id ?? 0;
        //}
        private void CancelUser()
        {
            _searchingIdentity = false;
            _user = null;
            _newProgram = null;
            _curriculum = null;
            _tcValidatorMessage = null;
            _identityNo = String.Empty;
            _showCaptcha = true;
            _captchaValidatorMessage = string.Empty;
            inputCaptcha = string.Empty;
            if (isDomainAllowed)
                ReloadCaptcha();
            StateHasChanged();
        }

        private async Task SaveEducatorProgram()
        {
            _dutyPlaceValidatorMessage = string.Empty;
            _educationOfficerDocumentValidatorMessage = string.Empty;
            _ecForProgram.GetValidationMessages().PrintJson("messages");
            if (!_ecForProgram.Validate())
                return;

            //if (_educatorPrograms.Where(x => x.DutyEndDate == null)?.Any(x => x.DutyType == DutyType.PermanentDuty) == false && _educatorProgram.DutyType == DutyType.TemporaryDuty)
            //{
            //    await SweetAlert.ConfirmAlert($"{L["Warning"]}",
            //    "En az 1 Asli Görev Yeri Seçmelisiniz!",
            //    SweetAlertIcon.warning, false, $"{L["Okey"]}", $"{L["Cancel"]}");
            //    _dutyPlaceValidatorMessage = "En az 1 Asli Görev Yeri Seçmelisiniz!";
            //    StateHasChanged();
            //    return;
            //}

            var lastDate = _user.Educator?.EducatorPrograms?.OrderByDescending(x => x.DutyEndDate)?.FirstOrDefault(x => x.Program.ExpertiseBranchId == _educatorProgram.Program.ExpertiseBranchId && x.DutyEndDate != null)?.DutyEndDate;
            if (lastDate != null && _educatorProgram.DutyStartDate < lastDate)
            {
                _programStartDateValidatorMessage = L["Dates don't match!"];
                return;
            }

            if (_educatorProgram.IsEducationOfficer == true)
            {
                var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
               $"{L["Are you sure you want to make this person education officer of the program?"]}",
               SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["No"]}");

                if (confirm)
                {
                    _educationOfficer = new EducationOfficerResponseDTO { StartDate = _educatorProgram.DutyStartDate, ProgramId = _educatorProgram.Program.Id };

                    if (string.IsNullOrEmpty(dropzoneOfficer._selectedFileName))
                    {
                        _educationOfficerDocumentValidatorMessage = L["File upload is required"];
                        _educationOfficer = new();
                        return;
                    }
                    else
                        await CallDropzoneOfficer();
                    _educationOfficers.Add(_educationOfficer);
                }
                else
                    return;
            }

            if (!string.IsNullOrEmpty(dropzoneProgram._selectedFileName))
                await CallDropzoneProgram();

            StateHasChanged();

            _educatorProgram.ProgramId = _educatorProgram.Program.Id;

            _user.Educator.EducatorPrograms.Add(_educatorProgram);

            _educatorProgramModal.CloseModal();
            _educatorProgram = new();
            _educationOfficer = new();

            StateHasChanged();
        }

        private async void OnOpenEducatorProgramAddModal()
        {
            if (_user.Educator?.EducatorExpertiseBranches?.Any() == false || (_user.Educator?.EducatorExpertiseBranches?.Any(x => x.ExpertiseBranchId == null) == true))
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, $"{L["Warning"]}",
                $"{L["To add program, you have to select an expertise branch."]}");
                return;
            }

            if (programId != null)
            {
                if (_user.Educator?.EducatorExpertiseBranches?.Any(x => x.ExpertiseBranchId == _program.ExpertiseBranchId) == false)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.warning, $"{L["Warning"]}",
                    $"{L["To add a program, you must add the expertise branch in the upper right corner of the page to the Academic Information section."]}");
                    return;
                }
            }

            _educatorProgram = new() { Documents = new List<DocumentResponseDTO>(), EducationOfficerDocuments = new List<DocumentResponseDTO>() };
            _ecForProgram = new EditContext(_educatorProgram);
            _educatorProgramModal.OpenModal();
        }


        private async Task OnRemoveEducatorExpertiseBranch(EducatorExpertiseBranchResponseDTO edx)
        {
            List<EducatorExpertiseBranchResponseDTO> afterRemovedlist = _user.Educator.EducatorExpertiseBranches.Clone();
            afterRemovedlist.Remove(afterRemovedlist.FirstOrDefault(x => x.ExpertiseBranchId == edx.ExpertiseBranchId));
            if (_user.Educator.EducatorPrograms.Any(x => x.Program.ExpertiseBranchId == edx.ExpertiseBranchId))
            {
                await SweetAlert.ConfirmAlert($"{L["Warning"]}",
                $"{L["You have to delete the related program record first, to delete this record."]}",
                SweetAlertIcon.warning, false, $"{L["Okey"]}", $"{L["Cancel"]}");
                StateHasChanged();
                return;
            }

            if (edx.SubBranchIds?.Count > 0)
            {
                foreach (var subBranchId in edx.SubBranchIds)
                {
                    if (_user.Educator.EducatorExpertiseBranches.Any(x => x.ExpertiseBranchId == subBranchId) && !afterRemovedlist.SelectMany(x => x.SubBranchIds).Contains(subBranchId))
                    {
                        await SweetAlert.ConfirmAlert($"{L["Warning"]}",
                    $"{L["You have to delete the sub branches records first, to delete this record."]}",
                    SweetAlertIcon.warning, false, $"{L["Okey"]}", $"{L["Cancel"]}");
                        StateHasChanged();
                        return;
                    }
                }

                foreach (var item in edx.SubBranchIds)
                {
                    if (!afterRemovedlist.SelectMany(x => x.SubBranchIds).Distinct().Contains(item))
                        expertiseBranches.Remove(allExpBranches.FirstOrDefault(x => x.Id == item));
                }
            }

            _user.Educator.EducatorExpertiseBranches.Remove(edx);

            //expertiseBranches = expertiseBranches.OrderBy(x => x.Name).ToList();

            StateHasChanged();
        }

        private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
        {
            var result = await HospitalService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", searchQuery, "and").Sort("Name"));
            return result.Items ?? new List<HospitalResponseDTO>();
        }
        private async Task<IEnumerable<UniversityResponseDTO>> SearchUniversities(string searchQuery)
        {
            var result = await EducatorService.GetUniversityPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", searchQuery, "and").Sort("Name"));
            return result.Items ?? new List<UniversityResponseDTO>();
        }

        //private void OnChangeHospital(HospitalResponseDTO hospital)
        //{
        //    _selectedStaffInstitution = hospital;
        //}
        private async void ReloadCaptcha()
        {
            captcha = null;
            captcha = (await CaptchaService.Get()).Item;
            _captchaValidatorMessage = string.Empty;
            //_model.CaptchaChiper = captcha.Chiper;
            StateHasChanged();
        }

        private void RemoveEducatorProgram(EducatorProgramResponseDTO educatorProgram)
        {
            _user.Educator.EducatorPrograms.Remove(educatorProgram);

            if (educatorProgram.IsEducationOfficer == true)
                _educationOfficers.Remove(_educationOfficers.FirstOrDefault(x => x.ProgramId == educatorProgram.ProgramId));
            StateHasChanged();
        }

        protected override ValueTask DisposeAsyncCore(bool disposing)
        {
            if (disposing)
                Dispatcher.Dispatch(new EducatorClearStateAction());
            return base.DisposeAsyncCore(disposing);
        }

        private async Task GetCKYSInfoByIdentityNo()
        {
            _loadingCKYSInfo = true;
            StateHasChanged();
            try
            {
                var response = await UserService.GetCKYSInfoByIdentityNo(_identityNo);

                if (!response.Result)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
                    return;
                }
                else
                {
                    if (response.Item.StaffTitleId != null && response.Item.StaffTitleId != _user.Educator.StaffTitleId)
                    {
                        _user.Educator.StaffTitle ??= new();
                        _user.Educator.StaffTitleId = response.Item.StaffTitleId;
                        _user.Educator.StaffTitle.Id = response.Item.StaffTitleId;
                        _user.Educator.StaffTitle.Name = response.Item.StaffTitleName;
                    }
                    _user.Educator.EducatorExpertiseBranches = GetEducatorExpertiseBranches(response.Item.DoctorExpertiseBranches) ?? new();
                    _user.Phone = response.Item.Phone;
                    _user.Email = response.Item.Email;

                    SetExpertiseBranchList();
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                SweetAlert.ErrorAlert();
                throw;
            }
            finally
            {
                _loadingCKYSInfo = false;
                StateHasChanged();
            }
        }
    }
}