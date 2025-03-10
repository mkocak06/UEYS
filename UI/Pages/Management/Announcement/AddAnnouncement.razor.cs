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

namespace UI.Pages.Management.Announcement;

public partial class AddAnnouncement
{
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IAnnouncementService AnnouncementService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private AnnouncementResponseDTO _announcement;
    private EditContext _ec;
    private bool _saving;
    private List<BreadCrumbLink> _links;
    private InputText _focusTarget;

    protected override void OnInitialized()
    {
        _announcement = new AnnouncementResponseDTO();
        _ec = new EditContext(_announcement);
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
                    To = "/management/announcements",
                    OrderIndex = 1,
                    Title = L["_List", L["Announcement"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Announcement"]]
                },
                };
    }

    private async Task Save()
    {

        if (!_ec.Validate()) return;
        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<AnnouncementDTO>(_announcement);
        dto.PublishDate = DateTime.UtcNow;
        try
        {
            var response = await AnnouncementService.Add(dto);

            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                NavigationManager.NavigateTo($"./management/announcements");
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