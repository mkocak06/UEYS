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


namespace UI.Pages.Management.Announcement;

public partial class AnnouncementDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IAnnouncementService AnnouncementService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private AnnouncementResponseDTO _announcement;
    private EditContext _ec;
    private bool _saving;
    private bool _loaded;
    private bool _notFound;
    private List<BreadCrumbLink> _links;
    protected override void OnInitialized()
    {
        _announcement = new AnnouncementResponseDTO();
        _ec = new EditContext(_announcement);
        _loaded = false;
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await AnnouncementService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _announcement = response.Item;
                _ec = new EditContext(_announcement);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();
                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/announcements");
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
                To = "/management/announcements",
                OrderIndex = 1,
                Title = L["_List", L["Announcement"]]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 1,
                Title = L["_Detail",L["Announcement"]]
            },
            };
        }
        await base.OnParametersSetAsync();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<AnnouncementDTO>(_announcement);
        try
        {
            var response = await AnnouncementService.Update((long)_announcement.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                NavigationManager.NavigateTo("./management/announcements");
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


    //private string GetAnnouncement()
    //{
    //    return _announcement is { Id: > 0 } ? $"{L["_Detail", L["Announcement"]]}: {_announcement.Name}" : "-";
    //}


}
