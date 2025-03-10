using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using UI.Models;
using UI.Services;

namespace UI.Pages.Management.FacultyDetails;

public partial class FacultyDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IProfessionService ProfessionService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private ProfessionResponseDTO _faculty;
    private EditContext _ec;
    private bool _saving;
    private bool _notFound;
    private bool _loaded;
    private List<BreadCrumbLink> _links;

    protected override void OnInitialized()
    {
        _faculty = new ProfessionResponseDTO();
        _ec = new EditContext(_faculty);
        _loaded = false;
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
                    To = "/management/faculties",
                    OrderIndex = 1,
                    Title = L["_List", L["Expertise Branch"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Expertise Branch"]]
                }
        };
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await ProfessionService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _faculty = response.Item;
                _ec = new EditContext(_faculty);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();
                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/faculties");
            }
        }
        await base.OnParametersSetAsync();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<ProfessionDTO>(_faculty);
        try
        {
            var response = await ProfessionService.Update(_faculty.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                NavigationManager.NavigateTo("./management/faculties");
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
        return _faculty is { Id: > 0 } ? $"{L["_Detail", L["Educational Institution / Faculty"]]}: {_faculty.Name}": "-";

    }
}


