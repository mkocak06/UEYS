using System.Collections.Generic;

namespace UI.Models;

public class BreadCrumbLink
{
    public int OrderIndex { get; set; }
    public string To { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public List<DropdownElement> DropdownList { get; set; }
}



public class DropdownElement
{
    public string Link { get; set; }
    public string Name { get; set; }
}