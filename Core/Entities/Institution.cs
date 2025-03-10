using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Institution : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Hospital> Hospitals { get; set; }
        public virtual ICollection<University> Universities { get; set; }
        public virtual ICollection<User> Users { get; set; }
        //public virtual ICollection<Educator> StaffEducators { get; set; }
    }
}
