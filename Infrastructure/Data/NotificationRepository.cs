using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class NotificationRepository : EfRepository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext dbContext;
        public NotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
