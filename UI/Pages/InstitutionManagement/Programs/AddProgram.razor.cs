using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Shared.RequestModels;
using Shared.ResponseModels;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.Programs;

public partial class AddProgram
{
    [Inject] public IProgramService ProgramService { get; set; }
    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IFacultyService FacultyService { get; set; }
    [Inject] public IProfessionService ProfessionService { get; set; }
    [Inject] public IHospitalService HospitalService { get; set; }
    [Inject] public IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
    //[Inject] public IAuthorizationDetailService AuthorizationDetailService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    private ProgramResponseDTO _program;
    private EditContext _ec;
    private EditContext _authorizationEc;
    private bool _saving;
    private AuthorizationDetailResponseDTO _authorizationDetail;
    private MyModal _authorizationDetailModal;
    private List<BreadCrumbLink> _links;
    private UniversityResponseDTO? _university = new();
    protected override void OnInitialized()
    {
        _program = new ProgramResponseDTO()
        {
            Faculty = new() { University = new() },
            Hospital = new(),
            AuthorizationDetails = new List<AuthorizationDetailResponseDTO>(),
            ExpertiseBranch = new(),
            Affiliation = new(),
            ProtocolProgram = new(),
        };
        _authorizationDetail = new AuthorizationDetailResponseDTO();
        _ec = new EditContext(_program);
        _authorizationEc = new EditContext(_authorizationDetail);
        _links = new()
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
                To = "/institution-management/programs",
                OrderIndex = 1,
                Title = L["Expertise Training Programs"]
            },new BreadCrumbLink()
            {
                IsActive = false,
                OrderIndex = 2,
                Title = @L["add_new", L["Expertise Training Program"]]
            },
        };
        base.OnInitialized();
    }
    private void OnChangeDescription(IList<string> descriptions)
    {
        _authorizationDetail.Descriptions = descriptions.ToList();
    }
    public async Task<string> AddItemOnEmptyResult(string input)
    {
        return input;
    }
    private void OnChangeAuthorizationDate(DateTime? date)
    {
        _authorizationDetail.AuthorizationDate = date;
        if (_authorizationDetail.AuthorizationCategory?.Duration != null)
            _authorizationDetail.AuthorizationEndDate = _authorizationDetail.AuthorizationDate.Value.AddDays(_authorizationDetail.AuthorizationCategory.Duration);
        StateHasChanged();
    }
    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<ProgramDTO>(_program);
        //if (dto.FacultyId == null)
        //{
        //    dto.Faculty = null;
        //}
        dto.Faculty = null;
        try
        {
            var response = await ProgramService.Add(dto);
            if (response.Result)
            {
                NavigationManager.NavigateTo($"./institution-management/programs/{response.Item.Id}");
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
            }
            else
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", response.Message);
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

    private async Task<IEnumerable<UniversityResponseDTO>> SearchUniversities(string searchQuery)
    {
        var result = await UniversityService.GetAll();
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
            new List<UniversityResponseDTO>();
    }

    private void OnChangeUniversity(UniversityResponseDTO university)
    {
        _university = university;
        if (university is null)
        {
            _program.Faculty = null;
            _program.FacultyId = null;
            _program.Hospital = null;
            _program.HospitalId = null;
            _program.ExpertiseBranch = null;
            _program.ExpertiseBranchId = null;
        }
    }

    private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculties(string searchQuery)
    {
        var result = await FacultyService.GetListByUniversityId(_university.Id);
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
            new List<FacultyResponseDTO>();
    }

    private void OnChangeFaculty(FacultyResponseDTO faculty)
    {
        if(faculty is null)
        {
            _program.Hospital = null;
            _program.HospitalId = null;
            _program.ExpertiseBranch = null;
            _program.ExpertiseBranchId = null;
        }
        _program.Faculty = faculty;
        _program.FacultyId = faculty?.Id ?? 0;
    }

    private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
    {
        var result = await HospitalService.GetListByUniversityId((long)_program.Faculty.UniversityId);
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).OrderBy(x => x.Name) :
            new List<HospitalResponseDTO>();
    }

    private void OnChangeHospital(HospitalResponseDTO hospital)
    {
        if(hospital is null)
        {
            _program.ExpertiseBranch = null;
            _program.ExpertiseBranchId = null;
        }
        _program.Hospital = hospital;
        _program.HospitalId = hospital?.Id;
    }

    private async Task<IEnumerable<AuthorizationCategoryResponseDTO>> SearchAuthorizationCategories(string searchQuery)
    {
        var result = await AuthorizationCategoryService.GetAll();
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).OrderBy(x => x.Name) :
            new List<AuthorizationCategoryResponseDTO>();
    }
    private void OnChangeAuthorizationCategory(AuthorizationCategoryResponseDTO authorizationCategory)
    {
        _authorizationDetail.AuthorizationCategory = authorizationCategory;
        _authorizationDetail.AuthorizationCategoryId = authorizationCategory?.Id;

        if (_authorizationDetail.AuthorizationDate.HasValue)
            _authorizationDetail.AuthorizationEndDate = _authorizationDetail.AuthorizationDate.Value.AddDays(authorizationCategory.Duration);
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
        _program.ExpertiseBranch = expertiseBranch;
        _program.ExpertiseBranchId = expertiseBranch?.Id;
    }
    private void OnOpenAuthorizationDetailList()
    {
        _authorizationDetailModal.OpenModal();

    }
    private void AddAuthorizationDetail()
    {
        if (!_authorizationEc.Validate())
        {
            return;
        }
        _authorizationDetailModal.CloseModal();
        if (_authorizationDetail.Id == 0)
        {
            _program.AuthorizationDetails.Add(_authorizationDetail);
        }
        else
        {
            var index = _program.AuthorizationDetails.ToList().FindIndex(x => x.Id == _authorizationDetail.Id);
            if (index >= 0)
            {
                var list = _program.AuthorizationDetails.ToList();
                list[index] = _authorizationDetail;

                _program.AuthorizationDetails = list;
            }

        }

        _authorizationDetail = new();
        _authorizationEc = new EditContext(_authorizationDetail);

    }
    private void RemoveAuthorizationDetail(AuthorizationDetailResponseDTO authorizationDetail)
    {
        _program.AuthorizationDetails ??= new List<AuthorizationDetailResponseDTO>();
        _program.AuthorizationDetails.Remove(authorizationDetail);
    }
    private void UpdateAuthorizationDetail(AuthorizationDetailResponseDTO authorizationDetail)
    {
        if (_authorizationDetail != null)
        {
            _authorizationDetail = authorizationDetail;
            _authorizationDetail.AuthorizationCategory = authorizationDetail.AuthorizationCategory;
            _authorizationDetailModal.OpenModal();
            _authorizationDetail = new();
        }
    }
}

