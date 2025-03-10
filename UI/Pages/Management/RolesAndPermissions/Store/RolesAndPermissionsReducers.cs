using System.Linq;
using Fluxor;

namespace UI.Pages.RolesAndPermissions.Store;

public static class RolesAndPermissionsReducers
{
    [ReducerMethod(typeof(RolesLoadAction))]
    public static RolesAndPermissionsState OnLoadRoles(RolesAndPermissionsState state)
    {
        return state with
        {
            RolesLoading = true,
        };
    }
    [ReducerMethod]
    public static RolesAndPermissionsState OnSetRoles(RolesAndPermissionsState state, RolesSetAction action)
    {
        return state with
        {
            RolesLoading = false,
            RolesLoaded = true,
            Roles = action.Roles
        };
    }
    
    [ReducerMethod(typeof(PermissionsLoadAction))]
    public static RolesAndPermissionsState OnLoadPermissions(RolesAndPermissionsState state)
    {
        return state with
        {
            PermissionsLoading = true,
        };
    }
    [ReducerMethod]
    public static RolesAndPermissionsState OnSetPermissions(RolesAndPermissionsState state, PermissionsSetAction action)
    {
        return state with
        {
            PermissionsLoading = false,
            PermissionsLoaded = true,
            Permissions = action.Permissions
        };
    }
    
    
    [ReducerMethod(typeof(RoleLoadAction))]
    public static RolesAndPermissionsState OnLoadRole(RolesAndPermissionsState state)
    {
        return state with
        {
            RolePermissionsLoading = true,
        };
    }
    [ReducerMethod]
    public static RolesAndPermissionsState OnSetRole(RolesAndPermissionsState state, RoleSetAction action)
    {
        return state with
        {
            RolePermissionsLoading = false,
            RolePermissionsLoaded = true,
            SelectedRole = action.Role
        };
    }
}