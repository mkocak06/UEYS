using Shared.Types;
using System;

namespace Shared.BaseModels
{
    public class OpinionFormBase
    {
        public long? Id { get; set; }
        public FormStatusType FormStatusType { get; set; }
        public DateTime? CancellationDate { get; set; }
        // DUTY COMMINTMENT
        public int? ComplianceToWorkingHours_DC { get; set; }
        public int? DutyResponsibility_DC { get; set; }
        public int? DutyExecution_DC { get; set; }
        public int? DutyAccomplishment_DC { get; set; }

        // MANAGEMENT SKILL
        public int? ProblemAnalysisAndSolutionAbility { get; set; }
        public int? OrganizationAndCoordinationAbility { get; set; }
        public int? CommunicationSkills { get; set; }

        // PROFESSİONAL ETHİCS
        public int? RelationsWithOtherStudents { get; set; }
        public int? RelationsWithEducators { get; set; }
        public int? RelationsWithOtherEmployees { get; set; }
        public int? RelationsWithPatients { get; set; }

        public int? ProfessionalPracticeAbility { get; set; }
        public int? Scientificness { get; set; }
        public int? TeamworkAdaptation { get; set; }
        public int? ResearchDesire { get; set; }
        public int? ResearchExecutionAndAccomplish { get; set; }
        public int? UsingResourcesEfficiently { get; set; }
        public int? BroadcastingAbility { get; set; }
        public string AdditionalExplanation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PeriodType? Period { get; set; }
        public RatingResultType? Result { get; set; }
        public bool? Navigate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsRepeating { get; set; }
        public bool? IsNotified { get; set; }

        public long? StudentId { get; set; }
        public long? EducatorId { get; set; }
        public long? ProgramManagerId { get; set; }
        public long? SecretaryId { get; set; }
        public long? ProgramId { get; set; }
    }
}
