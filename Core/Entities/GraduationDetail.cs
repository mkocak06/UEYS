using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class GraduationDetail : ExtendedBaseEntity
    {
        public string HigherEducationDetail { get; set; }
        public string GraduationUniversity { get; set; }
        public string GraduationFaculty { get; set; }
        public string GraduationField { get; set; }
        public string GraduationDate { get; set; }
        public string DiplomaNumber { get; set; }
        public bool? IsPhd { get; set; }

        public long? EducatorId { get; set; }
        public Educator Educator { get; set; }
    }
}
