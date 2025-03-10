using ApexCharts;
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
using UI.Validation;

namespace UI.Pages.VisitModule.Form
{
    public partial class PlannedRequests
    {
        [Inject] private IAuthenticationService AuthenticationService { get; set; }
        [Inject] private IAuditFormService AuditFormService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private PaginationModel<FormResponseDTO> _formPaginationModel;
        private bool _loading;
        private bool _isEditing = false;
        private bool _isReview = false;
        private long? _formId;
        private List<BreadCrumbLink> _links;
        private FilterDTO _filter;

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
                    Title = L["Planlanan Talepler"]
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
                            Value = VisitStatusType.CommitteeAppointed
                        },
                    }
                }
            };

            await GetVisitForms();
            await base.OnInitializedAsync();
        }

        private void OnCanceled()
        {
            if(_isReview) _isReview = false;
            _isEditing = false;
            StateHasChanged();
        }

        private async Task GetVisitForms()
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
            await GetVisitForms();
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
                await GetVisitForms();
            }
        }
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetVisitForms();
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

            await GetVisitForms();
        }

        #endregion

    }
}