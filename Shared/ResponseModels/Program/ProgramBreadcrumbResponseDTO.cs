using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.University;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Program
{
    public class ProgramBreadcrumbResponseDTO
    {
        public ProgramResponseDTO Program { get; set; }
        public IList<UniversityBreadcrumbDTO> Universities { get; set; }
        public IList<HospitalBreadcrumbDTO> Hospitals { get; set; }
        public IList<ProgramExpertiseBreadcrumbResponseDTO> RelatedPrograms { get; set; }
    }
}
