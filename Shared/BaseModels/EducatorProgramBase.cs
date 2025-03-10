using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.BaseModels
{
    public class EducatorProgramBase
    {
        public long? Id { get; set; }
        public long? EducatorId { get; set; }
        public long? ProgramId { get; set; }
        public DutyType? DutyType { get; set; }
        public DateTime? DutyStartDate { get; set; }
        public DateTime? DutyEndDate { get; set; }
        public string DocumentOrder { get; set; }
        public bool? IsEducationOfficer { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public virtual List<DocumentResponseDTO> EducationOfficerDocuments { get; set; }
        public DateTime? EducationOfficerDutyStartDate { get; set; }
        public DateTime? EducationOfficerDutyEndDate { get; set; }
        public DateTime? LastDutyEndDate { get; set; }
        public long? EducationOfficerId { get; set; }
    }
}
