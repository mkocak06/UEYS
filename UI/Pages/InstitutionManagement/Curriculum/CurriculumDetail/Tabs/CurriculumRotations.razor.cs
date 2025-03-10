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
using UI.Helper;

namespace UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Tabs
{
    public partial class CurriculumRotations
    {
        [Parameter] public long CurriculumId { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IRotationService RotationService { get; set; }
        [Inject] public ICurriculumRotationService CurriculumRotationService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IState<AppState> AppState { get; set; }

        private List<RotationResponseDTO> _rotations;
        private RotationResponseDTO _rotation;
        private FilterDTO _filter;
        private PaginationModel<CurriculumRotationResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private bool _isrequired = false;
        private EditContext _ec;
        private EditContext _rotationEc;
        private MyModal _rotationAddModal;
        private MyModal _rotationDetailModal;

        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<CurriculumDetailState> CurriculumDetailState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private bool Loaded => CurriculumDetailState.Value.CurriculumsLoaded;

        private ExpertiseBranchResponseDTO _expertisebranch;
        private bool forceRender;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _rotation = new RotationResponseDTO();
            _rotationEc = new EditContext(_rotation);

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
                        Value=CurriculumId
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
                pageSize = int.MaxValue,

                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "Rotation.Duration"
                    }
                }
            };
            SubscribeToAction<CurriculumDetailSetAction>((action) =>
            {
                forceRender = true;

            });

            await GetRotations();
        }
        private async Task GetRotations()
        {
            _paginationModel = await CurriculumRotationService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _rotations = _paginationModel.Items.Select(x => x.Rotation).ToList();
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetRotations();
        }
        private void OnRotationAddList()
        {

            _rotation = new RotationResponseDTO();
            _rotationEc = new EditContext(_rotation);

            _rotationAddModal.OpenModal();
        }
        private async Task OnRotationDetailListAsync(RotationResponseDTO rotationResponse)
        {
            if (rotationResponse.Id != null)
            {
                var response = await RotationService.GetByIdAsync((long)rotationResponse.Id);
                if (response.Result && response.Item != null)
                {
                    _rotationDetailModal.OpenModal();
                    _rotation = response.Item;
                    _rotationEc = new EditContext(_rotation);
                    _loaded = true;
                    StateHasChanged();
                }
            }
            else
            {
                SweetAlert.ErrorAlert();
            }
        }
        private void OnChangeIsRequired()
        {
            _rotation.IsRequired = !_isrequired;

            StateHasChanged();
        }
        private void OnChangeIsRequiredUpdate()
        {
            _rotation.IsRequired = !_rotation.IsRequired;

            StateHasChanged();
        }
        private void OnChangeExpertiseBranch(ExpertiseBranchResponseDTO expertiseBranch)
        {
            _rotation.ExpertiseBranch = expertiseBranch;
            _rotation.ExpertiseBranchId = expertiseBranch?.Id;

            StateHasChanged();
        }
        private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ExpertiseBranchService.GetPaginateList(filter);
            return result.Items ?? new List<ExpertiseBranchResponseDTO>();
        }
        private async Task AddRotation()
        {
            if (!_rotationEc.Validate()) return;

            _saving = true;
            StateHasChanged();
            var dto = new CurriculumRotationDTO
            {
                CurriculumId = CurriculumId,
                Rotation = Mapper.Map<RotationDTO>(_rotation),
                RotationId = _rotation.Id
            };
            try
            {
                var response = await CurriculumRotationService.PostAsync(dto);

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                    _rotations.Add(_rotation);
                    _rotationAddModal.CloseModal();
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
            await GetRotations();
        }
        private async Task UpdateRotation()
        {
            if (!_rotationEc.Validate()) return;
            _saving = true;
            StateHasChanged();
            //_rotation.CurriculumId = SelectedCurriculum.Id;

            var dto = Mapper.Map<RotationDTO>(_rotation);
            try
            {
                var response = await RotationService.Put((long)_rotation.Id, dto);
                if (response.Result)
                {

                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                    await GetRotations();
                    _rotationDetailModal.CloseModal();
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
        private async Task OnDeleteHandler(RotationResponseDTO rotation)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    var curriculumRotation = _paginationModel.Items.FirstOrDefault(x => x.RotationId == rotation.Id);
                    await CurriculumRotationService.Delete((long)curriculumRotation.Id);
                    _rotations.Remove(rotation);
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

            await GetRotations();
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
            await GetRotations();
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
            await GetRotations();
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
                await GetRotations();
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