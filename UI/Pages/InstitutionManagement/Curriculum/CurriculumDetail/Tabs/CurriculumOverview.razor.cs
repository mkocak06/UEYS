using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using UI.Helper;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Tabs
{
    public partial class CurriculumOverview
    {
        [Parameter] public bool IsEditing { get; set; }
        [Inject] public IState<CurriculumDetailState> CurriculumState { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IProfessionService ProfessionService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private CurriculumResponseDTO Curriculum => CurriculumState.Value.Curriculum;
        private CurriculumResponseDTO CurriculumAdd;
        private EditContext _ecUpdate, _ecAdd;
        private bool _loading;
        private bool _isactive;
        private ProfessionResponseDTO _profession = new();
        private int? _duration;
        protected override void OnInitialized()
        {
            _isactive = false;
            _ecUpdate = new EditContext(new CurriculumResponseDTO());
            CurriculumAdd = new CurriculumResponseDTO();
            CurriculumAdd.ExpertiseBranch = new ExpertiseBranchResponseDTO();
            _ecAdd = new EditContext(CurriculumAdd);
            _profession = null;
            _duration = Curriculum?.Duration;

            if (IsEditing)
            {
                SubscribeToAction<CurriculumDetailSetAction>(action =>
                {
                    _ecUpdate = new EditContext(action.Curriculum);
                });
                _profession = Curriculum?.ExpertiseBranch?.Profession;
            }
            base.OnInitialized();
        }
        private async Task Save()
        {
            if (IsEditing)
            {
                if (!_ecUpdate.Validate()) return;
            }
            else
            {
                if (!_ecAdd.Validate()) return;
            }

            try
            {

                if (IsEditing)
                {
                    if (_duration != Curriculum.Duration)
                    {
                        var _confirmation = await SweetAlert.ConfirmAlert("Süre Değişikliği", "Süre değişikliği yapmanız durumunda öğrencilerin süreleri etkilenecektir. Bilginize.", SweetAlertIcon.warning, false, "Onayla", "İptal Et");
                        if (!_confirmation)
                        {
                            return;
                        }
                    }

                    _loading = true;
                    StateHasChanged();

                    var dtoUpdate = Mapper.Map<CurriculumDTO>(Curriculum);
                    var response = await CurriculumService.Update((long)Curriculum.Id, dtoUpdate);
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Updated!"]);
                        Dispatcher.Dispatch(new CurriculumDetailSetAction(response.Item));
                    }
                    else
                        SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured"]);
                }
                else
                {
                    _loading = true;
                    StateHasChanged();

                    var dtoAdd = Mapper.Map<CurriculumDTO>(CurriculumAdd);
                    var response = await CurriculumService.Add(dtoAdd);
                    if (response.Result)
                    {
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Successfully Added"]);
                        Dispatcher.Dispatch(new CurriculumClearStateAction());
                        NavigationManager.NavigateTo($"/institution-management/curriculums/{response.Item.Id}");
                    }
                    else
                        SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured"]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            _loading = false;
            StateHasChanged();
        }
        private async Task CreateCopy()
        {
            if (!_ecUpdate.Validate()) return;
            var confirm = await SweetAlert.ConfirmAlert("Müfredat Kopyası", "Onayladığınız takdirde aynı müfredatın kopyası oluşturulacaktır.", SweetAlertIcon.warning, true, "Onayla", "Reddet"); // TODO: İngilizce dil desteği eklenecek
            if (confirm)
            {

                try
                {
                    _loading = true;
                    StateHasChanged();

                    var dtoUpdate = Mapper.Map<CurriculumDTO>(Curriculum);
                    var response = await CurriculumService.CreateCopy((long)Curriculum.Id, dtoUpdate);
                    if (response.Result)
                    {
                        SweetAlert.IconAlert(SweetAlertIcon.success, "", L["Copy of curriculum has been successfully created."]);
                        NavigationManager.NavigateTo($"/institution-management/curriculums/{response.Item.Id}");
                    }
                    else
                        SweetAlert.ToastAlert(SweetAlertIcon.error, L["An Error Occured"]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task<IEnumerable<ProfessionResponseDTO>> SearchEducationFields(string searchQuery)
        {
            var result = await ProfessionService.GetAll();
            return result.Result ? result.Item.Where(x => x.Name.ToLower(CultureInfo.CurrentCulture).Contains(searchQuery.ToLower(CultureInfo.CurrentCulture))).Take(10) :
                new List<ProfessionResponseDTO>();
        }
        private void OnChangeEducationField(ProfessionResponseDTO profession)
        {
            if (IsEditing)
            {
                _profession = profession;
                Curriculum.ExpertiseBranch = null;
            }
            else
            {
                _profession = profession;
                CurriculumAdd.ExpertiseBranch = null;
            }

            StateHasChanged();
        }
        private async Task<IEnumerable<ExpertiseBranchResponseDTO>> SearchExpertiseBranches(string searchQuery)
        {
            var response = await ExpertiseBranchService.GetPaginateList(FilterHelper.CreateFilter(1, int.MaxValue)
                                                                                    .Sort("Name", Shared.Types.SortType.ASC)
                                                                                    .Filter("ProfessionId", "eq", _profession?.Id ?? 0, "and")
                                                                                    .Filter("Name", "contains", searchQuery, "and"));
            return response.Items;
        }
        private void OnChangeExpertiseBranch(ExpertiseBranchResponseDTO expertiseBranch)
        {
            if (IsEditing)
            {
                Curriculum.ExpertiseBranch = expertiseBranch;
                Curriculum.ExpertiseBranchId = expertiseBranch?.Id;
            }
            else
            {
                CurriculumAdd.ExpertiseBranch = expertiseBranch;
                CurriculumAdd.ExpertiseBranchId = expertiseBranch?.Id;
            }

            StateHasChanged();
        }
        private void OnChangeIsActive1()
        {
            if (_isactive == false)
            {
                CurriculumAdd.IsActive = _isactive;
                StateHasChanged();
            }
        }
        private void OnChangeIsActive2()
        {
            if (_isactive == false)
            {
                CurriculumAdd.IsActive = !_isactive;
                StateHasChanged();
            }
        }
        private void OnChangeIsActiveUpdate()
        {
            Curriculum.IsActive = !Curriculum.IsActive;

            StateHasChanged();
        }
        private void OnChangeDuration(ChangeEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Value.ToString()))
                Curriculum.Duration = 0;
            else
                Curriculum.Duration = Convert.ToInt32(args.Value.ToString());

            StateHasChanged();
        }
    }
}