using Microsoft.AspNetCore.Http;
using Shared.Types;

namespace Shared.RequestModels
{
    public class FileUploadModelDTO
    {
        public DocumentTypes DocumentType { get; set; }
        public long EntityId { get; set; }
        public IFormFile File { get; set; }
    }
}
