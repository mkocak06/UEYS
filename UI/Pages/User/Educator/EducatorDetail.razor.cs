using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Pages.User.Educator.Tabs;
using UI.Services;
namespace UI.Pages.User.Educator;

public partial class EducatorDetail
{
    [Parameter] public long? Id { get; set; }
    [Inject] public IEducatorService EducatorService { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IState<EducatorDetailState> EducatorDetailState { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    //private bool Loaded => EducatorDetailState.Value.EducatorLoaded;
    //private bool _editing = false;
    private EducatorResponseDTO SelectedEducator => EducatorDetailState.Value.Educator;


    private List<BreadCrumbLink> _links;
    private bool forceRender;

    protected override async Task OnInitializedAsync()
    {
        //if (Id is not null)
        //{
            //_editing = true;
            //StateHasChanged();
            await GetBreadCrumbLinksForEditing();
        //}
        //else
        //{
        //    _editing = false;
        //    StateHasChanged();
        //    GetBreadCrumbLinksForAdding();
        //}
        await base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (forceRender)
        {
            forceRender = false;
            JSRuntime.InvokeVoidAsync("initDropdown");
        }
        base.OnAfterRender(firstRender);
    }
    protected override async void OnParametersSet()
    {
        if (Id != null)
        {
            //_editing = true;
            //StateHasChanged();
            //await GetBreadCrumbLinksForEditing();
            //StateHasChanged();
            Dispatcher.Dispatch(new EducatorDetailLoadAction((long)Id));
        }
        base.OnParametersSet();
    }
    private void GetBreadCrumbLinksForAdding()
    {
        forceRender = true;
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
                IsActive = true,
                To = "/educator/educators",
                OrderIndex = 1,
                Title =L["_List", L["Educator"]]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 1,
                Title = L["add_new",L["Educator"]]
            },
            };
    }
    private async Task GetBreadCrumbLinksForEditing()
    {
        //List<EducatorResponseDTO> educators = new();
        //try
        //{
        //    var response = await EducatorService.GetAll();
        //    if (response.Result)
        //    {
        //        foreach (var item in response.Item)
        //        {
        //            if (item.UserId != null)
        //                educators.Add(item);
        //        }
        //    }
        //}
        //catch (System.Exception)
        //{

        //}
        SubscribeToAction<EducatorDetailSetAction>((action) =>
        {
            forceRender = true;
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
                IsActive = true,
                To = "/educator/educators",
                OrderIndex = 1,
                Title = L["_List", L["Educator"]]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 2,
                Title = action?.Educator?.User?.Name,
                //DropdownList = educators.Select(x=> new DropdownElement()
                //{
                //    Link = "/educator/educators/"+x.Id,
                //    Name = x?.User?.Name
                //}).ToList()
            },
            };
        });
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
            Dispatcher.Dispatch(new EducatorClearStateAction());
        return base.DisposeAsyncCore(disposing);
    }
}
