using System.Collections.Generic;

namespace Shared.BaseModels;

public class RolePermissionBase
{
    public long RoleId { get; set; }
    public List<string> Permissions { get; set; }
}