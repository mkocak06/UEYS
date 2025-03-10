using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.Home
{

    public partial class Home
    {
        [Inject] IProgramService ProgramService { get; set; }
        [Inject] IHospitalService HospitalService { get; set; }
        [Inject] IUniversityService UniversityService { get; set; }
        [Inject] IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IState<AppState> AppState { get; set; }

        private ObservableCollection<MapLayerModel> MapLayers { get; set; }

        private MultiMapView _bigMap;
        private bool forceRender = false;
        private bool _fetching = false;
        private bool _fetching1 = false;
        private bool _fetching3 = false;
        private bool _fetching2 = false;
        private bool _fetchingx = false;
        private bool _fetching4 = false;
        private int _selectedMapReport = 0;
        private bool _isResult = false;

        private ExpertiseBranchResponseDTO _expertiseBranch;
        private AuthorizationCategoryResponseDTO _authorizationCategory;
        private int responseCount = 0;

        private bool FullScreenButton = false;

        private List<ReportResponseDTO> _reports;
        private UserHospitalDetailDTO _profile;
        private bool _profileLoading = true;
        private DetailedExcelExportModal _excelModal;

        protected override async Task OnInitializedAsync()
        {
            //try
            //{
            //	var response = await HospitalService.GetUserHospitalDetail();
            //	if (response.Result)
            //	{
            //		_profile = response.Item;
            //                 _profileLoading = false;
            //                 StateHasChanged();
            //	}
            //}
            //catch (Exception)
            //{

            //	throw;
            //}

            //MapLayers = new ObservableCollection<MapLayerModel>();
            //         await GetReport(0);
            forceRender = true;
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (forceRender)
            {
                forceRender = false;
            }
            base.OnAfterRender(firstRender);
        }

        private async Task<bool> AllHospitals()
        {
            _fetching1 = true;
            StateHasChanged();
            MapLayerModel model = new MapLayerModel()
            {
                MarkerType = Shared.Types.MarkerType.Blue2x,
                MarkerList = new List<MarkerModel>()
            };
            try
            {
                var response = await HospitalService.GetPaginateList(new Shared.FilterModels.Base.FilterDTO()
                {
                    page = 1,
                    pageSize = int.MaxValue,
                });
                if (response.Items is not null)
                {
                    foreach (var item in response.Items)
                    {
                        model.MarkerList.Add(new MarkerModel()
                        {
                            Content = $"<a class=\"btn btn-primary\" href=\"institution-management/hospitals/{item?.Id}\">{item?.Name}</a>",
                            LatLong = (item.Latitude is not null && item.Longitude is not null) ? new Tuple<float, float>(item.Latitude.GetValueOrDefault(), item.Longitude.GetValueOrDefault()) : new Tuple<float, float>(item.Province?.Latitude ?? 0, item.Province?.Longitude ?? 0)
                        });
                    }

                }
                _fetching3 = false;
                FullScreenButton = true;
                StateHasChanged();
                MapLayers.Add(model);
            }
            catch (Exception)
            {
                SweetAlert.ErrorAlert();
                _fetching3 = false;
                _selectedMapReport = 0;
                StateHasChanged();
            }
            return true;
        }

        private async Task<bool> AllUniversities()
        {
            _fetching3 = true;
            StateHasChanged();

            MapLayerModel model = new MapLayerModel()
            {
                MarkerType = Shared.Types.MarkerType.Red2x,
                MarkerList = new List<MarkerModel>()
            };
            try
            {
                var response = await UniversityService.GetPaginateList(new Shared.FilterModels.Base.FilterDTO()
                {
                    page = 1,
                    pageSize = int.MaxValue,
                });
                if (response.Items is not null)
                {
                    foreach (var item in response.Items)
                    {
                        model.MarkerList.Add(new MarkerModel()
                        {
                            Content = $"<a class=\"btn btn-primary\" href=\"institution-management/universities/{item?.Id}\">{item?.Name}</a>",
                            LatLong = (item.Latitude is not null && item.Longitude is not null) ? new Tuple<float, float>(item.Latitude.GetValueOrDefault(), item.Longitude.GetValueOrDefault()) : new Tuple<float, float>(item.Province?.Latitude ?? 0, item.Province?.Longitude ?? 0)
                        });
                    }
                    _fetching3 = false;
                    FullScreenButton = true;
                    StateHasChanged();
                }
                MapLayers.Add(model);
            }
            catch (Exception)
            {
                SweetAlert.ErrorAlert();
                _fetching3 = false;
                _selectedMapReport = 0;

                StateHasChanged();
            }
            return true;
        }
        private void OpenCanvas()
        {
            JsRuntime.InvokeVoidAsync("initQuickPanel");
            JsRuntime.InvokeVoidAsync("ShowAdvancedSearchAsync");
        }
        private string GetFilterCountString()
        {
            var count = AppState.Value.Filter?.Filter?.Filters?.Count(x => x.Field.Contains("Id"));
            if (count is null || count <= 0)
                return "";
            else
                return "<span class=\"badge badge-warning mr-1\">" + count + "</span>";
        }
        private async Task<bool> HospitalsByExpertiseId()
        {
            _fetching2 = true;
            _fetching = true;
            _isResult = false;
            StateHasChanged();

            MapLayerModel model = new MapLayerModel()
            {
                MarkerType = GetColor(_authorizationCategory?.Name),
                MarkerList = new List<MarkerModel>()
            };
            try
            {
                ResponseWrapper<List<ProgramsLocationResponseDTO>> response;
                response = await ProgramService.GetLocationsByExpertiseBranchId(_expertiseBranch?.Id, _authorizationCategory?.Id);
                if (response.Result)
                {
                    responseCount = response.Item.Count;
                    foreach (var item in response.Item)
                    {
                        model.MarkerList.Add(new MarkerModel()
                        {
                            Content = $"<a class=\"btn btn-primary\" href=\"institution-management/programs/{item.Id}\">{item.Hospital?.Name} <span class=\"badge badge-sm badge-secondary py-1\">{_expertiseBranch?.Name}</span></a>",
                            LatLong = (item.Hospital?.Latitude is not null && item.Hospital?.Longitude is not null) ? new Tuple<float, float>(item.Hospital.Latitude.GetValueOrDefault(), item.Hospital.Longitude.GetValueOrDefault()) : new Tuple<float, float>(item.Hospital.Province?.Latitude ?? 0, item.Hospital.Province?.Longitude ?? 0)
                        });
                    }
                    FullScreenButton = true;
                    _fetching2 = false;
                    _fetching = false;
                    _isResult = true;
                    MapLayers.Add(model);
                    StateHasChanged();
                }
            }
            catch (Exception)
            {
                SweetAlert.ErrorAlert();
                _fetching2 = false;
                _selectedMapReport = 0;

                StateHasChanged();
                return false;
            }
            return true;
        }
        private bool IsSelectedMapReport(int x)
        {
            return x == _selectedMapReport;
        }
        private MarkerType GetColor(String name)
        {
            switch (name)
            {
                case "0":
                    return MarkerType.Red;
                    break;
                case "1":
                    return MarkerType.Yellow;
                    break;
                case "2":
                    return MarkerType.Green;
                    break;
                case "3":
                    return MarkerType.DarkGreen;
                    break;
                case "9":
                    return MarkerType.LightBlue;
                    break;
                default:
                    return MarkerType.Violet;
                    break;
            }
        }

        private async Task GetReport(int x)
        {
            ChangeTab(x);
            _fetching = true;
            StateHasChanged();
            if (x == 0)
            {
                FullScreenButton = false;
            }
            else if (x == 1)
            {
                _fetching1 = true;
                StateHasChanged();
                if (await AllHospitals())
                {
                    FullScreenButton = true;
                }
                else
                {
                    _selectedMapReport = 0;
                }
                _fetching1 = false;

            }
            else if (x == 3)
            {
                await AllUniversities();
            }

            _fetching = false;
            StateHasChanged();
        }


        private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue);
            filter.Filter("Name", "contains", searchQuery, "and");
            filter.Sort("Name");

            var result = await ExpertiseBranchService.GetPaginateList(filter);
            return result.Items ?? new List<ExpertiseBranchResponseDTO>();
        }

        private async Task OnChangeExpertiseBranch(ExpertiseBranchResponseDTO expertiseBranch)
        {
            responseCount = 0;
            _expertiseBranch = expertiseBranch;
            if (expertiseBranch is null)
            {
                _authorizationCategory = null;
            }
            MapLayers.Clear();
            _isResult = false;
            StateHasChanged();
        }

        private async Task<IEnumerable<AuthorizationCategoryResponseDTO>> SearchAuthorizationCategory(string searchQuery)
        {
            var result = await AuthorizationCategoryService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).OrderBy(x => x.Name) :
                new List<AuthorizationCategoryResponseDTO>();
        }

        private async Task OnChangeAuthorizationCategory(AuthorizationCategoryResponseDTO authorizationCategory)
        {
            _authorizationCategory = authorizationCategory;
            MapLayers.Clear();
            _isResult = false;
            StateHasChanged();
        }

        private async Task ClearMap()
        {
            MapLayers.Clear();
        }
      
        protected override ValueTask DisposeAsyncCore(bool disposing)
        {
            return base.DisposeAsyncCore(disposing);
            _bigMap?.DisposeAsync();
        }

        private void ChangeTab(int tab)
        {
            if (_selectedMapReport != tab)
                MapLayers = new();
            _selectedMapReport = tab;
            if (tab == 2)
            {
                _authorizationCategory = null;
                _expertiseBranch = null;
                FullScreenButton = true;
            }
            StateHasChanged();
        }
    }
}