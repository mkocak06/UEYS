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

namespace UI.Pages.InstitutionManagement.Universities;

public partial class AddUniversity
{
    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IState<AppState> AppState { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IInstitutionService InstitutionService { get; set; }
    [Inject] public IProfessionService ProfessionService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private UniversityResponseDTO _university;
    private EditContext _ec;
    private bool _saving;
    private MyModal _facultyListModal;
    private List<ProfessionResponseDTO> _faculties;
    private SingleMapView _singleMapView;
    private List<BreadCrumbLink> _links;
    private InputMask _inputMask;
    private InputText _focusTarget;
    private bool _isprivate;

    protected override void OnInitialized()
    {
        _isprivate = false;
        if (!AppState.Value.ProvincesLoaded)
        {
            Dispatcher.Dispatch(new AppLoadProvincesAction());
        }
        _university = new UniversityResponseDTO();
        _ec = new EditContext(_university);
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
                To = "/institution-management/universities",
                OrderIndex = 1,
                Title = L["Universities"]
            },new BreadCrumbLink()
            {
                IsActive= false,
                OrderIndex = 2,
                Title = @L["add_new", L["Ministry / University"]]
            }
        };
        base.OnInitialized();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<UniversityDTO>(_university);
        try
        {
            var response = await UniversityService.Add(dto);
            if (response.Result)
            {
                NavigationManager.NavigateTo($"./institution-management/universities/{response.Item.Id}");
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
    private void PhoneNoChanged(string tel)
    {
        _university.Phone = tel;
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

    private void AddFaculty(ProfessionResponseDTO profession)
    {
        _university.Faculties ??= new List<FacultyResponseDTO>();
        _university.Faculties.Add(new FacultyResponseDTO()
        {
            UniversityId = _university.Id,
            Profession = profession,
            ProfessionId = profession.Id,
        });
    }
    private void OnChangeIsPrivate1()
    {
        if (_isprivate == false)
        {
            _university.IsPrivate = _isprivate;
            StateHasChanged();
        }


    }

    private void OnChangeIsPrivate2()
    {
        if (_isprivate == false)
        {
            _university.IsPrivate = !_isprivate;
            StateHasChanged();
        }


    }

    private void RemoveFaculty(FacultyResponseDTO universityFaculty)
    {
        _university.Faculties ??= new List<FacultyResponseDTO>();
        _university.Faculties.Remove(universityFaculty);
    }

    private void OnChangeProvince(ProvinceResponseDTO province)
    {
        _university.Province = province;
        _university.ProvinceId = province?.Id;
        _university.Latitude = province?.Latitude;
        _university.Longitude = province?.Longitude;
        _singleMapView.UpdateMap(_university.Latitude ?? 0, _university.Longitude ?? 0);
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
        _university.Institution = institution;
        _university.InstitutionId = institution?.Id;
    }

    private void OnChangeLocation(float lat, float lng)
    {
        _university.Latitude = lat;
        _university.Longitude = lng;
    }
    private void OnChangeLongitude(ChangeEventArgs changeEventArgs)
    {


        if (changeEventArgs == null)
        {
            _university.Latitude = null;
            return;
        }
        _university.Longitude = float.Parse(changeEventArgs.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);


        if (_university.Latitude != null && _university.Longitude != null)
        {
            _singleMapView.UpdateMap(_university.Latitude ?? 0, _university.Longitude ?? 0);
        }
    }
    private void OnChangeLatitude(ChangeEventArgs changeEventArgs)
    {

        if (changeEventArgs == null)
        {
            _university.Latitude = null;
            return;
        }



        _university.Latitude = float.Parse(changeEventArgs.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);



        if (_university.Latitude != null && _university.Longitude != null)
        {
            _singleMapView.UpdateMap(_university.Latitude ?? 0, _university.Longitude ?? 0);
        }
    }
}