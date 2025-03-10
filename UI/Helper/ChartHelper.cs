using System;
using System.Collections.Generic;
using System.Linq;
using ApexCharts;

namespace UI.Helper;

public static class ChartHelper
{
    public static ApexChartOptions<T> CreateHomepageOption<T>() where T : class
    {
        return new ApexChartOptions<T>()
        {
            Chart = new ApexCharts.Chart()
            {
                Toolbar = new Toolbar()
                {
                    Show = false
                },
            },
            Legend = new Legend()
            {
                Position = LegendPosition.Bottom
            },
            Title = new Title()
            {
                Align = Align.Center
            }
        };
    }

    public static ApexChartOptions<T> CreateGeneralMemberStatOption<T>() where T : class
    {
        return new ApexChartOptions<T>()
        {
            Chart = new ApexCharts.Chart()
            {
                Toolbar = new Toolbar()
                {
                    Show = true,
                    Tools = new ApexCharts.Tools()
                    {
                        Download = true,
                        Pan = false,
                        Reset = false,
                        Selection = false,
                        Zoom = false,
                        Zoomin = false,
                        Zoomout = false
                    }
                },
            },
            Legend = new Legend()
            {
                Position = LegendPosition.Bottom
            },
            DataLabels = new DataLabels()
            {
                Distributed = false,
                Enabled = true
            },
            Fill= new Fill { Colors = new List<string> { "#008ffb", "#00e396" } },
            Colors = new List<string> { "#008ffb", "#00e396" }
        };
    }

    public static ApexChartOptions<T> CreateHorizontalMemberStatOption<T>() where T : class
    {
        return new ApexChartOptions<T>()
        {
            PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    Horizontal = true
                }
            },
            Chart = new ApexCharts.Chart()
            {
                Toolbar = new Toolbar()
                {
                    Show = true,
                    Tools = new ApexCharts.Tools()
                    {
                        Download = true,
                        Pan = false,
                        Reset = false,
                        Selection = false,
                        Zoom = false,
                        Zoomin = false,
                        Zoomout = false
                    }
                },

            }
        };
    }

    public static ApexChartOptions<T> CreateTurkishLocale<T>(this ApexChartOptions<T> chart) where T : class
    {
        chart.Chart ??= new Chart();
        chart.Chart.DefaultLocale = "tr";
        chart.Chart.Locales = new List<ChartLocale>
        {
            new()
            {
                Name = "tr",
                Options = new LocaleOptions()
                {
                    Toolbar = new LocaleToolbar()
                    {
                        //Download = "İndir",
                        Selection = "Seçili Alan",
                        SelectionZoom = "Seçili Alan Yakınlaştır",
                        Reset = "Sıfırla",
                        Pan = "Kaydır",
                        ZoomIn = "Yakınlaştır",
                        ZoomOut = "Uzaklaştır"
                    }
                }
            }
        };
        return chart;
    }

    public static ApexChartOptions<T> CreateSimplePieChartOptions<T>() where T : class
    {
        return new ApexChartOptions<T>()
        {
            DataLabels = new DataLabels()
            {
                Enabled = true,
            },
            Tooltip = new Tooltip()
            {
                Y = new TooltipY()
                {
                    Formatter = "function(value, opts) {return value.toLocaleString('tr-TR')}"
                }
            },
            PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    Horizontal = true
                }
            },
            Chart = new ApexCharts.Chart()
            {
                Toolbar = new Toolbar()
                {
                    Show = true,
                    Tools = new ApexCharts.Tools()
                    {
                        Download = true,
                        Pan = false,
                        Reset = false,
                        Selection = false,
                        Zoom = false,
                        Zoomin = false,
                        Zoomout = false
                    }
                },
            },
            Debug = true
        };
    }
    public static ApexChartOptions<T> CreateRadialChartOptions<T>() where T : class
    {
        return new ApexChartOptions<T>()
        {
            Chart = new Chart()
            {
                Toolbar = new Toolbar()
                {
                    Show = true,
                    Tools = new ApexCharts.Tools()
                    {
                        Download = true,
                        Pan = false,
                        Reset = false,
                        Selection = false,
                        Zoom = false,
                        Zoomin = false,
                        Zoomout = false
                    }
                },
                Height = 350,
                Type = ChartType.RadialBar,
            },
            Tooltip = new Tooltip()
            {
                Enabled = true,
                Y = new TooltipY()
                {
                    Formatter = "function(value, opts) {return value.toLocaleString('tr-TR')}"
                }
            },
            PlotOptions = new PlotOptions
            {
                RadialBar = new PlotOptionsRadialBar()
                {
                    StartAngle = -135,
                    EndAngle = 135,
                    DataLabels = new RadialBarDataLabels()
                    {
                        Show = true,
                        Name = new RadialBarDataLabelsName()
                        {
                            FontSize = "16px",
                            Color = null,
                            OffsetY = 120
                        },
                        Value = new RadialBarDataLabelsValue()
                        {
                            OffsetY = 76,
                            FontSize = "22px",
                            Color = null,
                            Formatter = "function (val) { return '%' + val; }"
                        },
                        Total = new RadialBarDataLabelsTotal()
                        {
                            Show = false,
                        }
                    }
                },
                Bar = new PlotOptionsBar
                {
                    Horizontal = true
                }
            },
            Fill = new Fill()
            {
                Type = new List<FillType>() { FillType.Gradient },
                Gradient = new FillGradient()
                {
                    Shade = GradientShade.Dark,
                    ShadeIntensity = 0.15,
                    InverseColors = false,
                    OpacityFrom = 1,
                    OpacityTo = 1,
                    Stops = new List<double>() { 0, 50, 65, 91 },
                }
            },
            Stroke = new Stroke()
            {
                DashArray = 4
            },
            Labels = new List<string>() { "Median Ratio" },
            Debug = true
        };
    }

    public static ApexChartOptions<T> CreateTooltipFormatter<T>(this ApexChartOptions<T> chart, FormatterType type, string label = "") where T : class
    {
        var formatter = type switch
        {
            FormatterType.Percentage => "function(value, opts) { return value ? '%' + value.toLocaleString('tr-TR') : '-'}",
            FormatterType.FormattedValue => "function(value, opts) {return value ? value.toLocaleString('tr-TR') : '-'}",
            FormatterType.LabelFormatter => "function(value, opts) {return value ? " + label + ": value.toLocaleString('tr-TR') : '-'}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        chart.Tooltip = new Tooltip()
        {
            Enabled = true,
            Y = new TooltipY()
            {
                Formatter = formatter
            }
        };
        chart.Legend = new Legend()
        {
            Position = LegendPosition.Bottom
        };
        return chart;
    }


    public static ApexChartOptions<T> MakeLabelFontsBig<T>(this ApexChartOptions<T> chart) where T : class
    {
        chart.Xaxis ??= new XAxis();
        chart.Xaxis.Labels ??= new XAxisLabels();
        chart.Xaxis.Labels.Style ??= new AxisLabelStyle();
        chart.Xaxis.Labels.Style.FontSize = "14px";
        chart.Xaxis.Labels.Style.FontWeight = 700;
        return chart;
    }

    public static ApexChartOptions<T> MakeYLabelFontsBig<T>(this ApexChartOptions<T> chart, int index) where T : class
    {
        if (chart.Yaxis == null)
        {
            chart.Yaxis = new List<YAxis>();
            for (var i = 0; i < index + 1; i++)
            {
                chart.Yaxis.Add(new YAxis());
            }
        }
        chart.Yaxis[index] ??= new YAxis();
        chart.Yaxis[index].Labels ??= new YAxisLabels();
        chart.Yaxis[index].Labels.Style ??= new AxisLabelStyle();
        chart.Yaxis[index].Labels.Style.FontSize = "14px";
        chart.Yaxis[index].Labels.Style.FontWeight = 700;
        return chart;
    }

    public static ApexChartOptions<T> CreateDataLabelsFormatter<T>(this ApexChartOptions<T> chart, FormatterType type, string label = "") where T : class
    {
        var formatter = type switch
        {
            FormatterType.Percentage => "function(value, opts) {return value ? '%' + value.toLocaleString('tr-TR') : '-'}",
            FormatterType.FormattedValue => "function(value, opts) {return value ? value.toLocaleString('tr-TR') : '-'}",
            FormatterType.LabelFormatter => "function(value, opts) {return value ? " + label + ": value.toLocaleString('tr-TR') : '-'}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        chart.DataLabels = new DataLabels()
        {
            Enabled = true,
            Formatter = formatter
        };
        return chart;
    }

    public static ApexChartOptions<T> CreateLabels<T>(this ApexChartOptions<T> chart, string xLabel, string yLabel, double offset = -30) where T : class
    {
        chart.Xaxis = new XAxis
        {
            Title = new AxisTitle
            {
                Text = xLabel,
                Style = new AxisTitleStyle
                {
                    FontSize = "14px"
                }
            },
            Labels = new XAxisLabels
            {
                Style = new AxisLabelStyle
                {
                    FontSize = "16px",
                },
            },
            TickPlacement = TickPlacement.Between,
            Type = XAxisType.Category
        };
        chart.Yaxis = new List<YAxis>
        {
            new YAxis
            {
                Title = new AxisTitle
                {
                    Text = yLabel,
                    Style = new AxisTitleStyle
                    {
                        FontSize = "14px"
                    },
                    OffsetX = offset
                },
                Labels = new YAxisLabels
                {
                    Style = new AxisLabelStyle
                    {
                        FontSize = "16px"
                    }
                }

            }
        };
        return chart;
    }

    public enum FormatterType
    {
        Percentage,
        FormattedValue,
        LabelFormatter
    }
}