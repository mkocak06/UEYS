using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class StudentDependentProgramPaginateDTO
    {
        public long? Id { get; set; }

        public string ProgramName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsUnCompleted { get; set; }
        public int? RemainingDays { get; set; }
        public int? Duration { get; set; }
        public string Explanation { get; set; }
        public int? CalculatedRemainingDays { get; set; }
        public long? StudentId { get; set; }
        public long? DependentProgramId { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }

    }
}
