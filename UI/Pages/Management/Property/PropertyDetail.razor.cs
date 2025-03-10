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


namespace UI.Pages.Management.Property;

public partial class PropertyDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IPropertyService PropertyService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private PropertyResponseDTO _property;
    private EditContext _ec;
    private bool _notFound;
    private bool _saving;
    private bool _loaded;

    private List<BreadCrumbLink> _links;
    protected override void OnInitialized()
    {
        _property = new PropertyResponseDTO();
        _ec = new EditContext(_property);
        _loaded = false;
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await PropertyService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _property = response.Item;
                _ec = new EditContext(_property);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();

                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/propertyies");
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
                    To = "/management/properties",
                    OrderIndex = 1,
                    Title = L["_List", L["Property"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Property"]]
                }
        };
        }
        await base.OnParametersSetAsync();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<PropertyDTO>(_property);
        try
        {
            var response = await PropertyService.Update((long)_property.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                NavigationManager.NavigateTo("./management/properties");
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


    private string GetProperty()
    {
        return _property is { Id: > 0 } ? $"{L["_Detail", L["Property"]]}: {_property.Name}" : "-";
    }


}
