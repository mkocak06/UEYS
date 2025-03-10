using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class StudentPerfectionResponseDTO : StudentPerfectionBase
    {
        public long? Id { get; set; }
        public virtual StudentResponseDTO Student { get; set; }
        public virtual PerfectionResponseDTO Perfection { get; set; }
        public virtual ProgramResponseDTO Program { get; set; }
        public virtual EducatorResponseDTO Educator { get; set; }
    }
}
