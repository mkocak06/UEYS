namespace Core.Entities
{
    public class PerfectionProperty : BaseEntity
    {
        public long? PerfectionId { get; set; }
        public Perfection Perfection { get; set; }
        public long? PropertyId { get; set; }
        public Property Property { get; set; }
    }
}