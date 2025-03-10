using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class StudentCountResponseDTO : StudentCountBase
    {
        public long? Id { get; set; }
        public SubQuotaRequestResponseDTO SubQuotaRequest{ get; set; }
    }
}
