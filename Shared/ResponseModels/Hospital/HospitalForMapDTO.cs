using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Hospital
{
    public class HospitalForMapDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }
}
