using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.University;
using Shared.Types;


namespace Infrastructure.Data
{
    public class UniversityRepository : EfRepository<University>, IUniversityRepository
    {
        private readonly ApplicationDbContext dbContext;
        public UniversityRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<University> PaginateQuery(ZoneModel zone)
        {
            IQueryable<University> universities = dbContext.Universities.Include(x => x.Province).Include(x=>x.Institution).Where(x => !x.IsDeleted);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<University>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => provinceIds.Contains(x.ProvinceId.Value));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => universityIds.Contains(x.Id));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.Faculties.Any(a => facultyIds.Contains(a.Id)));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.Faculties.SelectMany(f => f.Hospitals).Any(h => hospitalIds.Contains(h.Id)));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.Faculties.SelectMany(f => f.Hospitals).SelectMany(x=>x.Programs).Any(h => programIds.Contains(h.Id)));
                }
                universities = universities.Where(predicates);
            }
            return universities;
        }

        public async Task<University> GetByIdWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Universities.AsSplitQuery()
                .Include(x => x.Province)
                .Include(x => x.Institution)
                .Include(x => x.Faculties).ThenInclude(x => x.Profession)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<UniversityBreadcrumbDTO>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId)
        {
            return await (from u in dbContext.Universities
                          join f in dbContext.Faculties.Where(x => x.IsDeleted == false) on u.Id equals f.UniversityId into fs
                          from fa in fs.DefaultIfEmpty()
                          join p in dbContext.Programs.Where(x => x.IsDeleted == false && x.ExpertiseBranchId == expertiseBranchId) on fa.Id equals p.FacultyId into ps
                          from pa in ps.DefaultIfEmpty()
                          where u.IsDeleted == false
                          select new UniversityBreadcrumbDTO { Id = u.Id, Name = u.Name, ProgramId = pa == null ? null : pa.Id }).AsNoTracking().Distinct().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        }

        public IQueryable<University> QueryableUniversities(Expression<Func<University, bool>> predicate)
        {
            return dbContext.Universities.AsSplitQuery()
                                         .Include(x => x.Institution)
                                         .Include(x => x.Province)
                                         .Where(predicate);
        }

        public IQueryable<University> QueryableUniversitiesForAffilitation()
        {
            return dbContext.Universities.AsSplitQuery()
                                         .Include(x => x.Faculties.Where(x => x.IsDeleted == false && x.Affiliations.Any()))
                                         .ThenInclude(x => x.Affiliations.Where(x => x.IsDeleted == false && x.HospitalId != null && x.HospitalId > 0))
                                         .ThenInclude(x=>x.Hospital)
                                         .Where(x => x.IsDeleted == false && x.Faculties.Any(f => f.Affiliations.Any(x=>x.HospitalId != null && x.HospitalId > 0)));
        }

        public long UniversityCountByInstitution(ZoneModel zone, long parentInstitutionId)
        {
            IQueryable<University> universities = dbContext.Universities.Where(x => x.IsDeleted == false);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                if (zone.Universities != null && zone.Universities.Any())
                {
                    return zone.Universities.Count(x => x.InstitutionId == parentInstitutionId);
                }
                else return 0;
            }

            return universities.Count(x => x.InstitutionId == parentInstitutionId);
        }

        public List<ReportResponseDTO> GetReports()
        {
            return dbContext.Universities.AsSplitQuery()
                                         .Include(x => x.Institution)
                                         .Include(x => x.Province).Where(x => x.InstitutionId != null && x.IsDeleted == false)
             .GroupJoin(dbContext.Institutions,
                    x => x.InstitutionId,
                    i => i.Id,
                    (x, institutions) => new
                    {
                        ParentInstitution = institutions.FirstOrDefault(),
                        UniversityType = x.IsPrivate
                    }).AsEnumerable()
                    .GroupBy(x => new
                    {
                        ParentInstitution = x.ParentInstitution,
                        UniversityType = x.UniversityType
                    })
                    .Select(g => new
                    {
                        Key = g.Key.ParentInstitution != null && g.Key.ParentInstitution.Name.Contains("YÖK")
                            ? "YÖK/" + (g.Key.UniversityType == true ? "Vakýf" : g.Key.UniversityType == false ? "Devlet" : "Bilinmiyor")
                            : g.Key.ParentInstitution.Name,
                        Value = g.Count(),
                        IsPrivate = g.Key.UniversityType,
                        ParentInstitutionId = g.Key.ParentInstitution.Id
                    })
                    .GroupBy(x => x.Key)
                    .Select(g => new ReportResponseDTO()
                    {
                        Key = g.Key,
                        Value = g.Sum(x => x.Value).ToString(),
                        IsPrivate = g.FirstOrDefault().IsPrivate,
                        ParentInstitutionId = g.FirstOrDefault().ParentInstitutionId
                    })
                    .ToList();
        }
    }
}
