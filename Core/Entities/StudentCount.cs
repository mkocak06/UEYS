using Shared.Types;

namespace Core.Entities
{
    public class StudentCount : ExtendedBaseEntity
    {
        public int? RequestedCount { get; set; }
        public int? SecretaryAllocatedCount { get; set; }
        public int? BoardAllocatedCount { get; set; }
        public QuotaType? QuotaType { get; set; }

        public long? SubQuotaRequestId { get; set; }
        public SubQuotaRequest SubQuotaRequest { get; set; }
    }
}
