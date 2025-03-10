using System.ComponentModel;

namespace Shared.Types
{
    public enum AssignmentType
    {
        [Description("Assignment Due To Increasing Knowledge")]
        AssignmentDueToIncreasingKnowledge,
        [Description("Assignment Due To Rotation(Outside Institution)")]
        AssignmentDueToRotation_OutsideInstitution,
        //[Description("Assignment Due To Rotation(Inside of Institution)")]
        //AssignmentDueToRotation_InsideOfInstitution,
        [Description("Increasing Knowledge Due To Lack Of Education")]
        IncreasingKnowledgeDueToLackOfEducation,
        [Description("External Rotation Due To Lack Of Education")]
        ExternalRotationDueToLackOfEducation,
        [Description("Internal Rotation Due To Lack Of Education")]
        InternalRotationDueToLackOfEducation,
        [Description("Assignment Due To Field Duty")]
        AssignmentDueToFieldDuty,
        [Description("Education Abroad")]
        EducationAbroad,
        [Description("Other")]
        Other,
        [Description("Under Protocol Program")]
        UnderProtocolProgram,
    }
}
