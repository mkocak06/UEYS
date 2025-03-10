using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StudentRotationPerfection : BaseEntity
    {
        public DateTime? ProcessDate { get; set; }
        public bool? IsSuccessful { get; set; }
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
        public long? PerfectionId { get; set; }
        public virtual Perfection Perfection { get; set; }
        public long? StudentRotationId { get; set; }
        public virtual StudentRotation StudentRotation { get; set; }
    }
}
