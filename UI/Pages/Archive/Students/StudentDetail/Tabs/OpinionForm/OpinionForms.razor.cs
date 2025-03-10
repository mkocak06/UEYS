using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using UI;
using UI.SharedComponents;
using Microsoft.AspNetCore.Authorization;
using UI.Services;
using UI.Models;
using UI.Helper;
using UI.Validation;
using Shared.Validations;
using UI.SharedComponents.Components;
using UI.SharedComponents.BlazorLeaflet;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Blazored.Typeahead;
using Microsoft.Extensions.Localization;
using AutoMapper;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Pages.Archive.Students.StudentDetail.Store;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs.OpinionForm
{
    public partial class OpinionForms
    {
        [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
        [Inject] private IOpinionService OpinionService { get; set; }
        [Inject] private IDocumentService DocumentService { get; set; }
        [Inject] private IStudentService StudentService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        public StudentOpinionForm studentOpinionFormForAdd { get; set; }
        public StudentOpinionForm studentOpinionFormForUpdate { get; set; }
        private List<OpinionFormResponseDTO> _opinionForms;
        private List<OpinionFormResponseDTO> _canceledOpinionForms;
        private FilterDTO _filter;
        private PaginationModel<OpinionFormResponseDTO> _paginationModel;
        private bool _loading = false;
        private bool _isEditing = false;
        private bool _isAdding = false;
        private OpinionFormResponseDTO OpinionForm = new();
        private List<DocumentResponseDTO> Documents = new();
        private MyModal _fileModal;
        private EditContext _ecPastOpinionForm;
        private MyModal _pastOpinionFormModal;
        private Dropzone dropzone;
        private OpinionFormResponseDTO _pastOpinionForm;
        private bool _saving = false;
        private string _documentValidatorMessage;
        private List<DocumentResponseDTO> _responseDocuments;
        private bool? _isLastOneDeleted;
        private UserForLoginResponseDTO _user => AuthenticationService.User;

        protected override async Task OnInitializedAsync()
        {
            _filter = new FilterDTO()
            {
                Filter = new()
                {
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
                        Field="Student.Id",
                        Operator = "eq",
                        Value = _student.Id
                    }
                },
                    Logic = "and"

                },

                Sort = new[]{new Sort()
            {
                Field = "Student.User.Name",
                Dir = SortType.ASC
            }}
            };

            await GetOpinions();
            await base.OnInitializedAsync();
        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetOpinions();
        }

        private async Task GetOpinions()
        {
            _paginationModel = await OpinionService.GetPaginateList(_filter);
            var response = await OpinionService.GetCanceledListByStudentId(_student.Id);
            _canceledOpinionForms = response.Item;
            if (_paginationModel.Items != null)
            {
                _opinionForms = _paginationModel.Items.OrderByDescending(x => x.StartDate).ToList();
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
            await CheckLastOne();
        }

        private async Task CheckLastOne()
        {
            List<OpinionFormResponseDTO> allOpinionForms = _opinionForms.Concat(_canceledOpinionForms).OrderByDescending(x => x.Period).ToList();
            _isLastOneDeleted = allOpinionForms?.FirstOrDefault()?.IsDeleted == true ? true : false;
            StateHasChanged();
        }

        private async Task OnDownloadHandler(OpinionFormResponseDTO opinion)
        {
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(opinion.Id.Value, DocumentTypes.OpinionForm);
                if (response.Result)
                {
                    Documents = response.Item;
                    _fileModal.OpenModal();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            _isEditing = false;
            StateHasChanged();
        }
        private async Task OnDetailHandler(OpinionFormResponseDTO opinion)
        {
            OpinionForm = opinion;
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(opinion.Id.Value, DocumentTypes.OpinionForm);
                if (response.Result)
                {
                    OpinionForm.Documents = response.Item;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _isEditing = true;
            StateHasChanged();
        }
        private async Task OnDeleteHandler(OpinionFormResponseDTO opinion)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["In case of canceling opinion form you have to re-add for it. Are you sure you want to cancel this opinion form?"]}",
                SweetAlertIcon.question, true, $"{L["Yes"]}", $"{L["No"]}");

            if (confirm)
            {
                try
                {
                    await OpinionService.Delete(opinion.Id.Value);
                    _opinionForms.Remove(opinion);
                    if (_student.Status == StudentStatus.TransferDueToNegativeOpinion || _student.Status == StudentStatus.EndOfEducationDueToNegativeOpinion)
                    {
                        var response = await OpinionService.CheckNegativeOpinions((long)opinion.StudentId);
                        if ((response.Item.IsTransferNecessary == false && _student.Status == StudentStatus.TransferDueToNegativeOpinion) || (response.Item.IsEndOfEducationNecessary == false && _student.Status == StudentStatus.EndOfEducationDueToNegativeOpinion))
                        {
                            _student.Status = StudentStatus.EducationContinues;
                            await StudentService.Update(_student.Id, Mapper.Map<StudentDTO>(_student));
                            NavigationManager.NavigateTo("/student/students");
                        }
                    }
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                    await GetOpinions();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
            }
        }

        private async void OnSaveOpinionForm()
        {
            await GetOpinions();
            _isAdding = false;
            StateHasChanged();
        }
        private string GetAverageClass(RatingResultType? avg)
        {
            if (avg == RatingResultType.Negative)
                return "table-danger";
            else if (avg == RatingResultType.Positive)
                return "table-success";
            else return "";
        }
        private async void OnUpdateOpinionForm()
        {
            await GetOpinions();
            _isEditing = false;
            StateHasChanged();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetOpinions();
        }
        #region FilterChangeHandlers

        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
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
            await GetOpinions();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetOpinions();
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

        private async Task DownloadFile()
        {
            try
            {
                var x = await OpinionService.DownloadExampleForm("9c5255f4-0fe3-4759-b117-b686b5fc2d78.xlsx");
                await JSRuntime.InvokeVoidAsync("saveAsFile", "Örnek Kanaat Formu.xlsx", Convert.ToBase64String(x.Item.FileContent));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
                throw;
            }
        }

        private async Task OnOpenPastOpinionFormModal()
        {
            _pastOpinionForm = new() { StudentId = _student.Id, SecretaryId = _user.Id, Secretary = new UserResponseDTO() { Name = _user.Name } };
            DateTime? startDate;
            if (_opinionForms?.Count > 0)
            {
                startDate = _opinionForms.OrderByDescending(x => x.EndDate).FirstOrDefault().EndDate?.AddDays(1);
                _pastOpinionForm.Period = (PeriodType)_opinionForms.Count + 1;
            }
            else
            {
                var response = await EducationTrackingService.GetEducationStartByStudentId(_student.Id);
                startDate = response.Item.ProcessDate;
                _pastOpinionForm.Period = PeriodType.Period_1;
            }
            _pastOpinionForm.StartDate = startDate;
            _pastOpinionForm.EndDate = startDate?.AddMonths(6);
            _ecPastOpinionForm = new(_pastOpinionForm);
            _documentValidatorMessage = L["This field is required"];
            OpinionFormRequestDTO opinionForm = new() { StudentId = _student.Id, StartDate = _pastOpinionForm.StartDate, EndDate = _pastOpinionForm.EndDate };
            var educationTrackings = await EducationTrackingService.GetTimeIncreasingRecordsByDate(opinionForm);

            _pastOpinionForm.EndDate = _pastOpinionForm.EndDate?.AddDays((int)educationTrackings.Item.Select(x=>x.AdditionalDays).Sum());

            StateHasChanged();
            _pastOpinionFormModal.OpenModal();
        }

        private async Task AddPastOpinionForm()
        {
            _documentValidatorMessage = null;
            if (!_ecPastOpinionForm.Validate())
            {
                return;
            }
            if (string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = L["This field is required"];
                StateHasChanged();
                return;
            }
            await CallDropzone();
            var dto = Mapper.Map<OpinionFormDTO>(_pastOpinionForm);
            try
            {
                _saving = true;
                StateHasChanged();
                var response = await OpinionService.Add(dto);
                if (response.Result)
                {
                    foreach (var item in _responseDocuments)
                    {
                        var documentDTO = Mapper.Map<DocumentDTO>(item);
                        documentDTO.EntityId = response.Item.Id.Value;
                        var result = await DocumentService.Update(item.Id, documentDTO);
                        if (!result.Result)
                            throw new Exception(result.Message);
                    }
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                    await GetOpinions();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", L["An Error Occured."]);
            }
            finally
            {
                _pastOpinionFormModal.CloseModal();
                _responseDocuments.Clear();
                _saving = false;
                dropzone.DisposeAsync();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzone()
        {
            _responseDocuments ??= new();
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    _responseDocuments.Add(result.Item);
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
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                dropzone.ResetStatus();
                StateHasChanged();
            }
        }

        private void OnChangeStartDate(DateTime? date)
        {
            _pastOpinionForm.StartDate = date?.Date;
            _pastOpinionForm.EndDate = _pastOpinionForm.StartDate?.AddMonths(6);
            StateHasChanged();

        }
    }
}