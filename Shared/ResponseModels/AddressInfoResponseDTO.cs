using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class AddressInfoResponseDTO
    {
        public string Address { get; set; }
        public int? ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}
