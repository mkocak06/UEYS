using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StudentsSpecificEducation : ExtendedBaseEntity
    {
        public long? SpesificEducationId { get; set; }
        public virtual SpecificEducation SpecificEducation { get; set; }    

        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }    

        public long? SpecificEducationPlaceId { get; set; }
        public virtual SpecificEducationPlace SpecificEducationPlace { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
