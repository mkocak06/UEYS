using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Hospital;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.DetailCards;

namespace UI.Pages.VisitModule.Form.Tabs
{
    public partial class CommitteeForm
    {
        [Parameter] public long? FormId { get; set; }
        [Parameter] public bool IsPreview { get; set; } = false;
        [Parameter] public EventCallback<bool> OnCancel { get; set; }
        [Inject] private IAuthenticationService AuthenticationService { get; set; }
        [Inject] private IAuthorizationDetailService AuthorizationDetailService { get; set; }
        [Inject] private IStudentService StudentService { get; set; }
        [Inject] private IMapper Mapper { get; set; }
        [Inject] private IAuditFormService AuditFormService { get; set; }
        [Inject] private IEducatorService EducatorService { get; set; }
        [Inject] private IDocumentService DocumentService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] IHospitalService HospitalService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private UserHospitalDetailDTO Profile { get; set; }


        private PaginationModel<FormResponseDTO> _formPaginationModel;
        private bool _loading;
        private FormResponseDTO _selectedForm = new();
        private EditContext _editContext;
        private bool _savingForm;
        private FilterDTO _filterEducator;
        private MyModal _educatorModal;
        private PaginationModel<EducatorPaginateResponseDTO> _educatorPaginationModel;
        private bool _educatorLoading;
        private bool _fileLoaded = true;
        private Dropzone dropzone;
        private string _documentValidatorMessage;
        private List<DocumentResponseDTO> _educatorDocuments = new();
        private bool forceRender;
        private bool _studentsLoading;
        private FilterDTO _filterStudent;
        private MyModal _studentModal;
        private PaginationModel<StudentPaginateResponseDTO> _studentPaginationModel;

        private PaginationModel<EducatorPaginateResponseDTO> _committeePaginationModel;
        private bool _committeeLoading;
        private FilterDTO _filterCommittee;
        private MyModal _committeeModal;

        private AuthorizationDetailsModal _authDetailsModal;
        private List<OnSiteVisitCommitteeResponseDTO> _committees = new();
        private List<string> _roles => AuthenticationService.Roles.Select(x => x.RoleName).ToList();
        protected override async Task OnInitializedAsync()
        {
            _editContext = new EditContext(_selectedForm);
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
            _filterCommittee = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "Name",
                Dir = SortType.ASC
            }}
            };
            _filterCommittee.pageSize = 5;
            _filterCommittee.Filter = new Filter()
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
                            Operator = "neq",
                            Value = EducatorType.NotInstructor
                        },
                         new Filter()
                        {
                            Field = "UserIsDeleted",
                            Operator = "eq",
                            Value = false
                        }

                },

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
        private async Task AssignCommitteToForm(FormResponseDTO form)
        {
            _selectedForm = form;
            _committeeModal.OpenModal();
            await GetCommittees();
        }
        private async Task GetCommittees()
        {
            _committeeLoading = true;
            StateHasChanged();

            try
            {
                _committeePaginationModel = await EducatorService.GetPaginateListOnly(_filterCommittee);
                forceRender = true;
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla yüklendi.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally
            {
                _committeeLoading = false;
                StateHasChanged();
            }
        }
        private async void SaveCommittee()
        {
            _savingForm = true;
            StateHasChanged();
            _selectedForm.OnSiteVisitCommittees = _committees;
            try
            {
                var response = await AuditFormService.UpdateForm(_selectedForm.Id, Mapper.Map<FormDTO>(_selectedForm));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla Kaydedildi");
                }
                else
                    SweetAlert.ErrorAlert();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally { _savingForm = false; StateHasChanged(); _committeeModal.CloseModal(); await GetSelectedForm(); }
        }

        private void OnAddCommittee(long userId)
        {
            if (_committees?.Any(x => x.UserId == userId) == true)
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Bu Kullanýcý Zaten Ekli!", "Eklemek istediðiniz üyenin zaten buraya ekli bir komisyon üyesidir.");
            else
            {
                _committees.Add(new OnSiteVisitCommitteeResponseDTO { UserId = userId });

            }
            StateHasChanged();
        }
        private void OnRemoveCommittee(long userId)
        {
            if (_committees.Any(x => x.UserId == userId) == false)
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Bu Kullanýcý Zaten Ekli Deðil!", "Kaldýrmak istediðiniz üye zaten buraya ekli deðil.");
            else
            {
                _committees.Remove(_selectedForm.OnSiteVisitCommittees.FirstOrDefault(x => x.UserId == userId));
            }
            StateHasChanged();
        }
        private string IsDisabled()
        {
            return IsPreview ? "radio-disabled" : "";
        }
        protected override async void OnParametersSet()
        {
            if (FormId != null)
            {
                await GetSelectedForm();
            }
            base.OnParametersSet();
        }

        private async Task GetSelectedForm()
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
                    _editContext = new EditContext(_selectedForm);
                    _committees = _selectedForm.OnSiteVisitCommittees.ToList();
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
        private async Task OpenStudentModal()
        {
            _studentModal.OpenModal();
            await GetStudents();
        }

        private async Task GetEducators()
        {
            _educatorLoading = true;
            StateHasChanged();

            try
            {
                _educatorPaginationModel = await EducatorService.GetPaginateListOnly(_filterEducator);
                forceRender = true;
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla yüklendi.");

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
        private async Task OpenModal()
        {
            _educatorModal.OpenModal();
            await GetEducators();
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
        public async Task<bool> CallDropzone()
        {
            _educatorDocuments ??= new();
            _selectedForm.CommitteeDocuments ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorDocuments.Add(result.Item);
                    _selectedForm.CommitteeDocuments.Add(result.Item);
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
        private async Task SendForm()
        {
            if (!_editContext.Validate()) return;
            _savingForm = true;
            StateHasChanged();
            if (_selectedForm.CommitteeOpinionType != null && _selectedForm.VisitStatusType < VisitStatusType.CommitteeEvaluated)
                _selectedForm.VisitStatusType = VisitStatusType.CommitteeEvaluated;
           
            if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                await CallDropzone();

            await UpdateForm();

            _savingForm = false; StateHasChanged(); NavigationManager.NavigateTo($"./visit-module/committee-done-requests");
        }
        private async Task UpdateForm()
        {
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
        }

        private void OnDateChanged(DateTime? date)
        {
            _selectedForm.VisitDate = date;
            UpdateForm();
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

        #region FilterCommittee

        private async Task OnChangeCommitteeSelectFilter(ChangeEventArgs args, string filterName)
        {
            var values = args.Value as string[];
            if (values != null)
            {
                var value = string.Join(",", values);
                _filterCommittee.Filter ??= new Filter();
                _filterCommittee.Filter.Filters ??= new List<Filter>();
                _filterCommittee.Filter.Logic ??= "and";
                _filterCommittee.page = 1;
                foreach (var item in values)
                {


                    if (_filterCommittee.Filter.Filters.Where(x => x.Field == filterName).All(x => x.Value.ToString() != item))
                    {
                        _filterCommittee.Filter.Filters.Add(new Filter()
                        {
                            Field = filterName,
                            Operator = "eq",
                            Value = item
                        });
                    }

                }
                foreach (var item in _filterCommittee.Filter.Filters.Where(x => x.Field == filterName).ToList())
                {

                    if (!values.Any(x => x == item.Value.ToString()))
                    {
                        _filterCommittee.Filter.Filters.Remove(item);
                    }
                }
                await GetCommittees();
            }
        }
        private async Task PaginationHandlerCommittee(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterCommittee.page = item1;
            _filterCommittee.pageSize = item2;
            await GetCommittees();
        }

        private async Task OnSortChangeCommittee(Sort sort)
        {
            _filterCommittee.Sort = new[] { sort };
            await GetCommittees();
        }

        private async Task OnChangeFilterCommittee(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filterCommittee.Filter ??= new Filter();
            _filterCommittee.Filter.Filters ??= new List<Filter>();
            _filterCommittee.Filter.Logic ??= "and";
            var index = _filterCommittee.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filterCommittee.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value
                });
            }
            else
            {
                _filterCommittee.Filter.Filters[index].Value = value;
            }
            await GetCommittees();
        }

        private async Task OnResetFilterCommittee(string filterName)
        {
            _filterCommittee.Filter ??= new Filter();
            _filterCommittee.Filter.Filters ??= new List<Filter>();
            _filterCommittee.Filter.Logic ??= "and";
            var index = _filterCommittee.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filterCommittee.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetCommittees();
            }
        }

        private bool IsFilteredCommittee(string filterName)
        {
            _filterCommittee.Filter ??= new Filter();
            _filterCommittee.Filter.Filters ??= new List<Filter>();
            _filterCommittee.Filter.Logic ??= "and";
            var index = _filterCommittee.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        #endregion
    }
}