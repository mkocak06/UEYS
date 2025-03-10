using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApexCharts;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Educator;
using UI.Helper;
using UI.Services;
using Microsoft.Extensions.Options;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.SharedComponents.Store;
using Fluxor;

namespace UI.SharedComponents.Dashboard
{
    public partial class HospitalChartByParentInstitution
    {
        [Inject] IState<AppState> AppState { get; set; }
        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IHospitalService HospitalService { get; set; }

        private List<ActivePassiveResponseModel> hospitalCounts = new List<ActivePassiveResponseModel>();
        private bool _chartLoaded = false;
        private ApexChartOptions<ActivePassiveResponseModel> _options;
        private ApexChart<ActivePassiveResponseModel> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            _options = ChartHelper.CreateHomepageOption<ActivePassiveResponseModel>().MakeLabelFontsBig().MakeYLabelFontsBig(0);

            await GetFieldNames();


            SubscribeToAction<ProgramsLoadAction>(async (action) =>
            {
                await GetFieldNames();
                await _apexChart.UpdateOptionsAsync(true,true,false);
            });
            await base.OnInitializedAsync();
        }

        private async Task GetFieldNames()
        {
            try
            {
                //Filter state'den gelecek şekilde düzenlenecek. Ancak zone kontrolünün nasıl yapılacağı öncesinde belli olmalı
                var response = await HospitalService.GetHospitalCountByParentInstitution(FilterHelper.CreateFilter(1, int.MaxValue).Filter("IsDeleted", "eq", false, "and"));
                if (response.Result)
                {
                    _chartLoaded = true;
                    hospitalCounts = response.Item;
                }
                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

