using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.University;
using Shared.Types;

namespace Infrastructure.Data
{
    public class HospitalRepository : EfRepository<Hospital>, IHospitalRepository
    {
        private readonly ApplicationDbContext dbContext;
        public HospitalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Hospital>> GetListByUniversityId(long uniId)
        {
            var hospitals = from h in dbContext.Hospitals
                            join f in dbContext.Faculties on h.FacultyId equals f.Id
                            where h.IsDeleted == false && f.IsDeleted == false && f.UniversityId == uniId
                            select h;

            var affiliatedHospitals = from f in dbContext.Faculties
                                      join a in dbContext.Affiliations on f.Id equals a.FacultyId
                                      where f.IsDeleted == false && a.IsDeleted == false && f.UniversityId == uniId
                                      select a.Hospital;

            return await hospitals.Union(affiliatedHospitals).OrderBy(x=>x.Name).ToListAsync();
        }

        public async Task<List<HospitalBreadcrumbDTO>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId)
        {
            return await (from h in dbContext.Hospitals
                          join p in dbContext.Programs.Where(x => x.IsDeleted == false && x.ExpertiseBranchId == expertiseBranchId) on h.Id equals p.HospitalId into ps
                          from pa in ps.DefaultIfEmpty()
                          where h.IsDeleted == false && pa.Id != null
                          select new HospitalBreadcrumbDTO { Id = h.Id, Name = h.Name, ProgramId = pa == null ? null : pa.Id }).AsNoTracking().Distinct().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        }

        public async Task<Hospital> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Hospitals.AsSplitQuery()
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Institution)
                //.Include(x => x.Manager)
                .Include(x=>x.Province)
                .FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id,cancellationToken);
        }

        public async Task<List<MapDTO>> GetListForMap(CancellationToken cancellationToken, long? universityId)
        {
            var universites = dbContext.Universities.Where(x => x.IsDeleted == false).Select(x => new MapDTO
            {
                Id = x.Id,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
                Type = Shared.Types.MapInstitutionType.University
            });
            var hospitals = dbContext.Hospitals.Where(x => x.IsDeleted == false).Select(x => new MapDTO
            {
                Id = x.Id,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
                Type = Shared.Types.MapInstitutionType.Hospital
            });
            if (universityId != null && universityId != 0)
            {
                universites = universites.Where(x => x.Id == universityId.Value);
                hospitals = from u in dbContext.Universities
                            join p in dbContext.Provinces.Include(x=>x.Hospitals) on u.ProvinceId equals p.Id
                            join h in dbContext.Hospitals on p.Id equals h.ProvinceId
                            where u.IsDeleted == false && h.IsDeleted == false && u.Id == universityId.Value
                            select new MapDTO
                            {
                                Id = h.Id,
                                Latitude = h.Latitude,
                                Longitude = h.Longitude,
                                Name = h.Name,
                                Type = Shared.Types.MapInstitutionType.Hospital
                            };
            }
            var result = await universites.Union(hospitals).OrderBy(x=>x.Name).ToListAsync(cancellationToken);

            return result;
        }

        public async Task<UserHospitalDetailDTO> GetUserHospitalDetail(CancellationToken cancellationToken, long userId)
        {
            return await (from u in dbContext.Users
                          join e in dbContext.Educators on u.Id equals e.UserId
                          join ep in dbContext.EducatorPrograms.Where(x=>x.DutyEndDate == null) on e.Id equals ep.EducatorId
                          join p in dbContext.Programs.Include(x => x.Students.Where(x => x.IsDeleted == false)).Include(x => x.EducatorPrograms) on ep.ProgramId equals p.Id
                          join h in dbContext.Hospitals.Include(x => x.Programs.Where(x => x.IsDeleted == false)) on p.HospitalId equals h.Id
                          where u.Id == userId
                              && e.IsDeleted == false
                              && p.IsDeleted == false
                              && h.IsDeleted == false
                          select new UserHospitalDetailDTO
                          {
                              Address = h.Address,
                              HospitalId = h.Id,
                              HospitalName = h.Name,
                              Latitude = h.Latitude,
                              Longitude = h.Longitude,
                              NumberOfEducator = p.EducatorPrograms.Count,
                              NumberOfProgram = h.Programs.Count,
                              NumberOfStudent = p.Students.Count
                          }).FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<Hospital> QueryableHospitals()
        {
            return dbContext.Hospitals.AsQueryable()
                                         .Include(x => x.Institution)
                                         .Include(x => x.Province)
                                         .Where(x => x.IsDeleted == true);

        }

        public IQueryable<HospitalChartModel> QueryableHospitalsForCharts(ZoneModel zone)
        {
            IQueryable<Hospital> hospitals = dbContext.Hospitals.Where(x => x.IsDeleted == false);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Hospital>();

                if (zone.Hospitals != null && zone.Hospitals.Any())
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => hospitalIds.Contains(x.Id));
                    hospitals = hospitals.Where(predicates);
                }
            }
            return hospitals.Select(x => new HospitalChartModel()
            {
                Id = x.Id,
                ProvinceName = x.Province.Name,
                ProvinceId = x.Province.Id,
                HospitalName = x.Name,
                HospitalId = x.Id,
                UniversityName = x.Faculty.University.Name,
                UniversityId = x.Faculty.University.Id,
                FacultyName = x.Faculty.Name,
                FacultyId = x.Faculty.Id,
                ProfessionName = x.Faculty.Profession.Name,
                ProfessionId = x.Faculty.ProfessionId,
                IsDeleted = x.IsDeleted,
                IsUniversityPrivate = x.Faculty.University.IsPrivate,
                ParentInstitutionId = x.InstitutionId,
                ParentInstitutionName = x.Institution.Name,
            });

        }
        public long HospitalCountByInstitution(ZoneModel zone, long parentInstitutionId)
        {
            IQueryable<Hospital> hospitals = dbContext.Hospitals.Where(x => x.IsDeleted == false);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                if (zone.Hospitals != null && zone.Hospitals.Any())
                {
                    return zone.Hospitals.Count(x=> x.InstitutionId == parentInstitutionId);
                }
                else return 0;
            }

            return hospitals.Count(x => x.InstitutionId == parentInstitutionId);
        }
    }
}
