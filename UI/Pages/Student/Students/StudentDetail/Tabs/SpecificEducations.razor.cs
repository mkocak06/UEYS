using AutoMapper;
using Fluxor;
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
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Student.Students.StudentDetail.Tabs
{
    public partial class SpecificEducations
    {
        [Inject] public IState<StudentDetailState> StudentDetailState { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStudentSpecificEducationService StudentSpecificEducationService { get; set; }
        [Inject] public ISpecificEducationService SpecificEducationService { get; set; }
        [Inject] public ISpecificEducationPlaceService SpecificEducationPlaceService { get; set; }
        [Inject] IDocumentService DocumentService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        private StudentResponseDTO Student => StudentDetailState.Value.Student;

        private List<StudentSpecificEducationResponseDTO> _studentSpecificEducations;
        private StudentSpecificEducationResponseDTO _studentSpecificEducation;
        private FilterDTO _filter;
        private PaginationModel<StudentSpecificEducationResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private bool _isSelectFromList = true;
        private EditContext _studentSpecificEducationEc;
        private MyModal _studentSpecificEducationModal;

        private Dropzone dropzone;


        private bool _fileLoaded = true;
        private List<DocumentResponseDTO> responseDocuments = new();
        private bool forceRender;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _studentSpecificEducation = new() { StudentId = Student.Id };
            _studentSpecificEducationEc = new(_studentSpecificEducation);

            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                {
                    new Filter()
                    {
                        Field="StudentId",
                        Operator="eq",
                        Value=Student.Id
                    },
                    new Filter()
                    {
                        Field="IsDeleted",
                        Operator="eq",
                        Value=false
                    }
                },
                    Logic = "and"

                },
                page = 1,
                pageSize = 10,

                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "SpecificEducation. Name"
                    }
                }
            };

            await GetStudentSpecificEducations();
        }
        private async Task GetStudentSpecificEducations()
        {
            _paginationModel = await StudentSpecificEducationService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _studentSpecificEducations = _paginationModel.Items.ToList();
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }
        private void OpenModalForAdd()
        {
            _studentSpecificEducation = new() { StudentId = Student.Id };
            _studentSpecificEducationEc = new EditContext(_studentSpecificEducation);

            _studentSpecificEducationModal.OpenModal();
        }
        private async void OpenModalForEdit(StudentSpecificEducationResponseDTO specificEducation)
        {
            try
            {
                var response = await StudentSpecificEducationService.GetById(specificEducation.Id);
                if (response.Result)
                {
                    _studentSpecificEducation = response.Item;
                    _studentSpecificEducationEc = new EditContext(_studentSpecificEducation);
                    StateHasChanged();
                    _studentSpecificEducationModal.OpenModal();
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }


        }
        private async Task DeleteStudentSpecificEducation(StudentSpecificEducationResponseDTO specificEducation)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await StudentSpecificEducationService.Delete((long)specificEducation.Id);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
                await GetStudentSpecificEducations();
            }
        }
        private async Task<IEnumerable<SpecificEducationPlaceResponseDTO>> SearchSpecificEducationPlaces(string searchQuery)
        {
            try
            {
                var result = await SpecificEducationPlaceService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("IsDeleted", "eq", false, "and"));
                return result.Items.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)));
            }
            catch (Exception e)
            {
                return new List<SpecificEducationPlaceResponseDTO>();
                Console.WriteLine(e);
            }
        }

        private void OnChangeSpecificEducationPlace(SpecificEducationPlaceResponseDTO specificEducationPlace)
        {
            _studentSpecificEducation.SpecificEducationPlace = specificEducationPlace;
            _studentSpecificEducation.SpecificEducationPlaceId = specificEducationPlace?.Id;
        }
        private async Task<IEnumerable<SpecificEducationResponseDTO>> SearchSpecificEducations(string searchQuery)
        {
            try
            {
                var result = await SpecificEducationService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Filter("IsDeleted", "eq", false, "and")
                    .Filter("CurriculumId", "eq", Student.CurriculumId, "and"));

                return result.Items.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<SpecificEducationResponseDTO>();
            }
        }

        private void OnChangeSpecificEducation(SpecificEducationResponseDTO specificEducation)
        {
            _studentSpecificEducation.SpecificEducation = specificEducation;
            _studentSpecificEducation.SpesificEducationId = specificEducation?.Id;
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
                }
                else
                {
                    throw new Exception();
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
                _fileLoaded = true;
                StateHasChanged();
            }
            return true;

        }
        private async Task SaveStudentSpecificEducation()
        { 
            if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {

                await CallDropzone(dropzone);
                _studentSpecificEducation.Documents ??= responseDocuments;
            }
            if (!_studentSpecificEducationEc.Validate()) return;

           

            _saving = true;
            StateHasChanged();
            ResponseWrapper<StudentSpecificEducationResponseDTO> response;
            try
            {
                if (_studentSpecificEducation.Id > 0)
                {
                    response = await StudentSpecificEducationService.Update(_studentSpecificEducation.Id, Mapper.Map<StudentSpecificEducationDTO>(_studentSpecificEducation));
                }
                else
                {
                    response = await StudentSpecificEducationService.Add(Mapper.Map<StudentSpecificEducationDTO>(_studentSpecificEducation));
                    if (_studentSpecificEducation.Documents?.Count > 0)
                    {

                        foreach (var item in _studentSpecificEducation.Documents)
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
                }

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Saved"]}");
                    _studentSpecificEducationModal.CloseModal();
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
            await GetStudentSpecificEducations();
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetStudentSpecificEducations();
        }

        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetStudentSpecificEducations();
        }

        #region FilterChangeHandlers
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
                    Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            await GetStudentSpecificEducations();
        }
        private async Task OnChangeCheckBoxFilter(ChangeEventArgs args, string filterName)
        {
            var value = (bool)args.Value;
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
                    Operator = "eq",
                    Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }
            await GetStudentSpecificEducations();
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
                await GetStudentSpecificEducations();
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