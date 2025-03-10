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

namespace UI.Pages.Management.SpecificEducationPlaces;

public partial class AddSpecificEducationPlace
{
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public ISpecificEducationPlaceService SpecificEducationPlaceService { get; set; }
    [Inject] public IProvinceService ProvinceService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private SpecificEducationPlaceResponseDTO _specificEducationPlace;
    private EditContext _ec;
    private bool _saving;
    private List<BreadCrumbLink> _links;
    private InputText _focusTarget;

    protected override void OnInitialized()
    {
        _specificEducationPlace = new SpecificEducationPlaceResponseDTO();
        _ec = new EditContext(_specificEducationPlace);
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
                    To = "/management/specific-education-places",
                    OrderIndex = 1,
                    Title = L["_List", L["Specific Education Place"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Specific Education Place"]]
                },
                };
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
            var response = await SpecificEducationPlaceService.Add(dto);

            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                NavigationManager.NavigateTo($"./management/specific-education-places");
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
}