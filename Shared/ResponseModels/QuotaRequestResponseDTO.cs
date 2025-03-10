using Shared.BaseModels;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class QuotaRequestResponseDTO : QuotaRequestBase
    {
        //public HospitalResponseDTO Hospital { get; set; }
        public long Id { get; set; }
        public ICollection<SubQuotaRequestResponseDTO> SubQuotaRequests { get; set; }
    }
}
