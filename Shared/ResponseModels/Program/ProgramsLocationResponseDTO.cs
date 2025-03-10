using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Program
{
    public class ProgramsLocationResponseDTO
    {
        public long Id { get; set; }
        public HospitalResponseDTO Hospital { get; set; }
        public AuthorizationCategoryResponseDTO ActiveAuthorizationDetailsAuthorizationCategory { get; set; }
    }
}
