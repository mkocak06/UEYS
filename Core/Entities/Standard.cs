using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Standard : ExtendedBaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public long? StandardCategoryId { get; set; }
        public virtual StandardCategory StandardCategory { get; set; }

        public long? CurriculumId { get; set; }
        public virtual Curriculum Curriculum { get; set; }

        public virtual ICollection<FormStandard> FormStandards { get; set; }
    }
}
