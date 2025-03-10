using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class StudentSpecificEducationBase 
    {
        public long? SpecificEducationPlaceId { get; set; }
        public long? StudentId { get; set; }
        public long? SpesificEducationId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
