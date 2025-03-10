using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.Affiliations;

public partial class AffiliationDetail
{
    [Parameter] public long? Id { get; set; }
    [Inject] public IAffiliationService AffiliationService { get; set; }
    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IDocumentService DocumentService { get; set; }
    [Inject] public IFacultyService FacultyService { get; set; }
    [Inject] public IHospitalService HospitalService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    private AffiliationResponseDTO _affiliation = new();
    private UniversityResponseDTO _affiliatingUniversity;
    private EditContext _ec;
    private bool _saving;
    private bool _loaded = false;
    private bool _notFound = false;
    private List<BreadCrumbLink> _links = new();
    private bool forceRender;
    private Dropzone dropzone;
    private MyModal UploaderModal;
    private ICollection<DocumentResponseDTO> _documents;
    protected override void OnInitialized()
    {

        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            await GetAffiliationDetail();
        }
        List<AffiliationResponseDTO> affiliations = new();
        try
        {
            var response = await AffiliationService.GetAll();
            if (response.Result)
                affiliations = response.Item;
        }
        catch (System.Exception)
        {

        }
        forceRender = true;
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
                    Title =_affiliatingUniversity?.Name+"-"+_affiliation.Hospital?.Name,
                    DropdownList = affiliations.Select(x=> new DropdownElement()
                    {
                        Link = "/institution-management/affiliations/"+x.Id,
                        Name = x.Faculty?.University?.Name+"-"+x.Hospital?.Name
                    }).ToList()
                },


        };
        StateHasChanged();
        await base.OnParametersSetAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (forceRender)
        {
            forceRender = false;
            JSRuntime.InvokeVoidAsync("initDropdown");
        }
        base.OnAfterRender(firstRender);
    }

    public async Task<bool> CallDropzone()
    {
        var result = await dropzone.SubmitFileAsync();
        if (result.Result)
        {
            await GetAffiliationDetail();
            return true;
        }
        else
        {
            return false;
        }
    }
    private async Task GetAffiliationDetail()
    {

        _loaded = false;
        try
        {
            var response = await AffiliationService.GetById((long)Id, DocumentTypes.Affiliation);
            if (response.Result && response.Item != null)
            {
                _affiliation = response.Item;
                _affiliatingUniversity = _affiliation.Faculty?.University;
                _documents = _affiliation.Documents;
                _ec = new EditContext(_affiliation);

                StateHasChanged();
            }
            else
            {
                _loaded = true;
                //await SweetAlert.ConfirmAlert(L["Page not found"], "", SweetAlertIcon.error, false, L["OK"], "");
                //NavigationManager.NavigateTo("./institution-management/affiliations");
                _notFound = true;

            }
        }
        catch (Exception)
        {
            await SweetAlert.ConfirmAlert(L["Hata Oluştu"], "", SweetAlertIcon.error, false, L["OK"], "");
            NavigationManager.NavigateTo("./institution-management/affiliations");
        }
        finally
        {
            _loaded = true;

        }
        StateHasChanged();

    }

    private async Task Save()
    {
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<AffiliationDTO>(_affiliation);
        try
        {
            var response = await AffiliationService.Update(_affiliation.Id, dto);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                await GetAffiliationDetail();
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
        }

        StateHasChanged();
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

    private void OnChangeAffiliatedHospital(HospitalResponseDTO hospital)
    {
        _affiliation.Hospital = hospital;
        _affiliation.HospitalId = hospital?.Id;
    }

    private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculties(string searchQuery)
    {
        var result = await FacultyService.GetListByUniversityId(_affiliatingUniversity.Id);

        return result.Result ? result.Item.Where(x => x.Programs.Any(x => x.ExpertiseBranch?.Profession?.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture)) ?? false)) :
            new List<FacultyResponseDTO>();
    }

    private void OnChangeAffiliatingFaculty(FacultyResponseDTO faculty)
    {
        _affiliation.Faculty = faculty;
        _affiliation.FacultyId = faculty?.Id;
    }

    private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
    {
        var response = await HospitalService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Sort("Name", Shared.Types.SortType.ASC).OrFilter(new string[] { "Name", "Province.Name" }, "contains", searchQuery));
        return response.Items;
    }
}

