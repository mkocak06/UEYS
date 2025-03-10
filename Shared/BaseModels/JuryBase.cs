using Shared.ResponseModels.Authorization;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class JuryBase
    {
        public long? Id { get; set; }
        public JuryType JuryType{ get; set; }

        public long? EducatorId { get; set; }
        public long? ThesisDefenceId { get; set; }
        public long? UserId { get; set; }
        public long? ExitExamId { get; set; }


        public ZoneStudentModel Zone { get; set; }
    }
}
