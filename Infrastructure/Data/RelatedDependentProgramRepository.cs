using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class RelatedDependentProgramRepository : EfRepository<RelatedDependentProgram>, IRelatedDependentProgramRepository
    {
        public RelatedDependentProgramRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
 