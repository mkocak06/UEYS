using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class FileResponseDTO
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string FileUrl { get; set; }
        public byte[] FileContent { get; set; }
    }
}
