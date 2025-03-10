using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EducatorAdministrativeTitle : BaseEntity
    {
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
        public long? AdministrativeTitleId { get; set; }
        public virtual Title AdministrativeTitle { get; set; }
    }
}
