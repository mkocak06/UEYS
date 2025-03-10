using Fluxor;
using Microsoft.AspNetCore.Components;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.ResponseModels.ProtocolProgram;
using UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail.Tabs
{
    public partial class ProgramProtocol
    {


        [Inject] IState<UniversityDetailState> UniversityDetaiilState { get; set; }
        [Inject] IProtocolProgramService ProtocolProgramService { get; set; }
        [Inject] IProgramService ProgramService { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        private UniversityResponseDTO _university => UniversityDetaiilState.Value.University;

        private List<ProtocolProgramByUniversityIdResponseDTO> _programs;
        private bool _loading = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _loading = true;
                var response = await ProtocolProgramService.GetListByUniversityId(_university.Id, ProgramType.Protocol);
                if (response.Result)
                {
                    _programs = response.Item;
                }
                else
                {
                    _programs = new();
                }
            }
            catch (Exception e)
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "", e.Message);
            }
            finally
            {
                _loading = false;
            }
            await base.OnInitializedAsync();
        }
    }
}