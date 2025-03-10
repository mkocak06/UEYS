using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Models;
using UI.Services;

namespace UI.Pages.Archive.Hospitals
{
    public partial class ArchiveHospitals
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IHospitalService HospitalService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<HospitalResponseDTO> _hospitals;
        private FilterDTO _filter;
        private PaginationModel<HospitalResponseDTO> _paginationModel;
        private bool _loading;

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
                        Field="IsDeleted",
                        Operator="eq",
                        Value=true
                    }

                },
                    Logic = "and"

                },
                Sort = new[]{new Sort()
            {
                Dir = SortType.ASC,
                Field = "Name"
            }}
            };
            await GetHospitals();
            await base.OnInitializedAsync();
        }

        private async Task GetHospitals()
        {
           
            _paginationModel = await HospitalService.GetArchiveList(_filter);
            if (_paginationModel.Items != null)
            {
                _hospitals = _paginationModel.Items;
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
            await GetHospitals();
        }

        private async Task OnUndeleteHandler(HospitalResponseDTO hospital)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to take back this item?"]}",
                SweetAlertIcon.question, true, $"{L["Take Back"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    var response = await HospitalService.Undelete(hospital.Id);

                    if (response.Result)
                    {
                        NavigationManager.NavigateTo($"./archive/hospitals");
                        _hospitals.Remove(hospital);
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
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

            await GetHospitals();
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
            await GetHospitals();
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
                await GetHospitals();
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