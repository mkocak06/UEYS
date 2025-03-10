using ApexCharts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.ResponseModels;
using Shared.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using UI.Helper;
using UI.SharedComponents.Components;
using UI.SharedComponents.DetailCards;
using UI.SharedComponents.Store;
using Shared.ResponseModels.Ekip;
using System.Linq;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Tabs
{
    public partial class Specialists
    {
        [Inject] public IState<ProgramDetailState> ProgramDetailState { get; set; }

        [Inject] ISpecialistDoctorService SpecialistDoctorService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }

        private List<PersonelResponseDTO> _specialists;
        private ProgramResponseDTO _program => ProgramDetailState.Value.Program.Program;

        private bool _loading;

        protected override async void OnInitialized()
        {
            _loading = true;
            StateHasChanged();

            await GetSpecialists();
            base.OnInitialized();
        }

        private async Task GetSpecialists()
        {
            try
            {
                var response = await SpecialistDoctorService.GetListByProgramId(_program.Id);

                if (response.Result) 
                {
                    _specialists = response.Item.OrderBy(x => x.ad).ToList();
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message);
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }
    }
}
