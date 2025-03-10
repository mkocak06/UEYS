using Shared.BaseModels;
using Shared.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class FormResponseDTO : FormBase
    {
        public virtual long Id { get;  set; }
        public virtual ProgramResponseDTO Program { get; set; }
        public virtual AuthorizationDetailResponseDTO AuthorizationDetail { get; set; }
        public virtual List<OnSiteVisitCommitteeResponseDTO> OnSiteVisitCommittees { get; set; }
        public virtual List<FormStandardResponseDTO> FormStandards { get; set; }
        public virtual List<DocumentResponseDTO> CommitteeDocuments { get; set; }
        public virtual List<DocumentResponseDTO> TUKDocuments { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? NumberOfEducator { get; set; }
        public int? NumberOfStudent { get; set; }
        public string CurrentAuthorizationCategory { get; set; }
        public string CurriculumName { get; set; }
    }
}
