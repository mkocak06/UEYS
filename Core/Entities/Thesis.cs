using Shared.Types;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Thesis : ExtendedBaseEntity
    {
        public string Subject { get; set; }
        public DateTime? SubjectDetermineDate { get; set; }
        public ThesisSubjectType_1? ThesisSubjectType_1 { get; set; }
        public ThesisSubjectType_2? ThesisSubjectType_2 { get; set; }

        public string ThesisTitle { get; set; }
        public DateTime? ThesisTitleDetermineDate { get; set; }

        public DateTime? DeadLine { get; set; }
        public DateTime? UploadDate { get; set; }

        public ThesisStatusType? Status { get; set; }
        public string DeleteReasonExplanation { get; set; }

        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }

        public virtual ICollection<ProgressReport> ProgressReports { get; set; }
        public virtual ICollection<AdvisorThesis> AdvisorTheses { get; set; }
        public virtual ICollection<EthicCommitteeDecision> EthicCommitteeDecisions { get; set; }
        public virtual ICollection<OfficialLetter> OfficialLetters { get; set; }
        public virtual ICollection<ThesisDefence> ThesisDefences { get; set; }
    }
}
