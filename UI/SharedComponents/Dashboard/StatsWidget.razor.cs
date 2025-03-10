using ApexCharts;
using Blazored.Typeahead;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.Validations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UI;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents;
using UI.SharedComponents.BlazorLeaflet;
using UI.SharedComponents.Components;
using UI.Validation;

namespace UI.SharedComponents.Dashboard
{
	public partial class StatsWidget
	{

		[Inject] IProgramService ProgramService { get; set; }
		[Inject] IUserService UserService { get; set; }
		[Inject] IStudentService StudentService { get; set; }
		[Inject] IJSRuntime JSRuntime { get; set; }

		private List<ActivePassiveResponseModel> programCounts;
		private bool forceRender;
		private List<ActivePassiveResponseModel> activePassiveResponses;
		private List<CountsByMonthsResponse> studentsCountByMonth;
		private ApexChartOptions<CountsByMonthsResponse> _options;
		private ApexChartOptions<ActivePassiveResponseModel> _options2;
		private ApexChartOptions<ActivePassiveResponseModel> _options3;
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			_options = ChartHelper.CreateHomepageOption<CountsByMonthsResponse>().MakeLabelFontsBig().MakeYLabelFontsBig(0);
			_options2 = ChartHelper.CreateHomepageOption<ActivePassiveResponseModel>().MakeLabelFontsBig().MakeYLabelFontsBig(0);
			_options3 = new ApexChartOptions<ActivePassiveResponseModel>();

			_options3.Tooltip = new ApexCharts.Tooltip
			{
				Y = new TooltipY
				{
					Formatter = @"function(value, opts) {
                    if (value === undefined) {return '';}
                    return Number(value).toLocaleString();}"
				}
			};

			_options3.PlotOptions = new PlotOptions
			{
                RadialBar = new PlotOptionsRadialBar
				{
                    StartAngle = -90,
                    EndAngle = 90,
                    DataLabels = new RadialBarDataLabels
					{
						Value = new RadialBarDataLabelsValue
						{
							Show = true,
							FontSize = "25",
							Formatter = @"function(value, opts) {
                    if (value === undefined) {return '';}
                    return Number(value).toLocaleString();}"
						},
						Total = new RadialBarDataLabelsTotal
						{
							Show = true,
							Label = "Toplam Program Sayýsý",
							FontSize = "14",
							Color = "red",
							Formatter = @"function (w) {
                    return w.globals.seriesTotals.reduce((a, b) => {return a + b}) }"
						}
					}
				}
			};
			await GetProgramsWidget();
			await GetActivePassiveRecords();
			await GetCountByMonth();
		}
		private async Task GetActivePassiveRecords()
		{
			activePassiveResponses ??= new List<ActivePassiveResponseModel>();
			try
			{
				var response = await UserService.GetActivePassiveForChart();
				if (response.Result)
					activePassiveResponses = response.Item;
				StateHasChanged();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}


		private async Task GetProgramsWidget()
		{
			programCounts ??= new List<ActivePassiveResponseModel>();
			try
			{
				var response = await ProgramService.GetProgramsCountForDashboard();
				if (response.Result)
					programCounts = response.Item;
				StateHasChanged();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private async Task GetCountByMonth()
		{
			studentsCountByMonth ??= new List<CountsByMonthsResponse>();
			try
			{
				var response = await StudentService.GetCountsByMonthsResponse();
				if (response.Result)
					studentsCountByMonth = response.Item;
				StateHasChanged();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
		private string GetYAxisLabel(decimal value)
		{
			return value.ToString();
		}
		private static string GetMonthName(int key)
		{
			return new DateTime(2010, key, 1)
				.ToString("MMM", CultureInfo.CurrentCulture);
		}
	}
}