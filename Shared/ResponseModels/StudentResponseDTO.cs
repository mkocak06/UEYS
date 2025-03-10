using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class StudentResponseDTO : StudentBase
    {
        public long Id { get; set; }

        public virtual UserAccountDetailInfoResponseDTO User { get; set; }
        public virtual ProgramResponseDTO Program { get; set; }
        public virtual ProgramResponseDTO TransferProgram { get; set; }
        public virtual ProgramResponseDTO OriginalProgram { get; set; }
		public virtual EducatorResponseDTO Advisor { get; set; }
        public virtual CurriculumResponseDTO Curriculum { get; set; }

        public virtual List<ThesisResponseDTO> Theses { get; set; }
        public virtual List<PerformanceRatingResponseDTO> PerformanceRatings { get; set; }
        public virtual List<OpinionFormResponseDTO> OpinionForms { get; set; }
        public virtual List<EducationTrackingResponseDTO> EducationTrackings { get; set; }
        public virtual List<StudentExpertiseBranchResponseDTO> StudentExpertiseBranches { get; set; }
        public virtual List<StudentRotationResponseDTO> StudentRotations { get; set; }
        public virtual List<StudentPerfectionResponseDTO> StudentPerfections { get; set; }
        public virtual List<ScientificStudyResponseDTO> ScientificStudies { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public virtual List<ExitExamResponseDTO> ExitExams{ get; set; }
        public bool? StartedOverWithExamForSameBranch { get; set; }
    }
}
