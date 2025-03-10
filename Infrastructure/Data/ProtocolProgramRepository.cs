using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.ProtocolProgram;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ProtocolProgramRepository : EfRepository<ProtocolProgram>, IProtocolProgramRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ProtocolProgramRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<ProtocolProgram> PaginateQuery(ZoneModel zone)
        {
            IQueryable<ProtocolProgram> protocolPrograms = dbContext.ProtocolPrograms.Where(x => !x.IsDeleted);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<ProtocolProgram>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => provinceIds.Contains(x.ParentProgram.Hospital.Province.Id));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => universityIds.Contains(x.ParentProgram.Hospital.Faculty.University.Id));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => facultyIds.Contains(x.ParentProgram.Hospital.Faculty.Id));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => hospitalIds.Contains(x.ParentProgram.Hospital.Id));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.ParentProgram.Hospital.Programs.Any(h => programIds.Contains(h.Id)));
                }
                protocolPrograms = protocolPrograms.Where(predicates);
            }
            return protocolPrograms;
        }

        public IQueryable<ProgramPaginateResponseDTO> PaginateListQuery(ZoneModel zone,
           Expression<Func<Program, bool>> predicate = null, bool isSearch = false)
        {
            IQueryable<Program> programs = predicate != null ? dbContext.Programs.Where(predicate) : dbContext.Programs;

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry &&
                isSearch == false)
            {
                var predicates = PredicateBuilder.False<Program>();

                if (zone.Provinces != null && zone.Provinces.Any())
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => provinceIds.Contains(x.Faculty.University.ProvinceId.Value));
                }

                if (zone.Universities != null && zone.Universities.Any())
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => universityIds.Contains(x.Faculty.UniversityId));
                }

                if (zone.Faculties != null && zone.Faculties.Any())
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => facultyIds.Contains(x.FacultyId.Value));
                }

                if (zone.Hospitals != null && zone.Hospitals.Any())
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => hospitalIds.Contains(x.HospitalId.Value));
                }

                if (zone.Programs != null && zone.Programs.Any())
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => programIds.Contains(x.Id));
                }

                if (zone.Students != null && zone.Students.Any())
                {
                    var programId = zone.Students.FirstOrDefault()?.OriginalProgramId;
                    predicates = predicates.Or(x => programId == x.Id);
                }

                programs = programs.Where(predicates);
            }

            return (from x in programs
                    let authorization = x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                                                              .FirstOrDefault(x => x.IsDeleted == false)
                    let affiliation = x.Hospital.Affiliations.FirstOrDefault(a => a.FacultyId == x.FacultyId)
                    let parentPrograms = x.ParentPrograms.Where(x => x.IsDeleted == false).OrderBy(x => x.Id).ToList()

                    select new ProgramPaginateResponseDTO()
                    {
                        Id = x.Id,
                        ProvinceName = x.Hospital.Province.Name,
                        ProvinceId = x.Hospital.ProvinceId,
                        ParentInstitutionName = x.Faculty.University.Institution.Name,
                        ParentInstitutionId = x.Faculty.University.InstitutionId,
                        ProfessionName = x.ExpertiseBranch.Profession.Name,
                        ProfessionId = x.ExpertiseBranch.ProfessionId,
                        UniversityName = x.Faculty.University.Name,
                        UniversityId = x.Faculty.UniversityId,
                        FacultyName = x.Faculty.Name,
                        FacultyId = x.Faculty.Id,
                        HospitalName = x.Hospital.Name,
                        HospitalId = x.HospitalId,
                        ExpertiseBranchName = x.ExpertiseBranch.Name,
                        ExpertiseBranchId = x.ExpertiseBranchId,
                        AuthorizationEndDate = authorization.AuthorizationEndDate,
                        AuthorizationVisitDate = authorization.VisitDate,
                        AuthorizationDecisionDate = authorization.AuthorizationDate,
                        AuthorizationDecisionNo = authorization.AuthorizationDecisionNo,
                        AuthorizationCategory = authorization.AuthorizationCategory.Name,
                        AuthorizationCategoryIsActive = authorization.AuthorizationCategory.IsActive,
                        AuthorizationCategoryColorCode = authorization.AuthorizationCategory.ColorCode,
                        CanMakeProtocol = x.ExpertiseBranch.ProtocolProgramCount == null ? false : true,
                        AuthorizationCategoryId = authorization.AuthorizationCategory.Id,
                        AffiliationId = affiliation.Id,
                        AffiliationProtocolNo = affiliation.ProtocolNo,
                        AffiliationHospitalName = affiliation.Hospital.Name,
                        AffiliationUniversityName = affiliation.Faculty.University.Name,
                        ProtocolProgramIdList = parentPrograms.Select(p => p.Id).ToList(),
                        ProgramType = parentPrograms.Select(x => x.Type).ToList(),
                        IsDeleted = x.IsDeleted,
                        IsAuditing = x.Forms.Any(x => x.AuthorizationDetailId == null || x.AuthorizationDetailId == 0),
                        IsPrincipal = x.ExpertiseBranch.IsPrincipal,
                        PrincipalBrancIdList = x.ExpertiseBranch.PrincipalBranches.Select(y => y.PrincipalBranchId).ToList(),
                        ProtocolStatus = affiliation != null ? "BK" : (parentPrograms.FirstOrDefault().Type == ProgramType.Protocol ? "P" :
                        (parentPrograms.FirstOrDefault().Type == ProgramType.Complement ? "T" : null)),
                        IsQuotaRequestable = authorization.AuthorizationCategory.IsQuotaRequestable
                    })
                    .OrderBy(x => x.HospitalName)
                    .ThenBy(x => x.ExpertiseBranchName);


            return programs.Select(x => new ProgramPaginateResponseDTO()
            {
                Id = x.Id,
                ProvinceName = x.Hospital.Province.Name,
                ProvinceId = x.Hospital.ProvinceId,
                ParentInstitutionName = x.Faculty.University.Institution.Name,
                ParentInstitutionId = x.Faculty.University.InstitutionId,
                ProfessionName = x.ExpertiseBranch.Profession.Name,
                ProfessionId = x.ExpertiseBranch.ProfessionId,
                UniversityName = x.Faculty.University.Name,
                UniversityId = x.Faculty.UniversityId,
                FacultyName = x.Faculty.Name,
                FacultyId = x.Faculty.Id,
                HospitalName = x.Hospital.Name,
                HospitalId = x.HospitalId,
                ExpertiseBranchName = x.ExpertiseBranch.Name,
                ExpertiseBranchId = x.ExpertiseBranchId,
                AuthorizationEndDate = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationEndDate,
                AuthorizationVisitDate = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().VisitDate,
                AuthorizationDecisionDate = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationDate,
                AuthorizationDecisionNo = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationDecisionNo,
                AuthorizationCategory = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.Name,
                AuthorizationCategoryIsActive = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.IsActive,
                AuthorizationCategoryColorCode = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.ColorCode,
                CanMakeProtocol = x.ExpertiseBranch.ProtocolProgramCount == null ? false : true,
                AuthorizationCategoryId = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.Id,
                AffiliationId =
                        dbContext.Affiliations.FirstOrDefault(a =>
                            a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId) != null
                            ? dbContext.Affiliations.FirstOrDefault(a =>
                                a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId).Id
                            : null,
                AffiliationProtocolNo =
                        dbContext.Affiliations
                            .FirstOrDefault(a => a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId)
                            .ProtocolNo ?? null,
                AffiliationHospitalName = dbContext.Affiliations
                        .FirstOrDefault(a => a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId).Hospital
                        .Name ?? null,
                AffiliationUniversityName = dbContext.Affiliations
                        .FirstOrDefault(a => a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId).Faculty
                        .University.Name ?? null,
                ProtocolProgramIdList = x.ParentPrograms.Where(x => x.IsDeleted == false).OrderBy(x => x.Id)
                        .Select(p => p.Id).ToList(),
                ProgramType = x.ParentPrograms.OrderBy(x => x.Id).Select(x => x.Type).ToList(),
                IsDeleted = x.IsDeleted,
                IsAuditing = x.Forms.Any(x => x.AuthorizationDetailId == null || x.AuthorizationDetailId == 0),
                IsPrincipal = x.ExpertiseBranch.IsPrincipal,
                PrincipalBrancIdList =
                        x.ExpertiseBranch.PrincipalBranches.Select(y => y.PrincipalBranchId).ToList(),
                ProtocolStatus =
                        dbContext.Affiliations.FirstOrDefault(a =>
                            a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId) != null ? "BK" :
                        x.ParentPrograms.OrderBy(x => x.Id).FirstOrDefault(x => x.IsDeleted == false).Type ==
                        ProgramType.Protocol ? "P" :
                        x.ParentPrograms.OrderBy(x => x.Id).FirstOrDefault(x => x.IsDeleted == false).Type ==
                        ProgramType.Complement ? "T" : null,
                IsQuotaRequestable = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.IsQuotaRequestable
            })
                .OrderBy(x => x.HospitalName)
                .ThenBy(x => x.ExpertiseBranchName);
        }


        public async Task<List<ProtocolProgramByUniversityIdResponseDTO>> GetListByUniversityId(CancellationToken cancellationToken, long uniId, ProgramType progType)
        {
            return await dbContext.ProtocolPrograms.Where(x => x.ParentProgram.Faculty.UniversityId == uniId && x.Type == progType && x.IsDeleted == false)
                                                   .Select(x => new ProtocolProgramByUniversityIdResponseDTO()
                                                   {
                                                       Id = x.Id,
                                                       ExpertiseBranch = x.ParentProgram.ExpertiseBranch.Name,
                                                       ManagerName = x.RelatedDependentPrograms.SelectMany(x => x.ChildPrograms.SelectMany(x => x.EducatorDependentPrograms)).FirstOrDefault(x => x.IsProgramManager == true).Educator.User.Name,
                                                       FacultyName = x.ParentProgram.Faculty.Name,
                                                       HospitalName = x.ParentProgram.Hospital.Name,
                                                       UniversityName = x.ParentProgram.Faculty.University.Name,
                                                       ChildProgramNames = x.RelatedDependentPrograms.FirstOrDefault(x => x.IsActive == true) != null ? x.RelatedDependentPrograms.FirstOrDefault(x => x.IsActive == true).ChildPrograms.Select(x => x.Program.ExpertiseBranch.Name).ToList() : null,
                                                   })
                                                   .ToListAsync(cancellationToken);
        }

        public async Task<ProtocolProgram> GetByIdWithSubRecords(CancellationToken cancellationToken, long id, ProgramType progType)
        {
            var result = await dbContext.ProtocolPrograms.AsSplitQuery().AsNoTracking()
                                                   .Include(x => x.ParentProgram).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)
                                                   .Include(x => x.ParentProgram).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.PrincipalBranches)
                                                   .Include(x => x.ParentProgram).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                                   .Include(x => x.ParentProgram).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                                                   .Include(x => x.RelatedDependentPrograms).ThenInclude(x => x.ChildPrograms).ThenInclude(x => x.Program.Hospital.Province)
                                                   .Include(x => x.RelatedDependentPrograms).ThenInclude(x => x.ChildPrograms).ThenInclude(x => x.Program.ExpertiseBranch)
                                                   .Include(x => x.RelatedDependentPrograms).ThenInclude(x => x.ChildPrograms).ThenInclude(x => x.EducatorDependentPrograms).ThenInclude(x => x.Educator.User)
                                                   .Include(x => x.RelatedDependentPrograms).ThenInclude(x => x.ChildPrograms).ThenInclude(x => x.EducatorDependentPrograms).ThenInclude(x => x.Educator.StaffTitle)
                                                   .Include(x => x.RelatedDependentPrograms).ThenInclude(x => x.ChildPrograms).ThenInclude(x => x.EducatorDependentPrograms).ThenInclude(x => x.Educator.AcademicTitle)
                                                   .FirstOrDefaultAsync(x => x.Id == id && x.Type == progType && x.IsDeleted == false, cancellationToken);
            result.RelatedDependentPrograms = result?.RelatedDependentPrograms?.OrderByDescending(x => x.IsActive)?.ToList();
            return result;
        }

        public async Task<ProtocolProgram> UpdateWithSubRecords(CancellationToken cancellationToken, long id, ProtocolProgram protocolProgram)
        {
            var existProtocolProgram = await GetByIdWithSubRecords(cancellationToken, id, protocolProgram.Type);
            dbContext.Entry(protocolProgram).State = EntityState.Modified;

            if (protocolProgram.RelatedDependentPrograms != null)
            {
                foreach (var rdp in protocolProgram.RelatedDependentPrograms)
                {
                    var existRDP = existProtocolProgram.RelatedDependentPrograms.FirstOrDefault(x => x.Id == rdp.Id);
                    dbContext.Entry(rdp).State = existRDP == null ? EntityState.Added : EntityState.Modified;

                    var rDPIds = protocolProgram.RelatedDependentPrograms.Select(x => x.Id).ToList();
                    if (existProtocolProgram.RelatedDependentPrograms != null)
                        foreach (var item in existProtocolProgram.RelatedDependentPrograms.Where(x => !rDPIds.Contains(x.Id)))
                            dbContext.Entry(item).State = EntityState.Deleted;

                    if (rdp.ChildPrograms != null)
                    {
                        foreach (var child in rdp.ChildPrograms)
                        {
                            var existChild = existRDP?.ChildPrograms?.FirstOrDefault(x => x.Id == child.Id);
                            dbContext.Entry(child).State = existChild == null ? EntityState.Added : EntityState.Modified;

                            var childIds = rdp.ChildPrograms.Select(x => x.Id).ToList();
                            if (existRDP?.ChildPrograms != null)
                                foreach (var item in existRDP.ChildPrograms.Where(x => !childIds.Contains(x.Id)))
                                    dbContext.Entry(item).State = EntityState.Deleted;

                            if (child.EducatorDependentPrograms != null)
                            {
                                foreach (var edp in child.EducatorDependentPrograms)
                                {
                                    var existEDP = existChild?.EducatorDependentPrograms?.FirstOrDefault(x => x.Id == edp.Id);
                                    dbContext.Entry(edp).State = existEDP == null ? EntityState.Added : EntityState.Modified;
                                }

                                var edpIds = child.EducatorDependentPrograms.Select(x => x.Id).ToList();
                                if (existChild?.EducatorDependentPrograms != null)
                                    foreach (var item in existChild.EducatorDependentPrograms.Where(x => !edpIds.Contains(x.Id)))
                                        dbContext.Entry(item).State = EntityState.Deleted;
                            }
                        }
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return protocolProgram;
        }

        public async Task<List<EducatorDependentProgramResponseDTO>> GetEducatorListForComplementProgram(CancellationToken cancellationToken, long programId)
        {
            var query = from e in dbContext.Educators
                        join ep in dbContext.EducatorPrograms on e.Id equals ep.EducatorId
                        join at in dbContext.Titles on e.AcademicTitleId equals at.Id
                        join st in dbContext.Titles on e.StaffTitleId equals st.Id
                        join u in dbContext.Users on e.UserId equals u.Id
                        where ep.ProgramId == programId && (ep.DutyEndDate == null || ep.DutyEndDate <= DateTime.UtcNow) && e.IsDeleted == false && u.IsDeleted == false
                        select new EducatorDependentProgramResponseDTO()
                        {
                            AcademicTitleName = at.Name,
                            StaffTitleName = st.Name,
                            EducatorName = u.Name,
                            EducatorId = e.Id,
                            Phone = u.Phone,
                        };

            var educators = await query.ToListAsync(cancellationToken);
            var eduOff = await dbContext.EducationOfficers.FirstOrDefaultAsync(x => x.ProgramId == programId && x.EndDate == null, cancellationToken);

            var educationOfficer = educators.FirstOrDefault(x => x.EducatorId == eduOff?.EducatorId);
            if (educationOfficer != null)
                educationOfficer.IsProgramManager = true;
            return educators;
        }
    }
}