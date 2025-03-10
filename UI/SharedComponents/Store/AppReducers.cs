using Fluxor;
using UI.Helper;

namespace UI.SharedComponents.Store;

public static class AppReducers
{
    [ReducerMethod]
    public static AppState OnSetMenus(AppState state, AppSetMenuAction action)
    {
        return state with
        {
            MenuCreated = true,
            Menu = action.Menu
        };
    }

    [ReducerMethod]
    public static AppState OnSetProvinces(AppState state, AppSetProvincesAction action)
    {
        return state with
        {
            ProvincesLoaded = true,
            Provinces = action.Provinces
        };
    }

   

    [ReducerMethod]
    public static AppState OnFailureProvinces(AppState state, AppFailureProvincesAction action)
    {
        return state with
        {
            ProvincesLoaded = false,
        };
    }

    [ReducerMethod]
    public static AppState OnFilterSet(AppState state, AppFilterSetAction action)
    {
        return state with
        {
            Filter = action.filter,
        };
    }
    [ReducerMethod]
    public static AppState OnFilterClear(AppState state, AppFilterClearAction action)
    {
        return state with
        {
            Filter = new(),
        };
    }

	[ReducerMethod]
	public static AppState OnDashboardFilterSet(AppState state, AppDashboardFilterSetAction action)
	{
		return state with
		{
			DashboardFilter = action.filter,
		};
	}
	[ReducerMethod]
	public static AppState OnDashboardFilterClear(AppState state, AppDashboardFilterClearAction action)
	{
		return state with
		{
			DashboardFilter = FilterHelper.CreateFilter(1, int.MaxValue).Filter("IsDeleted", "eq", false, "and"),
		};
	}
}