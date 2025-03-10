using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Student.Students.StudentDetail.Tabs.PerformanceRating
{
    public partial class PerformanceRatingDetail
    {
        [Parameter] public EventCallback<bool> OnSaveHandler { get; set; }
        [Parameter] public EventCallback<bool> OnUpdateHandler { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IState<StudentDetailState> StudentDetailState { get; set; }
        [Inject] public IPerformanceRatingService PerformanceRatingService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        private PerformanceRatingResponseDTO _pRating;
        private List<DocumentResponseDTO> responseDocuments = new();
        private PerformanceRatingResponseDTO PerformanceRating { get; set; }
        private UserForLoginResponseDTO _user => AuthenticationService.User;
        private EditContext _ec;
        private bool _loading = true;
        private bool[] Collapsed = { false, false, false, false, false, false };
        public bool _saving;
        Dropzone dropzone;
        private bool _fileLoaded = true;

        protected override async Task OnInitializedAsync()
        {
            var response = await PerformanceRatingService.GetByStudentId(_student.Id);

            _pRating = response.Item ?? new();

            _ec = new EditContext(_pRating);

            await base.OnInitializedAsync();
        }
        private string GetAverageClass(string avg)
        {
            if (string.IsNullOrEmpty(avg)) return "label label-xl label-info label-pill label-inline font-size-h4";
            double avgS = Convert.ToDouble(avg);
            if (avgS > 0 && avgS < 3)
                return "label label-xl label-danger label-pill label-inline font-size-h4";
            else if (avgS == 3)
                return "label label-xl label-warning label-pill label-inline font-size-h4";
            else if (avgS > 3)
                return "label label-xl label-success label-pill label-inline font-size-h4";
            else return "label label-xl  label-pill label-inline font-size-h4";
        }

        public async Task Save()
        {
            if (!_ec.Validate()) return;
            _saving = true;
            StateHasChanged();

            if (dropzone != null && !String.IsNullOrEmpty(dropzone._selectedFileName))
            {
                responseDocuments = new();
                await CallDropzone();
            }
            var dto = Mapper.Map<PerformanceRatingDTO>(_pRating);
            dto.StudentId = _student.Id;

            if (Convert.ToDouble(_pRating.OverallAverage) >= 3)
                dto.Result = RatingResultType.Positive;
            else
                dto.Result = RatingResultType.Negative;
            var docs = _pRating.Documents;
            try
            {
                if (_pRating.Id != null && _pRating.Id > 0)
                {
                    var response = await PerformanceRatingService.Update(_pRating.Id.Value, dto);
                    if (response.Result)
                    {
                        foreach (var item in _pRating.Documents)
                        {
                            var documentDTO = Mapper.Map<DocumentDTO>(item);
                            documentDTO.EntityId = response.Item.Id.Value;
                            var result = await DocumentService.Update(item.Id, documentDTO);
                            if (!result.Result)
                            {
                                throw new Exception(result.Message);
                            }
                        }
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                        await OnUpdateHandler.InvokeAsync(true);
                    }
                }
                else
                {
                    dto.CreateDate = DateTime.UtcNow;
                    var response = await PerformanceRatingService.Add(dto);
                    if (response.Result)
                    {
                        if (_pRating.Documents?.Count > 0)
                        {
                            foreach (var item in _pRating.Documents)
                            {
                                var documentDTO = Mapper.Map<DocumentDTO>(item);
                                documentDTO.EntityId = response.Item.Id.Value;
                                var result = await DocumentService.Update(item.Id, documentDTO);
                                if (!result.Result)
                                {
                                    throw new Exception(result.Message);
                                }
                            }
                        }
                        _pRating = response.Item;
                        _pRating.Documents = docs;
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                    }
                    else
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.error, L["An Error Occured."], L[response.Message]);
                    }
                    await OnSaveHandler.InvokeAsync(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            _saving = false;
            StateHasChanged();
        }

        public async Task<bool> CallDropzone()
        {
            _pRating.Documents ??= new List<DocumentResponseDTO>();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    _pRating.Documents.Add(result.Item);
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
    }
}