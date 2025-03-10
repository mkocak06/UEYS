using ApexCharts;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Student.Students.StudentDetail.Tabs
{
    public partial class ENabizOperations
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IENabizPortfolioService ENabizPortfolioService { get; set; }
        [Inject] public IState<StudentDetailState> StudentDetailState { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }

        private ENabizPortfolioResponseDTO _studentOperations = new();
        private StudentResponseDTO _student => StudentDetailState.Value.Student;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private bool _loadingFile;

        private List<ENabizPortfolioChartModel> _chartData = new();
        private ApexChartOptions<ENabizPortfolioChartModel> options;
        private ApexChartOptions<ENabizPortfolioChartModel> options1;
        private ApexChart<ENabizPortfolioChartModel> _apexChart;
        private ApexChart<ENabizPortfolioChartModel> _apexChart1;

        private List<ENabizPortfolioResponseDTO> studentOperations;

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

                var response = await ENabizPortfolioService.GetByUserId(_student.UserId);
                if (response.Result)
                {
                    _studentOperations = response.Item;
                }

                var chartResponse = await ENabizPortfolioService.GetChartDataByUserId(_student.UserId);
                if (chartResponse.Result)
                {
                    _chartData = chartResponse.Item;
                    _chartData.GroupBy(x => x.KlinikAdi).ToList().PrintJson("PrintJson");
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
                            Field="UserId",
                            Operator="eq",
                            Value=_student?.UserId
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

            var response = await ENabizPortfolioService.GetExcelByteArray(_student.UserId);
            if (response.Result)
            {
                await JsRuntime.InvokeVoidAsync("saveAsFile", $"{_student.User.Name.Trim().Replace(' ', '_')}_İşlemler_Listesi.xlsx", Convert.ToBase64String(response.Item));
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