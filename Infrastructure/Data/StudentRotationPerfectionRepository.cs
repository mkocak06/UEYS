using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels;
using Shared.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StudentRotationPerfectionRepository : EfRepository<StudentRotationPerfection>, IStudentRotationPerfectionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StudentRotationPerfectionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
