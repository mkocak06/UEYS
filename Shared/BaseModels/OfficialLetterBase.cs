using System;

namespace Shared.BaseModels
{
    public class OfficialLetterBase
    {
        public long? Id { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }

        public long? ThesisId { get; set; }
    }
}
