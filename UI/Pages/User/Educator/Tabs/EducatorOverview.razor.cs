using AutoMapper;
using Fluxor;
using Majorsoft.Blazor.Components.Common.JsInterop.Scroll;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Radzen;
using Shared.Extensions;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using Shared.Validations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.Management.ExpertiseBranches;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.User.Educator.Tabs
{
    public partial class EducatorOverview
    {
        //[Parameter] public bool IsEditing { get; set; }
        [Parameter] public long EducatorIdFromAdding { get; set; }
        [Inject] public IState<EducatorDetailState> EducatorState { get; set; }
        [Inject] public IEducatorService EducatorService { get; set; }
        [Inject] public ITitleService TitleService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IDocumentService Document { get; set; }
        [Inject] public IInstitutionService InstitutionService { get; set; }
        [Inject] public IFacultyService FacultyService { get; set; }
        [Inject] public IHospitalService HospitalService { get; set; }
        [Inject] public IUniversityService UniversityService { get; set; }
        [Inject] public IEducatorExpertiseBranchService EducatorExpertiseBranchService { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IEducatorProgramService EducatorProgramService { get; set; }
        [Inject] public IUserService UserService { get; set; }
        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }


        private List<EducatorExpertiseBranchResponseDTO> _selectedEducatorExpertiseBranches = new();
        private List<ExpertiseBranchResponseDTO> allExpBranches = new();
        private List<ExpertiseBranchResponseDTO> expertiseBranches = new();
        private EducatorResponseDTO Educator => EducatorState.Value.Educator;
        private EditContext _ecUpdate;
        public bool _loading;
        private bool _loadingUserInfo;
        private bool _loadingGraduationInfo;
        private bool _loadingCKYSInfo;
        private InputMask PhoneInput;
        private TitleResponseDTO _academicTitle;
        private TitleResponseDTO _staffTitle;
        private EditContext _ecForProgram;
        private List<DocumentResponseDTO> Documents = new();
        private MyModal FileModal;
        private bool isDisabled = false;
        private DateTime tueyDate = new DateTime(2009, 7, 18);

        private Dropzone dropzone;
        private Dropzone dropzoneDeclaration;
        private Dropzone dropzoneProgram;
        private Dropzone dropzoneOfficer;
        private Dropzone dropzoneChairman;
        private Dropzone dropzoneMember;
        private MyModal UploaderModal;
        private MyModal _educatorProgramModal;
        private MyModal _educatorStaffParentInstitutionModal;
        private List<DocumentResponseDTO> responseDocuments = new();
        private bool _fileLoaded = true;
        private EducatorProgramResponseDTO _selectedEducatorProgram = new();
        private EducatorProgramResponseDTO _educatorProgram = new();
        private List<UniversityResponseDTO> _universities;
        private List<HospitalResponseDTO> _hospitals;
        private EducatorStaffParentInstitutionResponseDTO _educatorStaffParentInstitution = new();
        //private EducationOfficerResponseDTO _educationOfficer = new();
        private string _documentValidatorMessage;
        private string _declarationDocumentValidatorMessage;
        private string _chairmanDocumentValidatorMessage;
        private string _memberDocumentValidatorMessage;
        private string _programStartDateValidatorMessage;
        private string _administrativeTitleValidatorMessage;
        private bool _oldIsEducationOfficer;
        private string _educationOfficerDocumentValidatorMessage;
        private bool _educatorLoaded = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _ecForProgram = new EditContext(new EducatorProgramResponseDTO());
            _ecUpdate = new EditContext(new EducatorResponseDTO());
            if (EducatorIdFromAdding != null && EducatorIdFromAdding > 0)
            {
                Dispatcher.Dispatch(new EducatorDetailLoadAction(EducatorIdFromAdding));

            }
            if (Educator != null)
            {
                _educatorLoaded = true;
            }
            //if (IsEditing)
            //{
            SubscribeToAction<EducatorDetailSetAction>(action =>
            {
                _ecUpdate = new EditContext(action.Educator);
                _educatorLoaded = true;
                StateHasChanged();
            });
            _selectedEducatorExpertiseBranches = Educator?.EducatorExpertiseBranches;
            //foreach (var item in Educator.EducatorAdministrativeTitles)
            //{
            //    _selectedAdministrativeTitles.Add(new TitleResponseDTO { Id = item.AdministrativeTitleId, Name = item.AdministrativeTitle.Name, TitleType = TitleType.Administrative });
            //}
            //}
            SearchUniversities();
            SearchHospitals();

            var result = await ExpertiseBranchService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue).Sort("Name", Shared.Types.SortType.ASC));
            allExpBranches = result.Items;

            expertiseBranches = allExpBranches.Where(x => x.IsPrincipal == true).ToList();
            if (Educator?.EducatorExpertiseBranches != null)
                foreach (var eduExpBr in Educator?.EducatorExpertiseBranches)
                    if (eduExpBr?.SubBranchIds?.Count > 0)
                        foreach (var subId in eduExpBr.SubBranchIds)
                            if (!Educator.EducatorExpertiseBranches.Any(x => x.ExpertiseBranchId == subId))
                                expertiseBranches.Add(allExpBranches.FirstOrDefault(x => x.Id == subId));

            var phdRecords = Educator?.GraduationDetails?.Where(x => x.IsPhd == true).ToList();

            //if (Educator.GraduationDetails?.Where(x => x.IsPhd == true).ToList() != null)
            //    foreach (var item in Educator.GraduationDetails?.Where(x => x.IsPhd == true).ToList())
            //        if (item.GraduationDate != null && (item.GraduationDate != "" ? DateTime.ParseExact(item.GraduationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null) < tueyDate)
            //            isDisabled = false; StateHasChanged();

        }
        private void AddExpertiseBranch()
        {
            Educator.EducatorExpertiseBranches ??= new();
            Educator.EducatorExpertiseBranches.Add(new EducatorExpertiseBranchResponseDTO());
            StateHasChanged();
        }

        private async void CheckConditionalEducator()
        {
            _documentValidatorMessage = string.Empty;
            _declarationDocumentValidatorMessage = string.Empty;
            StateHasChanged();

            if (Educator.IsConditionalEducator == true)
            {
                if (Educator.Documents != null && Educator.Documents.Count > 0)
                {
                    if (!Educator.Documents.Any(x => x.DocumentType == DocumentTypes.AssociateProfessorship))
                        _documentValidatorMessage = "Bu alan zorunludur!";
                    if (!Educator.Documents.Any(x => x.DocumentType == DocumentTypes.DeclarationDocument))
                        _declarationDocumentValidatorMessage = "Bu alan zorunludur!";
                }
                else
                {

                    _declarationDocumentValidatorMessage = "Bu alan zorunludur!";
                    _documentValidatorMessage = "Bu alan zorunludur!";
                }

            }
        }
        public async Task Save()
        {
            _ecUpdate.GetValidationMessages().PrintJson("messages");
            if (!_ecUpdate.Validate())
            {
                JSRuntime.InvokeVoidAsync("scrollToValidationSummary");
                return;
            }

            _loading = true;

            _documentValidatorMessage = string.Empty;
            _declarationDocumentValidatorMessage = string.Empty;
            _chairmanDocumentValidatorMessage = string.Empty;
            _memberDocumentValidatorMessage = string.Empty;
            _administrativeTitleValidatorMessage = string.Empty;


            if (Educator.IsConditionalEducator == true)
            {
                if (string.IsNullOrEmpty(dropzone._selectedFileName) && !Educator.Documents.Any(x => x.DocumentType == DocumentTypes.AssociateProfessorship))
                {
                    _documentValidatorMessage = "Bu alan zorunludur!";
                    _loading = false;
                    StateHasChanged();
                    return;
                }
                else if (!string.IsNullOrEmpty(dropzone._selectedFileName))
                    await CallDropzone();

                if (string.IsNullOrEmpty(dropzoneDeclaration._selectedFileName) && !Educator.Documents.Any(x => x.DocumentType == DocumentTypes.DeclarationDocument))
                {
                    _declarationDocumentValidatorMessage = "Bu alan zorunludur!";
                    _loading = false;
                    StateHasChanged();
                    return;
                }
                else if (!string.IsNullOrEmpty(dropzoneDeclaration._selectedFileName))
                    await CallDropzoneDeclaration();
            }
            if (Educator.IsForensicMedicineInstitutionEducator == true)
            {
                if (Educator.IsChairman == true)
                {
                    if (string.IsNullOrEmpty(dropzoneChairman._selectedFileName) && !Educator.Documents.Any(x => x.DocumentType == DocumentTypes.SpecializationBoardChairman))
                    {
                        _chairmanDocumentValidatorMessage = "Bu alan zorunludur!";
                        _loading = false;
                        StateHasChanged();
                        return;
                    }
                    else if (!string.IsNullOrEmpty(dropzoneChairman._selectedFileName))
                        await CallDropzoneChairman();
                }
                else
                {
                    if (string.IsNullOrEmpty(dropzoneMember._selectedFileName) && !Educator.Documents.Any(x => x.DocumentType == DocumentTypes.SpecializationBoardMember))
                    {
                        _memberDocumentValidatorMessage = "Bu alan zorunludur!";
                        _loading = false;
                        StateHasChanged();
                        return;
                    }
                    else if (!string.IsNullOrEmpty(dropzoneMember._selectedFileName))
                        await CallDropzoneMember();
                }
            }

            try
            {
                var userDto = Mapper.Map<UpdateUserAccountInfoDTO>(Educator.User);

                var userResponse = AuthService.UpdateUserAccount((long)Educator.UserId, userDto);
                var dto = Mapper.Map<EducatorDTO>(Educator);
                //dto.EducatorAdministrativeTitles = new();
                //if (_selectedAdministrativeTitles?.Count > 0)
                //    foreach (var item in _selectedAdministrativeTitles)
                //    {
                //        dto.EducatorAdministrativeTitles.Add(new EducatorAdministrativeTitleDTO { AdministrativeTitleId = item.Id, EducatorId = Educator.Id });
                //    }

                var response = await EducatorService.Update((long)Educator.Id, dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                    if (EducatorIdFromAdding > 0)
                        NavigationManager.NavigateTo($"/educator/educators/{Educator.Id}");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            _loading = false;
            StateHasChanged();
        }
        private async Task<IEnumerable<TitleResponseDTO>> SearchTitle(TitleType titleType, string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", searchQuery, "and").Filter("TitleType", "eq", titleType, "and").Sort("Name");

            var result = await TitleService.GetPaginateList(filter);

            return result.Items ?? new List<TitleResponseDTO>();
        }

        private async Task<IEnumerable<EducatorAdministrativeTitleResponseDTO>> SearchAdministrativeTitle(TitleType titleType, string searchQuery)
        {
            var filter = FilterHelper.CreateFilter(1, int.MaxValue).Filter("Name", "contains", searchQuery, "and").Filter("TitleType", "eq", titleType, "and").Sort("Name");

            var result = await TitleService.GetPaginateList(filter);
            List<EducatorAdministrativeTitleResponseDTO> educatorAdministrativeTitles = new();

            result.Items.ForEach(x =>
                educatorAdministrativeTitles.Add(new EducatorAdministrativeTitleResponseDTO() { AdministrativeTitle = x, AdministrativeTitleId = x.Id }));

            return educatorAdministrativeTitles ?? new List<EducatorAdministrativeTitleResponseDTO>();
        }
        private void OnChangeTitle(TitleType titleType, TitleResponseDTO titleResponse)
        {
            if (titleType == TitleType.Academic)
            {
                Educator.AcademicTitle = titleResponse;
                Educator.AcademicTitleId = titleResponse.Id;
            }
            else if (titleType == TitleType.Staff)
            {
                Educator.StaffTitle = titleResponse;
                Educator.StaffTitleId = titleResponse.Id;
            }
        }

        //private void OnChangeAdministrativeTitle(IList<TitleResponseDTO> values)
        //{
        //    _selectedAdministrativeTitles = values.ToList();
        //}

        private void OnChangeNewBranch(long? id, EducatorExpertiseBranchResponseDTO edx)
        {
            edx.ExpertiseBranchId = id;
            edx.ExpertiseBranch = allExpBranches.FirstOrDefault(x => x.Id == id);
            edx.SubBranchIds = allExpBranches.FirstOrDefault(x => x.Id == edx.ExpertiseBranchId).SubBranches?.Select(x => (long)x.SubBranchId)?.ToList();

            if (edx.SubBranchIds?.Count > 0)
                foreach (var subId in edx.SubBranchIds)
                    if (!expertiseBranches.Select(x => x.Id).Contains(subId))
                        expertiseBranches.Add(allExpBranches.FirstOrDefault(x => x.Id == subId));
            StateHasChanged();
        }

        private void OnChangeProgram(ProgramResponseDTO program)
        {
            _educatorProgram.Program = program;
            _educatorProgram.ProgramId = program?.Id;
            StateHasChanged();
        }

        private async Task<IEnumerable<ProgramResponseDTO>> SearchPrograms(string searchQuery)
        {
            string[] searchParams = searchQuery.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            List<Filter> filterList = new List<Filter>();

            foreach (var item in searchParams)
            {
                var filter0 = new Filter()
                {
                    Field = "Faculty.University.Name",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter0);
                var filter1 = new Filter()
                {
                    Field = "Hospital.Province.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter1);
                var filter2 = new Filter()
                {
                    Field = "ExpertiseBranch.Profession.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter2);
                var filter3 = new Filter()
                {
                    Field = "Hospital.Name",
                    Operator = "contains",
                    Value = item,
                };
                filterList.Add(filter3);
                var filter4 = new Filter()
                {
                    Field = "ExpertiseBranch.Name",
                    Operator = "contains",
                    Value = item
                };
                filterList.Add(filter4);
            }

            var finalResult = new PaginationModel<ProgramResponseDTO>() { Items = new List<ProgramResponseDTO>() };
            var expBrIdList = Educator.EducatorExpertiseBranches?.Select(x => x.ExpertiseBranchId)?.ToList();
            var hospitalId = Educator.EducatorPrograms?.FirstOrDefault(x => x.DutyEndDate == null)?.Program?.HospitalId;

            if (Educator.EducatorPrograms != null && Educator.EducatorPrograms?.Count > 0)
            {
                foreach (var item in Educator.EducatorPrograms)
                {
                    if (Educator.EducatorExpertiseBranches?.Count > 0)
                    {
                        foreach (var eduExpBr in Educator.EducatorExpertiseBranches)
                        {
                            if (eduExpBr.ExpertiseBranch.IsPrincipal == item.Program.ExpertiseBranch.IsPrincipal && item.DutyEndDate == null)
                            {
                                expBrIdList.Remove(eduExpBr.ExpertiseBranchId);
                            }
                        }
                    }
                }
            }
            foreach (var item in expBrIdList)
            {
                if (hospitalId != null)
                {
                    var result = await ProgramService.GetByHospitalAndExpertiseBranchId((long)hospitalId, (long)item);
                    if (result.Item != null)
                    {
                        var result_1 = result.Item.Name.ToLower().Contains(searchQuery.ToLower());
                        if (result_1)
                            finalResult.Items.Add(result.Item);
                    }
                }
                else
                {
                    var result = await ProgramService.GetListForSearchByExpertiseBranchId(new FilterDTO()
                    {
                        Filter = new Filter()
                        {
                            Logic = "or",
                            Filters = filterList
                        },
                        page = 1,
                        pageSize = int.MaxValue
                    }, (long)item, true);

                    if (result.Items != null)
                        foreach (var item_1 in result.Items)
                            finalResult.Items.Add(item_1);
                }
            }

            return finalResult.Items ?? new List<ProgramResponseDTO>();

        }

        private void OpenUploadModal(EducatorProgramResponseDTO edp)
        {
            _selectedEducatorProgram = new();
            _selectedEducatorProgram = edp;
            dropzone.ResetStatus();

            StateHasChanged();
            UploaderModal.OpenModal();
        }
        public async Task<bool> CallDropzone()
        {
            _educatorProgram.Documents ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzone.SubmitFileAsync();
                if (result.Result)
                {
                    if (dropzone.DocumentType == DocumentTypes.AssociateProfessorship)
                        Educator.Documents.Add(result.Item);
                    else
                        _educatorProgram.Documents.Add(result.Item);
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
                dropzone.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneProgram()
        {
            _educatorProgram.Documents ??= new();
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneProgram.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorProgram.Documents.Add(result.Item);
                    _educatorProgram.DocumentOrder = result.Item.BucketKey;
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
                dropzoneProgram.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneOfficer()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneOfficer.SubmitFileAsync();
                if (result.Result)
                {
                    _educatorProgram.EducationOfficerDocuments.Add(result.Item);
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
                dropzoneOfficer.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneDeclaration()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneDeclaration.SubmitFileAsync();
                if (result.Result)
                {
                    Educator.Documents.Add(result.Item);
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
                dropzoneDeclaration.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneChairman()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneChairman.SubmitFileAsync();
                if (result.Result)
                {
                    Educator.Documents.Add(result.Item);
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
                dropzoneChairman.ResetStatus();
                StateHasChanged();
            }
        }

        public async Task<bool> CallDropzoneMember()
        {
            _fileLoaded = false;
            StateHasChanged();
            try
            {
                var result = await dropzoneMember.SubmitFileAsync();
                if (result.Result)
                {
                    Educator.Documents.Add(result.Item);
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
                dropzoneMember.ResetStatus();
                StateHasChanged();
            }
        }

        private async Task SearchUniversities()
        {
            var universityResponse = await UniversityService.GetAll();
            _universities = universityResponse.Item;
            StateHasChanged();
        }

        //private void OnChangeInstitution(InstitutionResponseDTO institution)
        //{
        //    Educator.User.Institution = institution;
        //    Educator.User.InstitutionId = institution?.Id;
        //}

        private async Task SearchHospitals()
        {
            var hospitalResponse = await HospitalService.GetAll();
            _hospitals = hospitalResponse.Item;
        }

        //private void OnChangeHospital(HospitalResponseDTO hospital)
        //{
        //    Educator.StaffHospital = hospital;
        //    Educator.StaffHospitalId = hospital?.Id ?? 0;
        //}

        //private void OnChangeUniversity(UniversityResponseDTO university)
        //{
        //    Educator.StaffParentInstitution = university;
        //    Educator.StaffParentInstitutionId = university?.Id ?? 0;
        //}

        private async Task RemoveExpBranch(EducatorExpertiseBranchResponseDTO edx)
        {
            if (edx.EducatorId != null)
            {
                await EducatorExpertiseBranchService.RemoveEducatorExpBranch((long)edx.Id);
            }
            _selectedEducatorExpertiseBranches.Remove(edx);
            StateHasChanged();
        }
        private async void OnOpenUpdateModal(EducatorProgramResponseDTO educatorProgram)
        {
            var response = await EducatorProgramService.GetById(educatorProgram.Id);
            if (response.Result)
            {
                _educatorProgram = response.Item;
                _educatorProgram.LastDutyEndDate = Educator.EducatorPrograms.MaxBy(x => x.DutyEndDate)?.DutyEndDate;
                _educatorProgram.LastDutyEndDate.PrintJson("lastDate");
                _educatorProgram.Documents ??= new();
                _educatorProgram.EducationOfficerDocuments ??= new();
                StateHasChanged();
                _educatorProgramModal.OpenModal();
            }
            else
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L[response.Message]);
            }

            //_documentValidatorMessage = string.Empty;
            //_declarationDocumentValidatorMessage = string.Empty;
            ////_educatorProgram = educatorProgram;
            //_educationOfficer = Educator.EducationOfficers.FirstOrDefault(x => x.ProgramId == educatorProgram.ProgramId && x.EndDate == null);
            ////_educatorProgram.IsEducationOfficer = _educationOfficer == null ? false : true;
            //_oldIsEducationOfficer = (bool)_educatorProgram.IsEducationOfficer.Clone();
            _ecForProgram = new EditContext(_educatorProgram);
            _educatorProgramModal.OpenModal();
            //IsEditing = true;
        }

        private void OnOpenEducatorProgramAddModal()
        {
            if (Educator.EducatorExpertiseBranches == null || Educator.EducatorExpertiseBranches?.Count == 0 || (Educator.EducatorExpertiseBranches.Count == 1 && Educator.EducatorExpertiseBranches.FirstOrDefault().ExpertiseBranchId == null))
            {
                SweetAlert.IconAlert(SweetAlertIcon.question, $"{L["Warning"]}",
                $"{L["To add program, you have to select an expertise branch."]}");
                return;
            }
            _educatorProgram = new() { Documents = new(), EducationOfficerDocuments = new(), EducatorId = Educator.Id };
            _ecForProgram = new EditContext(_educatorProgram);
            _educatorProgramModal.OpenModal();
            //IsEditing = false;
        }

        private void OnOpenEducatorStaffParentInstitutionAddModal()
        {
            _educatorStaffParentInstitution = new();
            _ecForProgram = new EditContext(_educatorStaffParentInstitution);
            _educatorStaffParentInstitutionModal.OpenModal();
            //IsEditing = false;
        }

        private async Task OnDownloadHandler(EducatorProgramResponseDTO educatorProgram)
        {
            try
            {
                educatorProgram.Id.PrintJson("id : ");
                var response = await DocumentService.GetListByTypeByEntity(educatorProgram.Id.Value, DocumentTypes.PlaceOfDuty);
                ResponseWrapper<List<DocumentResponseDTO>> response_2 = new();
                var officer = Educator.EducationOfficers.FirstOrDefault(x => x.ProgramId == educatorProgram.ProgramId);
                if (officer != null)
                    response_2 = await DocumentService.GetListByTypeByEntity((long)officer.Id, DocumentTypes.EducationOfficerAssignmentLetter);
                if (response_2.Item != null)
                {
                    foreach (var item in response_2?.Item)
                    {
                        response.Item.Add(item);
                    }
                }

                if (response.Result)
                {
                    Documents = response.Item;
                    FileModal.OpenModal();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            StateHasChanged();
        }

        private async Task OnRemoveEducatorExpertiseBranch(EducatorExpertiseBranchResponseDTO edx)
        {
            List<EducatorExpertiseBranchResponseDTO> afterRemovedlist = Educator.EducatorExpertiseBranches.Clone();
            afterRemovedlist.Remove(afterRemovedlist.FirstOrDefault(x => x.ExpertiseBranchId == edx.ExpertiseBranchId));
            if (Educator.EducatorPrograms.Any(x => x.Program.ExpertiseBranchId == edx.ExpertiseBranchId))
            {
                SweetAlert.IconAlert(SweetAlertIcon.warning, $"{L["Warning"]}",
                $"{L["You have to delete the related program record first, to delete this record."]}");
                return;
            }

            if (edx.SubBranchIds?.Count > 0)
            {
                foreach (var subBranchId in edx.SubBranchIds)
                {
                    if (Educator.EducatorExpertiseBranches.Any(x => x.ExpertiseBranchId == subBranchId) && !afterRemovedlist.SelectMany(x => x.SubBranchIds).Contains(subBranchId))
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.warning, $"{L["Warning"]}",
                    $"{L["You have to delete the sub branches records first, to delete this record."]}");
                        return;
                    }
                }
                foreach (var item in edx.SubBranchIds)
                {
                    if (!afterRemovedlist.SelectMany(x => x.SubBranchIds).Distinct().Contains(item))
                        expertiseBranches.Remove(allExpBranches.FirstOrDefault(x => x.Id == item));
                }
            }

            Educator.EducatorExpertiseBranches.Remove(edx);

            _ecUpdate = new EditContext(Educator);
            StateHasChanged();
        }

        private async Task OnRemoveEducatorProgram(EducatorProgramResponseDTO educatorProgram)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Delete"]}", $"{L["Cancel"]}");
            if (confirm == true)
            {
                var response = await EducatorProgramService.Delete(educatorProgram.Id ?? 0);

                if (response.Result)
                {
                    Educator.EducatorPrograms.Remove(educatorProgram);
                    SweetAlert.IconAlert(SweetAlertIcon.success, L["Successfull"], L["Successfully Deleted"]);
                }
                else
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Successfull"], L[response.Message]);
                }

                //Educator.EducatorPrograms.Remove(educatorProgram);
                //var officer = Educator.EducationOfficers.FirstOrDefault(x => x.ProgramId == educatorProgram.ProgramId && x.EndDate == null);
                //if (officer != null)
                //{
                //    officer.EndDate = DateTime.UtcNow;
                //}
                //StateHasChanged();
                //SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["Use the 'Update' button to save the changes."]);
            }
        }

        private async Task AddEducatorProgram()
        {
            if (!_ecForProgram.Validate())
                return;

            if (_educatorProgram.IsEducationOfficer == true && _educatorProgram.EducationOfficerDocuments?.Count < 1)
            {
                var dropzoneResponse = await CallDropzoneOfficer();
                if (!dropzoneResponse)
                    return;
            }

            if (!string.IsNullOrEmpty(dropzoneProgram._selectedFileName))
                await CallDropzoneProgram();

            var response = new ResponseWrapper<EducatorProgramResponseDTO>();
            var mappedDTO = Mapper.Map<EducatorProgramDTO>(_educatorProgram);
            if (_educatorProgram?.Id != null)
                response = await EducatorProgramService.Update(_educatorProgram.Id ?? 0, mappedDTO);
            else
            {
                response = await EducatorProgramService.Add(mappedDTO);
                if (response.Result)
                    Educator.EducatorPrograms.Add(response.Item);
            }
            _educatorProgramModal.CloseModal();
        }

        private async Task SaveEducatorProgram()
        {
            _documentValidatorMessage = string.Empty;
            _declarationDocumentValidatorMessage = string.Empty;
            _educationOfficerDocumentValidatorMessage = string.Empty;
            _programStartDateValidatorMessage = String.Empty;
            _educatorProgram.Documents ??= new List<DocumentResponseDTO>();
            if (!_ecForProgram.Validate())
                return;

            //if (!IsEditing)
            //{
            //    var lastDate = Educator.EducatorPrograms?.OrderByDescending(x => x.DutyEndDate)?.FirstOrDefault(x => x.Program.ExpertiseBranchId == _educatorProgram.Program.ExpertiseBranchId && x.DutyEndDate != null)?.DutyEndDate;
            //    if (lastDate != null && _educatorProgram.DutyStartDate < lastDate)
            //    {
            //        _programStartDateValidatorMessage = L["Dates don't match!"];
            //        return;
            //    }
            //}

            if (!string.IsNullOrEmpty(dropzoneProgram._selectedFileName))
                await CallDropzoneProgram();


            //if (_oldIsEducationOfficer == false && _educatorProgram.IsEducationOfficer == true)
            //{
            //    _educationOfficer = new EducationOfficerResponseDTO { StartDate = IsEditing == false ? _educatorProgram.DutyStartDate : DateTime.UtcNow, ProgramId = _educatorProgram.Program.Id, DocumentOrder = Guid.NewGuid().ToString(), EducatorId = Educator.Id };

            //    if (string.IsNullOrEmpty(dropzoneOfficer._selectedFileName))
            //    {
            //        _educationOfficerDocumentValidatorMessage = L["This field is required"];
            //        _educationOfficer = new();
            //        return;
            //    }
            //    else
            //        await CallDropzoneOfficer();
            //    Educator.EducationOfficers.Add(_educationOfficer);
            //}
            //else if (_oldIsEducationOfficer == true && _educatorProgram.IsEducationOfficer == true)
            //{
            //    if (string.IsNullOrEmpty(dropzoneOfficer._selectedFileName) && _educationOfficer.Documents == null)
            //    {
            //        _educationOfficerDocumentValidatorMessage = L["This field is required"];
            //        return;
            //    }
            //    else if (!string.IsNullOrEmpty(dropzoneOfficer._selectedFileName))
            //        await CallDropzoneOfficer();
            //}
            //else if (_oldIsEducationOfficer == true && _educatorProgram.IsEducationOfficer == false)
            //{
            //    _educationOfficer.EndDate = DateTime.UtcNow;
            //    _educationOfficer = null;
            //}

            StateHasChanged();

            _educatorProgram.EducatorId = Educator.Id;
            _educatorProgram.ProgramId = _educatorProgram.Program.Id;

            //if (!IsEditing)
            //{
            //    Educator.EducatorPrograms.Add(_educatorProgram);
            //}

            _educatorProgramModal.CloseModal();

            StateHasChanged();
        }

        //private void TitleDateChange(DateTime? dt)
        //{
        //    Educator.TitleDate = dt;
        //    if (dt != null)
        //    {
        //        _titleDateValidatorMessage = string.Empty;
        //        StateHasChanged();
        //    }
        //    else if (dt == null && Educator.IsConditionalEducator == true)
        //    {
        //        _titleDateValidatorMessage = L["Bu alan zorunludur!"];
        //        StateHasChanged();
        //    }
        //}

        private void StartDateChange(DateTime? dt)
        {
            _educatorProgram.DutyStartDate = dt;
        }

        private void EndDateChange(DateTime? dt)
        {
            _educatorProgram.DutyEndDate = dt;
        }

        //async Task ConfirmInternalNavigation(LocationChangingContext locationChanging)
        //{
        //    if (_ecUpdate.IsModified())
        //    {
        //        var isConfirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Kaydedilmeyen alanlar silinsin mi?");
        //        if (!isConfirmed)
        //        {
        //            locationChanging.PreventNavigation();
        //        }
        //    }
        //}             

        private async Task GetUserInfoById()
        {
            _loadingUserInfo = true;
            StateHasChanged();
            try
            {
                var response = await UserService.GetUserInfoById((long)Educator.UserId);
                if (response.Item != null)
                {
                    //Educator.User.Address = response.Item.AddressInfo.Address;
                    SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Fetched"]);

                    if (Educator.User.Name != response.Item.Name + " " + response.Item.Surname)
                    {
                        Educator.User.Name = response.Item.Name + " " + response.Item.Surname;
                        SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["Use the 'Update' button to save the changes that occur as a result of the query."]);
                    }
                }
            }
            catch (Exception e)
            {
                SweetAlert.ErrorAlert();
                throw;
            }
            finally
            {
                _loadingUserInfo = false;
                StateHasChanged();
            }
        }

        private async Task GetGraduationInfoById()
        {
            _loadingGraduationInfo = true;
            StateHasChanged();
            try
            {
                var response = await UserService.GetGraduationInfoById((long)Educator.UserId);
                if (response.Result)
                {
                    if (response.Item?.Count == 0)
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["The graduation information of the person is not registered in YÖKSİS. The person should register the graduation information with YÖKSİS by contacting the central unit responsible for domestic student affairs of the university/universities from which he/she graduated. For international graduations, the person should contact the YÖK Equivalency Unit."]);
                    }
                    else
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Fetched"]);

                        Educator.GraduationDetails = response.Item;
                        SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["Use the 'Update' button to save the changes that occur as a result of the query."]);
                    }
                }
                else
                    SweetAlert.ErrorAlert();
            }
            catch (Exception e)
            {
                SweetAlert.ErrorAlert();
                throw;
            }
            finally
            {
                _loadingGraduationInfo = false;
                StateHasChanged();
            }
        }

        private async Task GetCKYSInfoById()
        {
            _loadingCKYSInfo = true;
            StateHasChanged();
            try
            {
                var responseAcademic = await UserService.GetAcademicInfoById((long)Educator.UserId);
                var response = await UserService.GetCKYSInfoById((long)Educator.UserId);

                if (!response.Result || !responseAcademic.Result)
                {
                    SweetAlert.ErrorAlert(L["There are no academic information of the person you query!"]);
                    return;
                }
                else
                {
                    if (responseAcademic.Item != null)
                    {
                        if (responseAcademic.Item.AcademicTitleId != null && responseAcademic.Item.AcademicTitleId != Educator.AcademicTitleId)
                        {
                            Educator.AcademicTitle.Id = responseAcademic.Item.AcademicTitleId;
                            Educator.AcademicTitle.Name = responseAcademic.Item.AcademicTitleName;
                        }
                    }

                    if (response.Item != null)
                    {
                        if (response.Item.StaffTitleId != null && response.Item.StaffTitleId != Educator.StaffTitleId)
                        {
                            Educator.StaffTitle.Id = response.Item.StaffTitleId;
                            Educator.StaffTitle.Name = response.Item.StaffTitleName;
                        }

                        foreach (var item in response.Item.DoctorExpertiseBranches)
                        {
                            if (!Educator.EducatorExpertiseBranches.Any(x => x.ExpertiseBranchId == item.ExpertiseBranchId))
                            {
                                if (!Educator.EducatorExpertiseBranches.Any(x => x.RegistrationBranchName == item.RegistrationBranchName))
                                {
                                    Educator.EducatorExpertiseBranches.Add(new EducatorExpertiseBranchResponseDTO()
                                    {
                                        ExpertiseBranchName = item.ExpertiseBranchName,
                                        RegistrationBranchName = item.RegistrationBranchName,
                                        RegistrationDate = item.RegistrationDate != "" ? DateTime.ParseExact(item.RegistrationDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) : null,
                                        RegistrationGraduationSchool = item.RegistrationGraduationSchool,
                                        ExpertiseBranchId = item.ExpertiseBranchId,
                                        RegistrationNo = item.RegistrationNo,
                                        SubBranchIds = item.SubBrIds
                                    });
                                }
                            }
                            else
                            {
                                var eduExpBr = Educator.EducatorExpertiseBranches.FirstOrDefault(x => x.ExpertiseBranchId == item.ExpertiseBranchId);
                                eduExpBr.RegistrationBranchName = item.RegistrationBranchName;
                                eduExpBr.RegistrationGraduationSchool = item.RegistrationGraduationSchool;
                                eduExpBr.RegistrationDate = item.RegistrationDate != "" ? DateTime.ParseExact(item.RegistrationDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) : null;
                                eduExpBr.RegistrationNo = item.RegistrationNo;
                            }
                        }
                    }

                    SweetAlert.IconAlert(SweetAlertIcon.warning, L["Warning"], L["Use the 'Update' button to save the changes that occur as a result of the query."]);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ErrorAlert();
                throw;
            }
            finally
            {
                _loadingCKYSInfo = false;
                StateHasChanged();
            }
        }

        private bool IsLastRecord(EducatorProgramResponseDTO educatorProgram)
        {
            var orderedList = Educator.EducatorPrograms.OrderByDescending(x => x.DutyStartDate).ToList();

            return orderedList.FirstOrDefault().Id == educatorProgram.Id;
        }

        private bool IsLastRecordForParentInstitution(EducatorStaffParentInstitutionResponseDTO educatorStaffParentInstitution)
        {
            var orderedList = Educator.StaffParentInstitutions.OrderByDescending(x => x.StartDate).ToList();
            return orderedList.FirstOrDefault().Id == educatorStaffParentInstitution.Id;
        }

    }
}