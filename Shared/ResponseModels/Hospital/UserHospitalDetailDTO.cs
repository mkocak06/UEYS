using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Hospital
{
    public class UserHospitalDetailDTO
    {
        public long HospitalId { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? NumberOfProgram { get; set; }
        public int? NumberOfEducator { get; set; }
        public int? NumberOfStudent { get; set; }
    }
}
