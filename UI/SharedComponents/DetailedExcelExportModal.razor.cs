using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Models.FilterModels;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.SharedComponents
{
    partial class DetailedExcelExportModal
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IHospitalService HospitalService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IProfessionService ProfessionService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] IState<AppState> AppState { get; set; }
        [Inject] IUniversityService UniversityService { get; set; }
        [Inject] IProvinceService ProvinceService { get; set; }


        private List<HospitalResponseDTO> _hospitals;
        private FilterDTO _filter;
        private PaginationModel<HospitalResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loadingFile;
        private MyModal _excelModal;
        private ProfessionResponseDTO _profession;
        private List<ExpertiseBranchResponseDTO> _expertiseBranches;
        private List<ExpertiseBranchResponseDTO> _filteredExpertiseBranches;
        private List<InstitutionResponseDTO> _filteredInstitutions;
        private List<UniversityResponseDTO> _filteredExcelUniversities;
        private HospitalResponseDTO _selectedHospital;
        private List<HospitalResponseDTO> _excelHospitals;
        private List<HospitalResponseDTO> _filteredExcelHospitals;
        private bool? _isPrincipal;
        private bool? _isPrivate;
        private ProgramFilter ExcelFilter => ProgramDetailState.Value.ProgramFilter;
        public FilterDTO Filter => AppState.Value.Filter;
        public Guid Guid = Guid.NewGuid();

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
                            Field = "IsDeleted",
                            Operator = "eq",
                            Value = false
                        }
                    },
                    Logic = "and"
                },
                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "Name"
                    }
                }
            };
            _links = new List<BreadCrumbLink>()
            {
                new BreadCrumbLink()
                {
                    IsActive = true,
                    To = "/",
                    OrderIndex = 0,
                    Title = L["Homepage"]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    To = "/institution-management/hospitals",
                    OrderIndex = 1,
                    Title = L["Places of Education"]
                }
            };
            await GetHospitals();

            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Sort("Name");
            var expResponse = await ExpertiseBranchService.GetPaginateList(filter);
            _expertiseBranches = expResponse?.Items;
            _filteredExpertiseBranches = _expertiseBranches;

            var uniResponse = await UniversityService.GetPaginateList(new FilterDTO()
            {
                page = 1,
                pageSize = int.MaxValue,
                Filter = new Shared.FilterModels.Base.Filter()
                {
                    Field = "IsDeleted",
                    Operator = "eq",
                    Value = false
                },
                Sort = new[]
                {
                new Sort()
                {
                    Dir = SortType.ASC,
                    Field = "Name"
                }
            }
            });

            var hospitalsResponse = await HospitalService.GetPaginateList(new FilterDTO()
            {
                page = 1,
                pageSize = int.MaxValue,
                Filter = new()
                {
                    Filters = new()
                {
                    new Filter()
                    {
                        Field = "IsDeleted",
                        Operator = "eq",
                        Value = false
                    }
                },
                    Logic = "and"
                },
                Sort = new[]
                {
                new Sort()
                {
                    Dir = SortType.ASC,
                    Field = "Name"
                }
            }
            });

            _excelHospitals = hospitalsResponse.Items;
            _filteredExcelHospitals = _excelHospitals;
            _filteredExcelUniversities = uniResponse.Items;
            _filteredInstitutions = new List<InstitutionResponseDTO>
            {
                new()
                {
                    Name = "KKTC Sağlık Bakanlığı",
                    Id = 3
                },
                new()
                {
                    Name = "T.C. Adalet Bakanlığı",
                    Id = 4
                },
                new()
                {
                    Name = "T.C. Sağlık Bakanlığı",
                    Id = 1
                },
                new()
                {
                    Name = "YÖK-Üni/Vakıf",
                    Id = 2
                },
                new()
                {
                    Name = "YÖK-Üni/Devlet",
                    Id = 2
                }
            };

            await base.OnInitializedAsync();
        }

        public void OpenModal()
        {
            if (_excelModal != null)
            {
                _excelModal.OpenModal();
            }
            else
            {
                Console.WriteLine("Modal referansı null!");
            }
        }
        public void CloseModal()
        {
            JsRuntime.InvokeVoidAsync("closeModal", Guid);
        }

        private async Task GetHospitals()
        {
            _paginationModel = await HospitalService.GetPaginateList(_filter);
            if (_paginationModel.Items != null)
            {
                _hospitals = _paginationModel.Items;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        private async Task DownloadDetailedExcelFile()
        {
            ExcelFilter.IsPrincipal = _isPrincipal;
            ExcelFilter.IsPrivate = _isPrivate;
            if (_loadingFile)
                return;

            _loadingFile = true;
            StateHasChanged();

            var response = await HospitalService.GetDetailedExcelByteArray(ExcelFilter);

            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"HospitalList_{DateTime.Now.ToString("yyyyMMdd")}.xlsx",
                    Convert.ToBase64String(response.Item));
                _loadingFile = false;
            }
            else
            {
                _loadingFile = false;
                SweetAlert.ErrorAlert();
            }

            StateHasChanged();
        }

        private async Task<IEnumerable<ProfessionResponseDTO>> SearchProfessions(string searchQuery)
        {
            var result = await ProfessionService.GetAll();
            return result.Result
                ? result.Item.OrderBy(x => x.Name).Where(x =>
                    x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)) &&
                    !ExcelFilter.ProfessionList.Any(y => y.Name == x.Name))
                : new List<ProfessionResponseDTO>();
        }

        private void ProfessionListChanged(IList<ProfessionResponseDTO> args)
        {
            ExcelFilter.ProfessionList = args;

            ChangeExpBranchList();
            ChangeHospitalList();
        }

        private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
        {
            return _filteredExpertiseBranches?.Where(x => !ExcelFilter.ExpertiseBranchList.Any(y => y.Name == x.Name) && x.Name
                       .ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) ??
                   new List<ExpertiseBranchResponseDTO>();
        }
        private void ExpertiseBranchListChanged(IList<ExpertiseBranchResponseDTO> args)
        {
            ExcelFilter.ExpertiseBranchList = args;
        }

        private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ProvinceService.GetPaginateList(filter);

            return result.Items?.Where(x => !ExcelFilter.ProvinceList.Any(y => y.Name == x.Name)) ?? new List<ProvinceResponseDTO>();
        }
        private void ProvinceListChanged(IList<ProvinceResponseDTO> args)
        {
            ExcelFilter.ProvinceList = args;
        }


        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitutions(string searchQuery)
        {
            return _filteredInstitutions
                .Where(x => !ExcelFilter.InstitutionList.Any(y => y.Name == x.Name) && x.Name
                    .ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)))
                .OrderBy(x => x.Name);
        }
        private void InstitutionListChanged(IList<InstitutionResponseDTO> args)
        {
            ExcelFilter.InstitutionList = args;

            bool yokUniVakıfSelected = ExcelFilter.InstitutionList.Any(i => i.Name == "YÖK-Üni/Vakıf");

            // YÖK-Üni Devlet seçilmiş mi?
            bool yokUniDevletSelected = ExcelFilter.InstitutionList.Any(i => i.Name == "YÖK-Üni/Devlet");

            if (yokUniVakıfSelected && !yokUniDevletSelected)
            {
                // Sadece YÖK-Üni/Vakıf seçildiyse
                _isPrivate = true;
            }
            else if (!yokUniVakıfSelected && yokUniDevletSelected)
            {
                // Sadece YÖK-Üni/Devlet seçildiyse
                _isPrivate = false;
            }
            else
            {
                // İkisi de seçilmişse veya ikisi de seçilmemişse
                _isPrivate = null;
            }
        }

        private async Task<IEnumerable<UniversityResponseDTO>> SearchUniversities(string searchQuery)
        {
            return _filteredExcelUniversities.Where(x => !ExcelFilter.UniversityList.Any(y => y.Name == x.Name) && x.Name
                    .ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)) &&
                    (ExcelFilter.InstitutionList?.Count < 1 || ExcelFilter.InstitutionList.Select(a => a.Id).Contains(x.InstitutionId.Value)) &&
                    (_isPrivate == null || (x.InstitutionId == 2 ? x.IsPrivate == _isPrivate : true)))
                    .OrderBy(x => x.Name);
        }
        private void UniversityListChanged(IList<UniversityResponseDTO> args)
        {
            ExcelFilter.UniversityList = args;
        }

        private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
        {
            return _filteredExcelHospitals
                .Where(x => !ExcelFilter.HospitalList.Any(y => y.Name == x.Name) && x.Name
                    .ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)) &&
                                                                (ExcelFilter.UniversityList?.Count < 1 || ExcelFilter.UniversityList.Select(a => a.Id).Contains(x.Faculty.UniversityId.Value)) &&
                                                                (ExcelFilter.InstitutionList?.Count < 1 || ExcelFilter.InstitutionList.Select(a => a.Id).Contains(x.Faculty.University.InstitutionId.Value)))
                .OrderBy(x => x.Name);
        }
        private void HospitalListChanged(IList<HospitalResponseDTO> args)
        {
            ExcelFilter.HospitalList = args;
        }

        private void TogglePrincipal()
        {
            if (_isPrincipal == true)
            {
                _isPrincipal = null;
            }
            else
            {
                _isPrincipal = true;
            }

            ChangeExpBranchList();
        }

        private void ToggleSecondary()
        {
            if (_isPrincipal == false)
            {
                _isPrincipal = null;
            }
            else
            {
                _isPrincipal = false;
            }

            ChangeExpBranchList();
        }

        private void ChangeExpBranchList()
        {
            _filteredExpertiseBranches =
                _expertiseBranches?.Where(x => ExcelFilter.ExpertiseBranchList.All(y => y.Name != x.Name) &&
                                               (!(ExcelFilter.ProfessionList?.Count > 0) || ExcelFilter.ProfessionList.Any(y => y.Id == x.ProfessionId)) &&
                                               (_isPrincipal == null || x.IsPrincipal == _isPrincipal)).ToList() ??
                new List<ExpertiseBranchResponseDTO>();
        }
        private void ChangeHospitalList()
        {
            _filteredExcelHospitals =
                _excelHospitals.Where(x => !(ExcelFilter.ProfessionList?.Count > 0) || ExcelFilter.ProfessionList.Any(y => y.Id == x.Faculty.ProfessionId)).ToList();
        }

        #region FilterChangeHandlers

        private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
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
                    Value = value
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = value;
            }

            await GetHospitals();
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
                await GetHospitals();
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
}
