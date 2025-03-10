using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.BaseModels
{
    public class ThesisDefenceBase
    {
        public long? Id { get; set; }
        public string Description { get; set; }
        public DateTime? ExamDate { get; set; }
        //public DateTime? ResultDate { get; set; }
        public DefenceResultType? Result { get; set; }
        public int? DefenceOrder { get; set; }
        public bool IsConcluded { get; set; }

        public long? ThesisId { get; set; }
        public long? HospitalId { get; set; }

        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }

    }
}
