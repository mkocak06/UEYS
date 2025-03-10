using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class StudentRotationPerfectionResponseDTO : StudentRotationPerfectionBase
    {
        public long? Id { get; set; }
        public virtual StudentRotationResponseDTO StudentRotation { get; set; }
        public virtual PerfectionResponseDTO Perfection { get; set; }
        public virtual EducatorResponseDTO Educator { get; set; }
    }
}
