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
    public partial class DashboardProgramFilters
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
        private FilterDTO DashboardFilter => AppState.Value.DashboardFilter;
        private bool _saving;
        private EditContext _EC;
        private DashboardProgramFilter DashboardProgramFilter => ProgramDetailState.Value.DashboardProgramFilter;
        Dictionary<string, bool?> _universityTypeDict = new Dictionary<string, bool?>()
           {
               {"Public University", true},
               {"Foundation University", false},
               {"Other", null}
           };
        Dictionary<string, bool?> _branchTypeDict = new Dictionary<string, bool?>()
        {
            {"Principal Branch", true},
            {"Sub Branch", false},
        };
        private IList<string> _universityTypeList = new List<string>();
        private IList<string> _branchTypeList = new List<string>();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            _EC = new(DashboardProgramFilter);
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
            return result.Items.OrderBy(x => x.Name).Where(x => !DashboardProgramFilter.InstitutionList.Any(y => y.Name == x.Name));
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
            return result.Items.OrderBy(x => x.Name).Where(x => !DashboardProgramFilter.UniversityList.Any(y => y.Name == x.Name));
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
            return result.Items.OrderBy(x => x.Name).Where(x => !DashboardProgramFilter.FacultyList.Any(y => y.Name == x.Name));
        }

        private async Task<IEnumerable<ProfessionResponseDTO>> SearchProfessions(string searchQuery)
        {
            var result = await ProfessionService.GetAll();
            return result.Result ? result.Item.OrderBy(x => x.Name).Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)) && !DashboardProgramFilter.ProfessionList.Any(y => y.Name == x.Name)) :
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
            return result.Items.OrderBy(x => x.Name).Where(x => !DashboardProgramFilter.HospitalList.Any(y => y.Name == x.Name));
        }

        private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ExpertiseBranchService.GetPaginateList(filter);

            return result.Items?.Where(x => !DashboardProgramFilter.ExpertiseBranchList.Any(y => y.Name == x.Name)) ?? new List<ExpertiseBranchResponseDTO>();
        }

        private async Task<IEnumerable<AuthorizationCategoryResponseDTO>> SearchAuthorizationCategory(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Filter("IsActive", "eq", true, "and");
            filter.Sort("Name");

            var result = await AuthorizationCategory.GetPaginateList(filter);

            return result.Items?.Where(x => !DashboardProgramFilter.AuthorizationCategoryList.Any(y => y.Name == x.Name)) ?? new List<AuthorizationCategoryResponseDTO>();
        }
        private async Task<IEnumerable<string>> SearchUniversityType(string searchQuery)
        {
            return _universityTypeDict.Keys;
        }
        private async Task<IEnumerable<string>> SearchBranchType(string searchQuery)
        {
            return _branchTypeDict.Keys;
        }
        private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ProvinceService.GetPaginateList(filter);

            return result.Items?.Where(x => !DashboardProgramFilter.ProvinceList.Any(y => y.Name == x.Name)) ?? new List<ProvinceResponseDTO>();
        }

        private void ResetFilter()
        {
            DashboardProgramFilter.UniversityList = new List<UniversityResponseDTO>();
            DashboardProgramFilter.HospitalList = new List<HospitalResponseDTO>();
            DashboardProgramFilter.ProfessionList = new List<ProfessionResponseDTO>();
            DashboardProgramFilter.ExpertiseBranchList = new List<ExpertiseBranchResponseDTO>();
            DashboardProgramFilter.ProvinceList = new List<ProvinceResponseDTO>();
            DashboardProgramFilter.InstitutionList = new List<InstitutionResponseDTO>();
            DashboardProgramFilter.FacultyList = new List<FacultyResponseDTO>();
            DashboardProgramFilter.AuthorizationCategoryList = new List<AuthorizationCategoryResponseDTO>();
            DashboardProgramFilter.UniversityTypeList = new List<bool?>();
            DashboardProgramFilter.BranchTypeList = new List<bool?>();
            _universityTypeList = new List<string>();
            _branchTypeList = new List<string>();
            StateHasChanged();

            //SetFilter(DashboardProgramFilter.UniversityList?.Select(x => x.Id)?.ToList(), "UniversityId");
            //SetFilter(DashboardProgramFilter.HospitalList?.Select(x => x.Id)?.ToList(), "HospitalId");
            //SetFilter(DashboardProgramFilter.ProfessionList?.Select(x => x.Id)?.ToList(), "ProfessionId");
            //SetFilter(DashboardProgramFilter.ExpertiseBranchList?.Select(x => (long)x.Id)?.ToList(), "ExpertiseBranchId");
            //SetFilter(DashboardProgramFilter.ProvinceList?.Select(x => x.Id)?.ToList(), "ProvinceId");
            //SetFilter(DashboardProgramFilter.InstitutionList?.Select(x => x.Id)?.ToList(), "InstitutionId");
            //SetFilter(DashboardProgramFilter.FacultyList?.Select(x => (long)x.Id)?.ToList(), "FacultyId");
            //SetFilter(DashboardProgramFilter.AuthorizationCategoryList?.Select(x => x.Id)?.ToList(), "AuthorizationCategoryId");
            //SetUniTypeFilter(DashboardProgramFilter.UniversityTypeList?.ToList(), "IsPrivate");

            Dispatcher.Dispatch(new AppDashboardFilterClearAction());
            //JSRuntime.InvokeVoidAsync("HideAdvancedSearchAsync");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
        }

        private void SetFilter(List<long> idList, string fieldName)
        {
            DashboardFilter.Filter ??= new Filter();
            DashboardFilter.Filter.Filters ??= new List<Filter>();
            Filter filter = DashboardFilter.Filter.Filters.FirstOrDefault(x => x.Field == fieldName);
            if (idList is not null && idList.Count() > 0)
            {
                if (filter is null)
                {
                    filter = new() { Filters = new(), Logic = "or", Field = fieldName };
                    DashboardFilter.Filter.Filters.Add(filter);
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
                    DashboardFilter.Filter.Filters.Remove(filter);
                }
            }
        }
        private void SetUniTypeFilter(List<bool?> uniTypeList, string fieldName)
        {
            DashboardFilter.Filter ??= new Filter();
            DashboardFilter.Filter.Filters ??= new List<Filter>();
            Filter filter = DashboardFilter.Filter.Filters.FirstOrDefault(x => x.Field == fieldName);
            if (uniTypeList is not null && uniTypeList.Count() > 0)
            {
                if (filter is null)
                {
                    filter = new() { Filters = new(), Logic = "or", Field = fieldName };
                    DashboardFilter.Filter.Filters.Add(filter);
                }
                else
                {
                    filter.Filters = new();
                }
                foreach (var item in uniTypeList)
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
                    DashboardFilter.Filter.Filters.Remove(filter);
                }
            }
        }

        private void InstitutionListChanged(IList<InstitutionResponseDTO> args)
        {
            DashboardProgramFilter.InstitutionList = args;
            SetFilter(DashboardProgramFilter.InstitutionList?.Select(x => x.Id)?.ToList(), "ParentInstitutionId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }

        private void UniversityListChanged(IList<UniversityResponseDTO> args)
        {
            DashboardProgramFilter.UniversityList = args;
            SetFilter(DashboardProgramFilter.UniversityList?.Select(x => x.Id)?.ToList(), "UniversityId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }

        private void FacultyListChanged(IList<FacultyResponseDTO> args)
        {
            DashboardProgramFilter.FacultyList = args;
            SetFilter(DashboardProgramFilter.FacultyList?.Select(x => (long)x.Id)?.ToList(), "FacultyId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }

        private void HospitalListChanged(IList<HospitalResponseDTO> args)
        {
            DashboardProgramFilter.HospitalList = args;
            SetFilter(DashboardProgramFilter.HospitalList?.Select(x => x.Id)?.ToList(), "HospitalId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }
        private void ProfessionListChanged(IList<ProfessionResponseDTO> args)
        {
            DashboardProgramFilter.ProfessionList = args;
            SetFilter(DashboardProgramFilter.ProfessionList?.Select(x => x.Id)?.ToList(), "ProfessionId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }

        private void ExpertiseBranchListChanged(IList<ExpertiseBranchResponseDTO> args)
        {
            DashboardProgramFilter.ExpertiseBranchList = args;
            SetFilter(DashboardProgramFilter.ExpertiseBranchList?.Select(x => (long)x.Id)?.ToList(), "ExpertiseBranchId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }

        private void AuthorizationCategoryListChanged(IList<AuthorizationCategoryResponseDTO> args)
        {
            DashboardProgramFilter.AuthorizationCategoryList = args;
            SetFilter(DashboardProgramFilter.AuthorizationCategoryList?.Select(x => x.Id)?.ToList(), "AuthorizationCategoryId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }
        private void UniversityTypeListChanged(IList<string> args)
        {
            _universityTypeList = args;
            foreach (var item in _universityTypeList)
            {
                DashboardProgramFilter.UniversityTypeList.Add(_universityTypeDict[item]);
            }
            SetUniTypeFilter(DashboardProgramFilter.UniversityTypeList?.ToList(), "IsUniversityPrivate");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }
        private void BranchTypeListChanged(IList<string> args)
        {
            _branchTypeList = args;
            foreach (var item in _branchTypeList)
            {
                DashboardProgramFilter.BranchTypeList.Add(_branchTypeDict[item]);
            }
            SetUniTypeFilter(DashboardProgramFilter.BranchTypeList?.ToList(), "IsPrincipal");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }

        private void ProvinceListChanged(IList<ProvinceResponseDTO> args)
        {
            DashboardProgramFilter.ProvinceList = args;
            SetFilter(DashboardProgramFilter.ProvinceList?.Select(x => x.Id)?.ToList(), "ProvinceId");
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }
        private string GetFilterCountString()
        {
            var count = DashboardFilter.Filter?.Filters?.Count(x => !x.Field.Contains("IsDeleted"));
            if (count is null || count <= 0)
                return "";
            else
                return "<span class=\"badge badge-warning mr-1\">" + count + "</span>";
        }
    }
}