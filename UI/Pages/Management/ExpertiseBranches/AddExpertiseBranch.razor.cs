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

namespace UI.Pages.Management.ExpertiseBranches;

public partial class AddExpertiseBranch
{
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IProfessionService ProfessionService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private ExpertiseBranchResponseDTO _branch;
    private EditContext _ec;
    private bool _saving;
    private MyModal _branchListModal;
    private List<ExpertiseBranchResponseDTO> _branches;
    private bool _isPrincipalBranch = false;
    private List<BreadCrumbLink> _links;

    private InputText _focusTarget;

    protected override void OnInitialized()
    {
        _branch = new ExpertiseBranchResponseDTO();
        _ec = new EditContext(_branch);
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
                    Title =  L["_List", L["Expertise Training Program"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Expertise Training Program"]]
                },
                };
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;


        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<ExpertiseBranchDTO>(_branch);
        try
        {
            var response = await ExpertiseBranchService.Add(dto);

            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                NavigationManager.NavigateTo($"./management/expertisebranches");
            }
            else
            {
                throw new Exception(response.Message);
            }
        }
        catch (Exception e)
        {
            SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
        }
        _saving = false;
        StateHasChanged();
    }
    private async Task<IEnumerable<ProfessionResponseDTO>> SearchFaculties(string searchQuery)
    {
        var result = await ProfessionService.GetAll();
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
            new List<ProfessionResponseDTO>();
    }

    private void OnChangeFaculty(ProfessionResponseDTO faculty)
    {
        _branch.Profession = faculty;
        _branch.ProfessionId = faculty?.Id ?? 0;
    }

    private void OnChangeIsPrincipalBranch()
    {
        _isPrincipalBranch = !_isPrincipalBranch;
        if (!_isPrincipalBranch)
        {
            _branch.PrincipalBranches = new List<RelatedExpertiseBranchResponseDTO>();
        }
        else
        {
            _branch.SubBranches = new List<RelatedExpertiseBranchResponseDTO>();
        }
        StateHasChanged();
    }

    private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
    {
        var filter = FilterHelper.CreateFilter(1, int.MaxValue);
        filter.Filter("Name", "contains", searchQuery, "and");
        filter.Sort("Name");

        var result = await ExpertiseBranchService.GetPaginateList(filter);

        return result.Items?.Where(x => !_branch.SubBranches.Any(y => y.SubBranchId == x.Id)) ?? new List<ExpertiseBranchResponseDTO>();
    }
    private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranchesForPrincipal(string searchQuery)
    {
        var filter = FilterHelper.CreateFilter(1, int.MaxValue);
        filter.Filter("Name", "contains", searchQuery, "and");
        filter.Sort("Name");

        var result = await ExpertiseBranchService.GetPaginateList(filter);
        return result.Items?.Where(x => !_branch.PrincipalBranches.Any(y => y.PrincipalBranchId == x.Id)) ?? new List<ExpertiseBranchResponseDTO>();
    }

    private void OnChangeSubBranches(IList<RelatedExpertiseBranchResponseDTO> values)
    {
        foreach (var item in values)
        {
            _branch.SubBranches.Add(new RelatedExpertiseBranchResponseDTO { SubBranchId = item.SubBranchId });
        }
    }

    private void OnChangePrincipalBranches(IList<RelatedExpertiseBranchResponseDTO> values)
    {
        foreach (var item in values)
        {
            _branch.PrincipalBranches.Add(new RelatedExpertiseBranchResponseDTO { PrincipalBranchId = item.PrincipalBranchId });
        }
    }
}