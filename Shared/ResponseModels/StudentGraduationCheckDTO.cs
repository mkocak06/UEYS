using Shared.Models;
using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class StudentGraduationCheckDTO
    {
        public long Id { get; set; }
        public string IdentityNo { get; set; }
        public long? OriginalProgramExpBrId { get; set; }
        public string Name { get; set; }
        public string OriginalProgramName { get; set; }
    }
}
