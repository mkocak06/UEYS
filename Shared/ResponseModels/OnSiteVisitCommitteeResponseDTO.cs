using Shared.BaseModels;
using Shared.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class OnSiteVisitCommitteeResponseDTO : OnSiteVisitCommitteeBase
    {
        public virtual long Id { get; set; }
        public virtual UserResponseDTO User { get; set; }
        public virtual FormResponseDTO Form { get; set; }
    }
}
