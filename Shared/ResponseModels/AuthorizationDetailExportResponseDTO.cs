using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class AuthorizationDetailExportResponseDTO
    {
        public DateTime? AuthorizationEndDate { get; set; }
        public DateTime? AuthorizationDecisionDate { get; set; }
        public DateTime? VisitDate { get; set; }
        public string AuthorizationDecisionNo { get; set; }
        public bool? AuthorizationCategoryIsActive { get; set; }
        public string AuthorizationCategory { get; set; }
        public string ColorCode { get; set; }
    }
}
