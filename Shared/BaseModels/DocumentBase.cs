using Shared.Types;

namespace Shared.BaseModels
{
    public class DocumentBase
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public bool? IsApprove { get; set; }
        public string FileUrl { get; set; }
        public byte[] FileContent { get; set; }
        public string ServiceUrl { get; set; }
        public DocumentTypes DocumentType { get; set; }
        public string BucketKey { get; set; }
        public long EntityId { get; set; }
    }
}
