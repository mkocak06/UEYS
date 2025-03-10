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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.DetailCards;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Components
{
    public partial class GridEducators
    {
        [Parameter] public long Id { get; set; }
        [Parameter] public bool AddEducator { get; set; } = false;
        [Parameter] public bool ByProgramId { get; set; } = false;
        [Parameter] public bool ByUniversityId { get; set; } = false;
        [Parameter] public bool ByHospitalId { get; set; } = false;
        [Parameter] public string FilterField { get; set; }
        [Parameter] public ProgramResponseDTO Program { get; set; }
        [Parameter] public EducationOfficerDTO educatorOfficer { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }

        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IState<AppState> AppState { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IEducatorService EducatorService { get; set; }
        [Inject] IProgramService ProgramService { get; set; }
        [Inject] IEducatorProgramService EducatorProgramService { get; set; }
        [Inject] IEducationOfficerService EducationOfficerService { get; set; }
        [Inject] ITitleService TitleService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Inject] IMapper Mapper { get; set; }

        private List<EducatorPaginateResponseDTO> _educators;
        private PaginationModel<EducatorPaginateResponseDTO> _paginationModel;

        private EducatorViewer _selectedEducatorDetail;
        private EducatorPaginateResponseDTO _selectedEducator;
        private EducatorProgramResponseDTO _educatorProgram = new();
        private MyModal FileModal;

        private bool forceRender = false;
        private MyModal _addEducatorModal;
        private MyModal _addDocumentModal;
        public Dropzone dropzoneOfficer;
        private EditForm form1;
        private string _officerValidationMessage;
        private EducatorPaginateResponseDTO _educator;
        private string _documentOrder;

        private List<DocumentResponseDTO> Documents = new();

        private bool _loading;
        private bool isKPSResult = false;
        private FilterDTO _filter = new();

        private string _identityNo = string.Empty;
        private bool _searching = false;
        private InputMask _inputMask;

        private UserWithEducatorInfoResponseDTO _userNotKPS;
        private AddUserWithEducatorInfoDTO _user;
        private TitleResponseDTO _academicTitle = new();
        private TitleResponseDTO _staffTitle = new();
        private string _tcValidatorMessage;

        private bool _adding;
        private EditContext _ec;

        private bool isAdmin = false;
        protected override async void OnInitialized()
        {
            _filter = new FilterDTO()
            {
                Filter = new()
                {
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
                    Logic = "and"

                },

                Sort = new[] { new Sort()
                {
                    Field = "Name",
                    Dir = SortType.ASC
                } }
            };

            if (ByProgramId)
            {
                _filter.Filter.Filters.Add(new() { Field = Program.ExpertiseBranch.IsPrincipal == true ? "PrincipalBranchDutyPlaceId" : "SubBranchDutyPlaceId", Operator = "eq", Value = Id });
            }
            else if (ByHospitalId)
            {
                _filter.Filter.Filters.Add(new() { Field ="DutyPlaceHospitalId", Operator = "eq", Value = Id });
            }
            else
            {
                _filter.Filter.Filters.Add(new()
                {
                    Filters = new()
                    {
                        new()
                        {
                            Field = "PrincipalBranchDutyPlaceUniversityId",
                            Operator = "eq",
                            Value = Id
                        },
                        new()
                        {
                            Field = "SubBranchDutyPlaceUniversityId",
                            Operator = "eq",
                            Value = Id
                        }
                    },
                    Logic = "or"
                });
            }
            await GetEducators();
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {

            if (forceRender)
            {
                forceRender = false;
                JSRuntime.InvokeVoidAsync("initTooltip");
            }

            base.OnAfterRender(firstRender);

        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetEducators();
        }

        private async Task GetEducators()
        {
            try
            {
                _loading = true;
                if (_filter?.Filter?.Filters?.Count == 0)
                {
                    _filter.Filter.Filters = null;
                    _filter.Filter.Logic = null;
                }
                _paginationModel = await EducatorService.GetPaginateListOnly(_filter);
                //else if (ByUniversityId)
                //{
                //    _paginationModel = await EducatorService.GetPaginateListByUniversityId(_filter, Id);
                //}
                //else if (ByHospitalId)
                //{
                //    _paginationModel = await EducatorService.GetPaginateListByHospitalId(_filter, Id);
                //}
                //else
                //{
                //    _paginationModel = await EducatorService.GetPaginateList(_filter);
                //}

                if (_paginationModel.Items != null)
                {
                    _educators = _paginationModel.Items;
                    forceRender = true;
                }
                else
                {
                    _educators = new();
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message);
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }
        private void OnEducatorDetailHandler(EducatorPaginateResponseDTO educator)
        {
            _selectedEducator = educator;
            _selectedEducator.Id = educator.Id;
            _selectedEducatorDetail.OpenModal();
        }



        private async Task OnChangeAdminHandler(EducatorPaginateResponseDTO educator)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Education Officer"]}", ProgramDetailState.Value.Program.Program.EducationOfficers.Count == 0 ?
                      $"{L["Are you sure you want to make this person education officer of the program?"]}" : $"{L["Are you sure you want to make this person education officer of the program? This choice will affect the current education officer."]}",
                      SweetAlertIcon.question, true, $"{L["Evet"]}", $"{L["No"]}");

            if (confirm)
            {
                _educator = educator;
                _officerValidationMessage = string.Empty;
                _addDocumentModal.OpenModal();
            }
            else
                return;
        }

        private async Task<bool> ChangeProgramManager()
        {
            bool result = false;
            if (dropzoneOfficer._selectedFileName == null)
            {
                _officerValidationMessage = "Bu alan zorunludur!";
                return false;
            }
            else
            {
                result = await CallDropzone();
                _addDocumentModal.CloseModal();
            }

            try
            {
                if (result)
                {
                    var response = await EducationOfficerService.ChangeProgramManager(new EducationOfficerDTO() { ProgramId = ProgramDetailState.Value.Program.Program.Id, EducatorId = _educator.Id, DocumentOrder = _documentOrder, StartDate = DateTime.UtcNow });
                    if (response.Result)
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                    else
                        SweetAlert.ErrorAlert();
                }
            }
            catch (Exception)
            {
                SweetAlert.ErrorAlert();
            }

            Dispatcher.Dispatch(new ProgramDetailLoadAction(Id));
            return true;
        }

        public async Task<bool> CallDropzone()
        {
            try
            {
                var result = await dropzoneOfficer.SubmitFileAsync();
                if (result.Result)
                {
                    _documentOrder = result.Item.BucketKey;
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
                dropzoneOfficer.ResetStatus();
                StateHasChanged();
            }

        }


        private void CancelEducator()
        {
            _user = null;
            _userNotKPS = null;
            StateHasChanged();
        }

        private void NAvigateToAddingPage()
        {
            NavigationManager.NavigateTo($"/educator/educators/add-educator/{Id}");
        }

        private async Task OnDownloadHandler(long id)
        {
            id.PrintJson("id");
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(id, DocumentTypes.EducationOfficerAssignmentLetter);
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


        #region EducatorFilter
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;
            await GetEducators();
        }

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
            await GetEducators();
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
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetEducators();
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

        private string GetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
                return (string)_filter.Filter.Filters[index].Value;
            return string.Empty;
        }

        #endregion
    }

}
