using Shared.BaseModels;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestModels
{
    public class FormDTO : FormBase
    {
        public virtual List<FormStandardDTO> FormStandards { get; set; }
        public virtual List<OnSiteVisitCommitteeDTO> OnSiteVisitCommittees { get; set; }
    }
}
