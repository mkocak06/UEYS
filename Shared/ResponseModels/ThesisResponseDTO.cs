using Shared.BaseModels;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class ThesisResponseDTO : ThesisBase
    {
        public virtual StudentResponseDTO Student { get; set; }
        public virtual ICollection<AdvisorThesisResponseDTO> AdvisorTheses { get; set; }
        public virtual ICollection<AdvisorThesisResponseDTO> DeletedAdvisorTheses { get; set; }
        public virtual ICollection<EthicCommitteeDecisionResponseDTO> EthicCommitteeDecisions { get; set; }
        public virtual ICollection<ProgressReportResponseDTO> ProgressReports { get; set; }
        public virtual ICollection<OfficialLetterResponseDTO> OfficialLetters { get; set; }
        public virtual ICollection<ThesisDefenceResponseDTO> ThesisDefences { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
