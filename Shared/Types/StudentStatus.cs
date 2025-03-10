using System.ComponentModel;

namespace Shared.Types
{
    public enum StudentStatus
    {
        [Description("Education Continues")]
        EducationContinues = 1,
        [Description("Graduated")]
        Gratuated = 2,
        [Description("Other")]
        Other = 3,
        [Description("Education Abroad")]
        Abroad = 4,
        [Description("Assigned")]
        Assigned = 5,
        [Description("Requires Transfer Due to Negative Opinion")]
        TransferDueToNegativeOpinion = 6,
        [Description("Requires End Of Education Due to Negative Opinion")]
        EndOfEducationDueToNegativeOpinion = 7,
        [Description("Education Ended")]
        EducationEnded = 8,
        [Description("Rotation")]
        Rotation = 9,
        [Description("Sent to Registration")]
        SentToRegistration = 10,
        [Description("Dismissed")]
        Dismissed = 11,
        [Description("Estimated Finish Date Past")]
        EstimatedFinishDatePast = 12,
        [Description("Education Failed")]
        EducationFailed = 13,
    }
}
