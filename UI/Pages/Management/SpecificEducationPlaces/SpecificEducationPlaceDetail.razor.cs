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
using UI.Models;
using UI.Services;


namespace UI.Pages.Management.SpecificEducationPlaces;

public partial class SpecificEducationPlaceDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public ISpecificEducationPlaceService SpecificEducationPlaceService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IProvinceService ProvinceService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private SpecificEducationPlaceResponseDTO _specificEducationPlace;
    private EditContext _ec;
    private bool _saving;
    private bool _loaded;
    private bool _notFound;
    private List<BreadCrumbLink> _links;
    protected override void OnInitialized()
    {
        _specificEducationPlace = new SpecificEducationPlaceResponseDTO();
        _ec = new EditContext(_specificEducationPlace);
        _loaded = false;
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await SpecificEducationPlaceService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _specificEducationPlace = response.Item;
                _ec = new EditContext(_specificEducationPlace);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();
            }
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
                To = "/management/specific-education-places",
                OrderIndex = 1,
                Title = L["_List", L["Specific Education Place"]]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 1,
                Title = L["_Detail",L["Specific Education Place"]]
            },
            };
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
        _specificEducationPlace.Province = province;
        _specificEducationPlace.ProvinceId = province?.Id;
    }
    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<SpecificEducationPlaceDTO>(_specificEducationPlace);
        try
        {
            var response = await SpecificEducationPlaceService.Update((long)_specificEducationPlace.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                NavigationManager.NavigateTo("./management/specific-education-places");
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


    private string GetSpecificEducationPlace()
    {
        return _specificEducationPlace is { Id: > 0 } ? $"{L["_Detail", L["Specific Education Place"]]}: {_specificEducationPlace.Name}" : "-";
    }

   
}
