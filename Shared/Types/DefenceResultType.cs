using System.ComponentModel;

namespace Shared.Types
{
    public enum DefenceResultType
    {
        [Description("Defence Date Determined")]
        InProgress,
        [Description("Successful")]
        Successful,
        [Description("Failed")]
        Failed,
        [Description("Successful with minor revision")]
        SuccessfulWithRevision,

    }
}
