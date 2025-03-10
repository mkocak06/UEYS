using AutoMapper;
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
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.VisitModule.Standard
{
    public partial class Standards
    {
        [Inject] private IStandardService StandardService { get; set; }
        [Inject] private IStandardCategoryService StandardCategoryService { get; set; }
        [Inject] private ICurriculumService CurriculumService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IMapper Mapper { get; set; }

        private List<StandardResponseDTO> _standards;
        private StandardResponseDTO _standard;
        private PaginationModel<StandardResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private FilterDTO _filter;
        private EditContext _standardEc;
        private MyModal _standardModal;
        private bool _saving;

        protected override async Task OnInitializedAsync()
        {
            _filter = new FilterDTO()
            {
                Sort = new[]{new Sort()
                {
                    Dir = SortType.ASC,
                    Field = "Name"
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
                    }
                }
            };

            _links = new List<BreadCrumbLink>()
            {
                new BreadCrumbLink()
                    {
                        IsActive = true,
                        To = "/",
                        OrderIndex = 0,
                        Title = L["Homepage"]
                    },new BreadCrumbLink()
                    {
                        IsActive = false,
                        To = "/visit-module/standards",
                        OrderIndex = 1,
                        Title = L["_List", L["Standard"]]
                    }
            };
            await GetStandards();
            await base.OnInitializedAsync();
        }

        private async Task GetStandards()
        {
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _paginationModel = await StandardService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _standards = _paginationModel.Items;
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
            _standard = new();
            _standardEc = new EditContext(_standard);

            _standardModal.OpenModal();
        }
        private void OpenModalForEdit(StandardResponseDTO standard)
        {
            _standard = standard;
            _standardEc = new EditContext(_standard);

            _standardModal.OpenModal();
        }
        private async Task<IEnumerable<StandardCategoryResponseDTO>> SearchStandardCategories(string searchQuery)
        {

            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Filter("IsDeleted", "eq", false, "and");
            filter.Sort("Name");

            var result = await StandardCategoryService.GetPaginateList(filter);
            return result.Items ?? new List<StandardCategoryResponseDTO>();

        }
        private void OnChangeStandardCategory(StandardCategoryResponseDTO standardCategory)
        {
            _standard.StandardCategory = standardCategory;
            _standard.StandardCategoryId = standardCategory?.Id;
        }
        private async Task<IEnumerable<CurriculumResponseDTO>> SearchCurriculums(string searchQuery)
        {

            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("ExpertiseBranch.Name", "contains", searchQuery, "and");
            filter.Filter("IsDeleted", "eq", false, "and");
            filter.Sort("ExpertiseBranch.Name");

            var result = await CurriculumService.GetPaginateList(filter);
            return result.Items ?? new List<CurriculumResponseDTO>();

        }
        private void OnChangeCurriculum(CurriculumResponseDTO curriculum)
        {
            if (curriculum == null || _standard.CurriculumId != curriculum.Id)
            {
                _standard.Curriculum = curriculum;
                _standard.CurriculumId = curriculum?.Id;
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
        private async Task OnDeleteHandler(StandardResponseDTO standard)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await StandardService.Delete(standard.Id);
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

        #region Filter
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
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetStandards();
        }
        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetStandards();
        }

        #endregion
    }
}
