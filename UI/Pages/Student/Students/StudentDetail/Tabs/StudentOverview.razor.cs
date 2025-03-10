using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApexCharts;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Helper;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Student.Students.StudentDetail.Tabs
{
    public partial class StudentOverview
    {
        [Parameter] public bool IsEditing { get; set; }
        [Inject] public IState<StudentDetailState> StudentState { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public IUserService UserService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public ICountryService CountryService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IStudentExpertiseBranchService StudentExpertiseBranchService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] IDocumentService DocumentService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private StudentResponseDTO Student => StudentState.Value.Student;
        private StudentResponseDTO _oldStudent;
        private List<ProgramResponseDTO> _studentFormerPrograms = new();
        private EditContext _ecUpdate, _ecAdd;
        private InputMask PhoneInput;
        private bool _saving;
        private List<StudentExpertiseBranchResponseDTO> _selectedStudentExpertiseBranches = new();
        private List<ExpertiseBranchResponseDTO> allExpBranches = new();
        private List<ExpertiseBranchResponseDTO> principalBranches = new();
        private List<ExpertiseBranchResponseDTO> subBranches = new();
        private bool _isNative = true;
        private ForeignType? _foreignType;
        private CountryResponseDTO _country = new();
        private PaginationModel<CountryResponseDTO> countryResponse = new();
        private ImageUpload _studentProfile;
        private bool _fileLoaded = true;
        private MyModal _OsymFileAddingModal;
        private Dropzone dropzone;
        private bool _isPermitted => AuthenticationService.IsPermitted(new List<PermissionEnum>() { PermissionEnum.StudentUpdate });
        protected override async Task OnInitializedAsync()
        {
            _ecUpdate = new EditContext(new StudentResponseDTO());
            _country = Student.User.Country;
            _foreignType = Student.User.ForeignType;
             _isNative = Student.User?.ForeignType == null;

            StateHasChanged();

            if (Student.User?.ForeignType == null && Student.User?.Country == null)
            {
                countryResponse = await CountryService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", "türkiye", "and"));
                _country = countryResponse.Items?.FirstOrDefault();
            }

            if (IsEditing)
            {
                SubscribeToAction<StudentDetailSetAction>(action =>
                {
                    _ecUpdate = new EditContext(action.Student);
                });

                _selectedStudentExpertiseBranches = Student.StudentExpertiseBranches;

                var expResponse = await ExpertiseBranchService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue)
                                                                                    .Sort("Name", Shared.Types.SortType.ASC));
                if (expResponse.Items != null)
                {
                    allExpBranches = expResponse.Items;
                    principalBranches = allExpBranches.Where(x => !x.PrincipalBranches.Any()).ToList();
                    subBranches = allExpBranches.Where(x => x.PrincipalBranches.Any()).ToList();
                }
            }

            if (Student?.Program?.IsDeleted == true)
            {
                SweetAlert.ConfirmAlert(L["Kapatılmış Program"], L["The {0} program to which the student is affiliated, has been closed on {1}. Make sure the necessary process is done.", Student?.Program?.Name, Student?.Program?.DeleteDate?.ToString("dd/MM/yyy")], SweetAlertIcon.warning, false, L["Anladım"], "");
            }

            FilterDTO filter = new()
            {
                Sort = new[]
                {
                    new Sort()
                    {
                        Field = "User.Name",
                        Dir = Shared.Types.SortType.ASC
                    }
                },
                Filter = new()
                {
                    Filters = new()
                    {
                        new Filter()
                        {
                            Field = "IsDeleted",
                        Operator = "eq",
                        Value = true,
                        },
                        new Filter()
                        {
                            Field="UserId",
                            Operator="eq",
                            Value=Student?.UserId
                        },
						 new Filter()
						{
							Field="DeleteReasonType",
							Operator="eq",
							Value=ReasonType.BranchChange
						}

					},
                    Logic = "and"

                },
                page = 1,
                pageSize = int.MaxValue,
            };
            var result = await StudentService.GetPaginateList(filter);
            if (result.Items != null)
            {
                foreach (var item in result.Items)
                {
                    if (item.Program != null)
                        _studentFormerPrograms.Add(item.Program);
                }
            }
            _oldStudent = Student?.Clone();

            await base.OnInitializedAsync();
        }
        public void ImageChanged(string imagePath)
        {
            Student.User.ProfilePhoto = imagePath;
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
                    Student.Documents.Add(result.Item);
                    _fileLoaded = true;
                    StateHasChanged();
                    _OsymFileAddingModal.CloseModal();
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
        private async Task Save()
        {
            if (!string.IsNullOrEmpty(Student?.User?.ProfilePhoto))
                _studentProfile.SavePhoto(Student.User.Id);
            try
            {
                Student.User.CountryId = _country.Id;
                Student.User.ForeignType = _foreignType;
                //var dto = Mapper.Map<UpdateUserAccountInfoDTO>(Student.User);
                var dto = Mapper.Map<StudentDTO>(Student);

                //var response = await AuthService.UpdateUserAccount((long)Student.User.Id, dto);
                var response = await StudentService.Update((long)Student.Id, dto);
                if (response.Result)
                {
                    if (_oldStudent.CurriculumId != Student?.CurriculumId)
                    {
                        await SweetAlert.ConfirmAlert("Eğitim Süresi Uyarısı", "Müfredatı değiştirdiniz, Eğitim Süre Takibinden gerekli uzatma veya kısaltma işlemlerini yapmanız gerekmektedir.", SweetAlertIcon.warning, false, "Anladım", "");
                    }
                    Dispatcher.Dispatch(new StudentDetailSetAction(response.Item));
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                    NavigationManager.NavigateTo($"./student/students/{Student.Id}");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            StateHasChanged();
        }

        private async Task AddExpBranch(StudentExpertiseBranchResponseDTO edx)
        {

            if (edx.StudentId == null)
            {
                var response = await StudentExpertiseBranchService.AddStudentExpBranch(new StudentExpertiseBranchDTO()
                {
                    StudentId = Student.Id,
                    ExpertiseBranchId = edx.ExpertiseBranchId,
                    RegistrationDate = edx.RegistrationDate
                });

                if (response.Result)
                {
                    edx.StudentId = response.Item.StudentId;
                    StateHasChanged();
                }
            }

            _selectedStudentExpertiseBranches.Add(new StudentExpertiseBranchResponseDTO()
            {
                IsPrincipal = null,
                RegistrationDate = DateTime.UtcNow,
                ExpertiseBranch = null
            });
            StateHasChanged();
        }

        private async Task RemoveExpBranch(StudentExpertiseBranchResponseDTO edx)
        {

            if (edx.StudentId != null)
            {
                await StudentExpertiseBranchService.RemoveStudentExpBranch((long)edx.StudentId, (long)edx.ExpertiseBranchId);
            }
            _selectedStudentExpertiseBranches.Remove(edx);
            StateHasChanged();
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
        private void OnChangeCountry(CountryResponseDTO country)
        {
            _country = country;
        }
        private async Task OnChangeNative()
        {
            _isNative = !_isNative;
            _foreignType = null;

            if (!_isNative)
            {
                _country = null;
            }
            else
            {
                countryResponse = await CountryService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", "türkiye", "and"));
                _country = countryResponse.Items?.FirstOrDefault();
            }

            StateHasChanged();
        }

        private void OnChangeCurriculum(CurriculumResponseDTO curriculum)
        {
            Student.Curriculum = curriculum;
            Student.CurriculumId = curriculum?.Id;
        }

        private string GetCurriculumClass()
        {
            return "form-control " + Student.Curriculum is not null ? "invalid" : "";
        }

        private async Task<IEnumerable<CurriculumResponseDTO>> SearchCurriculum(string searchQuery)
        {
            var result = await CurriculumService.GetByExpertiseBranchId((long)Student.Program.ExpertiseBranchId);
            return result.Result ? result.Item : new List<CurriculumResponseDTO>();
        }
    }
}