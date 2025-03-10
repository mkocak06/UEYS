using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Models;
using UI.Pages.User.Educator.Tabs;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Archive.Educators;

public partial class ArchiveEducators
{
    [Inject] private IEducatorService EducatorService { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    private List<EducatorPaginateResponseDTO> _educators;
    private FilterDTO _filter;
    private PaginationModel<EducatorPaginateResponseDTO> _paginationModel;
    private MyModal _detailModal;
    private EducatorPaginateResponseDTO _educator;
    private bool _loading = false;
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
                    },
                    new Filter()
                    {
                        Field="EducatorType",
                        Operator="eq",
                        Value=EducatorType.Instructor
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

        await GetEducators();

        await base.OnInitializedAsync();
    }

    private async Task OnSortChange(Sort sort)
    {
        _filter.Sort = new[] { sort };
        await GetEducators();
    }

    private async Task GetEducators()
    {
        _paginationModel = await EducatorService.GetArchiveList(_filter);
        if (_paginationModel.Items != null)
        {
            _educators = _paginationModel.Items;
            StateHasChanged();
        }
        else
        {
            _loading = true;
            SweetAlert.ErrorAlert();
        }
    }

    private async Task OnUndeleteHandler(EducatorPaginateResponseDTO educator)
    {
        var confirm = await SweetAlert.ConfirmAlert("Emin Misiniz?",
            "Bu öğeyi geri almak istediğinize emin misiniz?",
            SweetAlertIcon.question, true, "Geri Al", "İptal");
        if (confirm)
        {
            try
            {
                var response = await EducatorService.UnDelete((long)educator.Id);

                if (response.Result)
                {
                    NavigationManager.NavigateTo($"./archive/educators");
                    _educators.Remove(educator);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L[response.Message]);
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
    private async Task OnDetail(EducatorPaginateResponseDTO educator)
    {
        _educator = educator;
        StateHasChanged();
        _detailModal.OpenModal();
    }

    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetEducators();
    }
    private void OnDetailHandler(EducatorResponseDTO educator)
    {
        NavigationManager.NavigateTo($"/educator/educators/{educator.Id}");
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
        await GetEducators();
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
            await GetEducators();
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