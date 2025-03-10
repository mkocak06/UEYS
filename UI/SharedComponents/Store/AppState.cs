using System.Collections.Generic;
using System.Net;
using Fluxor;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.ResponseModels.Menu;
using Shared.ResponseModels.Wrapper;
using UI.Helper;
using UI.Models;
using UI.Models.FilterModels;

namespace UI.SharedComponents.Store;

public record AppState
{
    public bool MenuCreated { get; init; }
    public List<MenuResponseDTO> Menu { get; init; }
    public bool ProvincesLoaded { get; init; }
    public List<ProvinceResponseDTO> Provinces {get;init;}
    public FilterDTO Filter { get; set; }
    public FilterDTO DashboardFilter { get; set; }
}

public class AppFeature : Feature<AppState>
{
    public override string GetName() => "App";

    protected override AppState GetInitialState()
    {
        return new AppState
        {
            MenuCreated = false,
            Menu = new List<MenuResponseDTO>(),
            ProvincesLoaded = false,
            Provinces = new(),
            Filter = new(),
            DashboardFilter = FilterHelper.CreateFilter(1, int.MaxValue).Filter("IsDeleted", "eq", false, "and")
        };
    }
}

#region AppActions
public record AppLoadMenusAction();
public record AppSetMenuAction(List<MenuResponseDTO> Menu);
public record AppLoadProvincesAction();
public record AppFailureProvincesAction(ResponseWrapper<List<ProvinceResponseDTO>> error);
public record AppSetProvincesAction(List<ProvinceResponseDTO> Provinces);

public record AppFilterSetAction(FilterDTO filter);
public record AppFilterClearAction();

public record AppDashboardFilterSetAction(FilterDTO filter);
public record AppDashboardFilterClearAction();
public record ProfileSetAction();

#endregion