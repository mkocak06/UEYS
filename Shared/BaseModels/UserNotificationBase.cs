using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class UserNotificationBase
    {
        public bool? IsRead { get; set; }

        public long? UserId { get; set; }
        public long? NotificationId { get; set; }
    }
}
