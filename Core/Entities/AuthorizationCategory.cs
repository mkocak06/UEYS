using System.Collections.Generic;

namespace Core.Entities
{
    public class AuthorizationCategory : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ColorCode { get; set; }
        public bool? IsActive { get; set; }
        public long Duration { get; set; }
        public bool? IsQuotaRequestable { get; set; }
        public virtual ICollection<AuthorizationDetail> AuthorizationDetails { get; set; }
    }
}
