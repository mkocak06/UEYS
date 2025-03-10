using Shared.Types;

namespace Core.Entities
{
    public class Jury : ExtendedBaseEntity
    {
        public JuryType JuryType { get; set; }

        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
        public long? UserId { get; set; }
        public virtual User User { get; set; }
        public long? ThesisDefenceId { get; set; }
        public virtual ThesisDefence ThesisDefence { get; set; }
        public long? ExitExamId { get; set; }
        public virtual ExitExam ExitExam { get; set; }
    }
}
