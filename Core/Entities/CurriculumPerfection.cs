namespace Core.Entities
{
    public class CurriculumPerfection : ExtendedBaseEntity
    {
        public long? CurriculumId { get; set; }
        public virtual Curriculum Curriculum { get; set; }
        public long? PerfectionId { get; set; }
        public virtual Perfection Perfection { get; set; }
    }
}
