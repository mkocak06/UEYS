using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApexCharts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels.StatisticModels;
using UI.Helper;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Dashboard
{
    public partial class ProgramCountByParentInstitution
    {
        [Inject] IState<AppState> AppState { get; set; }

        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IProgramService ProgramService { get; set; }

        private List<ActivePassiveResponseModel> programCountsByIns = new List<ActivePassiveResponseModel>();
        private bool _chartLoaded = false;
        private ApexChartOptions<ActivePassiveResponseModel> _options;
        private ApexChart<ActivePassiveResponseModel> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            _options = new ApexChartOptions<ActivePassiveResponseModel>()
            {
                Tooltip = new()
                {
                    Y = new TooltipY
                    {
                        Formatter = @"function(value, opts) {
                    if (value === undefined) {return '';}
                    return Number(value).toLocaleString();}"
                    }
                },
                Legend = new Legend
                {
                    Position = LegendPosition.Bottom
                }
                //PlotOptions = new()
                //{
                //    Pie = new PlotOptionsPie
                //    {
                //        Donut = new PlotOptionsDonut
                //        {
                //            Labels = new DonutLabels
                //            {
                //                Total = new DonutLabelTotal { FontSize = "24px", Color = "#D807B8", Formatter = @"function (w) {return w.globals.seriesTotals.reduce((a, b) => { return (a + b) })}" }
                //            }
                //        }
                //    }
                //}
            };

            SubscribeToAction<ProgramsLoadAction>(async (action) =>
            {

                await GetFieldNames();
                await _apexChart.UpdateOptionsAsync(true,true,false);
            });
            await GetFieldNames();
            await base.OnInitializedAsync();
        }

        private async Task GetFieldNames()
        {
            try
            {
                var response = await ProgramService.GetProgramCountByParentInstitution(Filter);
                if (response.Result)
                {
                    _chartLoaded = true;
                    programCountsByIns = response.Item;
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

