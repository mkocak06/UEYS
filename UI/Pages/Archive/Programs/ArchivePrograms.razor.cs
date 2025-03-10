using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Models;
using UI.Services;

namespace UI.Pages.Archive.Programs
{
    public partial class ArchivePrograms
    {
        [Inject] private IProgramService ProgramService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<ProgramResponseDTO> _programs;
        private FilterDTO _filter;
        private PaginationModel<ProgramResponseDTO> _paginationModel;
        private bool _loading = false;
        [Inject] public IDispatcher Dispatcher { get; set; }
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
                Field = "Hospital.Province.Name",
                Dir = SortType.ASC
            }}
            };

            await GetPrograms();

            await base.OnInitializedAsync();
        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetPrograms();
        }

        private async Task GetPrograms()
        {

            _paginationModel = await ProgramService.GetArchiveList(_filter);
            if (_paginationModel.Items != null)
            {
                _programs = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task OnUndeleteHandler(ProgramResponseDTO program)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to take back this item?"]}",
                SweetAlertIcon.question, true, $"{L["Take Back"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    var response = await ProgramService.Undelete(program.Id);

                    if (response.Result)
                    {
                        NavigationManager.NavigateTo($"./archive/programs");
                        _programs.Remove(program);
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
                    }
                    else
                    {
                        SweetAlert.ConfirmAlert(L["Warning"], L[response.Message], SweetAlertIcon.error, false, "Tamam", "");
                    }
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

            await GetPrograms();
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