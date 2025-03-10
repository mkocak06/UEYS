using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StudentPerfection : ExtendedBaseEntity
    {
        public DateTime? ProcessDate { get; set; }
        public int? Experience { get; set; }
        public bool? IsSuccessful { get; set; }

        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public long? PerfectionId { get; set; }
        public virtual Perfection Perfection { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
    }
}
