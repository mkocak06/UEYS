using System;
using Fluxor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UI.SharedComponents.Dashboard.TurkeyMap
{
    public partial class TurkeyMap
    {
        [Parameter] public List<PlateColorOrderModel> PlateListColorInfo { get; set; }
        [Parameter] public EventCallback<string> OnCitySelect { get; set; }
        // [Parameter] public string[] ColorPalette { get; set; } = { "#caf0f8", "#ade8f4", "#90e0ef", "#48cae4", "#00b4d8", "#0096c7", "#0077b6", "#023e8a" };
        // [Parameter] public string[] ColorPalette { get; set; } = { "#99e2b4", "#88d4ab", "#78c6a3", "#67b99a", "#56ab91", "#469d89", "#358f80", "#248277" }; // Green
        // [Parameter] public string[] ColorPalette { get; set; } = { "#3fc1c0", "#20bac5", "#00b2ca", "#04a6c2", "#0899ba", "#0f80aa", "#16679a", "#1a5b92" };
        [Parameter] public string[] ColorPalette { get; set; } = { "#3fc1c0", "#00b2ca", "#0f80aa", "#1d4e89" };
        [Parameter] public string SelectedColor { get; set; } = "#ffc43d";
        [Parameter] public string DefaultBackgroundColor { get; set; } = "#adb5bd";
        public List<string> SelectedPlateCodes = new();
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IProvinceService ProvinceService { get; set; }

        private string Id;
        private string _hoveredCityName = "";

        private double _clientX = 0;
        private double _clientY = 0;
        private List<CityDetailsForMapModel> _provincesDetailForMap = new();

        protected override void OnInitialized()
        {
            Id = "TrMap-" + new Guid().ToString("N");


            SubscribeToAction<ProgramsLoadAction>(async (action) =>
            {
                var provinceFilter = action.filter.Filter?.Filters?.FirstOrDefault(x => x.Field == "ProvinceId")?.Filters?.Where(x => x.Field == "ProvinceId")?.Select(x => x.Value)?.ToList();

                if (provinceFilter != null && provinceFilter.Count > 0)
                    SelectedPlateCodes = provinceFilter.Where(x => x != null).Select(x => x.ToString()).ToList();
                else
                    SelectedPlateCodes.Clear();

                StateHasChanged();
            });
            GetProvincesForMap();
            base.OnInitialized();
        }
        private async void GetProvincesForMap()
        {
            try
            {
                var response = await ProvinceService.GetListForMap();
                if (response.Result)
                {
                    _provincesDetailForMap = response.Item;
                    //veritabanında kktc yerine lefkoşa olarak geçiyor.
                    _provincesDetailForMap.FirstOrDefault(x => x.ProvinceName.ToLower(System.Globalization.CultureInfo.CurrentCulture) == "LEFKOŞA".ToLower(System.Globalization.CultureInfo.CurrentCulture)).ProvinceName = "Kuzey Kıbrıs Türk Cumhuriyeti";
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SweetAlert.ErrorAlert();
            }
        }
        // protected override async Task OnAfterRenderAsync(bool firstRender)
        // {
        //     if (firstRender)
        //     {
        //         await JSRuntime.InvokeVoidAsync("mapHelper.init", Id, DotNetObjectReference.Create(this));
        //     }
        //     await base.OnAfterRenderAsync(firstRender);
        // }

        // [JSInvokable]
        // public async Task OnCitySelected(string plakaKodu)
        // {
        //     await OnCitySelect.InvokeAsync(plakaKodu);
        // 
        //     SweetAlert.ToastAlert(SweetAlertIcon.error, plakaKodu);
        // }

        private string GetHeatmapColor(string id)
        {
            if (SelectedPlateCodes.Any(x => x == id))
            {
                return SelectedColor;
            }
            var plateColorInfo = PlateListColorInfo?.FirstOrDefault(x => x.PlateCode == id);

            if (plateColorInfo != null)
            {
                return ColorPalette[plateColorInfo.Order];
            }
            else
            {
                return DefaultBackgroundColor;
            }
        }

        private async Task Select(string plateCode)
        {
            if (SelectedPlateCodes?.Any(x => x == plateCode) == true)
                SelectedPlateCodes.Remove(plateCode);
            else
                SelectedPlateCodes.Add(plateCode);

            await OnCitySelect.InvokeAsync(plateCode);
        }

        private void GetNameOnHover(string cityName)
        {
            _hoveredCityName = cityName;
        }

        private void ClearCityName()
        {
            _hoveredCityName = "";
        }

        private void MouseLocation(MouseEventArgs e)
        {
            _clientX = e.ClientX;
            _clientY = e.ClientY;
        }
    }
}

