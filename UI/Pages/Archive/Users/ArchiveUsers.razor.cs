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
using UI.Services;

namespace UI.Pages.Archive.Users;

public partial class ArchiveUsers
{
    [Inject] private IAuthService AuthService { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    private List<UserPaginateResponseDTO> _users;
    private FilterDTO _filter;
    private PaginationModel<UserPaginateResponseDTO> _paginationModel;
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

        await GetUsers();

        await base.OnInitializedAsync();
    }


    private async Task OnSortChange(Sort sort)
    {
        _filter.Sort = new[] { sort };
        await GetUsers();
    }

    private async Task GetUsers()
    {
        _paginationModel = await AuthService.GetArchiveList(_filter);
        if (_paginationModel.Items != null)
        {
            _users = _paginationModel.Items;
            StateHasChanged();
        }
        else
        {
            _loading = true;
            SweetAlert.ErrorAlert();
        }
    }

    private async Task OnUndeleteHandler(UserPaginateResponseDTO user)
    {
        var confirm = await SweetAlert.ConfirmAlert("Emin Misiniz?",
            "Bu öğeyi geri almak istediğinize emin misiniz?",
            SweetAlertIcon.question, true, "Geri Al", "İptal");
        if (confirm)
        {
            try
            {
                var response = await AuthService.UnDeleteUser(user.Id);

                if (response.Result)
                {
                    NavigationManager.NavigateTo($"./archive/users");
                    _users.Remove(user);
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
                }
                else
                {
                    SweetAlert.ConfirmAlert(L["Warning"], response.Message, SweetAlertIcon.warning, false, "Tamam", "");
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

        await GetUsers();
    }
    private void OnDetailHandler(StudentResponseDTO student)
    {
        NavigationManager.NavigateTo($"/student/students/{student.Id}");
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
        await GetUsers();
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
            await GetUsers();
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