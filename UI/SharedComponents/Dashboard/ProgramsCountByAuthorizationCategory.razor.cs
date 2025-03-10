using System;
using ApexCharts;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels.StatisticModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Services;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using Shared.ResponseModels.Educator;
using UI.SharedComponents.Store;
using Fluxor;
using System.Linq;
using Microsoft.Extensions.Options;

namespace UI.SharedComponents.Dashboard
{
    public partial class ProgramsCountByAuthorizationCategory
    {
        [Inject] IProgramService ProgramService { get; set; }
        [Inject] IState<AppState> AppState { get; set; }

        public FilterDTO Filter => AppState.Value.DashboardFilter;
        private List<AuthorizationCategoryChartModel> educationFieldCounts = new List<AuthorizationCategoryChartModel>();
        private bool _chartLoaded = false;
        private ApexChartOptions<AuthorizationCategoryChartModel> _options;
        private ApexChart<AuthorizationCategoryChartModel> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            _options = new ApexChartOptions<AuthorizationCategoryChartModel>()
            {
                NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                Chart = new()
                {
                    Toolbar = new Toolbar
                    {
                        Export = new ExportOptions
                        {
                            Png = new ExportPng
                            { Filename = "Yetki Kategorisine Bağlı Program Sayısı" },
                            Csv = new ExportCSV
                            {
                                Filename = "Yetki Kategorisine Bağlı Program Sayısı",
                                HeaderCategory = "sep=|" + Environment.NewLine + "Yetki Kategorisi",
                                HeaderValue = "Program Sayisi",
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
            await base.OnInitializedAsync();
        }

        private async Task GetFieldNames()
        {
            try
            {
                var response = await ProgramService.CountByAuthorizationCategory(Filter);
                if (response.Result)
                {
                    _chartLoaded = true;
                    educationFieldCounts = response.Item.OrderBy(x => x.SeriesName).ToList();

                    _options.Colors = educationFieldCounts.Select(x => x.ColorCode).ToList();
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

