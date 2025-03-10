using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class RelatedExpertiseBranchRepository : IRelatedExpertiseBranchRepository
    {
        private readonly ApplicationDbContext dbContext;

        public RelatedExpertiseBranchRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(CancellationToken cancellationToken, RelatedExpertiseBranch relatedExpertiseBranch)
        {
            await dbContext.RelatedExpertiseBranches.AddAsync(relatedExpertiseBranch);
        }
    }
}