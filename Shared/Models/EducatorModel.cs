using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models 
{ 
    public class EducatorModel
    {
        public string kadroUnvani { get; set; }
        public string akademikUnvani { get; set; }
        public List<ustKurum> ustKurumlar { get; set; }
    }

    public class ustKurum
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string StaffParentName { get; set; }
    }
}
