using System.ComponentModel;

namespace Shared.Types
{
    public enum ExcusedTransferType
    {
        [Description("Spouse Related")]
        SpouseRelated = 1,
        [Description("Health Related")]
        HealthRelated = 2
    }
}
