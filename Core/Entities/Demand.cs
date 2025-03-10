using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Demand : ExtendedBaseEntity
    {
        public string Subject { get; set; }
        public bool IsOpen { get; set; }

        public long UserId { get; set; }
        public User User{ get; set; }
    }
}
