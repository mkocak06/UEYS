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
using UI.Pages.InstitutionManagement.Programs.ProgramDetail;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Management.Portfolios;

public partial class AddPortfolio
{
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IPortfolioService PortfolioService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private PortfolioResponseDTO _portfolio;
    private EditContext _ec;
    private bool _saving;
    private SingleMapView _singleMapView;
    private InputText _focusTarget;
    private List<BreadCrumbLink> _links;

    protected override void OnInitialized()
    {
        _portfolio = new PortfolioResponseDTO();
        _ec = new EditContext(_portfolio);
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
                    To = "/management/portfolios",
                    OrderIndex = 1,
                    Title = L["_List",L["Portfolio"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Portfolio"]]
                },
                };
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        _portfolio.ExpertiseBranchId.PrintJson("port.exId");
        var dto = Mapper.Map<PortfolioDTO>(_portfolio);
        dto.PrintJson("");
        try
        {
            var response = await PortfolioService.Add(dto);

            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                NavigationManager.NavigateTo($"./management/portfolios");
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