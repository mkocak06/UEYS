using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;
using Shared.Types;

namespace Shared.ResponseModels
{
    public class ProgramResponseDTO : ProgramBase
    {
        public virtual FacultyResponseDTO Faculty { get; set; }
        public virtual HospitalResponseDTO Hospital { get; set; }
        public virtual ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
        public virtual AffiliationResponseDTO Affiliation { get; set; }
        public virtual ProtocolProgramResponseDTO ProtocolProgram { get; set; }
        public virtual ICollection<EducatorProgramResponseDTO> EducatorPrograms { get; set; }
        public virtual ICollection<EducationOfficerResponseDTO> EducationOfficers{ get; set; }
        public virtual ICollection<AuthorizationDetailResponseDTO> AuthorizationDetails { get; set; }
        public string Name
        {
            get
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Hospital?.Province?.Name?.ToLower() ?? "") + " - " /*+ this.Faculty?.University?.Name + " - " + this.Faculty?.Name + " - " */+ this.Hospital?.Name + " - " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.ExpertiseBranch?.Name?.ToLower() ?? "");
            }
        }
        public List<long> PrincipalBrancIdList { get; set; }
        public bool? IsMainProgram { get; set; }
        public bool? IsDependentProgram { get; set; }
        public ProgramType? ProtocolOrComplement { get; set; }
        public string ManuelProgramName{ get; set; }

    }
}
