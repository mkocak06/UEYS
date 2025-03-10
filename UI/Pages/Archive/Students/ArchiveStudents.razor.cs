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

namespace UI.Pages.Archive.Students;

public partial class ArchiveStudents
{
    [Inject] private IStudentService StudentService { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    private List<StudentResponseDTO> _students;
    private FilterDTO _filter;
    private PaginationModel<StudentResponseDTO> _paginationModel;
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
                        Field="IsHardDeleted",
                        Operator="eq",
                        Value=false
                    },
                    new Filter()
                    {
                        Field="Status",
                        Operator="neq",
                        Value=8
                    }
                },
                Logic = "and"
            },
            Sort = new[]{new Sort()
                {
                    Field = "User.Name",
                    Dir = SortType.ASC
                }}
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
        _paginationModel = await StudentService.GetArchiveList(_filter);
        if (_paginationModel.Items != null)
        {
            _students = _paginationModel.Items;
            StateHasChanged();
        }
        else
        {
            _loading = true;
            SweetAlert.ErrorAlert();
        }
    }

    private async Task OnUndeleteHandler(StudentResponseDTO student)
    {
        var confirm = await SweetAlert.ConfirmAlert("Emin Misiniz?",
            "Bu öğeyi geri almak istediğinize emin misiniz?",
            SweetAlertIcon.question, true, "Geri Al", "İptal");
        if (confirm)
        {
            try
            {
                Console.WriteLine(student.Id);
                var response = await StudentService.UnDelete(student.Id);

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
                    NavigationManager.NavigateTo($"./archive/students");
                    _students.Remove(student);
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

    private async Task OnCompletelyDelete(StudentResponseDTO student)
    {
        var confirm = await SweetAlert.ConfirmAlert("Emin Misiniz?",
            "Bu öğeyi tamamen silmek istediğinize emin misiniz?",
            SweetAlertIcon.question, true, "Tamamen Sil", "İptal");
        if (confirm)
        {
            try
            {
                Console.WriteLine(student.Id);
                var response = await StudentService.CompletelyDelete(student.Id);

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted Succesfully!"]}");
                    NavigationManager.NavigateTo($"./archive/students");
                    _students.Remove(student);
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L["Item can Deleted"]);
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
        await GetStudents();
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