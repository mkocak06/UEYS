using ApexCharts;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Tabs
{
    partial class EnabizOperationDetail
    {
        [Parameter] public UserType? UserType { get; set; }
        [Inject] public IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IENabizPortfolioService ENabizPortfolioService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }

        private DateTime? _startDate;
        private DateTime? _endDate;
        private bool _loadingFile;
        private ProgramResponseDTO _program => ProgramDetailState.Value.Program.Program;
        private List<ENabizPortfolioChartModel> _chartData = new();
        private ENabizPortfolioResponseDTO _operations = new();
        private ApexChartOptions<ENabizPortfolioChartModel> options;
        private ApexChartOptions<ENabizPortfolioChartModel> options1;
        private ApexChart<ENabizPortfolioChartModel> _apexChart;
        private ApexChart<ENabizPortfolioChartModel> _apexChart1;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                options = new ApexChartOptions<ENabizPortfolioChartModel>
                {
                    NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                    Chart = new Chart
                    {
                        Toolbar = new Toolbar
                        {
                            Show = false
                        },
                        DropShadow = new DropShadow
                        {
                            Enabled = true,
                            Color = "",
                            Top = 18,
                            Left = 7,
                            Blur = 10,
                            Opacity = 0.2d
                        }
                    },

                    DataLabels = new ApexCharts.DataLabels
                    {
                        OffsetY = -6d
                    },
                    Grid = new Grid
                    {
                        BorderColor = "#e7e7e7",
                        Row = new GridRow
                        {
                            Colors = new List<string> { "#f3f3f3", "transparent" },
                            Opacity = 0.5d
                        }
                    },
                    Markers = new Markers { Shape = ShapeEnum.Circle, Size = 5, FillOpacity = new Opacity(0.8d) },
                    Stroke = new Stroke { Curve = Curve.Smooth },
                    Legend = new Legend()
                    {
                        Position = LegendPosition.Bottom
                    },
                    Title = new Title()
                    {
                        Align = Align.Center
                    }
                };
                options1 = new ApexChartOptions<ENabizPortfolioChartModel>
                {
                    NoData = new ApexCharts.NoData { Text = "Hiç bir veri bulunamadı..." },
                    Chart = new Chart
                    {
                        Toolbar = new Toolbar
                        {
                            Show = false
                        },
                        DropShadow = new DropShadow
                        {
                            Enabled = true,
                            Color = "",
                            Top = 18,
                            Left = 7,
                            Blur = 10,
                            Opacity = 0.2d
                        }
                    },
                    DataLabels = new ApexCharts.DataLabels
                    {
                        OffsetY = -6d
                    },
                    Grid = new Grid
                    {
                        BorderColor = "#e7e7e7",
                        Row = new GridRow
                        {
                            Colors = new List<string> { "#f3f3f3", "transparent" },
                            Opacity = 0.5d
                        }
                    },

                    Markers = new Markers { Shape = ShapeEnum.Circle, Size = 5, FillOpacity = new Opacity(0.8d) },
                    Stroke = new Stroke { Curve = Curve.Smooth },
                    Legend = new Legend()
                    {
                        Position = LegendPosition.Bottom
                    },
                    Title = new Title()
                    {
                        Align = Align.Center
                    }
                };

                var response = await ENabizPortfolioService.GetByProgramId(_program.Id, UserType);
                if (response.Result)
                {
                    _operations = response.Item;
                }

                var chartResponse = await ENabizPortfolioService.GetChartDataByProgramId(_program.Id, UserType);
                if (chartResponse.Result)
                {
                    _chartData = chartResponse.Item;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            FilterDTO filter = new()
            {
                Sort = new[]
                {
                    new Sort()
                },
                Filter = new()
                {
                    Filters = new()
                    {
                        new Filter()
                        {
                            Field = "IsDeleted",
                        Operator = "eq",
                        Value = true,
                        },
                        new Filter()
                        {
                            Field="ProgramId",
                            Operator="eq",
                            Value=_program?.Id
                        },
                        new Filter()
                        {
                            Field="startDate",
                            Operator="eq",
                            Value=_startDate
                        },
                        new Filter()
                        {
                            Field="endDate",
                            Operator="eq",
                            Value=_endDate
                        }

                    },
                    Logic = "and"

                },
                page = 1,
                pageSize = int.MaxValue,
            };

            await base.OnInitializedAsync();
        }

        private async Task DownloadExcelFile()
        {
            if (_loadingFile)
            {
                return;
            }
            _loadingFile = true;
            StateHasChanged();

            var response = await ENabizPortfolioService.GetExcelByteArrayByProgram(_program.Id, UserType);
            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"{_program.Name}_İşlemler_Listesi.xlsx", Convert.ToBase64String(response.Item));
                _loadingFile = false;
            }
            else
            {
                SweetAlert.ErrorAlert();
            }
            StateHasChanged();
        }
    }
}
