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
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.InstitutionManagement.Hospitals;

public partial class AddHospital
{
    [Inject] public IState<AppState> AppState { get; set; }
    [Inject] public IHospitalService HospitalService { get; set; }
    [Inject] public IProvinceService ProvinceService { get; set; }
    [Inject] public IInstitutionService InstitutionService { get; set; }
    [Inject] public IAuthenticationService AuthenticationService{ get; set; }
    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IFacultyService FacultyService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private HospitalResponseDTO _hospital;
    private EditContext _ec;
    private bool _saving;
    private SingleMapView _singleMapView;
    private List<BreadCrumbLink> _links;
    private InputText _focusTarget;
    private InputMask _inputMask;
    private UniversityResponseDTO _university;
    private FacultyResponseDTO _faculty;

    protected override void OnInitialized()
    {
        if (!AppState.Value.ProvincesLoaded)
        {
            Dispatcher.Dispatch(new AppLoadProvincesAction());
        }
        _hospital = new HospitalResponseDTO() { Institution = new InstitutionResponseDTO()};
        _ec = new EditContext(_hospital);
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
                    IsActive = true,
                    To = "/institution-management/hospitals",
                    OrderIndex = 1,
                    Title = L["Institutions of Education"]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = @L["add_new", L["Institution of Education"]]
                }
        };
        base.OnInitialized();
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
        return result.Result ? result.Item.Where(x=> x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) : 
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
    }

    private void OnChangeLongitude(ChangeEventArgs changeEventArgs)
    {


        if (changeEventArgs == null)
        {
            _hospital.Latitude = null;
            return;
        }
        _hospital.Longitude = float.Parse(changeEventArgs.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
     
        
        if (_hospital.Latitude != null && _hospital.Longitude != null)
        {
            _singleMapView.UpdateMap(_hospital.Latitude ?? 0, _hospital.Longitude ?? 0);
        }
    }
    private void OnChangeLatitude(ChangeEventArgs changeEventArgs)
    {

        if (changeEventArgs == null)
        {
            _hospital.Latitude = null;
            return;
        }

        _hospital.Latitude = float.Parse(changeEventArgs.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        
        if (_hospital.Latitude != null && _hospital.Longitude != null)
        {
            _singleMapView.UpdateMap(_hospital.Latitude ?? 0, _hospital.Longitude ?? 0);
        }
    }
    private void PhoneNoChanged(string tel)
    {
        _hospital.Phone = tel;
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
                NavigationManager.NavigateTo($"./institution-management/hospitals/{response.Item.Id}");
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
        _saving = false;
        StateHasChanged();
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