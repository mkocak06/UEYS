using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum ReasonType
    {
        //ProcessType.Start
        [Description("Beginning Specialization Education")]
        BeginningSpecializationEducation = 1,
        [Description("Restarting Education by Judgment")]
        RestartByJudgment = 2,
        [Description("Restarting Education with Amnesty Law")]
        RestartingWithAmnestyLaw = 3,
        [Description("Branch Change")]
        BranchChange = 4,
        //[Description("Transfer")]
        //Transfer = 5,

        //ProcessType.TimeIncreasing
        [Description("Military Leave")]
        MilitaryLeave = 6,
        [Description("Maternity Leave")]
        MaternityLeave = 7,
        [Description("Health Report")]
        HealthReport = 8,
        [Description("Unpaid Vacation")]
        UnpaidVacation = 9,
        [Description("Time Extension Due to Failure in Rotation and Return to Own Institution")]
        TimeExtensionDueToFailureInRotation = 10,
        [Description("Relocation and Time Extension Due to Negative Opinion")]
        RelocationAndTimeExtensionDueToNegativeOpinion = 11,
        [Description("Extension by Application for Subjection (Curriculum)")]
        ExtensionByApplicationForSubjection = 12,
        [Description("Force Majeure")]
        ForceMajeure = 13,
        [Description("Marriage Leave")]
        MarriageLeave = 14,
        [Description("Death Leave")]
        DeathLeave = 15,
        [Description("Accompaniment")]
        Accompaniment = 16,
        [Description("Paternity Leave")]
        PaternityLeave = 17,
        [Description("Due to Unusual Circumstances")]
        DueToUnusualCircumstances = 18,
        [Description("Other")]
        Other = 19,

        //ProcessType.TimeDecreasing
        [Description("Counting the Education Period with the Decision of the Academic Board")]
        CountingTheEducationPeriodWithTheDecisionOfTheAcademicBoard = 20,
        [Description("Counting Education Periods Due to Legislation")]
        CountingEducationPeriodsDueToLegislation = 21,

        //ProcessType.Finish
        [Description("Left Due to Failure in the Final Exam")]
        LeftDueToFailureInTheFinalExam = 22,
        [Description("Left Due to Resignation")]
        LeftDueToResignation = 23,
        [Description("Successful Completion of the Specialization Training")]
        SuccessfullyCompleted = 24,
        [Description("Left with Consent")]
        LeftWithConsent = 25,
        [Description("Left Due to Failure in Thesis")]
        LeftDueToFailureInThesis = 26,
        [Description("Death")]
        LeftDueToDeath = 27,
        [Description("Unexcused Transfer")]
        UnexcusedTransfer = 28,
        [Description("Excused Transfer")]
        ExcusedTransfer = 29,
        [Description("Branch Change")]
        BranchChange_End = 30,

        //ProcessType.EstimatedFinish
        [Description("Estimated Finish of Education")]
        EstimatedFinish = 31,

        //ProcessType.Information
        [Description("Rotation")]
        Rotation = 32,
        [Description("Thesis Detemine Date")]
        ThesisDetemineDate = 33,
        [Description("Thesis Exam")]
        ThesisExam = 34,

        //ProcessType.Assignment
        [Description("Leaving the Institution Due to Assignment")]
        LeavingTheInstitutionDueToAssignment = 35,
        [Description("Completion Of Assignment")]
        CompletionOfAssignment = 36,
        [Description("Completion Of Education Abroad")]
        CompletionOfAssignmentAbroad = 37,
        [Description("Beginning to Assigned Institution")]
        BeginningToAssignedInstitution = 38,
        [Description("Beginning To Own Institution After Assignment")]
        BeginningToOwnInstitutionAfterAssignment = 39,

        //ProcessType.Finish
        [Description("KKTC Half Timed")]
        KKTCHalfTimed = 40,

        //burdan sonrası karışık
        [Description("Time Decreasing in Accordance With Law Number 1219")]
        TimeDecreasinginAccordanceWithLawNumber1219 = 41,
        [Description("Time Extension Due to Failure of First Thesis Defense")]
        TimeExtensionDueToFirstThesisDefence = 42,
        [Description("Time Decreasing Due to Success of First Thesis Defense")]
        TimeDecreasingDueToThesisDefence = 43,
        [Description("1st Thesis Defence")]
        FirstThesisDefence = 44,
        [Description("2nd Thesis Defence")]
        SecondThesisDefence = 45,
        [Description("Failure in the Exit Exam")]
        FailureInExitExam = 46,
        [Description("Successful in the Exit Exam")]
        SuccessfulInExitExam = 47,
        [Description("Exam Not Concluded")]
        ExamNotConcluded = 48,
        [Description("Leaving Due to Re-Passing the Exam")]
        LeavingDuetoRePassingtheExam = 49,
        [Description("End Of Education Due to Negative Opinion")]
        EndOfEducationDuetoNegativeOpinion = 50,
        [Description("Leaving the Institution Due to Rotation")]
        LeavingTheInstitutionDueToRotation = 51,
        [Description("Completion Of Rotation")]
        CompletionOfRotation = 52,
        [Description("Exemption From Rotation")]
        ExemptionOfRotation = 53,
        [Description("Left without Completing Rotation")]
        LeftWithoutCompletingRotation = 54,
        [Description("Transfer Failed")]
        TransferFailed = 55,
        [Description("3rd Thesis Defence")]
        ThirdThesisDefence = 56,
        [Description("4th Thesis Defence")]
        FourthThesisDefence = 57,
        [Description("Starting Over with Exam for the Same Expertise Branch")]
        StartingOverwithExamfortheSameExpertiseBranch = 58,
        [Description("Annual Leave")]
        AnnualLeave = 59,
        [Description("Cancellation of Negative Opinion by Court Decision")]
        CancellationofNegativeOpinionbyCourtDecision = 60,
        [Description("Ray Leave")]
        RayLeave = 61,
        [Description("Termination/Suspension of Civil Service")]
        TerminationSuspensionofCivilService = 62,
        [Description("Thesis Defence")]
        ThesisDefence = 63,
        [Description("Registration by mistake")]
        RegistrationByMistake = 64,
        [Description("Conviction")]
        Conviction = 65,
        [Description("Being Considered Independent")]
        BeingConsideredIndependent = 66,
        [Description("Exportation")]
        Exportation = 67,
        [Description("Banned from Profession")]
        BannedFromProfession = 68,
        [Description("Time Decreasing Due to Success of Second Thesis Defense")]
        TimeDecreasingDueToSecondThesisDefence = 69,
        [Description("First Thesis Defence Date Determined")]
        FirstThesisDefenceDateDetermined = 70,
        [Description("Successful First Thesis Defence")]
        SuccessfulFirstThesisDefence = 71,
        [Description("Unsuccessful First Thesis Defence")]
        UnsuccessfulFirstThesisDefence = 72,
        [Description("Second Thesis Defence Date Determined")]
        SecondThesisDefenceDateDetermined = 73,
        [Description("Successful Second Thesis Defence")]
        SuccessfulSecondThesisDefence = 74,
        [Description("Unsuccessful Second Thesis Defence")]
        UnsuccessfulSecondThesisDefence = 75,
        [Description("Dismission")]
        Dismission = 76,
    }
}
