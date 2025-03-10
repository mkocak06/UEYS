using System.Collections.Generic;
using Fluxor;
using Shared.ResponseModels;

namespace UI.Pages.RolesAndPermissions.Store;

public record RolesAndPermissionsState
{
    public bool RolesLoading { get; init; }
    public bool RolesLoaded { get; init; }
    public List<RoleResponseDTO> Roles { get; init; }
    
    public bool PermissionsLoading { get; init; }
    public bool PermissionsLoaded { get; init; }
    public List<PermissionResponseDTO> Permissions { get; init; }
    
    public bool RolePermissionsLoading { get; init; }
    public bool RolePermissionsLoaded { get; init; }
    
    public RoleResponseDTO SelectedRole { get; init; }
}

public class RolesAndPermissionsFeature : Feature<RolesAndPermissionsState>
{
    public override string GetName() => "RolesAndPermissions";

    protected override RolesAndPermissionsState GetInitialState()
    {
        return new RolesAndPermissionsState
        {
            RolesLoading = false,
            RolesLoaded = false,
            Roles = new List<RoleResponseDTO>(),
            PermissionsLoading = false,
            PermissionsLoaded = false,
            Permissions = new List<PermissionResponseDTO>(),
            RolePermissionsLoading = false,
            RolePermissionsLoaded = false,
            SelectedRole = null
        };
    }
}

#region DefinitionsActions
public record RolesLoadAction();
public record RolesSetAction(List<RoleResponseDTO> Roles);
public record RoleLoadAction(long Id);
public record RoleSetAction(RoleResponseDTO Role);
public record PermissionsLoadAction();
public record PermissionsSetAction(List<PermissionResponseDTO> Permissions);
#endregion