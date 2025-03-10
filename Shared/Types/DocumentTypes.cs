using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum DocumentTypes
    {
        [Description("Affiliation")]
        Affiliation,
        [Description("Protocol Program")]
        ProtocolProgram,
        [Description("Complement Program")]
        ComplementProgram,
        [Description("Thesis")]
        Thesis,
        [Description("Progress Report")]
        ProgressReport,
        [Description("Ethic Committee Decision")]
        EthicCommitteeDecision,
        [Description("Official Letter")]
        OfficialLetter,
        [Description("Thesis Final Exam")]
        ThesisFinalExam,
        [Description("Performance Rating")]
        PerformanceRating,
        [Description("Thesis Defence")]
        ThesisDefence,
        [Description("Opinion Form")]
        OpinionForm,
        [Description("Scientific Study")]
        ScientificStudies,
        [Description("Education Time Tracking")]
        EducationTimeTracking,
        [Description("Rotation")]
        StudentRotation,
        [Description("Place of Duty")]
        PlaceOfDuty,
        [Description("Scientific Study")]
        ScientificStudy,
        [Description("Associate Professorship")]
        AssociateProfessorship,
        [Description("Declaration Document")]
        DeclarationDocument,
        [Description("Image")]
        Image,
        [Description("ÖSYM Result Document")]
        OsymResultDocument,
        [Description("EPK Institutional Education Officer Appointment Decision")]
        EPKInstitutionalEducationOfficerAppointmentDecision,
        [Description("Education Officer Assignment Letter")]
        EducationOfficerAssignmentLetter,
        [Description("Related Dependent Program")]
        RelatedDependentProgram,
        [Description("Student Dependent Program")]
        StudentDependentProgram,
        [Description("Chairman of the Specialization Board")]
        SpecializationBoardChairman,
        [Description("Member of the Specialization Board")]
        SpecializationBoardMember,
        [Description("Transfer")]
        Transfer,
        [Description("Committee Form")]
        CommitteeForm,
        [Description("TUK Form")]
        TUKForm,
        [Description("Communique")]
        Communique,
        [Description("Fee Receipt")]
        FeeReceipt,
        [Description("Photocopy of Identity Card")]
        PhotocopyOfIdentityCard,
        [Description("Registration Control Form")]
        RegistrationControlForm,
        Null,
        [Description("Specific Education")]
        SpecificEducation,
    }
}
