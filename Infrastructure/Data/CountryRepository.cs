using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class CountryRepository : EfRepository<Country>, ICountryRepository
    {
        public CountryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}