using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UserNotificationRepository : EfRepository<UserNotification>, IUserNotificationRepository 
    {
        private readonly ApplicationDbContext dbContext;
        public UserNotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
