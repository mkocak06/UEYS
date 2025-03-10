using System.ComponentModel;

namespace Shared.Types
{
    public enum NonInstructorType
    {
        [Description("Thesis Advisor")]
        ThesisAdvisor,
        [Description("Thesis Defence Jury")]
        ThesisDefenceJury,
        [Description("Exit Exam Jury")]
        ExitExamJury
    }
}
