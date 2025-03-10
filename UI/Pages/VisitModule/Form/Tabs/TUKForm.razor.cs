using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.DetailCards;

namespace UI.Pages.VisitModule.Form.Tabs
{
    public partial class TUKForm
    {
        [Parameter] public long? FormId { get; set; }
        [Parameter] public EventCallback<bool> OnCancel { get; set; }
        [Inject] private IAuthenticationService AuthenticationService { get; set; }
        [Inject] private IMapper Mapper { get; set; }
        [Inject] private IAuditFormService AuditFormService { get; set; }
        [Inject] private IEducatorService EducatorService { get; set; }
        [Inject] private IStudentService StudentService { get; set; }
        [Inject] private IAuthorizationDetailService AuthorizationDetailService { get; set; }
        [Inject] private IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private PaginationModel<FormResponseDTO> _formPaginationModel;
        private bool _loading;
        private bool _studentsLoading;
        private FormResponseDTO _selectedForm = new();
        private EditContext _editContext;
        private bool _savingForm;
        private List<AuthorizationDetailResponseDTO> _authDetailList = new();
        private AuthorizationDetailResponseDTO _authorizationDetail = new();
        private EditContext _authorizationEc;
        private MyModal _authorizationDetailModal;

        private FilterDTO _filterEducator;
        private FilterDTO _filterStudent;
        private MyModal _educatorModal;
        private MyModal _studentModal;
        private PaginationModel<EducatorPaginateResponseDTO> _educatorPaginationModel;
        private PaginationModel<StudentPaginateResponseDTO> _studentPaginationModel;
        private bool _educatorLoading;
        private bool _fileLoaded = true;
        private Dropzone dropzone;
        private string _documentValidatorMessage;
        private List<DocumentResponseDTO> _educatorDocuments = new();
        private bool forceRender;
        private AuthorizationDetailsModal _authDetailsModal;
        private List<string> _roles => AuthenticationService.Roles.Select(x => x.RoleName).ToList();

        protected override async Task OnInitializedAsync()
        {
            _editContext = new EditContext(_selectedForm);
            _authorizationEc = new EditContext(_authorizationDetail);
            _filterEducator = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "Name",
                Dir = SortType.ASC
            }}
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
                            Field = "EducatorType",
                            Operator = "eq",
                            Value = EducatorType.Instructor
                        },
                         new Filter()
                        {
                            Field = "UserIsDeleted",
                            Operator = "eq",
                            Value = false
                        }
                },

            };
            _filterStudent = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "Name",
                Dir = SortType.ASC
            }}
            };
            _filterStudent.pageSize = 5;
            _filterStudent.Filter = new()
            {
                Logic = "and",
                Filters = new()
                {
                    new Filter()
                    {
                        Field = "IsDeleted",
                        Operator = "eq",
                        Value = false
                    },
                    new Filter()
                    {
                        Field = "UserIsDeleted",
                        Operator = "eq",
                        Value = false
                    },
                }
            };
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeSelectPicker");
            }
            if (forceRender)
            {
                forceRender = false;
                await JSRuntime.InvokeVoidAsync("initializeSelectPicker");

            }
        }
        public async Task<string> AddItemOnEmptyResult(string input)
        {
            return input;
        }
        protected override async void OnParametersSet()
        {
            if (FormId != null)
            {
                _loading = true;
                StateHasChanged();
                try
                {
                    var response = await AuditFormService.GetFormById(FormId.Value);
                    if (response.Result)
                    {
                        _selectedForm = response.Item;
                        _filterEducator.Filter.Filters.Add(new() { Field = _selectedForm.Program?.ExpertiseBranch?.IsPrincipal == true ? "PrincipalBranchDutyPlaceId" : "SubBranchDutyPlaceId", Operator = "eq", Value = _selectedForm.ProgramId });
                        _filterStudent.Filter.Filters.Add(new() { Field = "ProgramId", Operator = "eq", Value = _selectedForm.ProgramId });

                        var response2 = await AuthorizationDetailService.GetListByProgramId(_selectedForm.ProgramId.Value);
                        if (response2.Result)
                            _authDetailList = response2.Item;
                        else
                            SweetAlert.ErrorAlert();

                        _editContext = new EditContext(_selectedForm);
                    }
                    else
                        SweetAlert.ErrorAlert();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                }
                finally
                {
                    _loading = false;
                    StateHasChanged();
                }
            }
            base.OnParametersSet();
        }
        private async Task GetStudents()
        {
            _studentsLoading = true;
            StateHasChanged();
            try
            {
                _studentPaginationModel = await StudentService.GetPaginateListOnly(_filterStudent);
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message);
            }
            finally
            {
                _studentsLoading = false;
                StateHasChanged();
            }
        }
        private void OnChangeDescription(IList<string> descriptions)
        {
            _authorizationDetail.Descriptions = descriptions.ToList();
        }

        public async Task<bool> CallDropzone()
        {
            _educatorDocuments ??= new();
            _selectedForm.TUKDocuments ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorDocuments.Add(result.Item);
                    _selectedForm.TUKDocuments.Add(result.Item);
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
        private async Task GetEducators()
        {
            _educatorLoading = true;
            StateHasChanged();

            try
            {
                _educatorPaginationModel = await EducatorService.GetPaginateListOnly(_filterEducator);
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla yüklendi.");
                forceRender = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally
            {
                _educatorLoading = false;
                StateHasChanged();
            }
        }
        private async Task OpenEducatorModal()
        {
            _educatorModal.OpenModal();
            await GetEducators();
        }
        private async Task OpenStudentModal()
        {
            _studentModal.OpenModal();
            await GetStudents();
        }
        private void OnOpenAuthorizationDetailList()
        {
            _authorizationDetail = new() { AuthorizationDate = DateTime.UtcNow };
            _authorizationDetail.AuthorizationCategory = new AuthorizationCategoryResponseDTO();
            _authorizationEc = new EditContext(_authorizationDetail);
            StateHasChanged();
            _authorizationDetailModal.OpenModal();
        }

        private async Task<IEnumerable<AuthorizationCategoryResponseDTO>> SearchAuthorizationCategories(string searchQuery)
        {
            var result = await AuthorizationCategoryService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).OrderBy(x => x.Name) :
                new List<AuthorizationCategoryResponseDTO>();
        }
        private async void GetAuthorizationDetails(long programId)
        {
            try
            {
                var response = await AuthorizationDetailService.GetListByProgramId(programId);
                if (response.Result)
                {
                    _authDetailsModal.OpenModal(response.Item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
        }
        private void OnChangeAuthorizationDate(DateTime? date)
        {
            _authorizationDetail.AuthorizationDate = date;
            if (_authorizationDetail.AuthorizationCategory?.Duration != null)
                _authorizationDetail.AuthorizationEndDate = _authorizationDetail.AuthorizationDate.Value.AddDays(_authorizationDetail.AuthorizationCategory.Duration);
            StateHasChanged();
        }
        private void OnChangeAuthorizationCategory(AuthorizationCategoryResponseDTO authorizationCategory)
        {
            _authorizationDetail.AuthorizationCategory = authorizationCategory;
            _authorizationDetail.AuthorizationCategoryId = authorizationCategory?.Id;

            if (_authorizationDetail.AuthorizationDate.HasValue)
                _authorizationDetail.AuthorizationEndDate = _authorizationDetail.AuthorizationDate.Value.AddDays(authorizationCategory.Duration);
            StateHasChanged();
        }
        private async void AddAuthorizationDetail()
        {
            if (!_authorizationEc.Validate())
            {
                return;
            }

            _savingForm = true;
            StateHasChanged();

            if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                await CallDropzone();

            var dto = Mapper.Map<AuthorizationDetailDTO>(_authorizationDetail);
            dto.ProgramId = _selectedForm.ProgramId;
            try
            {
                var response = await AuthorizationDetailService.Add(dto);
                if (response.Result)
                {
                    _authDetailList.Add(response.Item);
                    _selectedForm.AuthorizationDetailId = response.Item.Id;
                    await UpdateForm();
                }
                else
                    SweetAlert.ErrorAlert();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SweetAlert.ErrorAlert();

            }
            finally
            {
                _authorizationDetailModal.CloseModal();
                _authorizationDetail = new();
                StateHasChanged();
            }
            NavigationManager.NavigateTo($"./institution-management/programs/{_selectedForm.ProgramId}");//check it
        }
        private async Task UpdateForm()
        {

            if (_selectedForm.VisitStatusType < VisitStatusType.TUKEvaluated)
                _selectedForm.VisitStatusType = VisitStatusType.TUKEvaluated;
            try
            {
                var response = await AuditFormService.UpdateForm(_selectedForm.Id, Mapper.Map<FormDTO>(_selectedForm));
                if (response.Result)
                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla Kaydedildi");
                else
                    SweetAlert.ErrorAlert();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally { _savingForm = false; StateHasChanged(); }
        }

        #region FilterEducator
        private async Task OnChangeSelectFilter(ChangeEventArgs args, string filterName)
        {
            var values = args.Value as string[];
            if (values != null)
            {
                var value = string.Join(",", values);
                _filterEducator.Filter ??= new Filter();
                _filterEducator.Filter.Filters ??= new List<Filter>();
                _filterEducator.Filter.Logic ??= "and";
                _filterEducator.page = 1;
                foreach (var item in values)
                {


                    if (_filterEducator.Filter.Filters.Where(x => x.Field == filterName).All(x => x.Value.ToString() != item))
                    {
                        _filterEducator.Filter.Filters.Add(new Filter()
                        {
                            Field = filterName,
                            Operator = "eq",
                            Value = item
                        });
                    }

                }
                foreach (var item in _filterEducator.Filter.Filters.Where(x => x.Field == filterName).ToList())
                {

                    if (!values.Any(x => x == item.Value.ToString()))
                    {
                        _filterEducator.Filter.Filters.Remove(item);
                    }
                }
                await GetEducators();
            }
        }
        private async Task PaginationHandlerEducator(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterEducator.page = item1;
            _filterEducator.pageSize = item2;
            await GetEducators();
        }

        private async Task OnSortChangeEducator(Sort sort)
        {
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

        #region StudentFilter
        private async Task OnResetStudentFilter(string filterName)
        {
            _filterStudent.Filter ??= new Filter();
            _filterStudent.Filter.Filters ??= new List<Filter>();
            _filterStudent.Filter.Logic ??= "and";
            var index = _filterStudent.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filterStudent.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetStudents();
            }
        }

        private bool IsStudentFiltered(string filterName)
        {
            _filterStudent.Filter ??= new Filter();
            _filterStudent.Filter.Filters ??= new List<Filter>();
            _filterStudent.Filter.Logic ??= "and";
            var index = _filterStudent.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        private async Task PaginationHandlerStudent(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterStudent.page = item1;
            _filterStudent.pageSize = item2;

            await GetStudents();
        }

        private async Task OnChangeStudentFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filterStudent.Filter ??= new Filter();
            _filterStudent.Filter.Filters ??= new List<Filter>();
            _filterStudent.Filter.Logic ??= "and";
            var index = _filterStudent.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filterStudent.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value
                });
            }
            else
            {
                _filterStudent.Filter.Filters[index].Value = value;
            }
            await GetStudents();
        }
        #endregion
    }
}