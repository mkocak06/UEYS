using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using UI.Services;

namespace UI.Pages.RolesAndPermissions.Store;

public class RolesAndPermissionsEffects
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly IState<RolesAndPermissionsState> _state;
    
    public RolesAndPermissionsEffects(IAuthenticationService authenticationService, IMapper mapper, IState<RolesAndPermissionsState> state)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
        _state = state;
    }
    
    [EffectMethod]
    public async Task LoadRoles(RolesLoadAction action, IDispatcher dispatcher)
    {
        var roles = await _authenticationService.GetRoles();
        dispatcher.Dispatch(new RolesSetAction(roles.ToList()));
    }
    
    [EffectMethod]
    public async Task LoadPermissions(PermissionsLoadAction action, IDispatcher dispatcher)
    {
        var permissions = await _authenticationService.GetPermissions();
        dispatcher.Dispatch(new PermissionsSetAction(permissions.ToList()));
    }
    
    [EffectMethod]
    public async Task LoadRoleDetail(RoleLoadAction action, IDispatcher dispatcher)
    {
        var role = await _authenticationService.GetRoleById(action.Id);
        dispatcher.Dispatch(new RoleSetAction(role.Item));
    }
}