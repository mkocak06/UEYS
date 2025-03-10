using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.Extensions;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Tabs

{

    public partial class CurriculumPerfections
    {
        [Parameter] public long CurriculumId { get; set; }
        [Parameter] public long RotationId { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IPerfectionService PerfectionService { get; set; }
        [Inject] public ICurriculumPerfectionService CurriculumPerfectionService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IPropertyService PropertyService { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IState<CurriculumDetailState> CurriculumDetailState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private List<PerfectionResponseDTO> _perfections;
        private PerfectionResponseDTO _perfection;
        private CurriculumPerfectionResponseDTO _curriculumPerfection;
        private FilterDTO _filter;
        private PaginationModel<PerfectionResponseDTO> _paginationModelPerfections;
        private bool _loading;
        private List<BreadCrumbLink> _links;
        private bool _loaded;
        private bool _saving;
        private StatusType _isActive;
        private EditContext _ec;
        private EditContext _perfectionEc;
        private MyModal _perfectionAddModal;
        private MyModal _perfectionDetailModal;
        private IList<string>? _selectedMethods;
        private IList<string>? _beenMethods;

        private bool Loaded => CurriculumDetailState.Value.CurriculumsLoaded;
        private bool forceRender;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isActive = StatusType.Passive;
            _perfection = new PerfectionResponseDTO()
            {
                MethodList = new(),
                LevelList = new(),
                Group = new(),
                PName = new(),
                Seniorty = new(),
            };
            _perfectionEc = new EditContext(_perfection);

            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                    {
                        new Filter()
                        {
                            Field="CurriculumId",
                            Operator="eq",
                            Value=CurriculumId
                        },
                        new Filter()
                        {
                            Field="IsDeleted",
                            Operator="eq",
                            Value=false
                        }
                    },
                    Logic = "and"

                },
                page = 1,
                pageSize = 10,

                //Sort = new[]      // TODO
                //{
                //    new Sort()
                //    {
                //        Dir = SortType.ASC,
                //        Field = "Name"
                //    }
                //}
            };
            if (RotationId != 0)
            {
                _filter.Filter.Filters.Add(new Filter()
                {
                    Field = "RotationId",
                    Operator = "eq",
                    Value = RotationId
                });
            }
            SubscribeToAction<CurriculumDetailSetAction>((action) =>
            {
                forceRender = true;
            });

            await GetPerfections();
        }


        private async Task GetPerfections()
        {
            _loading = true;
            StateHasChanged();
            _paginationModelPerfections = await PerfectionService.GetPaginateList(_filter);
            if (_paginationModelPerfections != null)
            {
                _perfections = _paginationModelPerfections.Items;
            }
            else
            {
                SweetAlert.ErrorAlert();
            }

            _loading = false;
            StateHasChanged();
        }

        private async Task OnSortChange(Sort sort)
        {
            _filter.Sort = new[] { sort };
            await GetPerfections();
        }



        private void OnPerfectionAddList()
        {
            _perfection = new PerfectionResponseDTO();
            _perfectionEc = new EditContext(_perfection);
            _perfectionAddModal.OpenModal();
        }
        private async Task OnPerfectionDetail(PerfectionResponseDTO perfectionResponse)
        {
            if (perfectionResponse.Id != null)
            {
                var response = await PerfectionService.GetByIdAsync((long)perfectionResponse.Id);
                if (response.Result && response.Item != null)
                {
                    _perfectionAddModal.OpenModal();
                    _perfection = response.Item;
                    _perfectionEc = new EditContext(_perfection);
                    _loaded = true;
                    StateHasChanged();
                }
            }
        }
        private async Task AddPerfection()
        {

            //for (int i = 0; i < _selectedMethods.Count; i++)
            //{
            //    if(_perfection.PerfectionMethod!=null)
            //        _perfection.PerfectionMethod = String.Join(",", _perfection.PerfectionMethod, _selectedMethods[i]);
            //    else
            //    {
            //        _perfection.PerfectionMethod = _selectedMethods[i];
            //    }
            //}
            if (!_perfectionEc.Validate()) return;

            _saving = true;
            StateHasChanged();
            if (RotationId != 0)
            {
                var dto = Mapper.Map<PerfectionDTO>(_perfection);
                dto.CurriculumRotationId = RotationId;
                try
                {
                    var response = await PerfectionService.PostRotationPerfection(CurriculumId, RotationId, dto);

                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                        _perfections.Add(_perfection);
                        _perfectionAddModal.CloseModal();

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
            }
            else
            {
                var dto = new CurriculumPerfectionDTO
                {
                    CurriculumId = CurriculumId,
                    Perfection = Mapper.Map<PerfectionDTO>(_perfection),
                    PerfectionId = _perfection.Id
                };

                try
                {
                    var response = await CurriculumPerfectionService.PostAsync(dto);

                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                        _perfections.Add(_perfection);
                        _perfectionAddModal.CloseModal();

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
            }
            _saving = false;
            StateHasChanged();
            await GetPerfections();
        }

        private async Task UpdatePerfection()
        {

            if (!_perfectionEc.Validate()) return;
            _saving = true;
            StateHasChanged();
            if (RotationId != 0)
            {
                var dto = Mapper.Map<PerfectionDTO>(_perfection);

                try
                {
                    var response = await PerfectionService.Put((long)_perfection.Id, dto);
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");
                        
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
                    await GetPerfections();
                    _perfectionAddModal.CloseModal();
                }
            }
            else
            {
                var dto = new CurriculumPerfectionDTO
                {
                    CurriculumId = CurriculumId,
                    Perfection = Mapper.Map<PerfectionDTO>(_perfection),
                    PerfectionId = _perfection.Id
                };
                try
                {

                    var response = await CurriculumPerfectionService.Put(CurriculumId, dto);
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Updated!"]}");

                        await GetPerfections();

                        _perfectionAddModal.CloseModal();

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
            }
            _saving = false;
            StateHasChanged();
            await GetPerfections();
        }
        private async Task OnDeleteHandler(PerfectionResponseDTO perfection)
        {
            var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
                $"{L["Are you sure you want to delete this item? This action cannot be undone."]}",
                SweetAlertIcon.question, true, $"{L["Make Passive"]}", $"{L["Cancel"]}");

            if (confirm)
            {
                try
                {
                    var currentPerfection = _paginationModelPerfections.Items.FirstOrDefault(x => x.Id == perfection.Id);
                    await PerfectionService.Delete((long)currentPerfection.Id);
                    _perfections.Remove(currentPerfection);
                    StateHasChanged();
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Deleted!"]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SweetAlert.ErrorAlert();
                    throw;
                }
            }
        }
        private async Task PaginationHandler(PaginationInfo val)
        {
            var (item1, item2) = (val.Page, val.PageSize);

            _filter.page = item1;
            _filter.pageSize = item2;

            await GetPerfections();
        }

        private async Task<IEnumerable<PropertyResponseDTO>> SearchName(string queryText)
        {
            var response = await PropertyService.GetByType(PropertyType.PerfectionName, _perfection.PerfectionType, queryText);
            return response.Result ? response.Item : new List<PropertyResponseDTO>();
        }
        private async Task<IEnumerable<PropertyResponseDTO>> SearchGroup(string queryText)
        {
            var response = await PropertyService.GetByType(PropertyType.PerfectionGroup, _perfection.PerfectionType, queryText);
            return response.Result ? response.Item : new List<PropertyResponseDTO>();
        }
        private async Task<IEnumerable<PropertyResponseDTO>> SearchSeniority(string queryText)
        {
            var response = await PropertyService.GetByType(PropertyType.PerfectionSeniorty, null, queryText);
            return response.Result ? response.Item : new List<PropertyResponseDTO>();
        }
        private async Task<IEnumerable<PropertyResponseDTO>> SearchLevel(string queryText)
        {
            var response = await PropertyService.GetByType(PropertyType.PerfectionLevel, null, queryText);
            return response.Result ? response.Item : new List<PropertyResponseDTO>();
        }
        private async Task<IEnumerable<PropertyResponseDTO>> SearchMethod(string queryText)
        {
            var response = await PropertyService.GetByType(PropertyType.PerfectionMethod, null, queryText);
            return response.Result ? response.Item : new List<PropertyResponseDTO>();
        }

        private void OnChangeLevels(IList<PropertyResponseDTO> values)
        {
            _perfection.LevelList = values.ToList();
        }

        private void OnChangeMethods(IList<PropertyResponseDTO> values)
        {
            _perfection.MethodList = values.ToList();
        }
        #region FilterChangeHandlers

        private async Task OnChangePerfectionTypeFilter(ChangeEventArgs args, string filterName)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            var filterValue = new PerfectionType?();

            if (PerfectionType.Clinical.GetDescription().ToLower().Contains(value.ToLower()))
            {
                Console.WriteLine(PerfectionType.Clinical.GetDescription());
                filterValue = PerfectionType.Clinical;
            }
            else if (PerfectionType.Interventional.GetDescription().ToLower().Contains(value.ToLower()))
            {
                filterValue = PerfectionType.Interventional;
            }
            else
            {
                filterValue = null;
            }

            var index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            if (index < 0)
            {
                _filter.Filter.Filters.Add(new Filter()
                {
                    Field = filterName,
                    Operator = "eq",
                    Value = filterValue
                });
            }
            else
            {
                _filter.Filter.Filters[index].Value = filterValue;
            }
            await GetPerfections();
        }

        private async Task OnChangeFilter(ChangeEventArgs args, string propertyType)
        {
            var value = (string)args.Value;
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            _filter.Filter.Filters.Add(new Filter()
            {
                Field = "PropertyType",
                Operator = propertyType,
                Value = value
            });

            await GetPerfections();
        }

        private async Task OnResetFilter(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";
            _filter.page = 1;

            Console.WriteLine(filterName);

            int index = 0;
            if (filterName == "PerfectionType")
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            }
            else
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Operator == filterName);
            }

            if (index >= 0)
            {
                _filter.Filter.Filters.RemoveAt(index);
                await JsRuntime.InvokeVoidAsync("clearInput", filterName);
                await GetPerfections();
            }
        }

        private bool IsFiltered(string filterName)
        {
            _filter.Filter ??= new Filter();
            _filter.Filter.Filters ??= new List<Filter>();
            _filter.Filter.Logic ??= "and";

            int index = 0;

            if (filterName == "PerfectionType")
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Field == filterName);
            }
            else
            {
                index = _filter.Filter.Filters.FindIndex(x => x.Operator == filterName);
            }
            return index >= 0;
        }
        #endregion
    }
}