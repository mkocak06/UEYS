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
using Shared.Types;
using UI.Helper;
using UI.Pages.RolesAndPermissions.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Management.RolesAndPermissions;

public partial class RoleAndPermissionSettings
{
    [Inject] private IState<RolesAndPermissionsState> RolesAndPermissionsState { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IMapper Mapper { get; set; }
    [Inject] private ISweetAlert SweetAlert { get; set; }
    [Inject] private IAuthenticationService AuthenticationService { get; set; }

    private List<RoleResponseDTO> Roles => RolesAndPermissionsState.Value.Roles;
    private List<PermissionResponseDTO> Permissions => RolesAndPermissionsState.Value.Permissions;
    private RoleResponseDTO SelectedRole => RolesAndPermissionsState.Value.SelectedRole;

    private bool _loading;

    private MyModal _roleDetail;
    private EditContext _ec;
    private RoleResponseDTO _editingRole;

    protected override void OnInitialized()
    {
        _editingRole = new RoleResponseDTO();
        _ec = new EditContext(_editingRole);
        
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
            if (action.Roles is {Count: > 0})
            {
                Dispatcher.Dispatch(new RoleLoadAction(action.Roles.First().Id));
            }
        });
        
        base.OnInitialized();
    }

    private void SelectRole(RoleResponseDTO role)
    {
        if (SelectedRole != null && SelectedRole.Id == role.Id)
        {
            return;
        }
        Dispatcher.Dispatch(new RoleLoadAction(role.Id));
    }

    private void OnEditRole(RoleResponseDTO role)
    {
        if (IsRoleBuiltIn(role))
        {
            SweetAlert.ToastAlert(SweetAlertIcon.warning, L["You cannot edit default roles!"]);
            return;
        }
        _editingRole = role.Clone();
        _ec = new EditContext(_editingRole);
        StateHasChanged();
        _roleDetail.OpenModal();
    }

    private async Task SaveRole()
    {
        _loading = true;
        _roleDetail.CloseModal();
        StateHasChanged();
        var dto = Mapper.Map<RoleDTO>(_editingRole);
        if (_editingRole.Id > 0)
        {
            await AuthenticationService.UpdateRole(_editingRole.Id, dto);
        }
        else
        {
            await AuthenticationService.AddRole(dto);
        }
        _loading = false;
        StateHasChanged();
        Dispatcher.Dispatch(new RolesLoadAction());
    }

    private async Task DeleteRole(RoleResponseDTO role)
    {
        if (IsRoleBuiltIn(role))
        {
            SweetAlert.ToastAlert(SweetAlertIcon.warning, L["You cannot edit default roles!"]);
            return;
        }
        var confirm = await SweetAlert.ConfirmAlert(
            L["Are you sure?"],
            L["Are you sure you want to delete this item? This action cannot be undone."],
            SweetAlertIcon.warning,
            true,
            L["Delete"],
            L["Cancel"]
        );
        if (confirm)
        {
            await AuthenticationService.DeleteRole(role.Id);
            Dispatcher.Dispatch(new RolesLoadAction());
        }
    }
    
    private bool IsAdded(PermissionResponseDTO permission)
    {
        return SelectedRole.Permissions.Any(x => x.Id == permission.Id);
    }

    private async Task TogglePermission(PermissionResponseDTO permission)
    {
        if (_loading)
        {
            return;
        }
        
        _loading = true;
        StateHasChanged();
        
        if (IsAdded(permission))
        {
            SelectedRole.Permissions.RemoveAll(x => x.Id == permission.Id);
            try
            {
                await AuthenticationService.RemovePermissionToRole(new RolePermission2DTO()
                {
                    PermissionId = permission.Id,
                    RoleId = SelectedRole.Id
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
            SelectedRole.Permissions.Add(permission);
            await AuthenticationService.AddPermissionToRole(new RolePermission2DTO()
            {
                PermissionId = permission.Id,
                RoleId = SelectedRole.Id
            });
        }
        
        _loading = false;
        StateHasChanged();
    }

    private bool IsRoleBuiltIn(RoleResponseDTO role)
    {
        return role is {RoleName: RoleConstants.SUPERADMIN or RoleConstants.ADMIN };
    }
}