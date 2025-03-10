using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StandardCategory : ExtendedBaseEntity
    {
        public string CategoryCode { get; set; }
        public string Name {  get; set; }
        public virtual ICollection<Standard> Standards { get; set;}
    }
}
