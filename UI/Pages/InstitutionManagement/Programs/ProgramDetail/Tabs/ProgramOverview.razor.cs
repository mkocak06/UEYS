using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Tabs
{
    public partial class ProgramOverview
    {
        [Inject] public IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IProfessionService ProfessionService { get; set; }
        [Inject] public IHospitalService HospitalService { get; set; }
        [Inject] public IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
        [Inject] public IAuthorizationDetailService AuthorizationDetailService { get; set; }
        [Inject] public IAffiliationService AffiliationService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private ProgramResponseDTO ProgramDetail => ProgramDetailState.Value.Program.Program;
        private EditContext _ec;
        private EditContext _authorizationEc;
        private bool _saving;
        private MyModal _authorizationDetailModal;
        private AuthorizationDetailResponseDTO _authorizationDetail;
        private AffiliationResponseDTO _affiliation;
        protected override void OnInitialized()
        {
            GetAffiliation();
            _authorizationDetail = new AuthorizationDetailResponseDTO();
            _authorizationDetail.AuthorizationCategory = new AuthorizationCategoryResponseDTO();
            _ec = new EditContext(ProgramDetail);
            _authorizationEc = new EditContext(_authorizationDetail);
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
        private async Task GetAffiliation()
        {
            if (ProgramDetail.HospitalId != null && ProgramDetail.FacultyId != null)
            {
                var response = await AffiliationService.GetByAffiliation((long)ProgramDetail.FacultyId, (long)ProgramDetail.HospitalId);
                if (response.Item != null)
                    _affiliation = response.Item;
                StateHasChanged();
            }
        }

        private async Task Save()
        {
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();
            var dto = Mapper.Map<ProgramDTO>(ProgramDetail);
            try
            {
                var response = await ProgramService.Update(ProgramDetail.Id, dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, "Başarıyla Güncellendi!");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, "Güncelleme Yetkiniz Bulunmamaktadır!");
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

            ProgramDetail.Faculty.University = university;
            ProgramDetail.Faculty.UniversityId = university?.Id ?? 0;
            if (university is null)
            {
                ProgramDetail.Hospital = null;
                ProgramDetail.HospitalId = null;
            }
        }

        private async Task<IEnumerable<FacultyResponseDTO>> SearchFaculties(string searchQuery)
        {
            var result = await FacultyService.GetListByUniversityId((long)ProgramDetail.Faculty.UniversityId);
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<FacultyResponseDTO>();
        }

        private void OnChangeFaculty(FacultyResponseDTO faculty)
        {
            ProgramDetail.Faculty = faculty;
            ProgramDetail.FacultyId = faculty?.Id ?? 0;
        }

        private async Task<IEnumerable<HospitalResponseDTO>> SearchHospitals(string searchQuery)
        {
            var result = await HospitalService.GetListByUniversityId(ProgramDetail.Faculty.University.Id);
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
                new List<HospitalResponseDTO>();
        }

        private void OnChangeHospital(HospitalResponseDTO hospital)
        {
            ProgramDetail.Hospital = hospital;
            ProgramDetail.HospitalId = hospital?.Id;
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
            ProgramDetail.ExpertiseBranch = expertiseBranch;
            ProgramDetail.ExpertiseBranchId = expertiseBranch?.Id;
        }
        private void OnOpenAuthorizationDetailList()
        {
            _authorizationDetail = new AuthorizationDetailResponseDTO();
            _authorizationDetail.AuthorizationCategory = new AuthorizationCategoryResponseDTO();
            _authorizationEc = new EditContext(_authorizationDetail);
            StateHasChanged();
            _authorizationDetailModal.OpenModal();
        }
        private async void AddAuthorizationDetail()
        {
            if (!_authorizationEc.Validate())
            {
                return;
            }
            var dto = Mapper.Map<AuthorizationDetailDTO>(_authorizationDetail);
            dto.ProgramId = ProgramDetail.Id;
            try
            {
                var response = new ResponseWrapper<AuthorizationDetailResponseDTO>();
                if (_authorizationDetail.Id == null)
                {
                    response = await AuthorizationDetailService.Add(dto);
                    ProgramDetail.AuthorizationDetails.Add(response.Item);
                    StateHasChanged();
                }
                else
                    response = await AuthorizationDetailService.Update((long)_authorizationDetail.Id, dto);
                if (response.Result)
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                else
                    SweetAlert.ErrorAlert();
            }
            catch (Exception ex)
            {
                ex.PrintJson("");
                SweetAlert.ErrorAlert();

            }
            finally
            {
                _authorizationDetailModal.CloseModal();
                _authorizationDetail = new();
            }
        }
        private async Task RemoveAuthorizationDetailAsync(AuthorizationDetailResponseDTO authorizationDetail)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm)
            {
                await AuthorizationDetailService.Delete((long)authorizationDetail.Id);
                ProgramDetail.AuthorizationDetails.Remove(authorizationDetail);
            }
        }
        private void UpdateAuthorizationDetail(AuthorizationDetailResponseDTO authorizationDetail)
        {
            _authorizationDetail = authorizationDetail;
            _authorizationEc = new EditContext(authorizationDetail);
            StateHasChanged();
            _authorizationDetailModal.OpenModal();
        }
    }
}
