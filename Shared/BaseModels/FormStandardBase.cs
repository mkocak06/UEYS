using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class FormStandardBase
    {
        public bool? InstitutionStatement { get; set; } 
        public bool? CommitteeStatement { get; set; } 
        public string CommitteeDescription { get; set; }
        public long? StandardId { get; set; }
        public long? FormId { get; set; }
    }
}
