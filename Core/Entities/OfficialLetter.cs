using System;

namespace Core.Entities
{
    public class OfficialLetter : ExtendedBaseEntity
    {
        public string Description { get; set; }
        public DateTime? Date { get; set; }

        public long? ThesisId { get; set; }
        public virtual Thesis Thesis { get; set; }
    }
}
