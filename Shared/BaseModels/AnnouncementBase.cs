using Shared.Types;
using System;

namespace Shared.BaseModels;

public class AnnouncementBase
{
    public string Title { get; set; }
    public string Explanation { get; set; }
    public DateTime? PublishDate { get; set; }

}