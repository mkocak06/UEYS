using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class UserNotificationResponseDTO : UserNotificationBase
    {
        public NotificationResponseDTO Notification { get; set; }
        public UserResponseDTO User { get; set; }

    }
}
