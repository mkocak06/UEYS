using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Profession : BaseEntity
    {
        public string Name { get; set; }
        public int Code { get; protected set; }
        public virtual ICollection<ExpertiseBranch> ExpertiseBranches { get; set; }
        public virtual ICollection<Faculty> Faculties{ get; set; }
    }    
}
