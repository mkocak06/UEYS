using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class AnnouncementRepository : EfRepository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}