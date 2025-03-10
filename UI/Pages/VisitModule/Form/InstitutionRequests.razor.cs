using ApexCharts;
using AutoMapper;
using Blazored.Typeahead;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using global::Microsoft.AspNetCore.Components;
using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using Shared.Validations;
using System.Net.Http;
using System.Net.Http.Json;
using UI;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents;
using UI.SharedComponents.BlazorLeaflet;
using UI.SharedComponents.Components;
using UI.SharedComponents.DetailCards;
using UI.Validation;

namespace UI.Pages.VisitModule.Form
{
    public partial class InstitutionRequests
    {
        [Inject] private IAuthenticationService AuthenticationService { get; set; }
        [Inject] private IEducatorService EducatorService { get; set; }

        [Inject] private IAuthorizationDetailService AuthorizationDetailService { get; set; }
        [Inject] private IAuditFormService AuditFormService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IMapper Mapper { get; set; }

        private PaginationModel<FormResponseDTO> _formPaginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private FilterDTO _filter;
        private FilterDTO _filterCommittee;
        private MyModal _committeeModal;
        private MyModal _visitDateModal;
        private PaginationModel<EducatorPaginateResponseDTO> _committeePaginationModel;
        private bool _committeeLoading;
        private FormResponseDTO _selectedForm = new();
        private bool _savingForm;
        private bool forceRender;
        private bool _isEditing = false;
        private long? _formId;

        private AuthorizationDetailsModal _authDetailsModal;
        private List<string> _roles => AuthenticationService.Roles.Select(x => x.RoleName).ToList();

        protected override async Task OnInitializedAsync()
        {
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
                    OrderIndex = 1,
                    Title = L["Kurum Talepleri"]
                },
            };
            _filter = new FilterDTO()
            {
                Sort = new[]{new Sort()
                {
                    Dir = SortType.ASC,
                    Field = "Program.Hospital.Name"
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
                        new Filter()
                        {
                            Field = "VisitStatusType",
                            Operator = "eq",
                            Value = VisitStatusType.RequestCreated
                        }
                    }
                }
            };
            _filterCommittee = new FilterDTO()
            {
                Sort = new[]{new Sort()
            {
                Field = "Name",
                Dir = SortType.ASC
            }}
            };
            _filterCommittee.pageSize = 5;
            _filterCommittee.Filter = new Filter()
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
                            Field = "EducatorType",
                            Operator = "neq",
                            Value = EducatorType.NotInstructor
                        },
                         new Filter()
                        {
                            Field = "UserIsDeleted",
                            Operator = "eq",
                            Value = false
                        }

                },

            };
            await GetUnassignedForms();
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeSelectPicker");
            }
            if (forceRender)
            {
                forceRender = false;
                await JSRuntime.InvokeVoidAsync("initializeSelectPicker");

            }
        }
        private void OnCanceled()
        {
            _isEditing = false;
            StateHasChanged();
        }
        private async void GetAuthorizationDetails(long programId)
        {
            programId.PrintJson("sqqsqsq");
            try
            {
                var response = await AuthorizationDetailService.GetListByProgramId(programId);
                if (response.Result)
                {
                    _authDetailsModal.OpenModal(response.Item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
        }
        private string GetTextStyle(string authNo)
        {
            if (authNo == "3")

                return "color:white";
            else return "";

        }
        private async Task GetCommittees()
        {
            _committeeLoading = true;
            StateHasChanged();

            try
            {
                _committeePaginationModel = await EducatorService.GetPaginateListOnly(_filterCommittee);
                forceRender = true;
                SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla yüklendi.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally
            {
                _committeeLoading = false;
                StateHasChanged();
            }
        }
        private async Task AssignCommitteToForm(FormResponseDTO form)
        {
            _selectedForm = form;
            _committeeModal.OpenModal();
            await GetCommittees();
        }
        private void VisitDateToForm(FormResponseDTO form)
        {
            if (_selectedForm.OnSiteVisitCommittees == null || _selectedForm.OnSiteVisitCommittees?.Any() == false)
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Komisyon Üyeleri Girilmedi", "Lütfen Ziyaret Tarihini belirlemeden önce komisyon üyelerini atayýnýz");
                return;
            }
            _selectedForm = form;
            _visitDateModal.OpenModal();
        }
        private async Task GetUnassignedForms()
        {
            if (_filter?.Filter?.Filters?.Count == 0)
            {
                _filter.Filter.Filters = null;
                _filter.Filter.Logic = null;
            }
            _loading = true;
            StateHasChanged();
            try
            {
                _formPaginationModel = await AuditFormService.GetFormPaginateList(_filter);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }

        }
        private async void SaveCommittee()
        {
            _savingForm = true;
            StateHasChanged();
            if (_selectedForm.OnSiteVisitCommittees?.Count > 0 && _selectedForm.VisitStatusType < VisitStatusType.CommitteeAppointed)
                _selectedForm.VisitStatusType = VisitStatusType.CommitteeAppointed;
            try
            {
                var response = await AuditFormService.UpdateForm(_selectedForm.Id, Mapper.Map<FormDTO>(_selectedForm));
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Baþarýyla Kaydedildi");
                    GetUnassignedForms();
                }
                else
                    SweetAlert.ErrorAlert();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SweetAlert.ErrorAlert();
            }
            finally { _savingForm = false; StateHasChanged(); _committeeModal.CloseModal(); _visitDateModal.CloseModal(); }
        }

        private void OnAddCommittee(long userId)
        {
            _selectedForm.OnSiteVisitCommittees ??= new();
            if (_selectedForm.OnSiteVisitCommittees?.Any(x => x.UserId == userId) == true)
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Bu Kullanýcý Zaten Ekli!", "Eklemek istediðiniz üyenin zaten buraya ekli bir komisyon üyesidir.");
            else
            {
                _selectedForm.OnSiteVisitCommittees.Add(new OnSiteVisitCommitteeResponseDTO { UserId = userId });
            }
            StateHasChanged();
        }
        private void OnRemoveCommittee(long userId)
        {
            _selectedForm.OnSiteVisitCommittees ??= new();
            if (_selectedForm.OnSiteVisitCommittees?.Any(x => x.UserId == userId) == false)
                SweetAlert.IconAlert(SweetAlertIcon.warning, "Bu Kullanýcý Zaten Ekli Deðil!", "Kaldýrmak istediðiniz üye zaten buraya ekli deðil.");
            else
            {
                _selectedForm.OnSiteVisitCommittees.Remove(_selectedForm.OnSiteVisitCommittees.FirstOrDefault(x => x.UserId == userId));
            }
            StateHasChanged();
        }
        #region FilterForm
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
            await GetUnassignedForms();
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
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetUnassignedForms();
            }
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetUnassignedForms();
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

            await GetUnassignedForms();
        }

        #endregion

        #region FilterCommittee

        private async Task OnChangeSelectFilter(ChangeEventArgs args, string filterName)
        {
            var values = args.Value as string[];
            if (values != null)
            {
                var value = string.Join(",", values);
                _filterCommittee.Filter ??= new Filter();
                _filterCommittee.Filter.Filters ??= new List<Filter>();
                _filterCommittee.Filter.Logic ??= "and";
                _filterCommittee.page = 1;
                foreach (var item in values)
                {


                    if (_filterCommittee.Filter.Filters.Where(x => x.Field == filterName).All(x => x.Value.ToString() != item))
                    {
                        _filterCommittee.Filter.Filters.Add(new Filter()
                        {
                            Field = filterName,
                            Operator = "eq",
                            Value = item
                        });
                    }

                }
                foreach (var item in _filterCommittee.Filter.Filters.Where(x => x.Field == filterName).ToList())
                {

                    if (!values.Any(x => x == item.Value.ToString()))
                    {
                        _filterCommittee.Filter.Filters.Remove(item);
                    }
                }
                await GetCommittees();
            }
        }
        private async Task PaginationHandlerCommittee(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filterCommittee.page = item1;
            _filterCommittee.pageSize = item2;
            await GetCommittees();
        }

        private async Task OnSortChangeCommittee(Sort sort)
        {
            _filterCommittee.Sort = new[] { sort };
            await GetCommittees();
        }

        private async Task OnChangeFilterCommittee(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filterCommittee.Filter ??= new Filter();
            _filterCommittee.Filter.Filters ??= new List<Filter>();
            _filterCommittee.Filter.Logic ??= "and";
            var index = _filterCommittee.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filterCommittee.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "contains",
                    Value = value
                });
            }
            else
            {
                _filterCommittee.Filter.Filters[index].Value = value;
            }
            await GetCommittees();
        }

        private async Task OnResetFilterCommittee(string filterName)
        {
            _filterCommittee.Filter ??= new Filter();
            _filterCommittee.Filter.Filters ??= new List<Filter>();
            _filterCommittee.Filter.Logic ??= "and";
            var index = _filterCommittee.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index >= 0)
            {
                _filterCommittee.Filter.Filters.RemoveAt(index);
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetCommittees();
            }
        }

        private bool IsFilteredCommittee(string filterName)
        {
            _filterCommittee.Filter ??= new Filter();
            _filterCommittee.Filter.Filters ??= new List<Filter>();
            _filterCommittee.Filter.Logic ??= "and";
            var index = _filterCommittee.Filter.Filters.FindIndex(x => x.Field == filterName);
            return index >= 0;
        }
        #endregion
    }
}