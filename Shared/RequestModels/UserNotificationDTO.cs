using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class UserNotificationDTO : UserNotificationBase
    {
        public NotificationDTO Notification { get; set; }

    }
}
