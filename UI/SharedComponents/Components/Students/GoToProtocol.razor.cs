using Microsoft.AspNetCore.Components;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Services;
using Fluxor;
using Microsoft.JSInterop;
using UI.Models;
using UI.Pages.Student.Students.StudentDetail.Store;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using System.Linq;
using System.Globalization;
using UI.SharedComponents.Components;
using Shared.Types;
using Shared.FilterModels.Base;

namespace UI.SharedComponents.Components.Students
{
    partial class GoToProtocol
    {
        [Parameter] public long ProgramId { get; set; }
        [Parameter] public long HospitalId { get; set; }
        [Parameter] public long UniversityId { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<StudentDetailState> StudentDetailState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<StudentPaginateResponseDTO> _students;
        private FilterDTO _filter = new();
        private bool _loading;
        private PaginationModel<StudentPaginateResponseDTO> _paginationModel;

        protected override async Task OnInitializedAsync()
        {
            _filter.Sort = new[]
            {
                new Sort()
                {
                    Field = "Name",
                    Dir = Shared.Types.SortType.ASC
                },
            };
            _filter.Filter = new()
            {
                Logic = "and",
                Filters = new()
                {
                    new Filter()
                    {
                        Field = "ProtocolProgramId",
                        Operator = "neq",
                        Value = ProgramId
                    },
                    new Filter
                    {
                        Field = "OriginalProgramId",
                        Operator = "eq",
                        Value = ProgramId
                    },
                    new Filter()
                    {
                        Field = "IsDeleted",
                        Operator = "eq",
                        Value = false
                    },
                    new Filter()
                    {
                        Field = "UserIsDeleted",
                        Operator = "eq",
                        Value = false
                    },
                }
            };
            await base.OnInitializedAsync();
            await GetStudents();
        }

        private async Task GetStudents()
        {
            try
            {
                _loading = true;

                _paginationModel = await StudentService.GetPaginateListOnly(_filter);
                if (_paginationModel.Items != null)
                {
                    _students = _paginationModel.Items;
                }
                else
                {
                    _students = new();
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message);
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
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
                await JSRuntime.InvokeVoidAsync("clearInput", filterName);
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
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetStudents();
        }

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
    }
}
