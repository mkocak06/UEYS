using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApexCharts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Educator;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Dashboard
{
	public partial class EducatorCountByUniversity
    {
        [Inject] IState<AppState> AppState { get; set; }

        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IEducatorService EducatorService { get; set; }

        private List<EducatorCountByProperty> educatorsCounts = new List<EducatorCountByProperty>();
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
                                Total = new DonutLabelTotal {Label = "Toplam", FontSize = "24px", Color = "#D807B8", Formatter = @"function (w) {return w.globals.seriesTotals.reduce((a, b) => { return (a + b) })}" }
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
                await _apexChart.UpdateOptionsAsync(true,true,false);
            });

            await GetFieldNames();
            await base.OnInitializedAsync();
        }

        private async Task GetFieldNames()
        {
            try
            {
                var response = await EducatorService.CountByUniversityType(Filter);
                if (response.Result)
                {
                    _chartLoaded = true;
                    educatorsCounts = response.Item;
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

