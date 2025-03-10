using System.Collections.Generic;

namespace Core.Entities
{
    public class SpecificEducation : ExtendedBaseEntity
    {
        public string Name { get; set; }

        public long? CurriculumId { get; set; }
        public virtual Curriculum Curriculum { get; set; }

        public virtual ICollection<StudentsSpecificEducation> StudentsSpecificEducations { get; set; }
    }
}
