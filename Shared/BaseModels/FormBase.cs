using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class FormBase
    {
   
        public long? AuthorizationDetailId { get; set; }
        public long? ProgramId { get; set; }
        public bool IsOnSiteVisit { get; set; }
        public CommitteeOpinionType? CommitteeOpinionType { get; set; }
        public VisitStatusType? VisitStatusType { get; set; }
        public string CommitteeDescription { get; set; }
        public string TUKDescription { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime? ApplicationDate { get; set; }
    }
}
