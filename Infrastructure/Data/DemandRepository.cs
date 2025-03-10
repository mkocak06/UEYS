using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class DemandRepository : EfRepository<Demand>, IDemandRepository
    {
        public DemandRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}