using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class LogResponseDTO
    {
        public long Id { get; set; }
        public LogType LogType { get; set; }
        public string Message { get; set; }
        public long? UserId { get; set; }
        public UserResponseDTO User { get; set; }
        public string UserIPAddress { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
