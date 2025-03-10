using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Pages.Hospitals.HospitalDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.InstitutionManagement.Hospitals.HospitalDetail.Tabs
{
    public partial class HospitalOverview
    {
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] IState<HospitalDetailState> HospitalDetailState { get; set; }
        [Inject] IState<AppState> AppState { get; set; }
        [Inject] IInstitutionService InstitutionService { get; set; }
        [Inject] IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IMapper Mapper { get; set; }

        private HospitalResponseDTO _hospital => HospitalDetailState.Value.Hospital;
        private List<ProvinceResponseDTO> Provinces => AppState.Value.Provinces;
        private bool _saving => HospitalDetailState.Value.UpdateHospitalDetailLoading;

        EditContext _ec;
        SingleMapView _map;
        private UniversityResponseDTO _university;
        private FacultyResponseDTO _faculty;

        protected override Task OnInitializedAsync()
        {
            if (!AppState.Value.ProvincesLoaded)
            {
                Dispatcher.Dispatch(new AppLoadProvincesAction());
            }
            _ec = new(_hospital);
            _university = _hospital?.Faculty?.University;
            _faculty = _hospital?.Faculty;

            return base.OnInitializedAsync();
        }
        private void OnChangeProvince(ProvinceResponseDTO province)
        {

            _hospital.Province = null;
            StateHasChanged();
            _hospital.Province = province;

            _hospital.ProvinceId = province?.Id;

            _hospital.Latitude = province?.Latitude;
            _hospital.Longitude = province?.Longitude;
            StateHasChanged();
        }
        private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
        {
            return await Task.FromResult(Provinces.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))));
        }

        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitutions(string searchQuery)
        {
            var result = await InstitutionService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<InstitutionResponseDTO>();
        }

        private void OnChangeInstitution(InstitutionResponseDTO institution)
        {
            _hospital.Institution = institution;
            _hospital.InstitutionId = institution?.Id;
        }

        private async Task<IEnumerable<UserAccountDetailInfoResponseDTO>> SearchUsers(string searchQuery)
        {
            var result = await AuthenticationService.GetUserAccountList();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(100) : new List<UserAccountDetailInfoResponseDTO>();
        }
        private void OnChangeManager(UserAccountDetailInfoResponseDTO manager)
        {
            _hospital.Manager = manager;
            _hospital.ManagerId = manager?.Id;
        }

        private void LocationChanged(float lat, float lng)
        {
            _hospital.Latitude = lat;
            _hospital.Longitude = lng;
            StateHasChanged();
        }
        private void OnChangeLatitude(float? lat)
        {
            _hospital.Latitude = lat;
            if (_map != null)
            {
                _map.UpdateMap(_hospital.Latitude ?? 0, _hospital.Longitude ?? 0);
            }
        }
        private void OnChangeLongitude(float? lng)
        {
            _hospital.Longitude = lng;
            if (_map != null)
            {
                _map.UpdateMap(_hospital.Latitude ?? 0, _hospital.Longitude ?? 0);
            }
        }
        private async Task Save()
        {
            if (!_ec.Validate()) return;

            var dto = Mapper.Map<HospitalDTO>(_hospital);
            Dispatcher.Dispatch(new HospitalDetailUpdateAction(_hospital.Id, dto));
        }

        private async Task<IEnumerable<UniversityResponseDTO>> SearchUniversities(string searchQuery)
        {
            var result = await UniversityService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<UniversityResponseDTO>();
        }

        private void OnChangeUniversity(UniversityResponseDTO university)
        {
            _university = university;
            if (university is null)
                _faculty = null;
        }
        private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculties(string searchQuery)
        {
            var result = await FacultyService.GetListByUniversityId(_university.Id);
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<FacultyResponseDTO>();
        }
        private void OnChangeFaculty(FacultyResponseDTO faculty)
        {
            _faculty = faculty;
            _hospital.FacultyId = faculty?.Id;
        }
    }
}
