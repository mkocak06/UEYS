using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Affiliation : ExtendedBaseEntity
    {
        public string ProtocolNo { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public DateTime? ProtocolEndDate { get; set; }

        public long? FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }
        public long? HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }
    }
}
