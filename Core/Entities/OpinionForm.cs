using Microsoft.VisualBasic;
using Org.BouncyCastle.Tsp;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OpinionForm : ExtendedBaseEntity
    {
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

        public PeriodType? Period { get; set; }
        public RatingResultType? Result { get; set; }
        public bool? IsRepeating { get; set; }
        public bool? IsNotified { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
        public long? ProgramManagerId { get; set; }
        public virtual Educator ProgramManager { get; set; }
        public long? SecretaryId { get; set; }
        public virtual User Secretary { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
    }
}
