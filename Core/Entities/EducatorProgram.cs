using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EducatorProgram : BaseEntity
    {
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }

        public DutyType DutyType { get; set; }
        public DateTime? DutyStartDate { get; set; }
        public DateTime? DutyEndDate { get; set; }
        public string DocumentOrder { get; set; }

    }
}
