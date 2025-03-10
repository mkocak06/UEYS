using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApexCharts;
using Fluxor;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Student;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Dashboard
{
    public partial class StudentChart
    {
        [Inject] IState<AppState> AppState { get; set; }

        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IStudentService StudentService { get; set; }

        private List<StudentCountByProperty> studentsCounts = new List<StudentCountByProperty>();
        private bool _chartLoaded = false;
        private ApexChartOptions<StudentCountByProperty> _options;
        private ApexChart<StudentCountByProperty> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            _options = new ApexChartOptions<StudentCountByProperty>()
            {
                NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                Chart = new()
                {
                    Toolbar = new Toolbar
                    {
                        Export = new ExportOptions
                        {
                            Png = new ExportPng
                            { Filename = "Üniversite Tipine Bağlı Öğrenci Sayısı" },
                            Csv = new ExportCSV
                            {
                                Filename = "Üniversite Tipine Bağlı Öğrenci Sayısı",
                                HeaderCategory = "sep=|" + Environment.NewLine + "Universite Tipi",
                                HeaderValue = "Ogrenci Sayisi",
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

            await GetFieldNames();
            await base.OnInitializedAsync();
        }

        private async Task GetFieldNames()
        {
            try
            {
                var response = await StudentService.CountByUniversityType(Filter);
                if (response.Result)
                {
                    _chartLoaded = true;
                    studentsCounts = response.Item;
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

