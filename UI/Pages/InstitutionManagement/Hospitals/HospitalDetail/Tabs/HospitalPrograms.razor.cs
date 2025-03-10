using Blazored.Typeahead;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UI;
using UI.Helper;
using UI.Models;
using UI.Pages.Hospitals.HospitalDetail.Store;
using UI.Services;
using UI.SharedComponents;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;
using UI.Validation;

namespace UI.Pages.InstitutionManagement.Hospitals.HospitalDetail.Tabs
{
    public partial class HospitalPrograms
    {
        [Inject] IDispatcher Dispatcher { get; set; }

        [Inject] IState<HospitalDetailState> HospitalDetailState { get; set; }
        [Inject] IProgramService ProgramService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        private HospitalResponseDTO _hospital => HospitalDetailState.Value.Hospital;

        private List<ProgramResponseDTO> _programs;

        private bool _loading = false;
        private bool _creatingPrograms = false;
        [Inject] public IJSRuntime JsRuntime { get; set; }

        private PaginationModel<ProgramResponseDTO> _paginationModel;

        private FilterDTO _filter;

        private List<BreadCrumbLink> _links;


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
                            Field="HospitalId",
                            Operator="eq",
                            Value=_hospital.Id
                        },
                        new Filter()
                        {
                            Field = "IsDeleted",
                            Operator = "eq",
                            Value = false
                        }
                    },
                    Logic = "and"

                },

                Sort = new[]
               {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "Hospital.Province.Name"
                    }
                }


            };

            await GetPrograms();
            await base.OnInitializedAsync();
        }
        private async Task GetPrograms()
        {

            _paginationModel = await ProgramService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _programs = _paginationModel.Items;
                StateHasChanged();
                _loading = false;
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
            await GetPrograms();
        }

        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetPrograms();
        }

        private async Task CreateProgramsByHospitalId()
        {

            if (_hospital.FacultyId is null or 0)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, "Bu hastanenin 'fakülte' bilgisi bulunmadığı için programlar oluşturulamadı.");
                return;
            }

            _creatingPrograms = true;
            StateHasChanged();
            try
            {

                var result = await ProgramService.CreateProgramsByHospitalId(_hospital.Id);

                if (result.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Programlar Başarıyla Oluşturuldu.");

                    await GetPrograms();
                }
                else
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.error, "Bir Hata Oluştu.");
                }
            }
            catch (Exception e)
            {

                SweetAlert.ToastAlert(SweetAlertIcon.error, "Bir Hata Oluştu.");
                Console.WriteLine(e.Message);
            }

            _creatingPrograms = false;
            StateHasChanged();
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
            await GetPrograms();
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
                await GetPrograms();
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