using ApexCharts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.StatisticModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Dashboard
{
    public partial class EducatorChart
    {
        [Inject] IState<AppState> AppState { get; set; }
        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IEducatorService EducatorService { get; set; }
        private ApexChart<EducatorCountByProperty> _apexChart;
        private List<EducatorCountByProperty> educatorsCounts = new List<EducatorCountByProperty>();
        private bool _chartLoaded = false;
        private ApexChartOptions<EducatorCountByProperty> _options;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _options = ChartHelper.CreateHomepageOption<EducatorCountByProperty>().MakeLabelFontsBig().MakeYLabelFontsBig(0);
            _options = new ApexChartOptions<EducatorCountByProperty>
            {
                NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                Chart = new()
                {
                    Toolbar = new Toolbar
                    {
                        Export = new ExportOptions
                        {
                            Png = new ExportPng
                            { Filename = "Akademik Unvana Bağlı Eğitici Sayısı" },
                            Csv = new ExportCSV
                            {
                                Filename = "Akademik Unvana Bağlı Eğitici Sayısı",
                                HeaderCategory = "sep=|" + Environment.NewLine + "Akademik Unvan",
                                HeaderValue = "Egitici Sayisi",
                                ColumnDelimiter = "|",
                                DateFormatter = "function (value) {  return new Date(value).toLocaleString(); }"

                            }
                        }
                    },
                },
                Legend = new Legend
                {
                    Position = LegendPosition.Bottom,
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
                    Pie = new PlotOptionsPie
                    {
                        Donut = new PlotOptionsDonut
                        {
                            Labels = new DonutLabels
                            {

                                Total = new DonutLabelTotal { Label = "Toplam", FontSize = "24px", Color = "#D807B8", Formatter = @"function (w) {return w.globals.seriesTotals.reduce((a, b) => { return (a + b) })}" }
                            }
                        }
                    }
                }
            };
            SubscribeToAction<ProgramsLoadAction>(async (action) =>
            {

                await GetFieldNames();
                await _apexChart.UpdateOptionsAsync(true,true,false);
            });
            await GetFieldNames();
        }

        private async Task GetFieldNames()
        {
            try
            {
                var response = await EducatorService.CountByAcademicTitle(Filter);
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

