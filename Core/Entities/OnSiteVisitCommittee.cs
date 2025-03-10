using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OnSiteVisitCommittee : BaseEntity
    {
        public long? UserId { get; set; }
        public virtual User User { get; set; }
        public long? FormId { get; set; }
        public virtual Form Form { get; set; }
    }
}
