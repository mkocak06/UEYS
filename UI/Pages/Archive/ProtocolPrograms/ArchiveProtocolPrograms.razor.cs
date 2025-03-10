using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Services;

namespace UI.Pages.Archive.ProtocolPrograms
{
    public partial class ArchiveProtocolPrograms
    {

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IProtocolProgramService ProtocolProgramService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<ProtocolProgramPaginatedResponseDTO> _protocolPrograms;
        private FilterDTO _filter;
        private PaginationModel<ProtocolProgramPaginatedResponseDTO> _paginationModel;
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
                Sort = new[]
              {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "ParentProgram.ExpertiseBranch.Name"
                    }
                }
            };
            await GetProtocolPrograms();
            await base.OnInitializedAsync();
        }

        private async Task GetProtocolPrograms()
        {
            
            _paginationModel = await ProtocolProgramService.GetArchiveList(_filter);
            if (_paginationModel.Items != null)
            {
                _protocolPrograms = _paginationModel.Items;
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
            await GetProtocolPrograms();
        }
        private async Task OnUndeleteHandler(ProtocolProgramPaginatedResponseDTO protocolProgram)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to take back this item?"]}",
                SweetAlertIcon.question, true, $"{L["Take Back"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    var response = await ProtocolProgramService.UnDelete(protocolProgram.Id);

                    if (response.Result)
                    {
                        NavigationManager.NavigateTo($"./archive/protocolprograms");
                        _protocolPrograms.Remove(protocolProgram);
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
                    }
                    else
                    {
                        throw new Exception(response.Message);
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

            await GetProtocolPrograms();
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
            await GetProtocolPrograms();
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
                await GetProtocolPrograms();
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
