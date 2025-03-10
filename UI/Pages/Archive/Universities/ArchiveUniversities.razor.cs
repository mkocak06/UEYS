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
using AutoMapper;
using Shared.RequestModels;
using UI.Models;

namespace UI.Pages.Archive.Universities
{
    public partial class ArchiveUniversities
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IUniversityService UniversityService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<UniversityResponseDTO> _universities;
        private FilterDTO _filter;
        private PaginationModel<UniversityResponseDTO> _paginationModel;
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
                    Field = "Name",
                    Dir = SortType.ASC
                }}
            };
            await GetUniversities();
            await base.OnInitializedAsync();
        }



        private async Task GetUniversities()
        {
            _paginationModel = await UniversityService.GetArchiveList(_filter);
            if (_paginationModel.Items != null)
            {
                _universities = _paginationModel.Items;
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
            await GetUniversities();
        }
        private async Task OnUndeleteHandler(UniversityResponseDTO university)
        {
            var confirm = await SweetAlert.ConfirmAlert("Emin Misiniz?",
                "Bu öðeyi geri almak istediðinize emin misiniz?",
                SweetAlertIcon.question, true, "Geri Al", "Ýptal");
            if (confirm)
            {
                try
                {
                    var response = await UniversityService.Undelete(university.Id);

                    if (response.Result)
                    {
                        NavigationManager.NavigateTo($"./archive/universities");
                        _universities.Remove(university);
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
        private async Task OnChangeFilterCheckBox(ChangeEventArgs args, string filterName)
        {
            var value = (bool)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
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
            await GetUniversities();
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetUniversities();
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
            await GetUniversities();
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
                await GetUniversities();
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