using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class ThesisBase
    {
        public long? Id { get; set; }

        public string Subject { get; set; }
        public DateTime? SubjectDetermineDate { get; set; }
        public ThesisSubjectType_1? ThesisSubjectType_1 { get; set; }
        public ThesisSubjectType_2? ThesisSubjectType_2 { get; set; }

        public string ThesisTitle { get; set; }
        public DateTime? ThesisTitleDetermineDate { get; set; }

        public DateTime? DeadLine { get; set; }
        public DateTime? UploadDate { get; set; }

        public ThesisStatusType? Status { get; set; }
        public bool IsDeleted { get; set; }
        public string DeleteReasonExplanation { get; set; }

        public long? StudentId { get; set; }
    }
}
