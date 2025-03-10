using System;

namespace Shared.ResponseModels.Mobile
{
    public class MobileAuthDetailResponseDTO
    {
        public DateTime? CreateDate { get; set; }
        public long? Id { get; set; }
        public DateTime? AuthorizationDate { get; set; }
        public DateTime? AuthorizationEndDate { get; set; }
        public DateTime? VisitDate { get; set; }
        public string AuthorizationDecisionNo { get; set; }
        public string Description { get; set; }
        public string Order { get; set; }

        public long? ProgramId { get; set; }
        public long? AuthorizationCategoryId { get; set; }
        public virtual MobileAuthCategoryResponseDTO AuthorizationCategory { get; set; }
    }
}
