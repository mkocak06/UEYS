using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using UI.Services;
using Fluxor;
using UI.SharedComponents.Store;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using Shared.FilterModels.Base;
using UI.Models.FilterModels;
using Shared.ResponseModels;
using System.Globalization;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using UI.Helper;
using UI.SharedComponents.Components;

namespace UI.SharedComponents.AdvancedFilters
{
    public partial class ProgramFilterForm
    {
        [Inject] IState<AppState> AppState { get; set; }
        [Inject] IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IInstitutionService InstitutionService { get; set; }
        [Inject] IUniversityService UniversityService { get; set; }
        [Inject] IFacultyService FacultyService { get; set; }
        [Inject] IProfessionService ProfessionService { get; set; }
        [Inject] IHospitalService HospitalService { get; set; }
        [Inject] IProvinceService ProvinceService { get; set; }
        [Inject] IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] IAuthorizationCategoryService AuthorizationCategory { get; set; }
        public FilterDTO Filter => AppState.Value.Filter;
        private bool _saving;
        private EditContext _EC;
        private ProgramFilter ProgramFilter => ProgramDetailState.Value.ProgramFilter;

        protected override void OnInitialized()
        {
            ProgramFilter.AuthorizationCategoryList.PrintJson("authcat");
            base.OnInitialized();
            _EC = new(ProgramFilter);
        }

        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitution(string searchQuery)
        {
            var result = await InstitutionService.GetPaginateList(new FilterDTO()
            {
                page = 1,
                pageSize = int.MaxValue,
                Filter = new Shared.FilterModels.Base.Filter()
                {
                    Field = "Name",
                    Logic = "and",
                    Operator = "contains",
                    Value = searchQuery
                }
            });
            return result.Items.OrderBy(x => x.Name).Where(x => !ProgramFilter.InstitutionList.Any(y => y.Name == x.Name));
        }

        private async Task<IEnumerable<UniversityResponseDTO>> SearchUniversities(string searchQuery)
        {
            var result = await UniversityService.GetPaginateList(new FilterDTO()
            {
                page = 1,
                pageSize = int.MaxValue,
                Filter = new Shared.FilterModels.Base.Filter()
                {
                    Field = "Name",
                    Logic = "and",
                    Operator = "contains",
                    Value = searchQuery
                }
            });
            return result.Items.OrderBy(x => x.Name).Where(x => !ProgramFilter.UniversityList.Any(y => y.Name == x.Name));
        }

        private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculty(string searchQuery)
        {
            var result = await FacultyService.GetPaginateList(new FilterDTO()
            {
                page = 1,
                pageSize = int.MaxValue,
                Filter = new Shared.FilterModels.Base.Filter()
                {
                    Field = "Name",
                    Logic = "and",
                    Operator = "contains",
                    Value = searchQuery
                }
            });
            return result.Items.OrderBy(x => x.Name).Where(x => !ProgramFilter.FacultyList.Any(y => y.Name == x.Name));
        }

        private async Task<IEnumerable<ProfessionResponseDTO>> SearchProfessions(string searchQuery)
        {
            var result = await ProfessionService.GetAll();
            return result.Result ? result.Item.OrderBy(x => x.Name).Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)) && !ProgramFilter.ProfessionList.Any(y => y.Name == x.Name)) :
                new List<ProfessionResponseDTO>();
        }

        private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
        {
            var result = await HospitalService.GetPaginateList(new FilterDTO()
            {
                page = 1,
                pageSize = int.MaxValue,
                Filter = new Shared.FilterModels.Base.Filter()
                {
                    Field = "Name",
                    Logic = "and",
                    Operator = "contains",
                    Value = searchQuery
                }
            });
            return result.Items.OrderBy(x => x.Name).Where(x => !ProgramFilter.HospitalList.Any(y => y.Name == x.Name));
        }

        private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ExpertiseBranchService.GetPaginateList(filter);

            return result.Items?.Where(x => !ProgramFilter.ExpertiseBranchList.Any(y => y.Name == x.Name)) ?? new List<ExpertiseBranchResponseDTO>();
        }

        private async Task<IEnumerable<AuthorizationCategoryResponseDTO>> SearchAuthorizationCategory(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await AuthorizationCategory.GetPaginateList(filter);

            return result.Items?.Where(x => !ProgramFilter.AuthorizationCategoryList.Any(y => y.Name == x.Name)) ?? new List<AuthorizationCategoryResponseDTO>();
        }

        private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ProvinceService.GetPaginateList(filter);

            return result.Items?.Where(x => !ProgramFilter.ProvinceList.Any(y => y.Name == x.Name)) ?? new List<ProvinceResponseDTO>();
        }

        private void ResetFilter()
        {
            ProgramFilter.UniversityList = new List<UniversityResponseDTO>();
            ProgramFilter.HospitalList = new List<HospitalResponseDTO>();
            ProgramFilter.ProfessionList = new List<ProfessionResponseDTO>();
            ProgramFilter.ExpertiseBranchList = new List<ExpertiseBranchResponseDTO>();
            ProgramFilter.ProvinceList = new List<ProvinceResponseDTO>();
            ProgramFilter.InstitutionList = new List<InstitutionResponseDTO>();
            ProgramFilter.FacultyList = new List<FacultyResponseDTO>();
            ProgramFilter.AuthorizationCategoryList = new List<AuthorizationCategoryResponseDTO>();
            ProgramFilter.StartDate = null;
            ProgramFilter.EndDate = null;
            StateHasChanged();

            SetFilter(ProgramFilter.UniversityList?.Select(x => x.Id)?.ToList(), "UniversityId");
            SetFilter(ProgramFilter.HospitalList?.Select(x => x.Id)?.ToList(), "HospitalId");
            SetFilter(ProgramFilter.ProfessionList?.Select(x => x.Id)?.ToList(), "ProfessionId");
            SetFilter(ProgramFilter.ExpertiseBranchList?.Select(x => (long)x.Id)?.ToList(), "ExpertiseBranchId");
            SetFilter(ProgramFilter.ProvinceList?.Select(x => x.Id)?.ToList(), "ProvinceId");
            SetFilter(ProgramFilter.InstitutionList?.Select(x => x.Id)?.ToList(), "InstitutionId");
            SetFilter(ProgramFilter.FacultyList?.Select(x => (long)x.Id)?.ToList(), "FacultyId");
            SetFilter(ProgramFilter.AuthorizationCategoryList?.Select(x => x.Id)?.ToList(), "AuthorizationCategoryId");

            ResetDateFilter("gte");
            ResetDateFilter("lte");

            JSRuntime.InvokeVoidAsync("HideAdvancedSearchAsync");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
        }

        private void SetFilter(List<long> idList, string fieldName)
        {
            Filter filter = Filter.Filter.Filters.FirstOrDefault(x => x.Field == fieldName);
            if (idList is not null && idList.Count() > 0)
            {
                if (filter is null)
                {
                    filter = new() { Filters = new(), Logic = "or", Field = fieldName };
                    Filter.Filter.Filters.Add(filter);
                }
                else
                {
                    filter.Filters = new();
                }
                foreach (var item in idList)
                {
                    filter.Filters.Add(new Shared.FilterModels.Base.Filter()
                    {
                        Field = fieldName,
                        Operator = "eq",
                        Value = item
                    });
                }
            }
            else
            {
                if (filter is not null)
                {
                    Filter.Filter.Filters.Remove(filter);
                }
            }
        }

        private void SetDateFilter(DateTime? date, string Operator)
        {
            Filter.Filter.Filters.Add(new Shared.FilterModels.Base.Filter()
            {
                Field = "AuthorizationEndDate",
                Operator = Operator,
                Value = date
            });

            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }
        private void ResetDateFilter(string Operator)
        {
            Filter filter = Filter.Filter.Filters.FirstOrDefault(x => x.Field == "AuthorizationEndDate" && x.Operator == Operator);

            filter.PrintJson("asdasda");
            if (filter != null)
            {
                Filter.Filter.Filters.Remove(filter);
            }

            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void InstitutionListChanged(IList<InstitutionResponseDTO> args)
        {
            ProgramFilter.InstitutionList = args;
            SetFilter(ProgramFilter.InstitutionList?.Select(x => x.Id)?.ToList(), "ParentInstitutionId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void UniversityListChanged(IList<UniversityResponseDTO> args)
        {
            ProgramFilter.UniversityList = args;
            SetFilter(ProgramFilter.UniversityList?.Select(x => x.Id)?.ToList(), "UniversityId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void FacultyListChanged(IList<FacultyResponseDTO> args)
        {
            ProgramFilter.FacultyList = args;
            SetFilter(ProgramFilter.FacultyList?.Select(x => (long)x.Id)?.ToList(), "FacultyId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void HospitalListChanged(IList<HospitalResponseDTO> args)
        {
            ProgramFilter.HospitalList = args;
            SetFilter(ProgramFilter.HospitalList?.Select(x => x.Id)?.ToList(), "HospitalId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }
        private void ProfessionListChanged(IList<ProfessionResponseDTO> args)
        {
            ProgramFilter.ProfessionList = args;
            SetFilter(ProgramFilter.ProfessionList?.Select(x => x.Id)?.ToList(), "ProfessionId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void ExpertiseBranchListChanged(IList<ExpertiseBranchResponseDTO> args)
        {
            ProgramFilter.ExpertiseBranchList = args;
            SetFilter(ProgramFilter.ExpertiseBranchList?.Select(x => (long)x.Id)?.ToList(), "ExpertiseBranchId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void AuthorizationCategoryListChanged(IList<AuthorizationCategoryResponseDTO> args)
        {
            ProgramFilter.AuthorizationCategoryList = args;
            SetFilter(ProgramFilter.AuthorizationCategoryList?.Select(x => x.Id)?.ToList(), "AuthorizationCategoryId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void ProvinceListChanged(IList<ProvinceResponseDTO> args)
        {
            ProgramFilter.ProvinceList = args;
            SetFilter(ProgramFilter.ProvinceList?.Select(x => x.Id)?.ToList(), "ProvinceId");
            Dispatcher.Dispatch(new ProgramsLoadAction(Filter));
            StateHasChanged();
        }

        private void StartDateChange(DateTime? dt)
        {
            ProgramFilter.StartDate = dt;
            if (ProgramFilter.StartDate == null)
            {
                ResetDateFilter("gte");
                return;
            }
            Console.WriteLine(ProgramFilter.StartDate);
            SetDateFilter(dt, "gte");
        }
        private void EndDateChange(DateTime? dt)
        {
            ProgramFilter.EndDate = dt;

            if (ProgramFilter.EndDate == null)
            {
                ResetDateFilter("lte");
                return;
            }
            SetDateFilter(dt, "lte");
        }
    }
}