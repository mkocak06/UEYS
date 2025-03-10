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
    public class StudentPerfectionRepository : EfRepository<StudentPerfection>, IStudentPerfectionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StudentPerfectionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<StudentPerfection> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.StudentPerfections.AsSplitQuery()
                                           .Include(x => x.Student)
                                           .Include(x => x.Perfection)
                                           .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false && x.Student.IsDeleted == false && x.Perfection.IsDeleted == false, cancellationToken);
        }

        public async Task<StudentPerfection> GetByStudentAndPerfection(CancellationToken cancellationToken, long studentId, long perfectionId)
        {
            return await dbContext.StudentPerfections.AsSplitQuery().AsNoTracking()
                                                     .Include(x => x.Educator).ThenInclude(x => x.User)
                                                     .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                                     .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                                     .Include(x=>x.Perfection)
                                                     .FirstOrDefaultAsync(x => x.StudentId == studentId && x.PerfectionId == perfectionId && x.IsDeleted == false, cancellationToken);
        }

        public async Task<List<StudentPerfectionResponseDTO>> GetListByStudentId(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType)
        {
            return await dbContext.StudentPerfections.Where(x => x.StudentId == studentId && x.IsDeleted == false && x.Perfection.PerfectionType == perfectionType).Select(x => new StudentPerfectionResponseDTO
            {
                Perfection = new PerfectionResponseDTO
                {
                    PName = new PropertyResponseDTO { Name = x.Perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName).Name }
                },
                Educator = new EducatorResponseDTO { User = new UserAccountDetailInfoResponseDTO { Name = x.Educator.User.Name } },
                Program = new ProgramResponseDTO
                {
                    Hospital = new HospitalResponseDTO { Name = x.Program.Hospital.Name, Province = new ProvinceResponseDTO { Name = x.Program.Hospital.Province.Name } },
                    ExpertiseBranch = new ExpertiseBranchResponseDTO { Name = x.Program.ExpertiseBranch.Name }
                },
                Experience = x.Experience,
                ProcessDate = x.ProcessDate,
                IsSuccessful = x.IsSuccessful,
                Id= x.Id,
                PerfectionId= x.PerfectionId,
            }).ToListAsync(cancellationToken);
        }

        public async Task<List<StudentPerfectionResponseDTO>> GetListByStudentIdWithoutType(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.StudentPerfections.Where(x => x.StudentId == studentId && x.IsDeleted == false).Select(x => new StudentPerfectionResponseDTO
            {
                Perfection = new PerfectionResponseDTO
                {
                    PName = new PropertyResponseDTO { Name = x.Perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName).Name }
                },
                Educator = new EducatorResponseDTO { User = new UserAccountDetailInfoResponseDTO { Name = x.Educator.User.Name } },
                Program = new ProgramResponseDTO
                {
                    Hospital = new HospitalResponseDTO { Name = x.Program.Hospital.Name, Province = new ProvinceResponseDTO { Name = x.Program.Hospital.Province.Name } },
                    ExpertiseBranch = new ExpertiseBranchResponseDTO { Name = x.Program.ExpertiseBranch.Name }
                },
                Experience = x.Experience,
                ProcessDate = x.ProcessDate,
                IsSuccessful = x.IsSuccessful,
                Id = x.Id,
                PerfectionId = x.PerfectionId,
            }).ToListAsync(cancellationToken);
        }
    }
}
