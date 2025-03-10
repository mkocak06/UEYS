using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Management.Provinces;

public partial class AddProvince
{
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IProvinceService ProvinceService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private ProvinceResponseDTO _province;
    private EditContext _ec;
    private bool _saving;
     private SingleMapView _singleMapView;
    private InputText _focusTarget;
    private List<BreadCrumbLink> _links;

    protected override void OnInitialized()
    {
        _province = new ProvinceResponseDTO();
        _ec = new EditContext(_province);
        base.OnInitialized();
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
                    To = "/management/provinces",
                    OrderIndex = 1,
                    Title = L["_List",L["Province"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Province"]]
                },
                };
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<ProvinceDTO>(_province);
        try
        {
            var response = await ProvinceService.Add(dto);
    
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                NavigationManager.NavigateTo($"./management/provinces");
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
    private void OnChangeLocation(float lat, float lng)
    {
        _province.Latitude = lat;
        _province.Longitude = lng;
    }
    private void OnChangeLongitude(ChangeEventArgs changeEventArgs)
    {


        if (changeEventArgs == null)
        {
            _province.Latitude = null;
            return;
        }
        _province.Longitude = float.Parse(changeEventArgs.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);


        if (_province.Latitude != null && _province.Longitude != null)
        {
            _singleMapView.UpdateMap(_province.Latitude ?? 0, _province.Longitude ?? 0);
        }
    }
    private void OnChangeLatitude(ChangeEventArgs changeEventArgs)
    {

        if (changeEventArgs == null)
        {
            _province.Latitude = null;
            return;
        }

        _province.Latitude = float.Parse(changeEventArgs.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

        if (_province.Latitude != null && _province.Longitude != null)
        {
            _singleMapView.UpdateMap(_province.Latitude ?? 0, _province.Longitude ?? 0);
        }
    }

}