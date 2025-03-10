using System;

namespace Shared.BaseModels
{
    public class EthicCommitteeDecisionBase
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? ThesisId { get; set; }
    }
}
