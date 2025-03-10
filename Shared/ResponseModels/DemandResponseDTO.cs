using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class DemandResponseDTO
    {
        public string Subject { get; set; }
        public bool IsOpen { get; set; }

        public long UserId { get; set; }
        public UserResponseDTO User { get; set; }
    }
}
