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


namespace UI.Pages.Management.Portfolios;

public partial class PortfolioDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IPortfolioService PortfolioService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private PortfolioResponseDTO _portfolio;
    private EditContext _ec;
    private bool _notFound;
    private bool _saving;
    private bool _loaded;

    private List<BreadCrumbLink> _links;
    protected override void OnInitialized()
    {
        _portfolio = new PortfolioResponseDTO();
        _ec = new EditContext(_portfolio);
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
                    To = "/management/portfolios",
                    OrderIndex = 1,
                    Title = L["_List", L["Portfolio"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = L["_Detail", L["Portfolio"]]
                }
        };
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await PortfolioService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _portfolio = response.Item;
                _ec = new EditContext(_portfolio);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();

                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/portfolios");
            }
        }
        await base.OnParametersSetAsync();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<PortfolioDTO>(_portfolio);
        try
        {
            //var response = await PortfolioService.Update(_portfolio.Id, dto);
            //if (response.Result)
            //{
            //    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
            //    NavigationManager.NavigateTo("./management/portfolios");
            //}
            //else
            //{
            //    throw new Exception(response.Message);
            //}
        }
        catch (Exception e)
        {
            SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
            Console.WriteLine(e.Message);
        }
        _saving = false;
        StateHasChanged();
    }

    private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
    {
        var filter = FilterHelper.CreateFilter(1, int.MaxValue);
        filter.Filter("Name", "contains", searchQuery, "and");
        filter.Sort("Name");

        var result = await ExpertiseBranchService.GetPaginateList(filter);
        return result.Items ?? new List<ExpertiseBranchResponseDTO>();
    }

    private void OnChangeExpertiseBranch(ExpertiseBranchResponseDTO expertiseBranch)
    {
        _portfolio.ExpertiseBranch = expertiseBranch;
        _portfolio.ExpertiseBranchId = expertiseBranch?.Id;
    }

}
