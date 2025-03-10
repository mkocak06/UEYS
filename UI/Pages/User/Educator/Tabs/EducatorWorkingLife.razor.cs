using ApexCharts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UI.Services;
using Shared.ResponseModels.Ekip;
using System.Linq;

namespace UI.Pages.User.Educator.Tabs
{
    partial class EducatorWorkingLife
    {
        [Inject] public IState<EducatorDetailState> EducatorState { get; set; }
        [Inject] public IEducatorService EducatorService { get; set; }
        private EducatorResponseDTO Educator => EducatorState.Value.Educator;
        private List<PersonelHareketiResponseDTO> workingPlaces = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var response = await EducatorService.WorkingLifeById(Educator.Id);
                if (response.Result)
                {
                    workingPlaces = response.Item?.OrderByDescending(x => x.baslama_tarihi).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await base.OnInitializedAsync();
        }
    }
}
