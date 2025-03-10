using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class AuthorizationDetail : ExtendedBaseEntity
    {
        public DateTime? AuthorizationDate { get; set; }
        public DateTime? AuthorizationEndDate { get; set; }
        public DateTime? VisitDate { get; set; }
        public string AuthorizationDecisionNo { get; set; }
        public string Description { get; set; }

        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }

        public long? AuthorizationCategoryId { get; set; }
        public virtual AuthorizationCategory AuthorizationCategory { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
    }
}
