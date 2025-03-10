using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.ComplementPrograms;

public partial class AddComplementProgram
{
    [Inject] public IProgramService ProgramService { get; set; }
    [Inject] public IUniversityService UniversityService { get; set; }
    [Inject] public IProtocolProgramService ProtocolProgramService { get; set; }
    [Inject] public IAuthorizationCategoryService AuthorizationCategoryService { get; set; }
    [Inject] public IDocumentService DocumentService { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public ISweetAlert SweetAlert { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    private ProtocolProgramResponseDTO _complementProgram = new();
    private EditContext _ec;
    private bool _saving;
    private List<BreadCrumbLink> _links;

    private Dropzone dropzone;
    private MyModal UploaderModal;
    private List<DocumentResponseDTO> responseDocuments = new();
    private bool _fileLoaded = true;
    private FilterDTO _filterProgram;

    protected override void OnInitialized()
    {
        _ec = new EditContext(_complementProgram);
        _filterProgram = new FilterDTO()
        {
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
                    To = "/institution-management/complement-programs",
                    OrderIndex = 1,
                    Title = L["Complement Programs"]
                },new BreadCrumbLink(){
                    IsActive = false,
                    OrderIndex = 2,
                    Title = @L["add_new", L["Complement Program"]]
                }
        };
        base.OnInitialized();
    }

    private async Task Save()
    {
        _complementProgram.Type = ProgramType.Complement;
        _ec.GetValidationMessages().PrintJson("validasyonlar");
        if (!_ec.Validate()) return;

        _saving = true;
        StateHasChanged();
        var dto = Mapper.Map<ProtocolProgramDTO>(_complementProgram);
        try
        {
            var response = await ProtocolProgramService.Add(dto);
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
                NavigationManager.NavigateTo($"./institution-management/complement-programs/{response.Item.Id}");
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

    private async Task<IEnumerable<ProgramResponseDTO>> SearchPrograms(string searchQuery)
    {
        string[] searchParams = searchQuery.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        List<Filter> filterList = new List<Filter>();
        filterList.Add(new() { Field = "ComplementProgram" });

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

    private void OnChangeProgram(ProgramResponseDTO program)
    {
        _complementProgram.ParentProgram = program;
        _complementProgram.ParentProgramId = program?.Id ?? 0;
    }
}