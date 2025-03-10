using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.StatisticModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Models;
using UI.Models.FilterModels;
using UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store;
using UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store;
using UI.Services;
using UI.SharedComponents.Store;

namespace UI.Pages.Home
{
    public partial class HomeV2
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] private IState<AppState> AppState { get; set; }
        [Inject] IState<ProgramDetailState> ProgramDetailState { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Inject] IProgramService ProgramService { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }

        private FilterDTO DashboardFilter => AppState.Value.DashboardFilter;
        private DashboardProgramFilter DashboardProgramFilter => ProgramDetailState.Value.DashboardProgramFilter;
        private List<ProvinceResponseDTO> Provinces => AppState.Value.Provinces;

        private List<PlateColorOrderModel> colorOrderList = new List<PlateColorOrderModel>();
        private bool _chartView = false;
        private List<long> _provinceList = new List<long>();
        protected override async Task OnInitializedAsync()
        {
            if (!AppState.Value.ProvincesLoaded)
            {
                Dispatcher.Dispatch(new AppLoadProvincesAction());
            }
            await PlateListColorInfo();

            await base.OnInitializedAsync();
        }

        //protected override void OnAfterRender(bool firstRender)
        //{
        //    if (firstRender)
        //        JsRuntime.InvokeVoidAsync("initStickyPanel");
        //    base.OnAfterRender(firstRender);
        //}
        private void OpenCanvas()
        {
            JsRuntime.InvokeVoidAsync("initQuickPanel");
            JsRuntime.InvokeVoidAsync("ShowAdvancedSearchAsync");
        }


        private void SetFilter(List<long> idList, string fieldName)
        {
            DashboardFilter.Filter ??= new Filter();
            DashboardFilter.Filter.Filters ??= new List<Filter>();
            Filter filter = DashboardFilter.Filter.Filters.FirstOrDefault(x => x.Field == fieldName);
            if (idList is not null && idList.Count() > 0)
            {
                if (filter is null)
                {
                    filter = new() { Filters = new(), Logic = "or", Field = fieldName };
                    DashboardFilter.Filter.Filters.Add(filter);
                }
                else
                {
                    filter.Filters = new();
                }
                foreach (var id in idList)
                {
                    filter.Filters.Add(new Shared.FilterModels.Base.Filter()
                    {
                        Field = fieldName,
                        Operator = "eq",
                        Value = id
                    });
                }
            }
            else
            {
                if (filter is not null)
                {
                    DashboardFilter.Filter.Filters.Remove(filter);
                }
            }
        }
        private void SelectCity(string plateCode)
        {
            if (DashboardProgramFilter.ProvinceList.Any(x => x.Id == Convert.ToInt32(plateCode)))
            {
                DashboardProgramFilter.ProvinceList.Remove(DashboardProgramFilter.ProvinceList.FirstOrDefault(x => x.Id == Convert.ToInt32(plateCode)));
            }
            else
            {
                DashboardProgramFilter.ProvinceList.Add(Provinces.FirstOrDefault(x => x.Id == Convert.ToInt32(plateCode)));
            }

            SetFilter(DashboardProgramFilter.ProvinceList?.Select(x => x.Id)?.ToList(), "ProvinceId");
            Dispatcher.Dispatch(new ProgramDashboardFilterSetAction(DashboardProgramFilter));
            Dispatcher.Dispatch(new ProgramsLoadAction(DashboardFilter));
            StateHasChanged();
        }

        private async Task PlateListColorInfo()
        {
            var response = await ProgramService.GetProgramCountByProvince(DashboardFilter);

            if (response.Item?.Count > 0)
            {
                var items = response.Item.Select(x => new ProgramMapModel()
                {
                    PlateCode = x.PlateCode,
                    ProgramCount = x.ProgramCount,
                }).OrderBy(x => x.ProgramCount).ToList();
                double minValue = items.Min(x => x.ProgramCount);
                double maxValue = items.Max(x => x.ProgramCount);

                var totalProgramCount = items.Sum(x => x.ProgramCount);

                foreach (var item in items)
                {
                    var percantage = (item.ProgramCount - minValue) / (maxValue - minValue) * 100;

                    int orderNo = 0;
                    if (percantage > 12.5 && percantage <= 25)
                    {
                        orderNo = 0;
                    }
                    else if (percantage > 25 && percantage <= 37.5)
                    {
                        orderNo = 1;
                    }
                    else if (percantage > 37.5 && percantage <= 50)
                    {
                        orderNo = 1;
                    }
                    else if (percantage > 50 && percantage <= 62.5)
                    {
                        orderNo = 2;
                    }
                    else if (percantage > 62.5 && percantage <= 75)
                    {
                        orderNo = 2;
                    }
                    else if (percantage > 75 && percantage <= 87.5)
                    {
                        orderNo = 3;
                    }
                    else if (percantage > 87.5 && percantage <= 100)
                    {
                        orderNo = 3;
                    }

                    colorOrderList.Add(new() { PlateCode = item.PlateCode, Order = orderNo });
                }

            }
        }

        protected override ValueTask DisposeAsyncCore(bool disposing)
        {
            return base.DisposeAsyncCore(disposing);
        }
    }
}
