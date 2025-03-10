using System.ComponentModel;

namespace Shared.Types
{
    public enum TitleType
    {
        [Description("Akademik")]
        Academic,
        [Description("Kadro")]
        Staff,
        [Description("İdari")]
        Administrative
    }
}
