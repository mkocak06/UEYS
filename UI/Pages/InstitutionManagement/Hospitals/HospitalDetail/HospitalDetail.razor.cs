using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.RequestModels;
using Shared.ResponseModels;
using UI.Models;
using UI.Pages.Hospitals.HospitalDetail.Store;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Hospitals.HospitalDetail;

public partial class HospitalDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IHospitalService HospitalService { get; set; }
    [Inject] public IState<HospitalDetailState> HospitalDetailState { get; set; }
    [Inject] public IProvinceService ProvinceService { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IInstitutionService InstitutionService { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private EditContext _ec;
    private HospitalResponseDTO _hospital => HospitalDetailState.Value.Hospital;
    private bool _loading => HospitalDetailState.Value.HospitalDetailLoading;
    private bool _loaded => HospitalDetailState.Value.HospitalDetailLoaded;
    private List<BreadCrumbLink> _links;
    private bool forceRender;



    private string _selectedTab = "overview";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        List<HospitalResponseDTO> hospitals = new();
        try
        {
            var response = await HospitalService.GetAll();
            if (response.Result)
                hospitals = response.Item;
        }
        catch (System.Exception)
        {

        }
        SubscribeToAction<HospitalDetailSetAction>((action) =>
        {
            forceRender = true;
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
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 2,
                    Title = _hospital.Name,
                    DropdownList = hospitals.Select(x=> new DropdownElement()
                    {
                        Link = "/institution-management/hospitals/"+x.Id,
                        Name = x.Name
                    }).ToList()
                },
            };
        });

        SubscribeToAction<HospitalDetailUpdateSuccessAction>((action) =>
        {
            Dispatcher.Dispatch(new HospitalDetailLoadAction((long)Id));
            SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
        });
        SubscribeToAction<HospitalDetailUpdateFailureAction>(async (action) =>
        {
            await SweetAlert.ConfirmAlert(L["Could not be updated"], "", SweetAlertIcon.error, false, L["OK"], "");
        });
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (forceRender)
        {
            forceRender = false;
            JSRuntime.InvokeVoidAsync("initDropdown");
        }
        base.OnAfterRender(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            Dispatcher.Dispatch(new HospitalDetailLoadAction((long)Id));
        }
        await base.OnParametersSetAsync();
    }

    private async Task<IEnumerable<ProvinceResponseDTO>> SearchProvinces(string searchQuery)
    {
        var result = await ProvinceService.GetAll();
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
            new List<ProvinceResponseDTO>();
    }

    private void OnChangeProvince(ProvinceResponseDTO province)
    {
        _hospital.Province = province;
        _hospital.ProvinceId = province?.Id;
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


    private string GetTitle()
    {
        return _hospital is { Id: > 0 } ? $"Hastane Detay: {_hospital.Name}" : "-";
    }

    private void SelectTab(string link)
    {
        if (_selectedTab != link)
        {
            _selectedTab = link;
            StateHasChanged();
        }
    }
    private bool IsSelected(string link)
    {
        return link == _selectedTab;
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
            Dispatcher.Dispatch(new HospitalDetailClearStateAction());
        return base.DisposeAsyncCore(disposing);
    }
}