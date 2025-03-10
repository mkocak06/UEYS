using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.ResponseModels;
using UI.Services;
using System.Linq;
using Shared.FilterModels.Base;
using Shared.Types;
using Microsoft.JSInterop;
using UI.Models;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.SharedComponents.Components;
using AutoMapper;
using Shared.RequestModels;
using UI.SharedComponents.Store;
using System.Globalization;
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.Helper;
using Humanizer;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs

{

    public partial class StudentThesis
    {
        [Parameter] public long Id { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IEducatorService EducatorService { get; set; }
        [Inject] public IRotationService RotationService { get; set; }
        [Inject] public IAdvisorThesisService AdvisorThesisService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IThesisService ThesisService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }

        [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
        [Inject] public IState<AppState> AppState { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }

        private AddStudentThesis AddStudentThesis { get; set; }
        private AddStudentThesis UpdateStudentThesis { get; set; }
        private AdvisorThesisDTO educatorThesis = new();
        private ThesisResponseDTO _thesis;
        private ThesisResponseDTO _thesisForUpdate;
        private FilterDTO _filterEducator;
        private FilterDTO _filter;
        private List<BreadCrumbLink> _links;
        private PaginationModel<EducatorResponseDTO> _educatorPaginationModel;
        private PaginationModel<ThesisResponseDTO> _thesisPaginationModel = new();

        private EditContext _ec;
        private EditContext _thesisUpdateEc;
        private EditContext _thesisAddEc;

        private MyModal _thesisAddModal;
        private MyModal _thesisDetailModal;
        private MyModal _educatorListModal;

        private bool _loaded;
        private bool _saving;
        private bool _loadingThesis;
        private bool _loadedThesisForUpdate=false;
        private bool _isSuccess;
        private bool _delete;
        private bool _educatorLoading;
        private bool forceRender;
        public bool _isAddingPage = false;
        public bool _isUpdatePage = false;

        private StudentResponseDTO SelectedStudent => StudentDetailState.Value.Student;


        protected override async Task OnInitializedAsync()
        {
            _isSuccess = false;
            _delete = false;

            _thesis = new ThesisResponseDTO()
            {
                AdvisorTheses = new List<AdvisorThesisResponseDTO>()
            };
            _thesisForUpdate = new();

            _thesisAddEc = new EditContext(_thesis);


            _filterEducator = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "User.Name",
                Dir = SortType.ASC
            }}
            };
            _filter = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "Subject",
                Dir = SortType.ASC
            }}
            };
            _filter.Filter = new Filter()
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
                        Field="StudentId",
                        Operator="eq",
                        Value=SelectedStudent.Id
                    }

                },

            };
            await GetTheses();

            await base.OnInitializedAsync();
        }


        public async Task GetTheses()
        {
            _loadingThesis = true;
            if (SelectedStudent.Id != 0)
            {
                try
                {
                    _thesisPaginationModel = await ThesisService.GetPaginateList(_filter);
                    if (_thesisPaginationModel.Items != null)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Thesis Information has been successfully loaded."]);
                    }
                    else
                    {
                        _loadingThesis = true;
                        SweetAlert.ErrorAlert();
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _loadingThesis = true;
                    StateHasChanged();
                }
                finally
                {
                    _loadingThesis = false;
                    StateHasChanged();
                }
            }

        }
        //private void OnIsActiveChangeHandler()
        //{
        //    _thesis.Result = !_thesis.Result;

        //    StateHasChanged();
        //}
        private void OnThesisAddList()
        {
            _delete = false;
            _thesis = new ThesisResponseDTO();
            _thesisAddEc = new EditContext(_thesis);

            _thesisAddModal.OpenModal();
        }

        private async Task<IEnumerable<EducatorResponseDTO>> SearchEducators(string searchQuery)
        {

            if (SelectedStudent.Program.Faculty != null)
            {
                var result = await EducatorService.GetListByUniversityId((long)SelectedStudent.Program.Faculty.UniversityId);
                return result.Result ? result.Item.Where(x => x.User.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<EducatorResponseDTO>();
            }
            else
            {
                return Enumerable.Empty<EducatorResponseDTO>();
            }
        }



        private async Task AddThesis()
        {
            if (!_thesisAddEc.Validate()) return;

            _saving = true;
            StateHasChanged();
            _thesis.StudentId = SelectedStudent.Id;
            var dto = Mapper.Map<ThesisDTO>(_thesis);

            try
            {
                var response = await ThesisService.PostAsync(dto);

                if (response.Result)
                {

                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");

                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred while adding a Thesis."]);
                Console.WriteLine(e.Message);
            }
            _thesisAddModal.CloseModal();
            _saving = false;
            StateHasChanged();
            await GetTheses();
        }
        private async Task UpdateThesis()
        {

            if (!_thesisUpdateEc.Validate()) return;
            _saving = true;
            StateHasChanged();
            _thesisForUpdate.StudentId = SelectedStudent.Id;
            var dto = Mapper.Map<ThesisDTO>(_thesisForUpdate);
            try
            {
                var response = await ThesisService.Put((long)_thesisForUpdate.Id, dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                    _thesisDetailModal.CloseModal();
                    await GetTheses();
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
        }
        //private void OnChangeResult1(ThesisResponseDTO thesisResponse)
        //{
        //    if (_issuccess == false)
        //    {
        //        thesisResponse.Result = _issuccess;
        //        SweetAlert.IconAlert(SweetAlertIcon.info, "", L["Training time extension information must be entered."]);
        //        StateHasChanged();
        //    }
        //}
        //private void OnChangeResult2(ThesisResponseDTO thesisResponse)
        //{
        //    if (_issuccess == false)
        //    {
        //        thesisResponse.Result = !_issuccess;
        //        StateHasChanged();
        //    }
        //}
        //private void OnChangeResultUpdate()
        //{
        //    _thesis.Result = !_thesis.Result;
        //    if (_thesis.Result == false)
        //    {
        //        SweetAlert.IconAlert(SweetAlertIcon.info, "", L["Training time extension information must be entered."]);
        //    }

        //    StateHasChanged();
        //}
        private async Task OnUpdateHandler(ThesisResponseDTO thesisResponse)
        {
            try
            {
                var response = await ThesisService.GetById(thesisResponse.Id.Value);
                if (response.Result)
                {
                    _thesisForUpdate = response.Item;
                    _isAddingPage = false;
                    _isUpdatePage = true;
                    StateHasChanged();
                }
                else
                {
                    SweetAlert.ErrorAlert();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
        }
        private async Task OnDeleteHandler(ThesisResponseDTO thesisResponse)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await ThesisService.Delete(thesisResponse.Id.Value);
                    _thesisPaginationModel.Items.Remove(thesisResponse);

                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                    await GetTheses();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An error occurred during Thesis deletion."]);
                    throw;
                }
            }
        }


        private async Task OpenEducatorModal()
        {
            await GetEducator();
            _educatorListModal.OpenModal();
        }
        private async Task GetEducator()
        {
            _educatorLoading = true;
            StateHasChanged();
            if (_filterEducator?.Filter?.Filters?.Count == 0)
            {
                _filterEducator.Filter.Filters = null;
                _filterEducator.Filter.Logic = null;
            }
            _educatorPaginationModel = await EducatorService.GetPaginateList(_filterEducator);
            _educatorLoading = false;
            StateHasChanged();
        }
        private bool IsValıdEducator(EducatorResponseDTO educator)
        {
            return _thesis.AdvisorTheses.Any(x => x.Educator.Id == educator.Id);
        }
        private async Task OnAddEducatorThesisHandler(EducatorResponseDTO educator)
        {

            educatorThesis = new AdvisorThesisDTO()
            {
                EducatorId = educator.Id.Value,
                ThesisId = _thesis.Id.Value,
                AdvisorAssignDate = DateTime.UtcNow
            };
            try
            {
                var response = await AdvisorThesisService.Add(educatorThesis);

                if (response.Result)
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                else
                    SweetAlert.ErrorAlert();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await GetTheses();
        }
        //private async Task OnRemoveEducatorThesisHandler(EducatorResponseDTO educator)
        //{
        //    //TO DO: modify this func
        //    var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
        //        $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
        //        SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
        //    Console.WriteLine(educator.Id);
        //    if (confirm)
        //    {
        //        try
        //        {
        //            await AdvisorThesisService.Delete(educator.Id.Value, _thesis.Id.Value);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //            SweetAlert.ErrorAlert();
        //            throw;
        //        }
        //        await GetTheses();

        //    }
        //}
        private string GetAdminStyle(AdvisorThesisResponseDTO item)
        {
            if (item.IsCoordinator == true)
                return "<span class=\"label label-inline label-light-danger font-weight-bold\">" + "K" + "</span>";
            else
                return "";
        }
        private async Task PaginationHandlerEducator(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterEducator.page = item1;
            _filterEducator.pageSize = item2;

            await GetEducator();
        }
        private async Task PaginationHandlerThesis(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetTheses();
        }
        #region Filter EducatorList
        private async Task OnSortChange(Sort sort)
        {
            _filterEducator.Sort = new[] { sort };
            await GetEducator();
        }

        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
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
            await GetEducator();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filterEducator.Filter ??= new Filter();
            _filterEducator.Filter.Filters ??= new List<Filter>();
            _filterEducator.Filter.Logic ??= "and";
            var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filterEducator.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetEducator();
            }
        }

        private bool IsFiltered(string filterName)
        {
            _filterEducator.Filter ??= new Filter();
            _filterEducator.Filter.Filters ??= new List<Filter>();
            _filterEducator.Filter.Logic ??= "and";
            var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        #endregion
        #region Filter ThesisList
        private async Task OnSortThesisChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetTheses();
        }

        private async Task OnChangeThesisFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filter.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            await GetTheses();
        }

        private async Task OnResetThesisFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetTheses();
            }
        }

        private bool IsThesisFiltered(string filterName)
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