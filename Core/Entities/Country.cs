using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Country : ExtendedBaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<User> Users { get; set; }
    }
}
