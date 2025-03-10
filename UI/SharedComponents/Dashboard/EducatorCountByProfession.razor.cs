using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ApexCharts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.StatisticModels;
using UI.Helper;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Dashboard
{
    public partial class EducatorCountByProfession
    {
        [Inject] IState<AppState> AppState { get; set; }

        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IEducatorService EducatorService { get; set; }

        private List<EducatorCountByProperty> educationFieldCounts = new List<EducatorCountByProperty>();
        private bool _chartLoaded = false;
        private ApexChartOptions<EducatorCountByProperty> _options;
        private ApexChart<EducatorCountByProperty> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            _options = new ApexChartOptions<EducatorCountByProperty>()
            {
                NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                Chart = new()
                {
                    Toolbar = new Toolbar
                    {
                        Export = new ExportOptions
                        {
                            Png = new ExportPng
                            { Filename = "Uzmanlık Alanına Bağlı Program Sayısı" },
                            Csv = new ExportCSV
                            {
                                Filename = "Uzmanlık Alanına Bağlı Program Sayısı",
                                HeaderCategory = "sep=|" + Environment.NewLine + "Uzmanlik Alani",
                                ColumnDelimiter = "|",
                                HeaderValue = "Program Sayisi",
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

                Legend = new Legend
                {
                    Position = LegendPosition.Bottom
                },
                PlotOptions = new()
                {
                    Pie = new PlotOptionsPie
                    {
                        StartAngle = -90,
                        EndAngle = 90,
                        Donut = new PlotOptionsDonut
                        {
                            Labels = new DonutLabels
                            {
                                Total = new DonutLabelTotal {Label = "Toplam", FontSize = "24px", Color = "#D807B8", Formatter = @"function (w) {return w.globals.seriesTotals.reduce((a, b) => { return (a + b) })}" }
                            }
                        }
                    }
                }
            };

            SubscribeToAction<ProgramsLoadAction>(async (action) =>
            {

                await GetFieldNames();
                await _apexChart.UpdateOptionsAsync(true, true, false);
            });
            await GetFieldNames();
            await base.OnInitializedAsync();
        }

        private async Task GetFieldNames()
        {
            try
            {
                var response = await EducatorService.CountByProfession(Filter);
                if (response.Result)
                {
                    _chartLoaded = true;
                    educationFieldCounts = response.Item;
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

