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
using Shared.ResponseModels;
using Shared.Types;
using UI.Pages.Archive.Students.StudentDetail.Store;
using System.IO;
using AutoMapper;
using Shared.RequestModels;
using Shared.FilterModels.Base;
using System.Globalization;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs
{
    public partial class ScientificStudies
    {
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IScientificStudyService ScientificStudyService { get; set; }
        [Inject] public IStudyService StudyService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
        private Dropzone dropzone;
        private List<DocumentResponseDTO> responseDocuments = new();
        private bool _fileLoaded = true;
        private bool _loading = false;
        private bool _saving;
        private string _documentValidatorMessage;

        private StudyResponseDTO _study;
        private List<ScientificStudyResponseDTO> _scientificStudies;
        private ScientificStudyResponseDTO _scientificStudy;

        private PaginationModel<ScientificStudyResponseDTO> _paginationModel;
        private FilterDTO _filter;

        private ScientificStudyResponseDTO _scientificStudyForUpdate;

        private MyModal _scientificStudyAddModal;
        private MyModal _scientificStudyUpdateModal;
        private MyModal UploaderModal;
        private MyModal FileModal;

        private List<DocumentResponseDTO> Documents = new();

        private EditContext _ec;
        private EditContext _ecUpdate;

        private StudentResponseDTO Student => StudentDetailState.Value.Student;
        protected override async Task OnInitializedAsync()
        {
            _scientificStudy = new();
            _ec = new EditContext(_scientificStudy);

            _scientificStudyForUpdate = new() { Study = new() };
            _ecUpdate = new EditContext(_scientificStudyForUpdate);

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
            };

            await GetScientificStudies();

            //await GetDocuments();
            await base.OnInitializedAsync();
        }

        //private async Task GetDocuments()
        //{
        //    _loading = true;
        //    StateHasChanged();
        //    try
        //    {
        //        var response = await DocumentService.GetListByTypeByEntity(_student.Id, DocumentTypes.ScientificStudies);
        //        if (response.Result)
        //        {
        //            responseDocuments = response.Item;

        //        }
        //        else
        //        {
        //            SweetAlert.ErrorAlert();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine();
        //    }
        //    finally
        //    {
        //        _loading = false;
        //        StateHasChanged();
        //    }

        //}
        //public async Task<bool> CallDropzone()
        //{
        //    _fileLoaded = false;
        //    StateHasChanged();
        //    try
        //    {
        //        var result = await dropzone.SubmitFileAsync();
        //        if (result != null)
        //        {
        //            await GetDocuments();
        //            _fileLoaded = true;
        //            StateHasChanged();
        //            UploaderModal.CloseModal();
        //            return true;
        //        }
        //        else
        //        {
        //            SweetAlert.ErrorAlert();
        //            return false;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _fileLoaded = false;
        //        Console.WriteLine(e.Message);
        //        return false;
        //    }
        //    finally
        //    {
        //        dropzone.ResetStatus();
        //        StateHasChanged();
        //    }
        //}

        private void AddingList()
        {
            _scientificStudy = new();
            _ec = new EditContext(_scientificStudy);
            responseDocuments = new();
            StateHasChanged();
            _scientificStudyAddModal.OpenModal();
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
                    responseDocuments.Add(result.Item);
                    _fileLoaded = true;
                    StateHasChanged();
                    UploaderModal.CloseModal();
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

        private async Task OnDownloadHandler(ScientificStudyResponseDTO scientificStudy)
        {
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(scientificStudy.Id.Value, DocumentTypes.ScientificStudy);
                if (response.Result)
                {
                    Documents = response.Item;
                    FileModal.OpenModal();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            StateHasChanged();
        }

        private void OnOpenUpdateModal(ScientificStudyResponseDTO scientificStudy)
        {
            _scientificStudyForUpdate = scientificStudy;
            _ecUpdate = new EditContext(_scientificStudyForUpdate);
            _scientificStudyUpdateModal.OpenModal();
        }

        private void OnChangeStudy(StudyResponseDTO study)
        {
            _scientificStudy.Study = study;
            _scientificStudy.StudyId = study.Id;
            StateHasChanged();
        }

        private async Task OnRemoveScientificStudy(ScientificStudyResponseDTO scientificStudy)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                try
                {
                    await ScientificStudyService.Delete(scientificStudy.Id.Value);
                    _scientificStudies.Remove(scientificStudy);
                    StateHasChanged();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
            }
        }

        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetScientificStudies();
        }

        private async Task GetScientificStudies()
        {
            _paginationModel = await ScientificStudyService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                if (_filter.Sort == null)
                    _scientificStudies = _paginationModel.Items.OrderBy(x => x.ProcessDate).ToList();
                else
                    _scientificStudies = _paginationModel.Items;

                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task<IEnumerable<StudyResponseDTO>> SearchStudies(string searchQuery)
        {
            var result = await StudyService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) : new List<StudyResponseDTO>();
        }

        private void OnChangeStudyUpdate(StudyResponseDTO study)
        {
            _scientificStudyForUpdate.Study = study;
            _scientificStudyForUpdate.StudyId = study.Id;
            StateHasChanged();
        }

        private async Task UpdateScientificStudy()
        {
            _documentValidatorMessage = string.Empty;
            if (_scientificStudyForUpdate.Documents?.Count == 0 && string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = "Bu alan zorunludur!";
                StateHasChanged();
                return;
            }
            else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                await CallDropzone();
            }

            _saving = true;
            StateHasChanged();
            _scientificStudyForUpdate.StudentId = Student.Id;
            var dto = Mapper.Map<ScientificStudyDTO>(_scientificStudyForUpdate);
            try
            {
                var response = await ScientificStudyService.Update(_scientificStudyForUpdate.Id.Value, dto);
                if (response.Result)
                {
                    await GetScientificStudies();

                    _scientificStudyUpdateModal.CloseModal();
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

        private async Task AddScientificStudy()
        {
            if (string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                _documentValidatorMessage = "Bu alan zorunludur!";
                StateHasChanged();
                return;
            }
            else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
            {
                await CallDropzone();
            }
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();

            _scientificStudy.StudentId = Student.Id;
            var dto = Mapper.Map<ScientificStudyDTO>(_scientificStudy);
            try
            {
                var response = await ScientificStudyService.Add(dto);
                if (response.Result)
                {
                    foreach (var item in responseDocuments)
                    {
                        var documentDTO = Mapper.Map<DocumentDTO>(item);
                        documentDTO.EntityId = (long)response.Item.Id;
                        var result = await DocumentService.Update(item.Id, documentDTO);
                        if (!result.Result)
                        {
                            throw new Exception(result.Message);
                        }
                    }
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");

                    await GetScientificStudies();
                    _scientificStudyAddModal.CloseModal();
                    StateHasChanged();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {

                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
                Console.WriteLine(e);
            }
            _saving = false;
            StateHasChanged();
        }
    }
}