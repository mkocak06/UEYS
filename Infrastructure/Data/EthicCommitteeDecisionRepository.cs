using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class EthicCommitteeDecisionRepository : EfRepository<EthicCommitteeDecision>, IEthicCommitteeDecisionRepository
    {
        private readonly ApplicationDbContext dbContext;
        public EthicCommitteeDecisionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
