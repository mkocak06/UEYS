using System.Collections.Generic;

namespace UI.Models;

public class MenuModel
{
    public string MenuItemName { get; set; }
    public string Link { get; set; }
    public string Icon { get; set; }
    public int Order { get; set; }
    public List<MenuModel> SubMenus { get; set; }
    public List<string> Permissions { get; set; }
}