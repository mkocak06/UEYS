using System.ComponentModel;

namespace Shared.Types
{
    public enum DutyType
    {
        [Description("Permanent Place of Duty")]
        PermanentDuty,
        [Description("Temporary Place of Duty")]
        TemporaryDuty
    }
}
