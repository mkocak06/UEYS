using System;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.SharedComponents.Components;
using UI.Services;
using ApexCharts;
using Shared.ResponseModels.Program;
using UI.Helper;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using Fluxor;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Dashboard
{
    public partial class ProgramReportsByUniversity
    {
        [Inject] IState<AppState> AppState { get; set; }

        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IProgramService ProgramService { get; set; }

        private List<ProgramsCountByUniversityTypeModel> _programsCounts { get; set; }
        private ApexChartOptions<ProgramsCountByUniversityTypeModel> options;
        private bool _chartLoaded = false;
        private ApexChart<ProgramsCountByUniversityTypeModel> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            options = new ApexChartOptions<ProgramsCountByUniversityTypeModel>
            {
                NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                Chart = new()
                {
                    Toolbar = new Toolbar
                    {
                        Export = new ExportOptions
                        {
                            Png = new ExportPng
                            { Filename = "Üniversite Tipine Bağlı Eğitici Sayısı" },
                            Csv = new ExportCSV
                            {
                                Filename = "Üniversite Tipine Bağlı Eğitici Sayısı",
                                HeaderCategory = "sep=|" + Environment.NewLine + "Universite Tipi",
                                HeaderValue = "Egitici Sayisi",
                                ColumnDelimiter = "|",
                                DateFormatter = "function (value) {  return new Date(value).toLocaleString(); }"

                            }
                        }
                    },
                },
                Tooltip = new()
                {
                    Y = new TooltipY
                    {
                        Formatter = @"function(value, opts) {
                    if (value === undefined) {return '';}
                    return Number(value).toLocaleString();}"
                    }
                },
                PlotOptions = new()
                {
                    Bar = new PlotOptionsBar
                    {
                        Horizontal = true
                    },
                    Pie = new PlotOptionsPie
                    {
                        StartAngle = -90,
                        EndAngle = 90,
                        Donut = new PlotOptionsDonut
                        {
                            Labels = new DonutLabels
                            {
                                Total = new DonutLabelTotal { Label = "Toplam", FontSize = "24px", Color = "#D807B8", Formatter = @"function (w) {return w.globals.seriesTotals.reduce((a, b) => { return (a + b) })}" }
                            }
                        }
                    }
                },
                Legend = new Legend
                {
                    Position = LegendPosition.Bottom
                }
            };
            SubscribeToAction<ProgramsLoadAction>(async (action) =>
            {

                await GetFieldNames();
                await _apexChart.UpdateOptionsAsync(true, true, false);
            });
            GetFieldNames();
        }

        private async Task GetFieldNames()
        {
            if (Filter != null)
            {
                try
                {


                    var response = await ProgramService.GetProgramsCountByUniversityType(Filter);
                    if (response.Result && response.Item != null)
                    {
                        _programsCounts = response.Item;
                        _chartLoaded = true;

                        StateHasChanged();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
