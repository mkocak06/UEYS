namespace Shared.BaseModels;

public class PermissionBase
{
    public long Id { get; set; }
    public string PermissionGroup { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MainGroup { get; set; }
    public string SubGroup { get; set; }
    public string Description2 { get; set; }
}