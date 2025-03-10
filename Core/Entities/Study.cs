using System.Collections.Generic;

namespace Core.Entities
{
    public class Study : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ScientificStudy> ScientificStudies { get; set; }
    }
}
