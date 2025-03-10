using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class InstitutionRepository : EfRepository<Institution>, IInstitutionRepository
    {
        public InstitutionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
 