using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Infrastructure.Data
{
    public class OfficialLetterRepository : EfRepository<OfficialLetter>, IOfficialLetterRepository
    {
        public OfficialLetterRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
