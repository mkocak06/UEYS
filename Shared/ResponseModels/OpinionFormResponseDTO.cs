using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class OpinionFormResponseDTO : OpinionFormBase
    {
        
        public StudentResponseDTO Student { get; set; }
        public EducatorResponseDTO Educator { get; set; }
        public EducatorResponseDTO ProgramManager { get; set; }
        public ProgramResponseDTO Program { get; set; }

        public UserResponseDTO Secretary { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public string OverallAverage => Shared.Extensions.EnumExtension.GetAverage(new int?[] {ComplianceToWorkingHours_DC,DutyResponsibility_DC,DutyExecution_DC,DutyAccomplishment_DC,
                                    ProblemAnalysisAndSolutionAbility,OrganizationAndCoordinationAbility,CommunicationSkills,
                                    RelationsWithOtherStudents, RelationsWithEducators, RelationsWithOtherEmployees, RelationsWithPatients,
                                    ProfessionalPracticeAbility, Scientificness, TeamworkAdaptation, ResearchDesire, ResearchExecutionAndAccomplish, UsingResourcesEfficiently, 
                                    BroadcastingAbility });
    }

}
