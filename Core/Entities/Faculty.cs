using Core.Entities.Koru;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Faculty : ExtendedBaseEntity
    {
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }

        public long UniversityId { get; set; }
        public virtual University University { get; set; }
        public long? ProfessionId { get; set; }
        public virtual Profession Profession { get; set; }

        public virtual ICollection<Affiliation> Affiliations { get; set; }
        public virtual ICollection<Program> Programs { get; set; }
        public virtual ICollection<Hospital> Hospitals { get; set; }
        public virtual ICollection<UserRoleFaculty> UserRoleFaculties { get; set; }
    }
}
