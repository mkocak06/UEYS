using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Shared.Constants;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Authorization;
using Shared.ResponseModels.Captcha;
using Shared.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;
using IAuthService = UI.Services.IAuthService;

namespace UI.Pages.User
{
    public partial class User
    {
        [Inject] public IState<AppState> AppState { get; set; }
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private IAuthenticationService AuthenticationService { get; set; }
        [Inject] private IUserService UserService { get; set; }
        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public ICaptchaService CaptchaService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IConfiguration Configuration { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<UserRoleResponseDTO> _userRoles;
        private RoleResponseDTO _role;
        private List<UserForRegisterDTO> _Users;
        private List<UserPaginateResponseDTO> _users;
        private List<BreadCrumbLink> _links;
        private PaginationModel<UserPaginateResponseDTO> _paginationModel;

        private EditContext _ec;
        private EditContext _ecupdate;
        private EditContext _ecUserRole;

        private KPSResultResponseDTO _KPSResult;
        private UserAccountDetailInfoResponseDTO _userforUpdate;
        private UserAccountDetailInfoResponseDTO _userforAdd;
        private UserAccountDetailInfoResponseDTO _userConveyor = new();
        private UserRolesModel _userRoleModel = new() { UserRoleList = new() };
        private ZoneModelDTO _zoneModel;
        private IList<UserRoleUniversityResponseDTO> _userRoleUniversities;
        private List<ZoneRegisterDTO> _zoneRegister = new();
        private List<ZoneRegisterDTO> _zonesUserRole = new();
        private IList<UniversityResponseDTO> _zoneUniversities;
        private IList<ProvinceResponseDTO> _zoneProvinces;
        private IList<FacultyResponseDTO> _zoneFaculties;
        private IList<ProgramResponseDTO> _zonePrograms;
        private IList<HospitalResponseDTO> _zoneHospitals;
        private List<DocumentResponseDTO> _userDocuments;
        private List<string> _roles => AuthenticationService.Roles.Select(x => x.RoleName).ToList();
        private ExpertiseBranchResponseDTO _expertiseBranch = new();
        private UpdateUserAccountInfoDTO _updateuser;
        private UserResponseDTO _userForIdentityChange = new();
        private FilterDTO _filter;

        private InputText _focusTarget;
        private MyModal _userAddModal;
        private MyModal _userDetailModal;
        private MyModal _changeIdentityNoModal;
        private MyModal _roleModal;
        private InputMask _inputMask;

        private string _identityNo = String.Empty;
        private string _tcValidatorMessage;

        private bool _searchingIdentity;
        private bool _loadingFile;
        private bool _saving;
        private bool _loading;
        private bool _loaded;
        private bool forceRender;

        private CaptchaResponseDTO captcha;
        private string inputCaptcha = string.Empty;
        private string _captchaValidatorMessage;
        private bool _showCaptcha = true;
        private bool _zoneModelLoading;
        private bool isDomainAllowed;

        //private UserRoleBase UserFilter => UserDetailState.Value.UserFilter;

        private Dropzone dropzone;
        private bool _fileLoaded = true;
        private string _documentValidatorMessage;
        private string _addressValidatorMessage;
        private string _roleValidatorMessage;
        private string _address;

        protected override async Task OnInitializedAsync()
        {
            _userforUpdate = new UserAccountDetailInfoResponseDTO();
            _userforAdd = new UserAccountDetailInfoResponseDTO() { Roles = new List<RoleResponseDTO>() };
            _ecupdate = new EditContext(_userforUpdate);
            _ecUserRole = new EditContext(_userRoleModel);
            _ec = new EditContext(_userforAdd);
            _filter = new FilterDTO()
            {
                Filter = new Filter()
                {
                    Filters = new List<Filter>()
                    {
                        new Filter()
                        {
                          Field = "IsDeleted",
                          Operator = "eq",
                          Value = false
                        }
                    },
                    Logic = "and"
                },
                Sort = new[]{new Sort()
            {
                Dir = SortType.ASC,
                Field = "Name"
            }}
            };
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
                    IsActive = false,
                    To = "/User/users",
                    OrderIndex = 1,
                    Title = L["_List", L["User"]]
                }
        };
            isDomainAllowed = IsDomainAllowed();
            await GetUsers();
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("initializeSelectPicker");
            }
            if (forceRender)
            {
                forceRender = false;
                await JsRuntime.InvokeVoidAsync("initializeSelectPicker");

            }
        }
        private bool IsDomainAllowed()
        {
            var allowedDomains = Configuration["AllowedDomain"];
            var currentDomain = new Uri(NavigationManager.Uri).Host;
            return allowedDomains.Contains(currentDomain);
        }
        private async Task GetUsers()
        {
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _paginationModel = await AuthService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _users = _paginationModel.Items;
                StateHasChanged();
                forceRender = true;
                StateHasChanged();
                _loading = false;
            }
            else
            {
                _loading = false;
                SweetAlert.ErrorAlert();
            }
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetUsers();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetUsers();
        }
        private async Task DownloadExcelFile()
        {
            if (_loadingFile)
            {
                return;
            }
            _loadingFile = true;
            StateHasChanged();
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }

            var response = await AuthService.GetExcelByteArray(_filter);

            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"UserList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
                _loadingFile = false;
            }
            else
            {
                SweetAlert.ErrorAlert();
            }
            StateHasChanged();
        }

        private async Task OnUserAddList()
        {
            CancelUser();
            _ec = new EditContext(_userforAdd);
            if (isDomainAllowed)
            {
                try
                {
                    var response = await CaptchaService.Get();
                    if (response.Result)
                    {
                        captcha = response.Item;

                        StateHasChanged();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            _userAddModal.OpenModal();
        }

        public async Task<bool> CallDropzone()
        {
            _userDocuments ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    _userDocuments.Add(result.Item);
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
        private async Task AddUser()
        {
            //_saving = true;
            _documentValidatorMessage = string.Empty;
            //_roleValidatorMessage = string.Empty;
            StateHasChanged();

            if (!_ec.Validate())
                return;

            //if (_role == null || _role?.Id == 0)
            //{
            //    _roleValidatorMessage = "Bu alan zorunludur!";
            //    StateHasChanged();
            //    return;
            //}


            if (_userforAdd.UserRoles.Any(x => x.Role.Code == "KES"))
            {
                if (string.IsNullOrEmpty(dropzone._selectedFileName))
                {
                    _documentValidatorMessage = "Bu alan zorunludur!";
                    StateHasChanged();
                    return;
                }
                else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                    await CallDropzone();
            }
            _zonesUserRole = new();
            if (_userforAdd.UserRoles != null && _userforAdd.UserRoles.Count > 0)
            {
                _userforAdd.UserRoles.ForEach(x => _zonesUserRole.Add(new ZoneRegisterDTO()
                {
                    RoleId = x.Role.Id,
                    ZoneIds = x.UserRoleHospitals?.Select(a => a.HospitalId.Value)?.ToList() ??
                    x.UserRoleProvinces?.Select(a => a.ProvinceId.Value)?.ToList() ??
                    x.UserRoleUniversities?.Select(a => a.UniversityId.Value)?.ToList() ??
                    x.UserRolePrograms?.Select(a => a.ProgramId.Value)?.ToList() ??
                    x.UserRoleFaculties?.Select(a => a.FacultyId.Value)?.ToList() ?? new()

                }));

            }
            UserForRegisterDTO dto = new()
            {
                IdentityNo = _identityNo,
                Name = _userforAdd?.Name,
                //Address = _address,
                BirthPlace = _userforAdd?.BirthPlace,
                BirthDate = _userforAdd?.BirthDate?.Date,
                Email = _userforAdd?.Email,
                Phone = _userforAdd?.Phone,
                InstitutionId = _userforAdd?.InstitutionId,
                Nationality = _userforAdd?.KPSResult?.Nationality,
                Gender = _userforAdd?.Gender,
                Zones = _zonesUserRole,
                SelectedRoleId = _zonesUserRole.FirstOrDefault(x => x.RoleId != 0).RoleId,
                //RoleIds = _userforAdd?.Roles?.Select(x => x.Id)?.ToList(),
                Documents = Mapper.Map<List<DocumentDTO>>(_userDocuments)
            };
            try
            {
                var response = await AuthService.CreateUserAccount(dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");

                    await GetUsers();
                    _userAddModal.CloseModal();
                }
                else
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, L[$"{response.Message}"]);
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //_saving = false;
            _userDocuments = null;
            StateHasChanged();
        }

        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitutions(string searchQuery)
        {
            var result = await InstitutionService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<InstitutionResponseDTO>();
        }
        private async void OnChangeRole(RoleResponseDTO value)
        {
            _role = value;
            _role.PrintJson("ROLE.....:");
            await GetZoneByRole(value.Id);

            //_userforAdd.Roles = values.ToList();
            //_userforAdd.RoleIds = values.Select(x => x.Id).ToList();
        }


        private void OnChangeRoleForUpdate(RoleResponseDTO value)
        {
            _userforUpdate.UserRoles.FirstOrDefault(x => x.Role.Id == value.Id).Role = value;
            _userforUpdate.UserRoles.FirstOrDefault(x => x.Role.Id == value.Id).Role.Id = value.Id;
            StateHasChanged();
        }
        private void OnChangeUniversityByZone(IList<UniversityResponseDTO> values)
        {
            _zoneUniversities = values;
            _zoneRegister = new List<ZoneRegisterDTO>()
            {
                new ZoneRegisterDTO()
                {
                    RoleId = _role.Id,
                    ZoneIds = _zoneUniversities.Select(x => x.Id).ToList(),
                }
            };
        }
        private void AddNewRole()
        {
            _userRoleModel.UserRoleList ??= new();
            _userRoleModel.UserRoleList.Add(new UserRoleResponseDTO());
        }


        private void PhoneNoUpdated(string tel)
        {
            _userforUpdate.Phone = tel;
        }

        private void PhoneNoUpdatedForAdd(string tel)
        {
            _userforAdd.Phone = tel;
        }

        private void OnOpenRoleModal(UserAccountDetailInfoResponseDTO user)
        {
            _userConveyor = user;
            _userRoleModel.UserRoleList = user.UserRoles.Clone();
            _ecUserRole = new EditContext(_userRoleModel);
            StateHasChanged();
            _roleModal.OpenModal();
        }
        private async Task OnUserDetail(UserPaginateResponseDTO userResponse)
        {
            _userforUpdate = new UserAccountDetailInfoResponseDTO();
            _loaded = false;
            StateHasChanged();
            if (userResponse.Id != null && userResponse.Id > 0)
            {
                try
                {
                    var response = await UserService.GetById(userResponse.Id);
                    if (response.Result)
                    {
                        _userforUpdate = response.Item;
                        _expertiseBranch = _userforUpdate.UserRoles?.FirstOrDefault(x => x.Role?.Code == RoleCodeConstants.ANA_BILIM_DALI_BASKANI && x.UserRoleFaculties.Any())?.UserRoleFaculties?.FirstOrDefault(x => x.ExpertiseBranchId != null)?.ExpertiseBranch;
                        _ecupdate = new EditContext(_userforUpdate);
                        _userDetailModal.OpenModal();
                    }
                    else { SweetAlert.ErrorAlert(response.Message ?? "Bir hata oluştu"); }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    _loaded = true;
                    StateHasChanged();
                }

            }
        }
        private async Task<IEnumerable<RoleResponseDTO>> SearchRoles(string searchQuery)
        {
            var response = await AuthService.GetRolesByUserRole();

            return response.Result ? response.Item.Where(x => (_userRoleModel.UserRoleList?.Count > 0 && _userRoleModel.UserRoleList.Any(a => a.Role?.Id != null) ? !_userRoleModel.UserRoleList.Select(a => a.Role?.Id).Contains(x.Id) : true) && x.RoleName.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).OrderBy(x => x.RoleName) :
           //var result = await AuthService.Roles();
           //return result is not null ? result.Where(x => _userforAdd?.Roles?.Any(y => y.Id == x.Id) == false && x.RoleName.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
           new List<RoleResponseDTO>();
        }

        #region Searchs ByUserRole
        private async Task<IEnumerable<UserRoleProvinceResponseDTO>> SearchProvincesByUserRole(string searchQuery, UserRoleResponseDTO userRole)
        {
            var response = await AuthService.GetZone(userRole.Role.Id);

            List<UserRoleProvinceResponseDTO> urp = new();

            response.Item.Provinces.Where(x => userRole.UserRoleProvinces?.Count > 0 ? !userRole.UserRoleProvinces.Select(a => a.ProvinceId).Contains(x.Id) : true).ToList().ForEach(x =>
                urp.Add(new UserRoleProvinceResponseDTO() { Province = x, ProvinceId = x.Id }));

            return urp.Where(x => x.Province.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) ??
           new List<UserRoleProvinceResponseDTO>();
        }
        private async Task<IEnumerable<UserRoleUniversityResponseDTO>> SearchUniversitiesByUserRole(string searchQuery, UserRoleResponseDTO userRole)
        {
            var response = await AuthService.GetZone(userRole.Role.Id);

            List<UserRoleUniversityResponseDTO> uru = new();


            response.Item.Universities.Where(x => userRole.UserRoleUniversities?.Count > 0 ? !userRole.UserRoleUniversities.Select(a => a.UniversityId).Contains(x.Id) : true).ToList().ForEach(x =>
                uru.Add(new UserRoleUniversityResponseDTO() { University = x, UniversityId = x.Id }));

            return uru.Where(x => x.University.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) ??
           new List<UserRoleUniversityResponseDTO>();
        }
        private async Task<IEnumerable<FacultyResponseDTO>> SearchFacultiesByZone(string searchQuery)
        {
            return _zoneModel.RoleCategory == RoleCategoryType.Faculty ? _zoneModel.Faculties.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
           new List<FacultyResponseDTO>();
        }
        private async Task<IEnumerable<UserRoleFacultyResponseDTO>> SearchFacultiesByUserRole(string searchQuery, UserRoleResponseDTO userRole)
        {
            var response = await AuthService.GetZone(userRole.Role.Id);
            List<UserRoleFacultyResponseDTO> urf = new();

            response.Item.Faculties.Where(x => userRole.UserRoleFaculties?.Count > 0 ? !userRole.UserRoleFaculties.Select(a => a.FacultyId).Contains(x.Id) : true).ToList().ForEach(x =>
                urf.Add(new UserRoleFacultyResponseDTO() { Faculty = x, FacultyId = x.Id }));

            return urf.Where(x => x.Faculty.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) ??
           new List<UserRoleFacultyResponseDTO>();
        }
        private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ExpertiseBranchService.GetPaginateList(filter);
            return result.Items ?? new List<ExpertiseBranchResponseDTO>();
        }
        private void OnChangeExpertiseBranch(ExpertiseBranchResponseDTO expertiseBranch, IList<UserRoleFacultyResponseDTO> userRoleFaculties)
        {
            _expertiseBranch = expertiseBranch;
            foreach (var item in userRoleFaculties)
            {
                item.ExpertiseBranch = _expertiseBranch;
                item.ExpertiseBranchId = _expertiseBranch?.Id;
            }
            StateHasChanged();
        }
        private async Task<IEnumerable<UserRoleProgramResponseDTO>> SearchProgramsByUserRole(string searchQuery, UserRoleResponseDTO userRole)
        {
            var response = await AuthService.GetZone(userRole.Role.Id);
            List<UserRoleProgramResponseDTO> urp = new();

            response.Item.Programs.Where(x => userRole.UserRolePrograms?.Count > 0 ? !userRole.UserRolePrograms.Select(a => a.ProgramId).Contains(x.Id) : true).ToList().ForEach(x =>
                urp.Add(new UserRoleProgramResponseDTO() { Program = x, ProgramId = x.Id }));

            return urp.Where(x => x.Program.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) ??
           new List<UserRoleProgramResponseDTO>();
        }
        private async Task<IEnumerable<UserRoleHospitalResponseDTO>> SearchHospitalsByUserRole(string searchQuery, UserRoleResponseDTO userRole)
        {
            var response = await AuthService.GetZone(userRole.Role.Id);
            List<UserRoleHospitalResponseDTO> urh = new();

            response.Item.Hospitals.Where(x => userRole.UserRoleHospitals?.Count > 0 ? !userRole.UserRoleHospitals.Select(a => a.HospitalId).Contains(x.Id) : true).ToList().ForEach(x =>
                urh.Add(new UserRoleHospitalResponseDTO() { Hospital = x, HospitalId = x.Id }));

            return urh.Where(x => x.Hospital.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) ??
           new List<UserRoleHospitalResponseDTO>();
        }
        #endregion
        private async Task GetZoneByRole(long roleId)
        {
            _zoneModelLoading = true;
            StateHasChanged();
            try
            {
                var response = await AuthService.GetZone(roleId);
                if (response.Result)
                {
                    _zoneModel = response.Item;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally { _zoneModelLoading = false; StateHasChanged(); }


        }
        private async Task OnDeleteHandler(UserPaginateResponseDTO user)
        {
            //öğrenci ya da eğitici rolü varsa
            if (user.RoleCodes.Any(x => x == "UO"))
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, L["Error"], L["Since this person is a student, you can only delete them from the student list!"]);
                return;
            }
            else if (user.RoleCodes.Any(x => x == "E"))
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, L["Error"], L["Since this person is a educator, you can only delete them from the educator list!"]);
                return;
            }

            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to make passive this user?"]}",
            SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    await AuthService.Delete((long)user.Id);
                    user.IsDeleted = true;
                    StateHasChanged();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["User Inactivated!"]}");
                    GetUsers();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
            }
        }

        private void SaveUserRole()
        {
            if (!_ecUserRole.Validate()) return;
            _userConveyor.UserRoles = _userRoleModel.UserRoleList.Clone();
            StateHasChanged();
            _roleModal.CloseModal();
        }
        private List<long> GetZoneIds(UserRoleResponseDTO userRole)
        {
            if (userRole?.Role?.CategoryId == null)
            {
                return new List<long>(); // Null check
            }
            switch ((RoleCategoryType)userRole.Role.CategoryId)
            {
                case RoleCategoryType.Province:
                    return userRole.UserRoleProvinces?.Select(a => a.ProvinceId.Value)?.ToList();
                    break;
                case RoleCategoryType.University:
                    return userRole.UserRoleUniversities?.Select(a => a.UniversityId.Value)?.ToList();
                    break;
                case RoleCategoryType.Faculty:
                    return userRole.UserRoleFaculties?.Select(a => a.FacultyId.Value)?.ToList();
                    break;
                case RoleCategoryType.Hospital:
                    return userRole.UserRoleHospitals?.Select(a => a.HospitalId.Value)?.ToList();
                    break;
                case RoleCategoryType.Program:
                    return userRole.UserRolePrograms?.Select(a => a.ProgramId.Value)?.ToList();
                    break;
                default:
                    return new List<long>();
                    break;
            }
        }
        private async Task UpdateUser()
        {
            _documentValidatorMessage = null;

            if (!_ecupdate.Validate()) return;

            //if (_role == null || _role.Id == 0)
            //{
            //    _roleValidatorMessage = "Bu alan zorunludur!";
            //    StateHasChanged();
            //    return;
            //}
            if (_userforUpdate.UserRoles.Any(x => x.Role.Code == "KES"))
            {
                if (string.IsNullOrEmpty(dropzone._selectedFileName) && !(_userforUpdate.Documents?.Count > 0))
                {
                    _documentValidatorMessage = "Bu alan zorunludur!";
                    StateHasChanged();
                    return;
                }
                else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                    await CallDropzone();
            }

            _saving = true;
            StateHasChanged();
            _zonesUserRole = new();
            if (_userforUpdate.UserRoles != null && _userforUpdate.UserRoles.Count > 0 && _zonesUserRole != null)
            {
                _userforUpdate.UserRoles.ForEach(x => _zonesUserRole.Add(new ZoneRegisterDTO()
                {
                    RoleId = x.Role.Id,
                    ZoneIds = GetZoneIds(x),
                    ExpertiseBranchId = x.Role.Code == RoleCodeConstants.ANA_BILIM_DALI_BASKANI ? x.UserRoleFaculties?.FirstOrDefault(x => x.ExpertiseBranchId != null)?.ExpertiseBranchId : null,

                }));

            }
            if (_userforUpdate != null)
            {
                var dto = Mapper.Map<UpdateUserAccountInfoDTO>(_userforUpdate);
                dto.Zones = _zonesUserRole;

                try
                {
                    List<long> userRoleIds = null;
                    if (_userforUpdate?.Id == AuthenticationService?.User?.Id &&
                                            !AuthenticationService.User.UserRoleIds.OrderBy(x => x).SequenceEqual(dto?.Zones?.Select(x => x.RoleId)?.OrderBy(x => x)) == true)
                    {
                        userRoleIds = dto?.Zones?.Select(x => x.RoleId).ToList();
                    }

                    var response = await AuthService.UpdateUserAccount((long)_userforUpdate.Id, dto, userRoleIds);

                    await AuthenticationService.Initialize();

                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                        await GetUsers();
                        _userDetailModal.CloseModal();
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                catch (Exception e)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], e.Message);
                    Console.WriteLine(e.Message);
                }
            }
            else
            {

            }
            _saving = false;
            StateHasChanged();
            await GetUsers();
        }

        private void OnChangeIsPassive()
        {
            _userforUpdate.IsPassive = !_userforUpdate.IsPassive;
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

        #region IdentiyMethods

        private void CancelUser()
        {
            _userforAdd = new UserAccountDetailInfoResponseDTO() { Roles = new List<RoleResponseDTO>() };
            _userForIdentityChange = new();
            _tcValidatorMessage = null;
            _identityNo = String.Empty;
            _role = null;
            _showCaptcha = true;
            _captchaValidatorMessage = string.Empty;
            inputCaptcha = string.Empty;
            if (isDomainAllowed)
                ReloadCaptcha();
            StateHasChanged();
        }

        private async Task SearchForIdentityChange()
        {
            _tcValidatorMessage = string.Empty;
            _identityNo.PrintJson("ideee");
            if (string.IsNullOrEmpty(_identityNo))
            {

                _tcValidatorMessage = L["This field must be filled!"];
                _tcValidatorMessage.PrintJson("1");
                return;
            }
            else if (!_identityNo.TcValidator())
            {
                _tcValidatorMessage = L["Invalid {0}", L["Identity Number"]];
                _tcValidatorMessage.PrintJson("2");
                return;
            }

            var response = await UserService.GetUserForChangeIdentityNoAndName(_identityNo?.Trim());

            if (response.Result)
            {
                _userForIdentityChange = response.Item;
                StateHasChanged();
            }
            else
                SweetAlert.IconAlert(SweetAlertIcon.info, "", L[response.Message]);
        }

        private async Task ChangeIdentityNoAndName()
        {
            _tcValidatorMessage = string.Empty;
            if (string.IsNullOrEmpty(_identityNo))
            {
                _tcValidatorMessage = L["This field must be filled!"];
                return;
            }
            else if (!_identityNo.TcValidator())
            {
                _tcValidatorMessage = L["Invalid {0}", L["Identity Number"]];
                return;
            }
            else if (_identityNo != _userForIdentityChange.IdentityNo)
            {
                _tcValidatorMessage = L["Invalid {0}", L["Identity Number"]];
                return;
            }

            var response = await UserService.ChangeIdentityNoAndName(_identityNo.Trim(), _userforUpdate.Id);

            if (response.Result)
            {
                SweetAlert.IconAlert(SweetAlertIcon.success, L["Successfull"], L["Successfully Updated!"]);
                _changeIdentityNoModal.CloseModal();
                _userDetailModal.CloseModal();
                GetUsers();
            }
            else
                SweetAlert.IconAlert(SweetAlertIcon.error, L["Error"], L[response.Message]);
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
                var response = await UserService.GetUserByIdentityNo(_identityNo?.Trim());
                if (response.Result)
                {
                    _userforAdd = new UserAccountDetailInfoResponseDTO
                    {
                        IdentityNo = response.Item.KPSResult.TCKN.ToString(),
                        BirthDate = response.Item.KPSResult.BirthDate,
                        BirthPlace = response.Item.KPSResult.BirthPlace,
                        Name = response.Item.KPSResult.Name + " " + response.Item.KPSResult.Surname,
                        //Address = response.Item.KPSResult.AddressInfo.Address,
                        Gender = response.Item.KPSResult.Gender,
                    };

                    //_address = response.Item.KPSResult.AddressInfo.Address;
                    _ec = new EditContext(_userforAdd);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Fetched"]);
                }
                else
                {
                    if (response.Item != null)
                    {
                        var confirm = await SweetAlert.ConfirmAlert($"{L["Pasif Kullanıcı"]}",
            $"{L["Bu kulanıcı sisteme pasif olarak eklidir. Kullanıcıyı aktif etmek ister misiniz?"]}",
            SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["No"]}");
                        if (confirm)
                        {
                            var dto = Mapper.Map<UpdateUserAccountInfoDTO>(response.Item);
                            dto.IsDeleted = false;
                            try
                            {
                                var updateResponse = await AuthService.UpdateUserAccount(dto.Id, dto);
                                if (updateResponse.Result)
                                {
                                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");

                                    await GetUsers();

                                    _userAddModal.CloseModal();
                                    CancelUser();

                                }
                                else
                                {
                                    throw new Exception(updateResponse.Message);
                                }

                            }
                            catch (Exception e)
                            {

                                SweetAlert.ErrorAlert(e.Message);
                            }
                        }
                        else
                        {
                            _userAddModal.CloseModal();
                            CancelUser();
                        }
                    }
                    else
                        SweetAlert.IconAlert(SweetAlertIcon.info, "", L[response.Message]);
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An Error Occured."]);
                Console.WriteLine(e.Message);
            }
            finally
            {
                _searchingIdentity = false;
                StateHasChanged();
            }
        }
        private void IdentyNoChanged(string id)
        {
            _identityNo = id;
        }
        private string GetIdentiyClass()
        {
            return "form-control " + (!string.IsNullOrEmpty(_tcValidatorMessage) ? "invalid" : "");
        }



        #endregion

        #region FilterChangeHandlers
        private async Task OnChangeSelectFilter(ChangeEventArgs args, string filterName)
        {
            var values = args.Value as string[];
            values.PrintJson("values...:");
            if (values != null)
            {
                var value = string.Join(",", values);
                _filter.Filter ??= new Filter();
                _filter.Filter.Filters ??= new List<Filter>();
                _filter.Filter.Logic ??= "and";
                _filter.page = 1;
                foreach (var item in values)
                {


                    if (_filter.Filter.Filters.Where(x => x.Field == filterName).All(x => x.Value.ToString() != item))
                    {
                        _filter.Filter.Filters.Add(new Filter()
                        {
                            Field = filterName,
                            Operator = "eq",
                            Value = item
                        });
                    }

                }
                foreach (var item in _filter.Filter.Filters.Where(x => x.Field == filterName).ToList())
                {

                    if (!values.Any(x => x == item.Value.ToString()))
                    {
                        _filter.Filter.Filters.Remove(item);
                    }
                }
                await GetUsers();
            }
        }
        private async Task OnResetSelectFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearSelectInput", filterName);
                await GetUsers();
            }
        }
        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filter.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value.Trim()
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            await GetUsers();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetUsers();
            }
        }

        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }

        #endregion
    }
}