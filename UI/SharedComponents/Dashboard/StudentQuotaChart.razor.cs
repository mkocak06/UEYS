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
using Shared.Extensions;
using Radzen.Blazor;
using System.Reflection.Emit;

namespace UI.SharedComponents.Dashboard
{
    public partial class StudentQuotaChart
    {
        [Inject] IState<AppState> AppState { get; set; }

        private FilterDTO Filter => AppState.Value.DashboardFilter;
        [Inject] IStudentService StudentService { get; set; }

        private List<StudentQuotaChartModel> studentsCountModelList = new List<StudentQuotaChartModel>();
        private bool _chartLoaded = false;
        private ApexChartOptions<StudentQuotaChartModel> _options;
        private ApexChart<StudentQuotaChartModel> _apexChart;
        protected override async Task OnInitializedAsync()
        {
            _options = ChartHelper.CreateHomepageOption<StudentQuotaChartModel>().MakeLabelFontsBig().MakeYLabelFontsBig(0);
            _options = new ApexChartOptions<StudentQuotaChartModel>
            {
                DataLabels = new DataLabels
                {
                    Style = new DataLabelsStyle
                    {
                        Colors = new List<string> { "#000000" }
                    },
                },
                Xaxis = new XAxis
                {
                    Labels = new ApexCharts.XAxisLabels
                    {
                        Style = new AxisLabelStyle
                        {
                            FontSize = "10px"
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
                var response = await StudentService.CountByQuotas(Filter);
                if (response.Result)
                {
                    _chartLoaded = true;
                    foreach (var item in response.Item)
                    {
                        item.Name = item.SeriesName.GetDescription();
                        int index = item.Name.IndexOf('(');
                        if (index != -1)
                        {
                            item.Name = item.Name.Substring(0, index).Trim();
                            int index_1 = item.Name.IndexOf('(');
                            if (index_1 != -1)
                            {
                                item.Name = item.Name.Substring(0, index_1).Trim();
                            }
                        }
                    }
                    studentsCountModelList = response.Item;
                    studentsCountModelList.PrintJson("asdasd");
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

