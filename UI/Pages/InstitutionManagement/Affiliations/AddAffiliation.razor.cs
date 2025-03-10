using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApexCharts;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.InstitutionManagement.Affiliations;

public partial class AddAffiliation
{
    [Inject] public IAffiliationService AffiliationService { get; set; }
    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IFacultyService FacultyService { get; set; }
    [Inject] public IProfessionService ProfessionService { get; set; }
    [Inject] public IDocumentService DocumentService { get; set; }
    [Inject] public IHospitalService HospitalService { get; set; }
    [Inject] public IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    private AffiliationResponseDTO _affiliation;
    private UniversityResponseDTO _affiliatingUniversity;
    private EditContext _ec;
    private bool _saving;
    private bool _fileLoaded = true;
    private List<BreadCrumbLink> _links;
    private Dropzone dropzone;
    private MyModal UploaderModal;
    private List<DocumentResponseDTO> responseDocuments = new();

    protected override void OnInitialized()
    {
        _affiliation = new AffiliationResponseDTO()
        {
            FacultyId = 0,
            HospitalId = 0,
        };
        _ec = new EditContext(_affiliation);
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
                    To = "/institution-management/affiliations",
                    OrderIndex = 1,
                    Title = L["Affiliations"]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = @L["add_new",L["Affiliation"]]
                }
        };
        base.OnInitialized();
    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<AffiliationDTO>(_affiliation);
        try
        {
            _affiliation.ProtocolDate.PrintJson("aff1");
            var response = await AffiliationService.Add(dto);
            if (response.Result)
            {
                foreach (var item in responseDocuments)
                {
                    var documentDTO = Mapper.Map<DocumentDTO>(item);
                    documentDTO.EntityId = response.Item.Id;
                    var result = await DocumentService.Update(item.Id, documentDTO);
                    if (!result.Result)
                    {
                        throw new Exception(result.Message);
                    }
                }
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                NavigationManager.NavigateTo($"./institution-management/affiliations/{response.Item.Id}");

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
        finally
        {
            _saving = false;
            responseDocuments.Clear();
        }

        StateHasChanged();
    }
    public async Task<bool> CallDropzone()
    {
        _fileLoaded = false;
        StateHasChanged();
        try
        {
            var result = await dropzone.SubmitFileAsync();
            if (result.Result)
            {
                responseDocuments.Add(result.Item);
                _fileLoaded = true;
                StateHasChanged();
                UploaderModal.CloseModal();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            _fileLoaded = true;
            Console.WriteLine(e.Message);
            return false;
        }

    }
    private async Task<IEnumerable<UniversityResponseDTO>> SearchAffiliatingUniversities(string searchQuery)
    {
        var response = await UniversityService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Sort("Name", Shared.Types.SortType.ASC).OrFilter(new string[] { "Name", "Province.Name" }, "contains", searchQuery));
        return response.Items;
    }
    private void OnChangeAffiliatingUniversity(UniversityResponseDTO university)
    {
        _affiliatingUniversity = university;
        _affiliation.FacultyId = 0;
        _affiliation.Faculty = null;
        StateHasChanged();
    }

    private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculties(string searchQuery)
    {
        var result = await FacultyService.GetListByUniversityId(_affiliatingUniversity.Id);
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(100) : new List<FacultyResponseDTO>();

        //return result.Result ? result.Item.Where(x => x.Programs.Any(x => x.ExpertiseBranch?.Profession?.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)) ?? false)) :
        //    new List<FacultyResponseDTO>();
    }

    private void OnChangeAffiliatingFaculty(FacultyResponseDTO faculty)
    {
        _affiliation.Faculty = faculty;
        _affiliation.FacultyId = faculty?.Id ?? 0;
    }

    private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
    {
        var response = await HospitalService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Sort("Name", Shared.Types.SortType.ASC).OrFilter(new string[] { "Name", "Province.Name" }, "contains", searchQuery));
        return response.Items;
    }
    private void OnChangeAffiliatedHospital(HospitalResponseDTO affiliatedHospital)
    {
        _affiliation.Hospital = affiliatedHospital;
        _affiliation.HospitalId = affiliatedHospital?.Id ?? 0;
    }
}

