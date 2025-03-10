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
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.VisitModule.Form.Tabs
{
    public partial class InstitutionForm
    {
        [Parameter] public long? FormId { get; set; }
        [Parameter] public EventCallback<bool> OnCancel { get; set; }
        [Inject] private IAuthenticationService AuthenticationService { get; set; }
        [Inject] private IMapper Mapper { get; set; }
        [Inject] private IAuditFormService AuditFormService { get; set; }
        [Inject] private IEducatorService EducatorService { get; set; }
        [Inject] private IDocumentService DocumentService { get; set; }
        [Inject] private IStudentService StudentService { get; set; }
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
        private FilterDTO _filter;
        private FilterDTO _filterEducator;
        private FilterDTO _filterCommittee;
        private MyModal _educatorModal;
        private MyModal _committeeModal;
        private MyModal _visitDateModal;
        private PaginationModel<EducatorPaginateResponseDTO> _educatorPaginationModel;
        private PaginationModel<EducatorPaginateResponseDTO> _committeePaginationModel;
        private bool _educatorLoading;
        private bool _fileLoaded = true;
        private Dropzone dropzone;
        private string _documentValidatorMessage;
        private List<DocumentResponseDTO> _educatorDocuments = new();
        private bool forceRender;
        private bool _isCommitteeAppointed = false;
        private bool _studentsLoading;
        private bool _committeeLoading;
        private FilterDTO _filterStudent;
        private MyModal _studentModal;
        private PaginationModel<StudentPaginateResponseDTO> _studentPaginationModel;
        private List<string> _roles => AuthenticationService.Roles.Select(x => x.RoleName).ToList();
        private bool _isOnSiteVisit;


        protected override async Task OnInitializedAsync()
        {
            _editContext = new EditContext(_selectedForm);
            _filter = new FilterDTO()
            {
                Sort = new[]{new Sort()
                {
                    Dir = SortType.ASC,
                    Field = "Program.Hospital.Name"
                }},
                Filter = new()
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
                            Field = "VisitDate",
                            Operator = "eq",
                            Value = null
                        }
                    }
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
            //await GetUnassignedForms();
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
                        _isOnSiteVisit = _selectedForm.IsOnSiteVisit;
                        if (_selectedForm.OnSiteVisitCommittees?.Count > 0)
                            _isCommitteeAppointed = true;
                        _filterEducator.Filter.Filters.Add(new() { Field = _selectedForm.Program?.ExpertiseBranch?.IsPrincipal == true ? "PrincipalBranchDutyPlaceId" : "SubBranchDutyPlaceId", Operator = "eq", Value = _selectedForm.ProgramId });
                        _filterStudent.Filter.Filters.Add(new() { Field = "ProgramId", Operator = "eq", Value = _selectedForm.ProgramId });
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
        private async Task UpdateForm()
        {
            if (!_editContext.Validate()) return;

            _savingForm = true;
            StateHasChanged();
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

        private void OnChangeIsActiveUpdate1()
        {
            _isOnSiteVisit = true;

            StateHasChanged();
        }

        private void OnChangeIsActiveUpdate2()
        {
            _isOnSiteVisit = false;

            StateHasChanged();
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

        private async void SaveCommittee()
        {
            _savingForm = true;
            StateHasChanged();
            try
            {
                if (_selectedForm.VisitDate != null)
                {
                    _selectedForm.VisitStatusType = VisitStatusType.CommitteeAppointed;
                    NavigationManager.NavigateTo($"./visit-module/planned-requests");
                }
                _selectedForm.IsOnSiteVisit = _isOnSiteVisit;
                var response = await AuditFormService.UpdateForm(_selectedForm.Id, Mapper.Map<FormDTO>(_selectedForm));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla Kaydedildi");
                    //GetUnassignedForms();
                }
                else
                    SweetAlert.ErrorAlert();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally { _savingForm = false; StateHasChanged(); _committeeModal.CloseModal(); _visitDateModal.CloseModal(); }
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
        private async Task GetUnassignedForms()
        {
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _loading = true;
            StateHasChanged();
            try
            {
                _formPaginationModel = await AuditFormService.GetFormPaginateList(_filter);

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
        private bool IsFilteredCommittee(string filterName)
        {
            _filterCommittee.Filter ??= new Filter();
            _filterCommittee.Filter.Filters ??= new List<Filter>();
            _filterCommittee.Filter.Logic ??= "and";
            var index = _filterCommittee.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
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
        private void OnAddCommittee(EducatorPaginateResponseDTO educator)
        {
            _selectedForm.OnSiteVisitCommittees ??= new();
            if (_selectedForm.OnSiteVisitCommittees?.Any(x => x.UserId == educator.UserId) == true)
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Bu Kullanýcý Zaten Ekli!", "Eklemek istediðiniz üyenin zaten buraya ekli bir komisyon üyesidir.");
            else
            {
                _selectedForm.OnSiteVisitCommittees.Add(new OnSiteVisitCommitteeResponseDTO { User = new() { Name = educator.Name, UserRoles = educator.Roles.Select(x => new UserRoleResponseDTO() { Role = new() { RoleName = x } }).ToList() }, UserId = educator.UserId });
            }
            StateHasChanged();
        }
        private void OnRemoveCommittee(long userId)
        {
            _selectedForm.OnSiteVisitCommittees ??= new();
            if (_selectedForm.OnSiteVisitCommittees?.Any(x => x.UserId == userId) == false)
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Bu Kullanýcý Zaten Ekli Deðil!", "Kaldýrmak istediðiniz üye zaten buraya ekli deðil.");
            else
            {
                _selectedForm.OnSiteVisitCommittees.Remove(_selectedForm.OnSiteVisitCommittees.FirstOrDefault(x => x.UserId == userId));
            }
            StateHasChanged();
        }
        private async Task PaginationHandlerCommittee(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterCommittee.page = item1;
            _filterCommittee.pageSize = item2;
            await GetCommittees();
        }
        private async Task AssignCommitteToForm()
        {
            _committeeModal.OpenModal();
            await GetCommittees();
        }

        private void VisitDateToForm()
        {
            if (_selectedForm.OnSiteVisitCommittees == null || _selectedForm.OnSiteVisitCommittees?.Any() == false)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Komisyon Üyeleri Girilmedi", "Lütfen Ziyaret Tarihini belirlemeden önce komisyon üyelerini atayýnýz");
                return;
            }
            _visitDateModal.OpenModal();
        }
        private async Task SendForm()
        {
            if (!_editContext.Validate()) return;
            _savingForm = true;
            StateHasChanged();
            _selectedForm.VisitStatusType = VisitStatusType.CommitteeEvaluated;

            await UpdateForm();

            _savingForm = false;
            StateHasChanged();
            NavigationManager.NavigateTo($"./visit-module/committee-done-requests");
        }
    }
}