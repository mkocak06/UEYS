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


namespace UI.Pages.Management.Provinces;

public partial class ProvinceDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IProvinceService ProvinceService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private ProvinceResponseDTO _province;
    private EditContext _ec;
    private bool _notFound;
    private bool _saving;
    private bool _loaded;

    private List<BreadCrumbLink> _links;
    protected override void OnInitialized()
    {
        _province = new ProvinceResponseDTO();
        _ec = new EditContext(_province);
        _loaded = false;
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
                    Title = L["_List", L["Province"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Province"]]
                }
        };
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await ProvinceService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _province = response.Item;
                _ec = new EditContext(_province);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();

                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/provinces");
            }
        }
        await base.OnParametersSetAsync();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<ProvinceDTO>(_province);
        try
        {
            var response = await ProvinceService.Update(_province.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                NavigationManager.NavigateTo("./management/provinces");
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


    private string GetTitle()
    {
        return _province is { Id: > 0 } ? $"{L["_Detail", L["Province"]]}: {_province.Name}" : "-";
    }

   
}
