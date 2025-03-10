using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class AccountInfoResponseDTO
    {
        public long AdminId { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public bool Result { get; set; }
        public SerializationInfo Message { get; set; }
    }
}
