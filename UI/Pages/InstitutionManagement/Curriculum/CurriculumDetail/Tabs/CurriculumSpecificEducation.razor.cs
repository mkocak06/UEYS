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
    public partial class CurriculumSpecificEducation
    {
        [Parameter] public long CurriculumId { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public ISpecificEducationService SpecificEducationService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }

        private List<SpecificEducationResponseDTO> _specificEducations;
        private SpecificEducationResponseDTO _specificEducation;
        private FilterDTO _filter;
        private PaginationModel<SpecificEducationResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private bool _isSelectFromList = true;
        private EditContext _specificEducationEc;
        private MyModal _specificEducationModal;


        private bool forceRender;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _specificEducation = new() { CurriculumId = CurriculumId };
            _specificEducationEc = new(_specificEducation);

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

            await GetSpecificEducations();
        }
        private async Task GetSpecificEducations()
        {
            _paginationModel = await SpecificEducationService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _specificEducations = _paginationModel.Items.ToList();
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
            _specificEducation = new() { CurriculumId = CurriculumId };
            _specificEducationEc = new EditContext(_specificEducation);

            _specificEducationModal.OpenModal();
        }
        private void OpenModalForEdit(SpecificEducationResponseDTO specificEducation)
        {
            _specificEducation = specificEducation;
            _specificEducationEc = new EditContext(_specificEducation);

            _specificEducationModal.OpenModal();
        }
        private async Task DeleteSpecificEducation(SpecificEducationResponseDTO specificEducation)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    await SpecificEducationService.Delete((long)specificEducation.Id);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
                await GetSpecificEducations();
            }
        }

        private async Task SaveSpecificEducation()
        {
            if (!_specificEducationEc.Validate()) return;

            _saving = true;
            StateHasChanged();
            ResponseWrapper<SpecificEducationResponseDTO> response;
            try
            {
                if (_specificEducation.Id > 0)
                {
                    response = await SpecificEducationService.Update(_specificEducation.Id, Mapper.Map<SpecificEducationDTO>(_specificEducation));
                }
                else
                {
                    response = await SpecificEducationService.Add(Mapper.Map<SpecificEducationDTO>(_specificEducation));
                }

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Saved"]}");
                    _specificEducationModal.CloseModal();
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
            await GetSpecificEducations();
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetSpecificEducations();
        }

        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetSpecificEducations();
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
            await GetSpecificEducations();
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
            await GetSpecificEducations();
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
                await GetSpecificEducations();
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