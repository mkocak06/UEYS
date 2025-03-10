using Fluxor;
using Microsoft.AspNetCore.Components;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Threading.Tasks;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Tabs;

public partial class ProgramStatus
{
    [Inject] private IState<ProgramDetailState> ProgramDetailState { get; set; }
    [Inject] private IAffiliationService AffiliationService { get; set; }
    [Inject] private IProtocolProgramService ProtocolProgramService { get; set; }
    [Inject] private ISweetAlert SweetAlert { get; set; }
    private ProgramResponseDTO ProgramDetail => ProgramDetailState.Value.Program.Program;
    private bool _loadingAffiliaton;
    private bool _loadingProtocol;
    private bool _loadingComplemantary;
    private ProtocolProgramResponseDTO _protocolProg;
    private ProtocolProgramResponseDTO _complemantaryProg;
    private AffiliationResponseDTO _affiliation;

    protected override async Task OnInitializedAsync()
    {

        await GetAffiliation();
        await GetProtocolProgram();
        await GetComplemantaryProgram();
        await base.OnInitializedAsync();
    }

    private async Task GetAffiliation()
    {
        if (ProgramDetail.HospitalId != null && ProgramDetail.FacultyId != null)
        {
            _loadingAffiliaton = true;
            StateHasChanged();
            try
            {
                var response = await AffiliationService.GetByAffiliation((long)ProgramDetail.FacultyId, (long)ProgramDetail.HospitalId);
                if (response.Item != null)
                {
                    //SweetAlert.ToastAlert(SweetAlertIcon.success, L["Affiliation Information has been successfully loaded."]);
                    _affiliation = response.Item;
                }
            }
            catch (Exception)
            {
                _loadingAffiliaton = true;
                StateHasChanged();
            }
            finally
            {
                _loadingAffiliaton = false;
                StateHasChanged();
            }
        }

    }
    private async Task GetProtocolProgram()
    {
        if (ProgramDetail.Id != 0)
        {
            _loadingProtocol = true;
            StateHasChanged();
            try
            {
                var response = await ProtocolProgramService.GetByProgramId(ProgramDetail.Id, ProgramType.Protocol);
                if (response.Item != null)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Protocol Information has been successfully loaded."]);
                    _protocolProg = response.Item;
                }
            }
            catch (Exception)
            {
                _loadingProtocol = true;
                StateHasChanged();
            }
            finally { _loadingProtocol = false; StateHasChanged(); }
        }

    }
    private async Task GetComplemantaryProgram()
    {
        if (ProgramDetail.Id != 0)
        {
            _loadingComplemantary = true;
            StateHasChanged();
            try
            {
                var response = await ProtocolProgramService.GetByProgramId(ProgramDetail.Id, ProgramType.Complement);
                if (response.Item != null)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Protocol Information has been successfully loaded."]);
                    _complemantaryProg = response.Item;
                }
            }
            catch (Exception)
            {
                _loadingComplemantary = true;
                StateHasChanged();
            }
            finally { _loadingComplemantary = false; StateHasChanged(); }
        }

    }
    #region OldDependentProgramCodes
    //private void OnAddProgram(ProgramResponseDTO program)
    //{
    //    if (IsAddedSubProgram(program))
    //    {
    //        return;
    //    }
    //    _protocolProgram.ChildPrograms ??= new List<DependentProgramResponseDTO>();
    //    _protocolProgram.ChildPrograms.Add(new DependentProgramResponseDTO()
    //    {
    //        ProgramId = program.Id,
    //        ProtocolProgramId = _protocolProgram.Id
    //    });
    //}

    //private void OnRemoveProgram(ProgramResponseDTO program)
    //{
    //    var dependentProgram = new DependentProgramResponseDTO()
    //    {
    //        ProgramId = program.Id,
    //        ProtocolProgramId = _protocolProgram.Id,
    //    };

    //    _protocolProgram.ChildPrograms.Remove(dependentProgram);
    //}

    //private void OnAddAuthDetail()
    //{
    //    if (!_authorizationEc.Validate())
    //    {
    //        return;
    //    }
    //    _authorizationDetailModal.CloseModal();
    //}

    //private void OnRemoveAuthDetail(AuthorizationDetailResponseDTO authorizationDetail)
    //{
    //    _protocolProgram.AuthorizationDetails.Remove(authorizationDetail);
    //    StateHasChanged();
    //}

    //private async Task SaveProgramProtocol()
    //{
    //    if (_protocolProgram.ChildPrograms != null && _protocolProgram.ChildPrograms.Any())
    //    {
    //        try
    //        {
    //            _saving = true;
    //            StateHasChanged();
    //            var dto = Mapper.Map<ProtocolProgramDTO>(_protocolProgram);
    //            if (_protocolProgram.Id > 0)
    //            {
    //                await ProtocolProgramService.Update(dto);
    //            }
    //            else
    //            {
    //                var responseWrapper = await ProtocolProgramService.Add(dto);
    //                if (responseWrapper.Result)
    //                {
    //                    ProgramDetail.ProtocolProgramId = responseWrapper.Item.Id;
    //                    var dto2 = Mapper.Map<ProgramDTO>(ProgramDetail);
    //                    if (dto2.FacultyId == null)
    //                    {
    //                        dto2.Faculty = null;
    //                    }
    //                    await ProgramService.Update(ProgramDetail.Id, dto2);
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            SweetAlert.ErrorAlert();
    //            Console.WriteLine(e);
    //        }
    //    }
    //    _saving = false;
    //    StateHasChanged();
    //}

    //private async Task OnOpenProgramListModal()
    //{
    //    _filter = new FilterDTO()
    //    {
    //        Sort = new[]{new Sort()
    //        {
    //            Field = "ExpertiseBranch.Name",
    //            Dir = SortType.ASC
    //        }}
    //    };
    //    _programListModal.OpenModal();
    //    await GetPrograms();
    //}

    //private void OnOpenAuthDetailModal()
    //{
    //    _authorizationDetail = new AuthorizationDetailResponseDTO
    //    {
    //        AuthorizationCategory = new AuthorizationCategoryResponseDTO()
    //    };
    //    _authorizationEc = new EditContext(_authorizationDetail);
    //    StateHasChanged();
    //    _authorizationDetailModal.OpenModal();
    //}

    //private async Task GetPrograms()
    //{
    //    _paginationModel = await ProgramService.GetPaginateList(_filter);
    //    if (_paginationModel.Items != null && _paginationModel.Items.Count != 0)
    //    {
    //        _programs = _paginationModel.Items;
    //        StateHasChanged();
    //    }
    //    else
    //    {
    //        _loading = true;
    //        SweetAlert.ErrorAlert();
    //    }
    //}

    //private async Task PaginationHandler(PaginationInfo val)
    //{
    //    var (item1, item2) = (val.Page, val.PageSize);

    //    _filter.page = item1;
    //    _filter.pageSize = item2;

    //    await GetPrograms();
    //}

    //private async Task<IEnumerable<AuthorizationCategoryResponseDTO>> SearchAuthorizationCategories(string searchQuery)
    //{
    //    var result = await AuthorizationCategoryService.GetAll();
    //    return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
    //        new List<AuthorizationCategoryResponseDTO>();
    //}
    //private void OnChangeAuthorizationCategory(AuthorizationCategoryResponseDTO authorizationCategory)
    //{
    //    _authorizationDetail.AuthorizationCategory = authorizationCategory;
    //    _authorizationDetail.AuthorizationCategoryId = authorizationCategory?.Id;
    //}
    //private bool IsAddedSubProgram(ProgramResponseDTO programResponseDto)
    //{
    //    if (programResponseDto.Id == ProgramDetail.Id)
    //    {
    //        return true;
    //    }
    //    return _protocolProgram.ChildPrograms != null && _protocolProgram.ChildPrograms?.ToList().FindIndex(x => x.ProgramId == programResponseDto.Id) >= 0;
    //}
    #endregion
}