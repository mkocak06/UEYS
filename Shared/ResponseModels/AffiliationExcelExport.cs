using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class AffiliationExcelExport
    {
        public long Id { get; set; }
        public string UniversityName { get; set; }
        public string FacultyName { get; set; }
        public string HospitalName { get; set; }
        public string ProtocolNo { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public DateTime? ProtocolEndDate { get; set; }
        public int StudentCount { get; set; }
        public int EducatorCount { get; set; }
    }
}
