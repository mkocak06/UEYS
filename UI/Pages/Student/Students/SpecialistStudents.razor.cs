using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Pages.InstitutionManagement.ProtocolPrograms;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;


namespace UI.Pages.Student.Students;

public partial class SpecialistStudents
{
    [Inject] private IStudentService StudentService { get; set; }
    [Inject] private IState<AppState> AppState { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    private List<StudentPaginateResponseDTO> _students;
    private StudentResponseDTO _student;
    private FilterDTO _filter;
    private PaginationModel<StudentPaginateResponseDTO> _paginationModel;
    private bool _loading = true;
    private bool forceRender;
    private List<BreadCrumbLink> _links;
    private bool _saving;
    private bool _loadingFile;
    private StudentStatus? _studentStatus = null;
    protected override async Task OnInitializedAsync()
    {
        _student = new StudentResponseDTO();

        _filter = new FilterDTO()
        {
            Filter = new()
            {
                Filters = new()
                {
                    new Filter()
                    {
                        Field="Status",
                        Operator="eq",
                        Value=8
                    },
                    //new Filter()
                    //{
                    //    Field="ConditionallyGraduated",
                    //    Operator="neq",
                    //    Value=true
                    //}
                },
                Logic = "and"
            },
            Sort = new[]{new Sort()
                {
                    Field = "Name",
                    Dir = SortType.ASC
                }}
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
                    To = "/student/specialistStudents",
                    OrderIndex = 1,
                    Title = L["Students Who Completed Specialization Education"]
                }
        };
        await GetStudents();

        await base.OnInitializedAsync();
    }
    private async Task OnSortChange(Sort sort)
    {
        _filter.Sort = new[] { sort };
        await GetStudents();
    }

    private async Task GetStudents()
    {
        _loading = true;
        StateHasChanged();
        try
        {
            _paginationModel = await StudentService.GetPaginateListOnly(_filter);
            if (_paginationModel.Items != null)
            {
                _students = _paginationModel.Items.OrderBy(x => x.HospitalName).ToList();
                _loading = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();

    }

    private async Task DownloadExcelFile()
    {
        if (_loadingFile)
        {
            return;
        }
        _loadingFile = true;
        StateHasChanged();
        if (_filter?.Filter?.Filters?.Count == 0)
        {
            _filter.Filter.Filters = null;
            _filter.Filter.Logic = null;
        }

        var response = await StudentService.GetExcelByteArray(_filter);

        if (response.Result)
        {
            await JsRuntime.InvokeVoidAsync("saveAsFile", $"StudentList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", Convert.ToBase64String(response.Item));
            _loadingFile = false;
        }
        else
        {
            SweetAlert.ErrorAlert();
        }
        StateHasChanged();
    }

    private async Task PaginationHandler(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filter.page = item1;
        _filter.pageSize = item2;

        await GetStudents();
    }
    private void OnDetailHandler(StudentResponseDTO student)
    {
        NavigationManager.NavigateTo($"/student/students/{student.Id}");
    }

    #region FilterChangeHandlers

    private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
    {
        var value = (string)args.Value;
        if (filterName == "Status") _studentStatus = (StudentStatus)Enum.Parse(typeof(StudentStatus), value);
        _filter.Filter ??= new Filter();
        _filter.Filter.Filters ??= new List<Filter>();
        _filter.Filter.Logic ??= "and";
        _filter.page = 1;
        var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
        if (index < 0)
        {
            _filter.Filter.Filters.Add(new Filter()
            {
                Field = filterName,
                Operator = "contains",
                Value = value?.Trim()
            });
        }
        else
        {
            _filter.Filter.Filters[index].Value = value;
        }
        await GetStudents();
    }

    private async Task OnResetFilter(string filterName)
    {
        _filter.Filter ??= new Filter();
        _filter.Filter.Filters ??= new List<Filter>();
        _filter.Filter.Logic ??= "and";
        _filter.page = 1;
        var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
        if (index >= 0)
        {
            _filter.Filter.Filters.RemoveAt(index);
            await JsRuntime.InvokeVoidAsync("clearInput", filterName);
            if (filterName == "Status") _studentStatus = null;
            await GetStudents();
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