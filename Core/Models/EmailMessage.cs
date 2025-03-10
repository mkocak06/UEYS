using System.Collections.Generic;
using System.IO;

namespace Core.Models
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            AttachedFiles = new();
        }

        public List<string> To { get; set; } = new();
        public List<string> CC { get; set; } = new();
        public List<string> Bcc { get; set; } = new();
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<AttachedFile> AttachedFiles { get; set; }
    }
    public class AttachedFile
    {
        public MemoryStream AttachmentStream { get; set; }
        public string AttachmentTitle { get; set; }
        public string ContentType { get; set; }
    }
}
