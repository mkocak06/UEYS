using System;

namespace Shared.BaseModels
{
    public class RotationBase
    {
        public string Duration { get; set; }
        public bool? IsRequired { get; set; }

        public long? ExpertiseBranchId { get; set; }
    }
}
