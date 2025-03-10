using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail.Tabs
{
    public partial class UniversityHospitals
    {
        [Inject] public IHospitalService HospitalService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IState<UniversityDetailState> UniversityDetailState { get; set; }
        [Inject] public IState<AppState> AppState { get; set; }
        [Inject] public IProvinceService ProvinceService { get; set; }
        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private List<HospitalResponseDTO> _hospitals;
        private HospitalResponseDTO _hospital;
        private FilterDTO _filter;
        private PaginationModel<HospitalResponseDTO> _paginationModel;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private EditContext _ec;
        private MyModal _addHospitalToUniversityModal;
        private UniversityResponseDTO University => UniversityDetailState.Value.University;
        private List<HospitalResponseDTO> Hospitals => UniversityDetailState.Value.Hospitals;
        private bool HospitalsLoaded => UniversityDetailState.Value.HospitalsLoaded;
        private bool HospitalsLoading => UniversityDetailState.Value.HospitalsLoading;
        private FacultyResponseDTO _faculty;
        private InputText _focusTarget;
        private InputMask _inputMask;
        private SingleMapView _singleMapView;

        private bool _mapLoading = false;
        protected override async Task OnInitializedAsync()
        {
            _hospital = new();
            _ec = new EditContext(_hospital);
            //Dispatcher.Dispatch(new HospitalsLoadAction(University.Id));

            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                {

                    new Filter()
                    {
                        Field="Faculty.UniversityId",
                        Operator="eq",
                        Value=University.Id
                    }

                },
                    Logic = "and"

                },
                page = 1,
                pageSize = 20,

                Sort = new[]
                {
                    new Sort()
                    {
                        Dir = SortType.ASC,
                        Field = "Name"
                    }
                }
            };
            await GetHospitals();
           base.OnInitialized();
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
        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetHospitals();
        }

        private async Task OnOpenHospitalToUniversity()
        {
            _mapLoading = true;
            StateHasChanged();

            _hospital = new();
            _ec = new EditContext(_hospital);
            _addHospitalToUniversityModal.OpenModal();
            await Task.Delay(300);

            _mapLoading = false;
            StateHasChanged();
            await Task.Delay(100);

            await _singleMapView.SetMapOnOpenModal(_addHospitalToUniversityModal.Guid);
        }
        private async Task Save()
        {
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();
            var dto = Mapper.Map<HospitalDTO>(_hospital);
            try
            {
                var response = await HospitalService.Add(dto);
                if (response.Result)
                {
                    await GetHospitals();
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
            _addHospitalToUniversityModal.CloseModal();
            _saving = false;
            StateHasChanged();
        }

        private void OnChangeLongitude(float? lng)
        {

            _hospital.Longitude = lng;


            if (_hospital.Latitude != null && _hospital.Longitude != null)
            {
                _singleMapView.UpdateMap(_hospital.Latitude ?? 0, _hospital.Longitude ?? 0);
            }
        }
        private void OnChangeLatitude(float? lat)
        {


            _hospital.Latitude = lat;

            if (_hospital.Latitude != null && _hospital.Longitude != null)
            {
                _singleMapView.UpdateMap(_hospital.Latitude ?? 0, _hospital.Longitude ?? 0);
            }
        }
        private void PhoneNoChanged(string tel)
        {
            _hospital.Phone = tel;
        }
        private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculties(string searchQuery)
        {
            var result = await FacultyService.GetListByUniversityId(University.Id);

            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).StartsWith(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<FacultyResponseDTO>();
        }
        private void OnChangeProvince(ProvinceResponseDTO province)
        {
            _hospital.Province = province;
            _hospital.ProvinceId = province?.Id;
            _hospital.Latitude = province?.Latitude;
            _hospital.Longitude = province?.Longitude;
            StateHasChanged();

            _singleMapView.UpdateMap(_hospital.Latitude ?? 0, _hospital.Longitude ?? 0);
        }
        private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
        {
            return await Task.FromResult(AppState.Value.ProvincesLoaded ? AppState.Value.Provinces.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) : new List<ProvinceResponseDTO>());
        }

        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitutions(string searchQuery)
        {
            var result = await InstitutionService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<InstitutionResponseDTO>();
        }

        private void OnChangeInstitution(InstitutionResponseDTO institution)
        {
            _hospital.Institution = institution;
            _hospital.InstitutionId = institution?.Id;
        }

        private void OnChangeLocation(float lat, float lng)
        {
            _hospital.Latitude = lat;
            _hospital.Longitude = lng;
            StateHasChanged();
        }
        private void OnChangeFaculty(FacultyResponseDTO faculty)
        {
            _hospital.Faculty = faculty;
            _hospital.FacultyId = faculty.Id??0;
        }
        private void OnChangeHospital(HospitalResponseDTO hospital)
        {
            _hospital = hospital;
            _hospital.Id = hospital?.Id ?? 0;
            _hospital.Faculty = _faculty;
            _hospital.FacultyId = _faculty.Id;
        }
      
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetHospitals();
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
            await GetHospitals();
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