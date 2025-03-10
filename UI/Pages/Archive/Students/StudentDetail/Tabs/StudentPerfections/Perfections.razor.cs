using Microsoft.AspNetCore.Components;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Services;
using Fluxor;
using Microsoft.JSInterop;
using UI.Models;
using UI.Pages.Archive.Students.StudentDetail.Store;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using System.Linq;
using System.Globalization;
using UI.SharedComponents.Components;
using Shared.Types;
using Shared.FilterModels.Base;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs.StudentPerfections
{
    partial class Perfections
    {
        [Parameter]
        public bool IsEditing { get; set; }
        [Parameter]
        public PerfectionType PerfectionType { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStudentPerfectionService StudentPerfectionService { get; set; }
        [Inject] public IPerfectionService PerfectionService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IEducatorProgramService EducatorProgramService { get; set; }
        [Inject] public IEducationOfficerService EducationOfficerService{ get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private List<StudentPerfectionResponseDTO> _studentPerfections;
        private List<PerfectionResponseDTO> _perfections = new();
        private bool _loadingPerfections;
        private bool _saving;
        private List<BreadCrumbLink> _links;
        private bool Loaded => StudentDetailState.Value.StudentLoaded;
        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        private bool forceRender;

        private StudentPerfectionResponseDTO _studentPerfection;
        private StudentPerfectionResponseDTO _studentPerfectionUpdateModel;
        private PaginationModel<PerfectionResponseDTO> _paginationModelPerfections;

        private MyModal _addStudentPerfectionModal;
        private MyModal _studentPerfectionDetailModal;
        private EditContext _ec;
        private EditContext _ecStudentPerfection;
        private ProgramResponseDTO _updatedProgram;
        private EducatorResponseDTO _updatedEducator;
        private bool _loaded;
        private ProgramResponseDTO _newProgram;

        private bool _isSuccessful;
        private string _dateValidatorMessage;
        private bool _loadingFile;
        private FilterDTO _filter;

        [Parameter] public long Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _studentPerfection = new StudentPerfectionResponseDTO();
            _studentPerfectionUpdateModel = new StudentPerfectionResponseDTO();
            _updatedProgram = new ProgramResponseDTO();
            _ec = new EditContext(_studentPerfection);
            _ecStudentPerfection = new EditContext(_studentPerfectionUpdateModel);

            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                {

                    new Filter()
                    {
                        Field="CurriculumId",
                        Operator="eq",
                        Value=_student.CurriculumId
                    },
                    new Filter()
                    {
                        Field="IsDeleted",
                        Operator="eq",
                        Value=false
                    },
                    new Filter()
                    {
                        Field="PerfectionType",
                        Operator="eq",
                        Value=PerfectionType
                    },
                    new Filter()
                    {
                        Field="StudentId",
                        Operator="eq",
                        Value=_student.Id
                    }
                },
                    Logic = "and"

                },
                page = 1,
                pageSize = 10,
            };

            await base.OnInitializedAsync();
            await LoadPerfections();
        }

        private async Task LoadPerfections()
        {
            if (_student.CurriculumId != null)
            {
                try
                {
                    _loadingPerfections = true;
                    StateHasChanged();
                    _paginationModelPerfections = await PerfectionService.GetPaginateList(_filter);
                    _paginationModelPerfections.Page.PrintJson("page");
                    _paginationModelPerfections.PageSize.PrintJson("PageSize");
                    _paginationModelPerfections.TotalItemCount.PrintJson("TotalItemCount");
                    _paginationModelPerfections.TotalPages.PrintJson("TotalPages");
                    if (_paginationModelPerfections != null)
                    {
                        _perfections = _paginationModelPerfections.Items;
                        var response = await StudentPerfectionService.GetListByStudentId(_student.Id, PerfectionType);
                        if (response.Result)
                            _studentPerfections = response.Item;
                        else
                        {
                            SweetAlert.ErrorAlert();
                            _perfections = new();
                        }
                    }
                    else
                        SweetAlert.ErrorAlert();

                    StateHasChanged();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during the loading of Perfection Information."]);
                }
                finally
                {
                    _loadingPerfections = false;
                    StateHasChanged();
                }
            }
            else
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during the loading of Perfection Information."]);
                _perfections = new();
            }
            StateHasChanged();

        }

        private void OnAddStudentPerfectionModal(PerfectionResponseDTO perfection)
        {
            //if (!IsPerfectionChosen(perfection))
            //{
            //    _saving = true;
            //    try
            //    {
            //        var response = await StudentPerfectionService.PostAsync(new StudentPerfectionDTO() { PerfectionId = perfection.Id.Value, StudentId = _student.Id });
            //        if (response.Result)
            //        {
            //            SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
            //            await LoadPerfections();
            //        }
            //        else
            //        {
            //            throw new Exception();
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred while adding a Perfection Target."]);
            //    }
            //    finally
            //    {
            //        _saving = false;
            //        StateHasChanged();
            //    }
            //}
            _studentPerfection = new StudentPerfectionResponseDTO()
            {
                Perfection = perfection,
                PerfectionId = perfection.Id.Value,
                StudentId = _student.Id,
                Student = _student
            };

            _ec = new EditContext(_studentPerfection);
            _addStudentPerfectionModal.OpenModal();
        }

        private async Task AddStudentPerfection()
        {
            if (!_ec.Validate())
            {
                return;
            }
            var dto = Mapper.Map<StudentPerfectionDTO>(_studentPerfection);
            try
            {
                var response = await StudentPerfectionService.PostAsync(dto);
                if (response.Result)
                {
                    _addStudentPerfectionModal.CloseModal();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                    _dateValidatorMessage = null;
                    await LoadPerfections();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An Error Occured."]);
            }

        }

        private async Task OnRemovePerfectionHandler(PerfectionResponseDTO perfection)
        {

            if (IsPerfectionChosen(perfection))
            {
                _saving = true;
                try
                {
                    await StudentPerfectionService.Delete(_student.Id, perfection.Id.Value);
                    await LoadPerfections();
                }
                catch (Exception)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during perfection target deletion."]);
                }
                finally
                {
                    _saving = false;
                    StateHasChanged();
                }
            }
        }

        private bool IsPerfectionChosen(PerfectionResponseDTO perfection)
        {
            return _studentPerfections?.Any(x => x.PerfectionId == perfection.Id.Value) ?? false;
        }

        private async Task<IEnumerable<EducatorResponseDTO>> SearchEducators(string searchQuery)
        {
            var result = await EducationOfficerService.GetListByProgramId(_studentPerfection.ProgramId.Value);
            return result.Result ? result.Item.Select(x => x.Educator).Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<EducatorResponseDTO>();
        }

        private async Task<IEnumerable<EducatorResponseDTO>> SearchUpdatedEducators(string searchQuery)
        {
            var result = await EducationOfficerService.GetListByProgramId(_updatedProgram.Id);
            return result.Result ? result.Item.Select(x => x.Educator).Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<EducatorResponseDTO>();
        }

        private void OnChangeEducator(EducatorResponseDTO educator)
        {
            _studentPerfection.Educator = educator;
            _studentPerfection.EducatorId = educator.Id;
        }

        private void OnChangeUpdatedEducator(EducatorResponseDTO educator)
        {
            _updatedEducator = educator;
            _updatedEducator.Id = educator.Id;

            _studentPerfectionUpdateModel.EducatorId = _updatedEducator?.Id;
        }

        private async Task OnStudentPerfectionDetail(PerfectionResponseDTO perfection)
        {
            if (perfection.Id != 0)
            {
                var response = await StudentPerfectionService.GetByStudentAndPerfectionIdAsync(_student.Id, perfection.Id.Value);

                if (response.Result && response.Item != null)
                {
                    _studentPerfectionUpdateModel = response.Item;
                    _updatedProgram = _studentPerfectionUpdateModel?.Program;
                    _updatedEducator = _studentPerfectionUpdateModel?.Educator;
                    _ecStudentPerfection = new EditContext(_studentPerfectionUpdateModel);
                    _loaded = true;
                    StateHasChanged();
                    _studentPerfectionDetailModal.OpenModal();
                }

            }
            await base.OnParametersSetAsync();
        }
        private async Task UpdateStudentPerfection()
        {
            if (!_ecStudentPerfection.Validate()) return;
            _saving = true;
            StateHasChanged();

            _studentPerfectionUpdateModel.ProgramId = _updatedProgram.Id;

            var dto = Mapper.Map<StudentPerfectionDTO>(_studentPerfectionUpdateModel);
            try
            {
                var response = await StudentPerfectionService.PutAsync((long)_studentPerfectionUpdateModel.Id, dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");

                    _studentPerfectionDetailModal.CloseModal();
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
            }
            _saving = false;
            StateHasChanged();
            await LoadPerfections();
        }
        private async Task CompleteAllPerfections()
        {
            try
            {
                var response = await StudentPerfectionService.CompleteAllStudentPerfections((long)_student.Id, PerfectionType);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");

                    _studentPerfectionDetailModal.CloseModal();
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
            }
            await LoadPerfections();
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

            result = await ProgramService.GetListForSearch(new FilterDTO()
            {
                Filter = new Filter()
                {
                    Logic = "or",
                    Filters = filterList
                },
                page = 1,
                pageSize = int.MaxValue
            });

            return result.Items ?? new List<ProgramResponseDTO>();
        }

        private string GetProgramClass()
        {
            return "form-control " + _newProgram is not null ? "invalid" : "";
        }

        private void OnChangeProgram(ProgramResponseDTO program)
        {
            _newProgram = program;
            _studentPerfection.ProgramId = program.Id;
            _studentPerfection.Educator = null;
            _studentPerfection.EducatorId = null;

            StateHasChanged();
        }

        private void OnChangeProgramUpdated(ProgramResponseDTO program)
        {
            _updatedProgram = program;
            _studentPerfectionUpdateModel.ProgramId = program.Id;
            _studentPerfectionUpdateModel.Educator = null;
            _studentPerfectionUpdateModel.EducatorId = null;

            StateHasChanged();
        }

        private async Task DownloadExcelFileClinical()
        {
            if (_loadingFile)
            {
                return;
            }
            _loadingFile = true;
            StateHasChanged();
            SweetAlert.IconAlert(SweetAlertIcon.success, "İndirme Başlatıldı", L["Download has been started, this proces may take while."]);

            var response = await PerfectionService.GetExcelByteArrayClinical(_student.Id);
            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"{_student.User.Name.Trim().Replace(' ', '_')}_Klinik_Yetkinlik_Listesi.xlsx", Convert.ToBase64String(response.Item));
                _loadingFile = false;
            }
            else
            {
                SweetAlert.ErrorAlert();
            }
            StateHasChanged();
        }

        private async Task DownloadExcelFileInterventional()
        {
            if (_loadingFile)
            {
                return;
            }
            _loadingFile = true;
            StateHasChanged();
            SweetAlert.IconAlert(SweetAlertIcon.success, "İndirme Başlatıldı", L["Download has been started, this proces may take while."]);

            var response = await PerfectionService.GetExcelByteArrayInterventional(_student.Id);
            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"{_student.User.Name.Trim().Replace(' ', '_')}_Girişimsel_Yetkinlik_Listesi.xlsx", Convert.ToBase64String(response.Item));
                _loadingFile = false;
            }
            else
            {
                SweetAlert.ErrorAlert();
            }
            StateHasChanged();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            val.PrintJson("value");
            _filter.page = item1;
            _filter.pageSize = item2;

            await LoadPerfections();
        }

        private async Task OnChangeFilter(ChangeEventArgs args, string propertyType)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            _filter.Filter.Filters.Add(new Filter()
            {
                Field = "PropertyType",
                Operator = propertyType,
                Value = value
            });

            await LoadPerfections();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            Console.WriteLine(filterName);

            int index = 0;
            if (filterName == "PerfectionType")
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            }
            else
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Operator == filterName);
            }

            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await LoadPerfections();
            }
        }

        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";

            int index = 0;

            if (filterName == "PerfectionType")
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            }
            else
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Operator == filterName);
            }
            return index >= 0;
        }
    }
}
