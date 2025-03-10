using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApexCharts;
using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.ProtocolPrograms;

public partial class EditProtocolProgram
{
    [Inject] public IProgramService ProgramService { get; set; }
    [Inject] public IEducatorService EducatorService { get; set; }
    [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }

    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IProtocolProgramService ProtocolProgramService { get; set; }
    [Inject] public IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Parameter] public long? Id { get; set; }

    private ProtocolProgramResponseDTO _protocolProgram;
    private EducatorResponseDTO _educator;
    private DependentProgramResponseDTO _dependentProgram = new();
    private EditContext _ec;
    private EditContext _educatorEc;
    private EditContext _authorizationEc;
    private EditContext _detailEc;
    private bool _saving;
    private FilterDTO _filter;
    private AuthorizationDetailResponseDTO _authorizationDetail;
    private MyModal _authorizationDetailModal;
    private MyModal _isAdminModal;
    private MyModal _detailModal;

    private MyModal _programListModal;
    private MyModal _educatorListModal;

    private PaginationModel<ProgramResponseDTO> _programPaginationModel;
    private PaginationModel<EducatorPaginateResponseDTO> _educatorPaginationModel;
    private bool _programsLoading;
    private bool _educatorLoading;
    private bool _loading = true;
    private List<BreadCrumbLink> _links;
    private List<long> _principalBranchIdList = new();
    /// <summary>
    /// filter-sort
    /// </summary>
    private FilterDTO _filterProgram;
    private FilterDTO _filterEducator;
    private Dropzone dropzone;
    private Dictionary<int, Dropzone> dropzoneRDP = new();
    private MyModal UploaderModal;
    private ICollection<DocumentResponseDTO> _documents;
    private DateTime? _protocolDate;
    Dictionary<string, RadzenDataGrid<DependentProgramResponseDTO>> grid = new();
    RadzenDataGrid<EducatorDependentProgramResponseDTO> gridEducators;
    private RelatedDependentProgramResponseDTO _relatedDependentProgram = new();
    private bool _ppLoading;
    private MyModal _cancelModal;
    private EditContext _cancelEc;
    private string _dateValidatorMessage;
    private long _dpId;
    private bool _fileLoaded = true;
    private string _rdpDecisionDateValidator;

    protected override void OnInitialized()
    {
        _protocolProgram = new ProtocolProgramResponseDTO
        {
            RelatedDependentPrograms = new List<RelatedDependentProgramResponseDTO>(),
        };
        _authorizationDetail = new AuthorizationDetailResponseDTO();
        _ec = new EditContext(_protocolProgram);
        _detailEc = new EditContext(_dependentProgram);
        _authorizationEc = new EditContext(_authorizationDetail);
        _educator = new EducatorResponseDTO();
        _educatorEc = new EditContext(_educator);
        _filterEducator = new FilterDTO()
        {
            Filter = new()
            {
                Filters = new()
                {

                    new Filter()
                    {
                        Field="IsDeleted",
                        Operator="eq",
                        Value=false
                    }

                },
                Logic = "and"

            },

            Sort = new[]{new Sort()
            {
                Field = "Name",
                Dir = SortType.ASC
            }}
        };
        _filterProgram = new FilterDTO()
        {
            Filter = new()
            {
                Filters = new()
                {

                    new Filter()
                    {
                        Field="IsDeleted",
                        Operator="eq",
                        Value=false
                    }

                },
                Logic = "and"

            },

            Sort = new[]{new Sort()
            {
                Field = "Hospital.Province.Name",
                Dir = SortType.ASC
            }}
        };
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
                    To = "/institution-management/protocol-programs",
                    OrderIndex = 1,
                    Title = L["Protocol Programs"]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = @L["_Detail", L["Protocol Program"]]
                }
        };
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Id != null)
        {
            await GetProtocolProgramDetail();
            _ec = new EditContext(_protocolProgram);
        }
        await base.OnParametersSetAsync();
    }
    void RowRender(RowRenderEventArgs<DependentProgramResponseDTO> args)
    {
        if (args.Data?.EducatorDependentPrograms.Count > 0)
        {
            args.Attributes.Add("class", $"{(IsManager(args.Data.EducatorDependentPrograms) ? "table-danger font-weight-bold" : "")};");
            args.Attributes.Add("style", $"font-weight: {(IsManager(args.Data.EducatorDependentPrograms) ? "bold" : "normal")};");
        }
    }
    void RowRenderForEducator(RowRenderEventArgs<EducatorDependentProgramResponseDTO> args)
    {
        args.Attributes.Add("class", $"{(args.Data.IsProgramManager.Value ? "table-danger font-weight-bold" : "")};");
        args.Attributes.Add("style", $"font-weight: {(args.Data.IsProgramManager.Value ? "bold" : "normal")};");
    }
    private bool IsManager(ICollection<EducatorDependentProgramResponseDTO> edps)
    {
        if (edps.Count > 0 && edps != null)
        {
            return edps.Any(x => x.IsProgramManager == true);
        }
        else
            return false;
    }
    private async void AddToRDP()
    {
        _relatedDependentProgram = new()
        {
            ProtocolProgramId = _protocolProgram.Id,
            IsActive = true,
            ChildPrograms = new(),
            Revision = _protocolProgram.RelatedDependentPrograms.OrderByDescending(x => x.Revision).FirstOrDefault() != null ? _protocolProgram.RelatedDependentPrograms.OrderByDescending(x => x.Revision).FirstOrDefault().Revision + 1 : 0,
        };
        dropzoneRDP[(int)_relatedDependentProgram.Revision] = new();
        
        foreach (var rdp in _protocolProgram.RelatedDependentPrograms)
        {
            rdp.IsActive = false;
            foreach (var dp in rdp.ChildPrograms)
                foreach (var edp in dp.EducatorDependentPrograms)
                    edp.IsProgramManager = false;
        }
        _protocolProgram.RelatedDependentPrograms.Add(_relatedDependentProgram);
        StateHasChanged();
    }
    private async Task GetProtocolProgramDetail()
    {
        _ppLoading = true;
        StateHasChanged();
        try
        {
            var response = await ProtocolProgramService.Get((long)Id, DocumentTypes.ProtocolProgram, ProgramType.Protocol);
            if (response.Result)
            {
                _protocolProgram = response.Item;
                if (response.Item != null && response.Item.RelatedDependentPrograms?.Count > 0)
                {
                    _protocolProgram.RelatedDependentPrograms = _protocolProgram.RelatedDependentPrograms.OrderByDescending(x => x.IsActive).ToList();
                }

                var expBrFilter = _filterProgram.Filter.Filters.FirstOrDefault(x => x.Field == "ExpertiseBranchIds");

                if (expBrFilter != null)
                {
                    _principalBranchIdList = new();
                    _filterProgram.Filter.Filters.Remove(expBrFilter);
                }
                _protocolProgram.ParentProgram.ExpertiseBranch.PrincipalBranches.ForEach(x => _principalBranchIdList.Add((long)x.PrincipalBranchId));

                _filterProgram.Filter.Filters.Add(new() { Field = "ExpertiseBranchIds", Operator = "eq", Value = _principalBranchIdList });
                _documents = response.Item.Documents;
                _loading = false;
                StateHasChanged();
            }
            else
            {
                throw new Exception(response.Message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            SweetAlert.ErrorAlert();
        }
        finally
        {
            _ppLoading = false;
            StateHasChanged();
        }
    }
    public async Task<bool> CallDropzone()
    {
        var result = await dropzone.SubmitFileAsync();
        if (result.Result)
        {
            _protocolProgram.Documents.Add(result.Item);
            StateHasChanged();
            return true;
        }
        else
        {
            return false;
        }
    }
    private async Task Save()
    {
        if (!_ec.Validate()) return;

        foreach (var rdp in _protocolProgram.RelatedDependentPrograms)
        {
            if (rdp.DecisionDate == null)
            {
                _rdpDecisionDateValidator = L["This field is required"];
                StateHasChanged();
                return;
            }
        }

        if (_protocolProgram.RelatedDependentPrograms.Any(x => _protocolProgram.ParentProgram.ExpertiseBranch.ProtocolProgramCount > x.ChildPrograms.Count))
        {
            SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L["In order to create a program protocol, the number of dependent programs in the protocol/revision must be at least {0}.", _protocolProgram.ParentProgram.ExpertiseBranch.ProtocolProgramCount]);
            return;
        }

        _saving = true;
        StateHasChanged();

        foreach (var item in _protocolProgram.RelatedDependentPrograms)
        {
            item.Revision.PrintJson("rev");
            var sssss = dropzoneRDP[(int)item.Revision]._selectedFileName;
            sssss.PrintJson("sss");
            if ((dropzoneRDP[(int)item.Revision]._selectedFileName) != null)
            {
                await CallDropzoneRDP((int)item.Revision);
            }
        }

        try
        {
            var response = await ProtocolProgramService.Update(_protocolProgram.Id, _protocolProgram);
            if (response.Result)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                await GetProtocolProgramDetail();
            }
            else if (response.Item.MissingExpertiseBranches?.Count > 0)
            {
                var branches = string.Join(", ", response.Item.MissingExpertiseBranches.Select(x => x.Name));
                SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["Any branch in the institutions that make up the protocol and which has an instructor with an intensive care specialist certificate cannot be excluded from the program.\r\n\t\t  \r\n\t\tFollowing branches has to be selected:\r\n\t\t\r\n\t\t  \r\n\t\t{0}", branches]);
                _saving = false;
                StateHasChanged();
                return;
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

    private async Task PaginationHandlerPrograms(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);
        //var filter = new FilterDTO()
        //{
        //    page = item1,
        //    pageSize = item2
        //};
        _filterProgram.page = item1;
        _filterProgram.pageSize = item2;
        _programsLoading = true;
        StateHasChanged();
        _programPaginationModel = await ProgramService.GetPaginateListForProtocolProgram(_filterProgram);
        _programsLoading = false;
    }
    private async Task GetPrograms()
    {
        _programsLoading = true;
        StateHasChanged();
        if (_filterProgram?.Filter?.Filters?.Count == 0)
        {
            _filterProgram.Filter.Filters = null;
            _filterProgram.Filter.Logic = null;
        }
        _programPaginationModel = await ProgramService.GetPaginateListForProtocolProgram(_filterProgram);
        _programsLoading = false;
        StateHasChanged();
    }
    private async Task OnOpenProgramListModal(RelatedDependentProgramResponseDTO rdp)
    {
        _relatedDependentProgram = rdp;
        if (_protocolProgram.ParentProgram is null)
        {
            SweetAlert.IconAlert(SweetAlertIcon.warning, "", "Önce Program seçiniz");
        }
        else
        {
            _programListModal.OpenModal();
            await GetPrograms();
        }
    }
    private void OnChangeProgram(ProgramResponseDTO program)
    {
        _protocolProgram.ParentProgram = program;
        _protocolProgram.ParentProgramId = program?.Id ?? 0;
        _principalBranchIdList = _protocolProgram.ParentProgram.PrincipalBrancIdList;

        _filterProgram.Filter.Filters.FirstOrDefault(x => x.Field == "ExpertiseBranchIds").Value = _principalBranchIdList;
    }
    private async Task<IEnumerable<ProgramResponseDTO>> SearchPrograms(string searchQuery)
    {
        string[] searchParams = searchQuery.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        List<Filter> filterList = new List<Filter>();
        filterList.Add(new() { Field = "ProtocolProgram" });

        foreach (var item in searchParams)
        {
            var filter0 = new Filter()
            {
                Field = "UniversityName",
                Operator = "contains",
                Value = item
            };
            filterList.Add(filter0);
            var filter1 = new Filter()
            {
                Field = "ProvinceName",
                Operator = "contains",
                Value = item,
            };
            filterList.Add(filter1);
            var filter2 = new Filter()
            {
                Field = "ProfessionName",
                Operator = "contains",
                Value = item,
            };
            filterList.Add(filter2);
            var filter3 = new Filter()
            {
                Field = "HospitalName",
                Operator = "contains",
                Value = item,
            };
            filterList.Add(filter3);
            var filter4 = new Filter()
            {
                Field = "ExpertiseBranchName",
                Operator = "contains",
                Value = item
            };
            filterList.Add(filter4);
        }

        var result = await ProgramService.GetListForSearch(new FilterDTO()
        {
            Filter = new Filter()
            {
                Logic = "or",
                Filters = filterList
            },
            page = 1,
            pageSize = int.MaxValue
        });

        return result.Items ?? new List<ProgramResponseDTO>();
    }
    //private async Task DeleteDependentProgram()
    //{
    //    if (_dependentProgram.CancelingDate == null || _dependentProgram.CancelingProtocolNo == null)
    //    {
    //        _dateValidatorMessage = L["Canceling Date and Canceling Protocol No cannot be empty!"];
    //        StateHasChanged();
    //        return;
    //    }
    //    var programManager = _dependentProgram.EducatorDependentPrograms.FirstOrDefault(x => x.IsProgramManager == true);
    //    if (programManager != null)
    //        programManager.IsProgramManager = false;

    //    StateHasChanged();
    //    grid[_relatedDependentProgram.Revision.ToString()].Reload();
    //    _cancelModal.CloseModal();
    //}
    private async Task OnDeleteDependentProgramHandlerAsync(RelatedDependentProgramResponseDTO relatedDependentProgram, DependentProgramResponseDTO dependentProgram)
    {
        _relatedDependentProgram = relatedDependentProgram;
        _dependentProgram = dependentProgram;

        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
        if (!confirm)
            return;

        var programManager = _dependentProgram.EducatorDependentPrograms.FirstOrDefault(x => x.IsProgramManager == true);
        if (programManager != null)
            programManager.IsProgramManager = false;

        _relatedDependentProgram.ChildPrograms.Remove(dependentProgram);
        StateHasChanged();
        grid[_relatedDependentProgram.Revision.ToString()].Reload();
    }
    private async void OnDeleteRelatedDependentProgramHandler(RelatedDependentProgramResponseDTO rdp)
    {
        if (rdp.Id != null)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (!confirm)
                return;
        }
        _protocolProgram.RelatedDependentPrograms.Remove(rdp);

        StateHasChanged();
    }

    private async void OnActiveRelatedDependentProgramHandler(RelatedDependentProgramResponseDTO rdp)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Active"] + " " + L["Version"]}",
                $"{L["Bu Versiyonu \"Aktif Versiyon\" Yapmak İstediğinize Emin Misiniz? Diğer aktif versiyon -varsa- pasif hale getirilecek."]}",
                SweetAlertIcon.question, true, $"{L["Evet"]}", $"{L["No"]}");
        if (confirm)
        {
            foreach (var item in _protocolProgram.RelatedDependentPrograms.Where(x => x.IsActive = true))
            {
                item.IsActive = false;
            }
            rdp.IsActive = true;
            _protocolProgram.RelatedDependentPrograms = _protocolProgram.RelatedDependentPrograms.OrderByDescending(x => x.IsActive).ToList();
            StateHasChanged();
        }
        else return;
    }
    private void OnUpdateDependentProgramHandler(RelatedDependentProgramResponseDTO relatedDependentProgram, DependentProgramResponseDTO dependentProgram)
    {
        _dependentProgram = dependentProgram;
        _relatedDependentProgram = relatedDependentProgram;
        _detailEc = new EditContext(_dependentProgram);
        StateHasChanged();
        //_dependentProgram.Documents ??= new();
        _detailModal.OpenModal();
    }

    private void OnAddDependentProgramHandler(ProgramResponseDTO program)
    {
        _dependentProgram = new DependentProgramResponseDTO()
        {
            ProgramId = program.Id,
            Program = program,
            RelatedDependentProgramId = _relatedDependentProgram.Id,
            IsAdministratorProgram = false,
        };
        _detailEc = new EditContext(_dependentProgram);
        StateHasChanged();
        _detailModal.OpenModal();

    }
    private async void UpdateDetail()
    {
        if (!_detailEc.Validate()) return;

        //if (!string.IsNullOrEmpty(dropzoneDependentProgram._selectedFileName))
        //    await CallDropzoneDependentProgram();

        //var dto = Mapper.Map<DependentProgramDTO>(_dependentProgram);

        //try
        //{
        //    var response = await DependentProgramService.Update(_dependentProgram.Id, dto);
        //    if (response.Result)
        //    {
        //        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
        //        //await GetProtocolProgramDetail();
        //    }
        //    else
        //        throw new Exception(response.Message);
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e);
        //    SweetAlert.ErrorAlert(e.Message ?? L["An error occured"]);
        //}

        StateHasChanged();
        _dependentProgram = new();
        _detailModal.CloseModal();
    }

    public async Task<bool> CallDropzoneRDP(int revision)
    {
        _fileLoaded = false;
        StateHasChanged();
        try
        {
            var response = await dropzoneRDP[revision].SubmitFileAsync();
            if (response.Result)
            {
                var rdp = _protocolProgram.RelatedDependentPrograms.FirstOrDefault(x => x.Revision == revision);
                rdp.DocumentId = response.Item.Id;

                _fileLoaded = true;
                StateHasChanged();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            _fileLoaded = false;
            Console.WriteLine(e.Message);
            return false;
        }
        finally
        {
            dropzoneRDP[revision].ResetStatus();
            StateHasChanged();
        }
    }
    private async void AddDetail()
    {
        if (!_detailEc.Validate()) return;
        _dependentProgram.EducatorDependentPrograms ??= new();
        if (!_relatedDependentProgram.ChildPrograms.Any(x => x.ProgramId == _dependentProgram.ProgramId))
            _relatedDependentProgram.ChildPrograms.Add(_dependentProgram);
        StateHasChanged();

        _detailModal.CloseModal();
        _programListModal.CloseModal();
        grid[_relatedDependentProgram.Revision.ToString()].Reload();
        _dependentProgram = new();
    }

    private bool IsValidProgram(ProgramResponseDTO program)
    {
        return _protocolProgram.ParentProgramId != null && _relatedDependentProgram.ChildPrograms.All(x => x.Program.ExpertiseBranchId != program.ExpertiseBranchId);
    }

    private async Task OnAddEducatorDependentProgramHandler(EducatorPaginateResponseDTO educator)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Birim Eğitim Sorumlusu"]}",
                $"{L["Eklediğiniz Eğitici \"Birim Eğitim Sorumlusu\" Mu? Diğer Bağlı Programlardaki Tüm Eğitim Sorumluları Kaldırılacak."]}",
                SweetAlertIcon.question, true, $"{L["Evet"]}", $"{L["No"]}");

        if (confirm)
        {
            foreach (var child in _protocolProgram.RelatedDependentPrograms.FirstOrDefault(x => x.Id == _dependentProgram.RelatedDependentProgramId).ChildPrograms)
            {
                foreach (var edp in child.EducatorDependentPrograms)
                {
                    edp.IsProgramManager = false;
                }
            }
        }

        _dependentProgram.EducatorDependentPrograms.Add(new() { Educator = new() { AcademicTitleName = educator.AcademicTitle, StaffTitleName = educator.StaffTitle, User = new() { Name = educator.Name, Phone = educator.Phone } }, EducatorId = educator.Id, IsProgramManager = confirm });

        StateHasChanged();
        await gridEducators.Reload();
    }
    private async Task OnRemoveEducatorDependentProgramHandler(EducatorPaginateResponseDTO educator)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");

        if (confirm)
            _dependentProgram.EducatorDependentPrograms.Remove(_dependentProgram.EducatorDependentPrograms.FirstOrDefault(x => x.EducatorId == educator.Id));
    }
    private void OnRemoveList(EducatorDependentProgramResponseDTO edp, DependentProgramResponseDTO dependentProgram)
    {
        _protocolProgram.RelatedDependentPrograms.FirstOrDefault(x => x.Id == dependentProgram.RelatedDependentProgramId).ChildPrograms.FirstOrDefault(x => x.Id == dependentProgram.Id).EducatorDependentPrograms.Remove(edp);

        StateHasChanged();

        gridEducators.Reload();
    }
    private async Task OnRemoveChildEducatorHandler(EducatorResponseDTO educator, DependentProgramResponseDTO dependentProgram)
    {
        var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
            $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
            SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
        Console.WriteLine(educator.Id);
        if (confirm)
        {
            _dependentProgram.EducatorDependentPrograms.Remove(_dependentProgram.EducatorDependentPrograms.FirstOrDefault(x => x.EducatorId == educator.Id));
            StateHasChanged();
            gridEducators.Reload();
        }
    }
    private bool IsValıdEducator(EducatorPaginateResponseDTO educator)
    {
        return _protocolProgram.RelatedDependentPrograms.FirstOrDefault(x => x.Id == _dependentProgram.RelatedDependentProgramId).ChildPrograms.Where(x => x.ProgramId == _dependentProgram.ProgramId).Any(x => x.EducatorDependentPrograms.Any(x => x.EducatorId == educator.Id));
    }
    private async Task GetEducator(long dpId)
    {
        _educatorLoading = true;
        _dpId = dpId;
        StateHasChanged();
        if (_filterEducator?.Filter?.Filters?.Count == 0)
        {
            _filterEducator.Filter.Filters = null;
            _filterEducator.Filter.Logic = null;
        }
        //_educatorPaginationModel = await EducatorService.GetPaginateList(_filterEducator);
        //TODO: replace with this:
        _educatorPaginationModel = await EducatorService.GetPaginateListByProgramId(_filterEducator, _dpId);
        _educatorLoading = false;
        StateHasChanged();
    }
    private async Task PaginationHandlerEducator(PaginationInfo val)
    {
        var (item1, item2) = (val.Page, val.PageSize);

        _filterEducator.page = item1;
        _filterEducator.pageSize = item2;
        _educatorPaginationModel = await EducatorService.GetPaginateListByProgramId(_filterEducator, _dpId);
        _educatorLoading = true;
        StateHasChanged();
    }

    private async Task OpenEducatorModal(DependentProgramResponseDTO dependentProgram)
    {
        _dependentProgram = new();
        _dependentProgram.Id = dependentProgram.Id;
        _dependentProgram = dependentProgram;
        _educatorPaginationModel = null;
        _educatorListModal.OpenModal();
        await GetEducator((long)dependentProgram.ProgramId);
    }

    private void OnOpenAuthorizationDetailList()
    {
        _authorizationDetail = new AuthorizationDetailResponseDTO();
        _authorizationEc = new EditContext(_authorizationDetail);
        _authorizationDetailModal.OpenModal();
    }
    private async Task<IEnumerable<AuthorizationCategoryResponseDTO>> SearchAuthorizationCategories(string searchQuery)
    {
        var result = await AuthorizationCategoryService.GetAll();
        return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))) :
            new List<AuthorizationCategoryResponseDTO>();
    }
    private void OnChangeAuthorizationCategory(AuthorizationCategoryResponseDTO authorizationCategory)
    {
        _authorizationDetail.AuthorizationCategory = authorizationCategory;
        _authorizationDetail.AuthorizationCategoryId = authorizationCategory?.Id;
    }
    private void AddAuthorizationDetail()
    {
        if (!_authorizationEc.Validate()) return;
        _authorizationDetailModal.CloseModal();
        if (_authorizationDetail != null)
        {
            _authorizationDetail = new();
        }
    }
    //private void RemoveAuthorizationDetail(AuthorizationDetailResponseDTO authorizationDetail)
    //{
    //    _protocolProgram.AuthorizationDetails ??= new List<AuthorizationDetailResponseDTO>();
    //    _protocolProgram.AuthorizationDetails.Remove(authorizationDetail);
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
    void RowExpand(DependentProgramResponseDTO dependentProgram)
    {
        _dependentProgram = dependentProgram;
        if (dependentProgram.EducatorDependentPrograms == null)
        {
            dependentProgram.EducatorDependentPrograms = dependentProgram.EducatorDependentPrograms.Where(o => o.DependentProgramId == dependentProgram.Id).ToList();
        }
    }

    private void GoToAddEducator()
    {
        NavigationManager.NavigateTo($"./educator/educators/add-educator");
    }
    private string GetManagerName()
    {
        if (_protocolProgram.RelatedDependentPrograms?.FirstOrDefault(x => x.IsActive)?.ChildPrograms?.Count > 0)
        {

            foreach (var item in _protocolProgram.RelatedDependentPrograms?.FirstOrDefault(x => x.IsActive).ChildPrograms)
            {
                if (item.EducatorDependentPrograms?.Count > 0)
                {

                    foreach (var item2 in item.EducatorDependentPrograms)
                    {
                        if (item2.IsProgramManager.Value == true)
                        {
                            return item2.Educator.User.Name;
                        }
                    }
                }
            }
        }
        return "<cite>" + " Henüz program yöneticisi belirlenmemiştir. " + "</cite>";
    }
    // sort-filter modal
    private async Task OnSortChange(Sort sort)
    {
        _filterProgram.Sort = new[] { sort };
        await GetPrograms();
    }

    private async Task OnChangeFilter(ChangeEventArgs args, string filterName)
    {
        var value = (string)args.Value;
        _filterProgram.Filter ??= new Filter();
        _filterProgram.Filter.Filters ??= new List<Filter>();
        _filterProgram.Filter.Logic ??= "and";
        var index = _filterProgram.Filter.Filters.FindIndex(x => x.Field == filterName);
        if (index < 0)
        {
            _filterProgram.Filter.Filters.Add(new Filter()
            {
                Field = filterName,
                Operator = "contains",
                Value = value
            });
        }
        else
        {
            _filterProgram.Filter.Filters[index].Value = value;
        }
        await GetPrograms();
    }
    
    private async Task OnEducatorChangeFilter(ChangeEventArgs args, string filterName)
    {
        var value = (string)args.Value;
        _filterEducator.Filter ??= new Filter();
        _filterEducator.Filter.Filters ??= new List<Filter>();
        _filterEducator.Filter.Logic ??= "and";
        var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
        if (index < 0)
        {
            _filterEducator.Filter.Filters.Add(new Filter()
            {
                Field = filterName,
                Operator = "contains",
                Value = value
            });
        }
        else
        {
            _filterEducator.Filter.Filters[index].Value = value;
        }
        await GetEducator(_dpId);
    }

    private async Task OnResetFilter(string filterName)
    {
        _filterProgram.Filter ??= new Filter();
        _filterProgram.Filter.Filters ??= new List<Filter>();
        _filterProgram.Filter.Logic ??= "and";
        var index = _filterProgram.Filter.Filters.FindIndex(x => x.Field == filterName);
        if (index >= 0)
        {
            _filterProgram.Filter.Filters.RemoveAt(index);
            await JSRuntime.InvokeVoidAsync("clearInput", filterName);
            await GetPrograms();
        }
    }
    
    private async Task OnResetFilterEducator(string filterName)
    {
        _filterEducator.Filter ??= new Filter();
        _filterEducator.Filter.Filters ??= new List<Filter>();
        _filterEducator.Filter.Logic ??= "and";
        var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
        if (index >= 0)
        {
            _filterEducator.Filter.Filters.RemoveAt(index);
            await JSRuntime.InvokeVoidAsync("clearInput", filterName);
            await GetEducator(_dpId);
        }
    }

    private bool IsFiltered(string filterName)
    {
        _filterProgram.Filter ??= new Filter();
        _filterProgram.Filter.Filters ??= new List<Filter>();
        _filterProgram.Filter.Logic ??= "and";
        var index = _filterProgram.Filter.Filters.FindIndex(x => x.Field == filterName);
        return index >= 0;
    }
    
    private bool IsFilteredEducator(string filterName)
    {
        _filterEducator.Filter ??= new Filter();
        _filterEducator.Filter.Filters ??= new List<Filter>();
        _filterEducator.Filter.Logic ??= "and";
        var index = _filterEducator.Filter.Filters.FindIndex(x => x.Field == filterName);
        return index >= 0;
    }
}