using System;

namespace Core.Entities
{
    public class Announcement : ExtendedBaseEntity
    {
        public string Title { get; set; }
        public string Explanation { get; set; }
        public DateTime? PublishDate{ get; set; }

    }
}
