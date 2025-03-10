using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.ComplementPrograms;

public partial class ComplementPrograms
{
    [Inject] private IProtocolProgramService ProtocolProgramService { get; set; }
    [Inject] private ISweetAlert SweetAlert { get; set; }
    [Inject] private IJSRuntime JsRuntime { get; set; }
    private ProtocolProgramResponseDTO _complementProgram;
    private List<ProtocolProgramPaginatedResponseDTO> _complementPrograms;
    private FilterDTO _filter;
    private PaginationModel<ProtocolProgramPaginatedResponseDTO> _paginationModel;
    private bool _loading = false;
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private MyModal _deleteModal;
    private string _dateValidatorMessage;
    private List<BreadCrumbLink> _links;

    protected override async Task OnInitializedAsync()
    {
        _complementProgram = new ProtocolProgramResponseDTO();
        _filter = new FilterDTO()
        {
            Sort = new[]{new Sort()
            {
                Field = "ParentProgram.ExpertiseBranch.Name",
                Dir = SortType.ASC
            }}
        };
        _filter.Filter = new Filter()
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
                        Field="Type",
                        Operator="eq",
                        Value=ProgramType.Complement
                    }

                },

        };
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
                    To = "/institution-management/complement-programs",
                    OrderIndex = 1,
                    Title = L["Complement Programs"]
                }
        };
        await GetPrograms();

        await base.OnInitializedAsync();
    }

    private async Task OnSortChange(Sort sort)
    {
        _filter.Sort = new[] { sort };
        await GetPrograms();
    }
    private async Task DeleteComplementProgram()
    {
        if (_complementProgram.CancelingDate == null || _complementProgram.CancelingProtocolNo == null)
        {
            _dateValidatorMessage = L["Canceling Date and Canceling Decision No cannot be empty!"];
            StateHasChanged();
            return;
        }

        try
        {
            var response = await ProtocolProgramService.Update(_complementProgram.Id, _complementProgram);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Item Deleted!"]);
                await GetPrograms();
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
        finally
        {
            StateHasChanged();
            _deleteModal.CloseModal();
        }
    }

    private async Task OnDeleteHandler(ProtocolProgramPaginatedResponseDTO complementProgram)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Cancel Protocol"]}", $"{L["Cancel"]}");

        if (confirm)
        {
            _deleteModal.OpenModal();
            var cpToDelete = await ProtocolProgramService.Get(complementProgram.Id, DocumentTypes.ComplementProgram, ProgramType.Complement);
            _complementProgram = cpToDelete.Item;
        }
    }

    private async Task GetPrograms()
    {
        if (_filter?.Filter?.Filters?.Count == 0)
        {
            _filter.Filter.Filters = null;
            _filter.Filter.Logic = null;
        }

        _paginationModel = await ProtocolProgramService.GetPaginateList(_filter);
        if (_paginationModel.Items != null)
        {
            _complementPrograms = _paginationModel.Items;
            StateHasChanged();
        }
        else
        {
            _loading = true;
            SweetAlert.ErrorAlert();
        }
    }
    
    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetPrograms();
    }
    private void OnDetailHandler(ProtocolProgramResponseDTO protocolProgram)
    {
        NavigationManager.NavigateTo($"/institution-management/complement-programs/{protocolProgram.Id}");
    }

    #region FilterChangeHandlers

    private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
    {
        var value = (string)args.Value;
        _filter.Filter ??= new Filter();
        _filter.Filter.Filters ??= new List<Filter>();
        _filter.Filter.Logic ??= "and";
		_filter.page= 1;
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
		_filter.page= 1;
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