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
using Shared.ResponseModels.Wrapper;

namespace UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Tabs
{
    public partial class CurriculumStandards
    {
        [Parameter] public long CurriculumId { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStandardService StandardService { get; set; }
        [Inject] public IStandardCategoryService StandardCategoryService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IState<AppState> AppState { get; set; }

        private List<StandardResponseDTO> _standards;
        private StandardResponseDTO _standard;
        private StandardResponseDTO _searchingStandard = new();
        private StandardCategoryResponseDTO _standardCategory = new();
        private FilterDTO _filter;
        private PaginationModel<StandardResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private bool _isSelectFromList = true;
        private EditContext _standardEc;
        private MyModal _standardModal;

        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private bool forceRender;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _standard = new() { CurriculumId = CurriculumId };
            _standardEc = new(_standard);

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
                pageSize = 10,

                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "Name"
                    }
                }
            };

            await GetStandards();
        }
        private async Task GetStandards()
        {
            _paginationModel = await StandardService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _standards = _paginationModel.Items.ToList();
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
            _standardCategory = new();
            _searchingStandard = new();
            _standard = new() { CurriculumId = CurriculumId };
            _standardEc = new EditContext(_standard);

            _standardModal.OpenModal();
        }
        private void OpenModalForEdit(StandardResponseDTO standard)
        {
            _standardCategory = standard.StandardCategory;
            _searchingStandard = standard;
            _standard = standard;
            _standardEc = new EditContext(_standard);

            _standardModal.OpenModal();
        }
        private async Task DeleteStandard(StandardResponseDTO standard)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await StandardService.Delete((long)standard.Id);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
                await GetStandards();
            }
        }
        private async Task<IEnumerable<StandardResponseDTO>> SearchStandards(string searchQuery)
        {
            if (_standard.StandardCategoryId is not null and not 0)
            {

                var filter = FilterHelper.CreateFilter(1, int.MaxValue);
                filter.Filter("Name", "contains", searchQuery.ToLower(CultureInfo.CurrentCulture), "and");
                filter.Filter("IsDeleted", "eq", false, "and");
                filter.Filter("StandardCategoryId", "eq", _standard.StandardCategoryId, "and");
                filter.Sort("Name");

                var result = await StandardService.GetPaginateListDistinct(filter);
                return result.Items ?? new List<StandardResponseDTO>();
            }
            else { return new List<StandardResponseDTO>(); }
        }
        private void OnChangeStandard(StandardResponseDTO standard)
        {
            if (standard != null)
            {
                _standard.Name = standard.Name;
                _standard.Code = standard.Code;
                _standard.Description = standard.Description;
                _searchingStandard.Name = standard.Name;
            }
            else
            {
                _searchingStandard.Name = string.Empty;
                _standard.Name = string.Empty;
                _standard.Code = string.Empty;
                _standard.Description = string.Empty;
            }
            _standardEc = new EditContext(_standard);
        }

        private async Task<IEnumerable<StandardCategoryResponseDTO>> SearchStandardCategories(string searchQuery)
        {

            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery.ToLower(CultureInfo.CurrentCulture), "and");
            filter.Filter("IsDeleted", "eq", false, "and");
            filter.Sort("Name");

            var result = await StandardCategoryService.GetPaginateList(filter);
            return result.Items ?? new List<StandardCategoryResponseDTO>();

        }

        private void OnChangeStandardCategory(StandardCategoryResponseDTO standardCategory)
        {
            if (standardCategory == null || _standard.StandardCategoryId != standardCategory?.Id)
            {
                _standard = new() { CurriculumId = CurriculumId };
                _standard.StandardCategory = standardCategory;
                _standardCategory= standardCategory;
                _standard.StandardCategoryId = standardCategory?.Id;
            }
            _standardEc = new EditContext(_standard);
        }

        private void OnChangeSelectForm(bool isSelectForm)
        {
            _isSelectFromList = isSelectForm;
            if (isSelectForm)
            {
                _standard.Name = null;
                _standard.Code = null;
                _standard.Description = null;
                _standardEc = new EditContext(_standard);
            }
        }
        private async Task SaveStandard()
        {
            if (!_standardEc.Validate()) return;

            _saving = true;
            StateHasChanged();
            ResponseWrapper<StandardResponseDTO> response;
            try
            {
                if (_standard.Id > 0)
                {
                    response = await StandardService.Update(_standard.Id, Mapper.Map<StandardDTO>(_standard));
                }
                else
                {
                    response = await StandardService.Add(Mapper.Map<StandardDTO>(_standard));
                }

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Saved"]}");
                    _standardModal.CloseModal();
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
            await GetStandards();
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetStandards();
        }

        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetStandards();
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
            await GetStandards();
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
            await GetStandards();
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
                await GetStandards();
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