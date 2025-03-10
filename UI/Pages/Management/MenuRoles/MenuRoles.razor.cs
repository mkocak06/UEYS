using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.Constants;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Menu;
using Shared.Types;
using UI.Helper;
using UI.Pages.RolesAndPermissions.Store;
using UI.Services;
using UI.SharedComponents.Components;
using UI.SharedComponents.Store;

namespace UI.Pages.Management.MenuRoles;

public partial class MenuRoles
{
    [Inject] private IState<RolesAndPermissionsState> RolesAndPermissionsState { get; set; }
    [Inject] private IState<AppState> AppState { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IMapper Mapper { get; set; }
    [Inject] private ISweetAlert SweetAlert { get; set; }
    [Inject] private IAuthenticationService AuthenticationService { get; set; }

    private List<RoleResponseDTO> _roles = new();
    private List<MenuResponseDTO> Menus => AppState.Value.Menu;

    private RoleResponseDTO _selectedRole => RolesAndPermissionsState.Value.SelectedRole;
    private List<RoleResponseDTO> Roles => RolesAndPermissionsState.Value.Roles;

    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        if (!RolesAndPermissionsState.Value.RolesLoaded)
        {
            Dispatcher.Dispatch(new RolesLoadAction());
        }
        else
        {
            Dispatcher.Dispatch(new RoleLoadAction(RolesAndPermissionsState.Value.Roles.First().Id));
        }

        if (!RolesAndPermissionsState.Value.PermissionsLoaded)
        {
            Dispatcher.Dispatch(new PermissionsLoadAction());
        }

        SubscribeToAction<RolesSetAction>(action =>
        {
            if (action.Roles is { Count: > 0 })
            {
                Dispatcher.Dispatch(new RoleLoadAction(action.Roles.First().Id));
            }
        });

        if (!AppState.Value.MenuCreated)
        {
            Dispatcher.Dispatch(new AppLoadMenusAction());
        }

        await base.OnInitializedAsync();
    }

    private async void AddDefaultMenu()
    {
        Console.WriteLine(Roles.Count);
        foreach (var item in Roles)
        {

            await AuthenticationService.AddMenuToRole(new RoleMenuDTO()
            {
                MenuId = 1,
                RoleId = item.Id
            });

        }
    }
    private async void SelectRole(RoleResponseDTO role)
    {
        if (_selectedRole != null && _selectedRole.Id == role.Id)
        {
            return;
        }

        Dispatcher.Dispatch(new RoleLoadAction(role.Id));
    }



    private bool IsAdded(MenuResponseDTO menu)
    {
        return _selectedRole.Menus.Any(x => x.Id == menu.Id);
    }

    private async Task ToggleMenu(MenuResponseDTO menu)
    {
        if (_loading)
        {
            return;
        }



        _loading = true;
        StateHasChanged();

        if (IsAdded(menu))
        {
            if (menu.ParentId != null && _selectedRole.Menus.FirstOrDefault(x => x.Id == menu.ParentId.Value).ChildMenus.Count == 1)
            {
                _selectedRole.Menus.RemoveAll(x => x.Id == menu.ParentId.Value);
                await AuthenticationService.RemoveMenuToRole(new RoleMenuDTO()
                {
                    MenuId = menu.ParentId.Value,
                    RoleId = _selectedRole.Id
                });
            }
            _selectedRole.Menus.RemoveAll(x => x.Id == menu.Id);
            try
            {
                await AuthenticationService.RemoveMenuToRole(new RoleMenuDTO()
                {
                    MenuId = menu.Id,
                    RoleId = _selectedRole.Id
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        else
        {
            if (menu.ParentId != null && !IsAdded(menu.ParentMenu))
            {
                _selectedRole.Menus.Add(menu.ParentMenu);
                await AuthenticationService.AddMenuToRole(new RoleMenuDTO()
                {
                    MenuId = menu.ParentId.Value,
                    RoleId = _selectedRole.Id
                });
            }
            _selectedRole.Menus.Add(menu);
            await AuthenticationService.AddMenuToRole(new RoleMenuDTO()
            {
                MenuId = menu.Id,
                RoleId = _selectedRole.Id
            });
        }

        _loading = false;
        StateHasChanged();
    }

    private bool IsRoleBuiltIn(RoleResponseDTO role)
    {
        return role is { RoleName: RoleConstants.SUPERADMIN or RoleConstants.ADMIN };
    }
}