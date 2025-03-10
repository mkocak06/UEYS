using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StudentsSpecificEducationRepository : EfRepository<StudentsSpecificEducation>, IStudentsSpecificEducationRepository
    {
        public StudentsSpecificEducationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
