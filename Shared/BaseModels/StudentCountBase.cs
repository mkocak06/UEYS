using Shared.Types;

namespace Shared.BaseModels
{
    public class StudentCountBase
    {
        public QuotaType? QuotaType { get; set; }
        public int? RequestedCount { get; set; }
        public int? SecretaryAllocatedCount { get; set; }
        public int? BoardAllocatedCount { get; set; }

        public long? SubQuotaRequestId { get; set; }
    }
}
