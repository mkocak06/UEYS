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


namespace UI.Pages.Management.Titles;

public partial class TitleDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public ITitleService TitleService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private TitleResponseDTO _title;
    private EditContext _ec;
    private bool _saving;
    private bool _loaded;
    private bool _notFound;
    private List<BreadCrumbLink> _links;
    protected override void OnInitialized()
    {
        _title = new TitleResponseDTO();
        _ec = new EditContext(_title);
        _loaded = false;
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await TitleService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _title = response.Item;
                _ec = new EditContext(_title);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();
                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/titles");
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
                To = "/management/titles",
                OrderIndex = 1,
                Title = L["_List", L["Title"]]
            },
            new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 1,
                Title = L["_Detail",L["Title"]]
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
        var dto = Mapper.Map<TitleDTO>(_title);
        try
        {
            var response = await TitleService.Update((long)_title.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                NavigationManager.NavigateTo("./management/titles");
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
        return _title is { Id: > 0 } ? $"{L["_Detail", L["Title"]]}: {_title.Name}" : "-";
    }

   
}
