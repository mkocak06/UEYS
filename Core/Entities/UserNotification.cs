using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserNotification : ExtendedBaseEntity
    {
        public bool? IsRead { get; set; } = false;

        public long? UserId { get; set; }
        public User User { get; set; }
        public long? NotificationId { get; set; }
        public Notification Notification { get; set; }
    }
}
