using Shared.Types;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Form : ExtendedBaseEntity
    {
        public CommitteeOpinionType? CommitteeOpinionType { get; set; }
        public VisitStatusType? VisitStatusType { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string CommitteeDescription { get; set; }
        public string TUKDescription { get; set; }
        public bool IsOnSiteVisit { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public long? AuthorizationDetailId { get; set; }
        public virtual AuthorizationDetail AuthorizationDetail { get; set; }
        public virtual ICollection<OnSiteVisitCommittee> OnSiteVisitCommittees { get; set; }
        public virtual ICollection<FormStandard> FormStandards { get; set; }
    }
}
