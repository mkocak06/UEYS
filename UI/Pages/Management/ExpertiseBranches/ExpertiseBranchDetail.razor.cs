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


namespace UI.Pages.Management.ExpertiseBranches;

public partial class ExpertiseBranchDetail
{
    [Parameter] public long? Id { get; set; }

    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public IProfessionService ProfessionService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private ExpertiseBranchResponseDTO _branch;
    private EditContext _ec;
    private bool _saving;
    private bool _loaded;
    private bool _notFound;
    private List<BreadCrumbLink> _links;
    private IList<ExpertiseBranchResponseDTO> _subBranches = new List<ExpertiseBranchResponseDTO>();
    private IList<ExpertiseBranchResponseDTO> _principalBranches = new List<ExpertiseBranchResponseDTO>();

    protected override void OnInitialized()
    {
        _branch = new ExpertiseBranchResponseDTO();
        _ec = new EditContext(_branch);
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
                    To = "/management/expertisebranches",
                    OrderIndex = 1,
                    Title = L["_List", L["Expertise Training Program"]]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = @L["_Detail", L["Expertise Training Program"]]
                }
        };
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            var response = await ExpertiseBranchService.GetById((long)Id);
            if (response.Result && response.Item != null)
            {
                _branch = response.Item;
                if (_branch.IsPrincipal == false)
                {
                    foreach (var item in _branch.PrincipalBranches)
                    {
                        _principalBranches.Add(item.PrincipalBranch);
                    }
                }
                else
                {
                    foreach (var item in _branch.SubBranches)
                    {
                        _subBranches.Add(item.SubBranch);
                    }
                }
                _ec = new EditContext(_branch);
                _loaded = true;
                StateHasChanged();
            }
            else
            {
                _loaded = true;
                _notFound = true;
                StateHasChanged();
                //await SweetAlert.ConfirmAlert($"{L["Page Not Found!"]}", "", SweetAlertIcon.error, false, $"{L["Okey"]}", "");
                //NavigationManager.NavigateTo("./management/expertisebranches");
            }
        }
        await base.OnParametersSetAsync();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;

        StateHasChanged();

        try
        {
            var dto = Mapper.Map<ExpertiseBranchDTO>(_branch);
            var response = await ExpertiseBranchService.Update((long)_branch.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                NavigationManager.NavigateTo("./management/expertisebranches");
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
        return _branch is { Id: > 0 } ? $"{L["_Detail", L["Expertise Training Program"]]}: {_branch.Name}" : "-";
    }

    private async Task<IEnumerable<ProfessionResponseDTO>> SearchFaculties(string searchQuery)
    {
        var result = await ProfessionService.GetAll();
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
            new List<ProfessionResponseDTO>();
    }

    private void OnChangeFaculty(ProfessionResponseDTO faculty)
    {
        _branch.Profession = faculty;
        _branch.ProfessionId = faculty?.Id ?? 0;
    }

    private void OnChangeIsPrincipalBranch()
    {
        _branch.IsPrincipal = !_branch.IsPrincipal;
        StateHasChanged();
    }

    private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchSubExpertiseBranches(string searchQuery)
    {
        var filter = FilterHelper.CreateFilter(1, int.MaxValue);
        filter.Filter("Name", "contains", searchQuery, "and");
        filter.Sort("Name");

        var result = await ExpertiseBranchService.GetPaginateList(filter);

        return result.Items?.Where(x => !_branch.SubBranches.Any(y => y.SubBranchId == x.Id)) ?? new List<ExpertiseBranchResponseDTO>();
    }
    private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranchesForPrincipales(string searchQuery)
    {
        var filter = FilterHelper.CreateFilter(1, int.MaxValue);
        filter.Filter("Name", "contains", searchQuery, "and");
        filter.Sort("Name");

        var result = await ExpertiseBranchService.GetPaginateList(filter);

        return result.Items?.Where(x => !_branch.PrincipalBranches.Any(y => y.PrincipalBranchId == x.Id)) ?? new List<ExpertiseBranchResponseDTO>();
    }

    private void OnChangeSubBranches(IList<ExpertiseBranchResponseDTO> values)
    {
        _branch.SubBranches = new();
        foreach (var item in values)
        {
            _branch.SubBranches.Add(new RelatedExpertiseBranchResponseDTO { SubBranchId = item.Id });
        }
    }

    private void OnChangePrincipalBranches(IList<ExpertiseBranchResponseDTO> values)
    {
        _branch.PrincipalBranches = new();
        foreach (var item in values)
        {
            _branch.PrincipalBranches.Add(new RelatedExpertiseBranchResponseDTO { PrincipalBranchId = item.Id });
        }
    }

    private void OnChangePortfolioIndex(int? value)
    {
        _branch.PortfolioIndexRateToCapacityIndex = value;
        _branch.EducatorIndexRateToCapacityIndex = 100 - value;
        StateHasChanged();
    }

    private void OnChangeEducatorIndex(int? value)
    {
        _branch.EducatorIndexRateToCapacityIndex = value;
        _branch.PortfolioIndexRateToCapacityIndex = 100 - value;
        StateHasChanged();
    }
}
