using System.ComponentModel;

namespace Shared.Types;

public enum ColorType
{
    [Description("Mavi")]
    Primary,
    [Description("Gri")]
    Secondary,
    [Description("Yeşil")]
    Success,
    [Description("Kırmızı")]
    Danger,
    [Description("Turuncu")]
    Warning,
    [Description("Mor")]
    Info,
    [Description("Siyah")]
    Dark
}