using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SpecificEducationPlace : ExtendedBaseEntity
    {
        public string Name { get; set; }
        public Province Province { get; set; }
        public long? ProvinceId { get; set; }
        public virtual ICollection<StudentsSpecificEducation> StudentsSpecificEducations { get; set; }
    }
}
