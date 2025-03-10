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

namespace UI.Pages.Management.FacultyDetails;

public partial class AddFaculty
{
    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
   
    [Inject] public IProfessionService ProfessionService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private ProfessionResponseDTO _faculty;
    private EditContext _ec;
    private bool _saving;
    private MyModal _facultyListModal;
    private List<ProfessionResponseDTO> _faculties;
    private List<BreadCrumbLink> _links;
    private InputText _focusTarget;

    protected override void OnInitialized()
    {
        _faculty = new ProfessionResponseDTO();
        _ec = new EditContext(_faculty);
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
                    Title = L["_List",L["Expertise Branch"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Expertise Branch"]]
                },
                };
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<ProfessionDTO>(_faculty);
        try
        {
            var response = await ProfessionService.Add(dto);
           

            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                NavigationManager.NavigateTo($"./management/faculties");
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