using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class NotificationResponseDTO : NotificationBase
    {
        public long? Id { get; set; }

        public virtual UserResponseDTO User { get; set; }
    }
}
