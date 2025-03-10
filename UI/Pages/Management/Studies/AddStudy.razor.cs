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

namespace UI.Pages.Management.Studies;

public partial class AddStudy
{
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IStudyService StudyService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private StudyResponseDTO _study;
    private EditContext _ec;
    private bool _saving;
    private List<BreadCrumbLink> _links;
    private InputText _focusTarget;

    protected override void OnInitialized()
    {
        _study = new StudyResponseDTO();
        _ec = new EditContext(_study);
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
                    To = "/management/studies",
                    OrderIndex = 1,
                    Title = L["_List", L["Study"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Study"]]
                },
                };
    }

    private async Task Save()
    {
        _study.Name.PrintJson("bir");

        if (!_ec.Validate()) return;
        _study.Name.PrintJson("iki");
        _saving = true;
        StateHasChanged();
        _study.Name = _study.Name.ToUpper();
        var dto = Mapper.Map<StudyDTO>(_study);
        _study.Name.PrintJson("üç");

        try
        {
            var response = await StudyService.Add(dto);
            _study.Name.PrintJson("dört");


            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                NavigationManager.NavigateTo($"./management/studies");
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