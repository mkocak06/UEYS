using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels.StatisticModels;
using Shared.Types;

namespace Infrastructure.Data
{
    public class ProvinceRepository : EfRepository<Province>, IProvinceRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ProvinceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<CityDetailsForMapModel>> DetailsForMap(CancellationToken cancellationToken)
        {
            return await dbContext.Provinces.Select(x => new CityDetailsForMapModel()
            {
                ProvinceName = x.Name,
                ProgramCount = x.Hospitals.Where(x => x.IsDeleted == false).SelectMany(x => x.Programs.Where(x => x.IsDeleted == false && x.AuthorizationDetails.Any(x => x.AuthorizationCategory.IsActive == true))).Count(),
                UniversityCount = x.Universities.Where(x => x.IsDeleted == false).Count(),
                HospitalCount = x.Hospitals.Where(x => x.IsDeleted == false && x.Programs.Any(x=>x.AuthorizationDetails.Any(x=>x.AuthorizationCategory.IsActive == true))).Count(),
                FacultyCount = x.Universities.Where(x => x.IsDeleted == false).SelectMany(x => x.Faculties).Where(x => x.IsDeleted == false).Count(),
                StudentCount = x.Hospitals.Where(x => x.IsDeleted == false).SelectMany(x => x.Programs.Where(x => x.IsDeleted == false /*&& x.AuthorizationDetails.Any(x => x.AuthorizationCategory.IsActive == true)*/))
                                                                          .SelectMany(x => x.OriginalStudents.Where(x => !x.IsDeleted && !x.IsHardDeleted && !x.User.IsDeleted && x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration && x.Status != StudentStatus.EducationEnded))
                                                                          .Count(),
                EducatorCount = x.Hospitals.Where(x => x.IsDeleted == false).SelectMany(x => x.Programs.Where(x => x.IsDeleted == false /*&& x.AuthorizationDetails.Any(x => x.AuthorizationCategory.IsActive == true)*/))
                                                                           .SelectMany(x => x.EducatorPrograms.Where(x => !x.Educator.IsDeleted && !x.Educator.User.IsDeleted && (x.DutyEndDate == null || DateTime.UtcNow <= x.DutyEndDate) && x.Educator.EducatorType == EducatorType.Instructor))
                                                                           .Select(x => x.Educator).Distinct()
                                                                           .Count(),
            }).ToListAsync(cancellationToken);
        }
    }
}
