using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApexCharts;
using Microsoft.AspNetCore.Components;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using UI.Helper;
using UI.Services;
using Microsoft.Extensions.Options;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using Fluxor;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Dashboard
{
    public partial class EducatorCountByInsProfession
    {
        [Inject] IState<AppState> AppState { get; set; }

        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IEducatorService EducatorService { get; set; }

        private List<CountsByProfessionInstitutionModel> countsModelList = new List<CountsByProfessionInstitutionModel>();
        private bool _chartLoaded = false;
        private ApexChartOptions<CountsByProfessionInstitutionModel> _options;
        private ApexChart<CountsByProfessionInstitutionModel> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            _options = ChartHelper.CreateHomepageOption<CountsByProfessionInstitutionModel>().MakeLabelFontsBig().MakeYLabelFontsBig(0);
            _options = new ApexChartOptions<CountsByProfessionInstitutionModel>
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
                NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                Chart = new()
                {
                    Toolbar = new Toolbar
                    {
                        Export = new ExportOptions
                        {
                            Png = new ExportPng
                            { Filename = "Üst Kurum/Uzmanlık Alanına Bağlı Eğitici Sayısı" },
                            Csv = new ExportCSV
                            {
                                Filename = "Üst Kurum/Uzmanlık Alanına Bağlı Eğitici Sayısı",
                                HeaderCategory = "sep=|" + Environment.NewLine + "Üst Kurum",
                                HeaderValue = "Program Sayisi",
                                ColumnDelimiter = "|",
                                DateFormatter = "function (value) {  return new Date(value).toLocaleString(); }"

                            }
                        }
                    },
                },
                Legend = new Legend
                {
                    Position = LegendPosition.Bottom
                },
                PlotOptions = new PlotOptions
                {
                    Bar = new PlotOptionsBar
                    {
                        Horizontal = true
                    }
                },
                DataLabels = new DataLabels
                {
                    Style = new DataLabelsStyle
                    {
                        Colors = new List<string> { "#000000" }
                    },
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
                var response = await EducatorService.CountsByProfessionInstitution(Filter);
                if (response.Result)
                {
                    _chartLoaded = true;
                    countsModelList = response.Item;
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

