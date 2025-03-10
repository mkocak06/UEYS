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
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.Services;
namespace UI.Pages.Archive.Students.StudentDetail;

public partial class ArchiveStudentDetail
{
    [Parameter] public long? Id { get; set; }
    [Inject] public IStudentService StudentService { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IState<ArchiveStudentDetailState> StudentDetailState { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private bool Loaded => StudentDetailState.Value.StudentLoaded;
    private bool _editing = false;
    private StudentResponseDTO SelectedStudent => StudentDetailState.Value.Student;

    private List<BreadCrumbLink> _links;
    private bool forceRender;

    protected override async Task OnInitializedAsync()
    {
        if (Id is not null)
        {
            _editing = true;
            StateHasChanged();
            await GetBreadCrumbLinksForEditing();
        }
        else
        {
            _editing = false;
            StateHasChanged();
            GetBreadCrumbLinksForAdding();
        }
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
            _editing = true;
            StateHasChanged();
            Dispatcher.Dispatch(new StudentDetailLoadAction((long)Id));
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
                To = "/archive/students",
                OrderIndex = 1,
                Title = L["Specialization Student Informations"]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 1,
                Title = L["add_new",L["Specialization Student"]]
            },
            };
    }
    private async Task GetBreadCrumbLinksForEditing()
    {
        List<BreadCrumbSearchResponseDTO> students = new();
        try
        {
            var response = await StudentService.GetListForBreadCrumb();
            if (response.Result)
                students = response.Item;
        }
        catch (System.Exception)
        {

        }
        SubscribeToAction<StudentDetailSetAction>((action) =>
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
                To = "/archive/students",
                OrderIndex = 1,
                Title = L["Specialization Student Informations"]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 2,
                Title = action.Student.User.Name,
                DropdownList = students.Select(x=> new DropdownElement()
                {
                    Link = "/archive/students/"+x.Id,
                    Name = x.Name
                }).ToList()
            },
            };
        });
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        return base.DisposeAsyncCore(disposing);
        if (disposing)
            Dispatcher.Dispatch(new StudentClearStateAction());
    }
}
