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
using Shared.Types;

namespace Infrastructure.Data
{
    public class AffiliationRepository : EfRepository<Affiliation>, IAffiliationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AffiliationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Affiliation> QueryableAffiliations()
        {
            return dbContext.Affiliations.AsSplitQuery().AsNoTracking()
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                /*.Include(x => x.Faculty).ThenInclude(x => x.Programs).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)*/
                .Include(x => x.Hospital)
                .Where(x => x.IsDeleted == false);
        }

        public IQueryable<Affiliation> PaginateQuery(ZoneModel zone)
        {
            IQueryable<Affiliation> affiliations = dbContext.Affiliations.Include(x => x.Faculty).ThenInclude(x => x.University).Include(x => x.Hospital).ThenInclude(x => x.Faculty.University).Where(x => !x.IsDeleted);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Affiliation>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => provinceIds.Contains(x.Hospital.Province.Id));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => universityIds.Contains(x.Hospital.Faculty.University.Id));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => facultyIds.Contains(x.Hospital.Faculty.Id));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => hospitalIds.Contains(x.Hospital.Id));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.Hospital.Programs.Any(h => programIds.Contains(h.Id)));
                }
                affiliations = affiliations.Where(predicates);
            }
            return affiliations;
        }

        public async Task<Affiliation> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Affiliations.AsSplitQuery().AsNoTracking()
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Faculty).ThenInclude(x => x.Programs).ThenInclude(x => x.ExpertiseBranch)
                .ThenInclude(x => x.Profession)
                .Include(x => x.Hospital).ThenInclude(x => x.Province)
                .Include(x => x.Hospital).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Affiliation> GetWithFacultyHospitalId(CancellationToken cancellationToken, long facultyid,
            long hospitalid)
        {
            return await dbContext.Affiliations.AsSplitQuery().AsNoTracking()
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Hospital)
                .FirstOrDefaultAsync(x => x.HospitalId == hospitalid && x.FacultyId == facultyid, cancellationToken);
        }

        public async Task<List<Affiliation>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId)
        {
            return await dbContext.Affiliations.AsSplitQuery()
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Faculty).ThenInclude(x => x.Programs).ThenInclude(x => x.ExpertiseBranch)
                .ThenInclude(x => x.Profession)
                .Include(x => x.Hospital)
                .Where(x => x.HospitalId == hospitalId && x.IsDeleted == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Affiliation>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            return await dbContext.Affiliations.AsSplitQuery()
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Faculty).ThenInclude(x => x.Programs).ThenInclude(x => x.ExpertiseBranch)
                .ThenInclude(x => x.Profession)
                .Include(x => x.Hospital)
                .Where(x => x.Faculty.UniversityId == uniId && x.IsDeleted == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AffiliationExcelExport>> ExcelExport(CancellationToken cancellationToken)
        {
            return await dbContext.Affiliations.Select(x => new AffiliationExcelExport()
                {
                    Id = x.Id,
                    ProtocolNo = x.ProtocolNo,
                    ProtocolDate = x.ProtocolDate,
                    ProtocolEndDate = x.ProtocolEndDate,
                    UniversityName = x.Faculty.University.Name,
                    FacultyName = x.Faculty.Name,
                    HospitalName = x.Hospital.Name,
                    EducatorCount = x.Hospital.Programs.SelectMany(x => x.EducatorPrograms
                    .Where(x => x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false &&
                    (x.DutyEndDate >= DateTime.UtcNow || x.DutyEndDate == null) && x.Educator.EducatorType != EducatorType.NotInstructor)).Count(),
                    StudentCount = x.Hospital.Programs.SelectMany(x =>
                        x.OriginalStudents.Where(x =>
                            x.IsDeleted == false && x.User.IsDeleted == false && x.IsHardDeleted == false)).Count()
                })
                .ToListAsync(cancellationToken);
        }
    }
}