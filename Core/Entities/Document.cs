using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Document : ExtendedBaseEntity
    {
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
