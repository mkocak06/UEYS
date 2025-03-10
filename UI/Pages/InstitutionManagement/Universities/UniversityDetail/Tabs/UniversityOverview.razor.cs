using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using UI.Helper;
using UI.Pages.Hospitals.HospitalDetail.Store;
using UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail.Tabs
{
    public partial class UniversityOverview
    {
        [Inject] public IState<UniversityDetailState> UniversityState { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IProfessionService ProfessionService { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public IState<AppState> AppState { get; set; }
        private UniversityResponseDTO University => UniversityState.Value.University;
        private EditContext _ec;
        private bool _loading;
        private MyModal _facultyListModal;
        private List<ProfessionResponseDTO> _faculties;
        private SingleMapView _singleMapView;

        protected override void OnInitialized()
        {
            _ec = new EditContext(new UniversityResponseDTO());
            
            SubscribeToAction<UniversityDetailSetAction>(action =>
            {
                _ec = new EditContext(action.University);
            });

            if (!AppState.Value.ProvincesLoaded)
            {
                Dispatcher.Dispatch(new AppLoadProvincesAction());
            }
            base.OnInitialized();
        }

        private async Task Save()
        {
            if (!_ec.Validate()) return;

            var dto = Mapper.Map<UniversityDTO>(University);
            try
            {
                _loading = true;
                StateHasChanged();
                await UniversityService.Update(University.Id, dto);
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            _loading = false; 
            StateHasChanged();
        }
        
        private async Task OnOpenFacultyList()
        {
            _facultyListModal.OpenModal();
            var response = await ProfessionService.GetAll();
            if (response.Result)
            {
                _faculties = response.Item;
                StateHasChanged();
            }
        }

        private void AddFaculty()
        {
            University.Faculties ??= new List<FacultyResponseDTO>();
            University.Faculties.Add(new FacultyResponseDTO()
            {
                UniversityId = University.Id
            });
            StateHasChanged();
        }

        private void RemoveFaculty(FacultyResponseDTO universityFaculty)
        {
            University.Faculties ??= new List<FacultyResponseDTO>();
            University.Faculties.Remove(universityFaculty);
        }
        
        private void OnChangeProvince(ProvinceResponseDTO province)
        {
            University.Province = province;
            University.ProvinceId = province?.Id;
            University.Latitude = province?.Latitude;
            University.Longitude = province?.Longitude;
            if (_singleMapView != null)
            {
                _singleMapView.UpdateMap(University.Latitude ?? 0, University.Longitude ?? 0);
            }
        }
        private void OnChangeLatitude(float? lat)
        {
            University.Latitude = lat;
            if (_singleMapView != null)
            {
                _singleMapView.UpdateMap(University.Latitude ?? 0, University.Longitude ?? 0);
            }
        }
        private void OnChangeLongitude(float? lng)
        {
            University.Longitude = lng;
            if (_singleMapView != null)
            {
                _singleMapView.UpdateMap(University.Latitude ?? 0, University.Longitude ?? 0);
            }
        }
        private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
        {
            return await Task.FromResult(AppState.Value.ProvincesLoaded ? AppState.Value.Provinces.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) : new List<ProvinceResponseDTO>());
        }

        private async Task<IEnumerable<InstitutionResponseDTO>> SearchInstitutions(string searchQuery)
        {
            var result = await InstitutionService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<InstitutionResponseDTO>();
        }

        private void OnChangeInstitution(InstitutionResponseDTO institution)
        {
            University.Institution = institution;
            University.InstitutionId = institution?.Id;
        }

        private void OnChangeIsPrivatetUpdate()
        {
            University.IsPrivate = !University.IsPrivate;

            StateHasChanged();
        }

        private void LocationChanged(float lat, float lng )
        {
            University.Latitude = lat;
            University.Longitude = lng;
            StateHasChanged();
        }
    }
}