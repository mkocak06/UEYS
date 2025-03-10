using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Core.Models.DetailedReportModels;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.StatisticModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ProgramRepository : EfRepository<Program>, IProgramRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ProgramRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public IQueryable<Program> QueryablePrograms(Expression<Func<Program, bool>> predicate)
        {
            return dbContext.Programs.AsSplitQuery()
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.EducatorPrograms)
                .Include(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)
                .Include(x => x.AuthorizationDetails).ThenInclude(x => x.AuthorizationCategory)
                .Include(x => x.Hospital).ThenInclude(x => x.Province).AsNoTracking()
                .Include(x => x.ExpertiseBranch).Where(predicate);
        }

        public List<ActivePassiveResponseModel> ProgramCountForDashboard()
        {
            List<ActivePassiveResponseModel> programCount = new List<ActivePassiveResponseModel>();

            var pCount = dbContext.Programs.Count(x =>
                x.ParentPrograms.Any(a => a.Type == ProgramType.Protocol && a.IsDeleted == false) &&
                x.IsDeleted == false);
            var cCount = dbContext.Programs.Count(x =>
                x.ParentPrograms.Any(a => a.Type == ProgramType.Complement && a.IsDeleted == false) &&
                x.IsDeleted == false);

            programCount.Add(new ActivePassiveResponseModel()
            {
                ActiveRecordsCount = pCount,
                SeriesName = "Protocol Programs"
            });
            programCount.Add(new ActivePassiveResponseModel()
            {
                ActiveRecordsCount = cCount,
                SeriesName = "Complement Programs"
            });

            return programCount;
        }

        public IQueryable<ProgramPaginateForQuotaResponseDTO> GetProgramsForQuota(ZoneModel zone)
        {
            IQueryable<Program> programs = dbContext.Programs;

            if (zone.Hospitals != null && zone.Hospitals.Any())
            {
                var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                programs = dbContext.Programs.Where(x => hospitalIds.Contains(x.HospitalId.Value));
            }

            return programs.Select(x => new ProgramPaginateForQuotaResponseDTO()
            {
                Id = x.Id,
                AssociateProfessorCount = x.EducatorPrograms.Where(y => y.Educator.User.IsDeleted == false && y.Educator.IsDeleted == false && (y.DutyEndDate == null || y.DutyEndDate <= DateTime.UtcNow) && y.Educator.AcademicTitle.Code == "DOC").Count(),
                ProfessorCount = x.EducatorPrograms.Where(y => y.Educator.User.IsDeleted == false && y.Educator.IsDeleted == false && (y.DutyEndDate == null || y.DutyEndDate <= DateTime.UtcNow) && (y.Educator.AcademicTitle.Code == "PROF" || y.Educator.AcademicTitle.Code == "EG")).Count(),
                DoctorLecturerCount = x.EducatorPrograms.Where(y => y.Educator.User.IsDeleted == false && y.Educator.IsDeleted == false && (y.DutyEndDate == null || y.DutyEndDate <= DateTime.UtcNow) && y.Educator.AcademicTitle.Code == "DR").Count(),
                ChiefAssistantCount = x.EducatorPrograms.Where(y => y.Educator.User.IsDeleted == false && y.Educator.IsDeleted == false && (y.DutyEndDate == null || y.DutyEndDate <= DateTime.UtcNow) && y.Educator.AcademicTitle.Code == "BAS").Count(),
                TotalEducatorCount = x.EducatorPrograms.Where(y => y.Educator.User.IsDeleted == false && y.Educator.IsDeleted == false && (y.DutyEndDate == null || y.DutyEndDate <= DateTime.UtcNow)).Count(),
                CurrentStudentCount = x.Students.Where(y => !y.IsDeleted && !y.IsHardDeleted && y.EducationTrackings.FirstOrDefault(z => z.IsDeleted == false && z.ReasonType == ReasonType.EstimatedFinish).ProcessDate > DateTime.UtcNow).Count(),
                StudentWhoLast6MonthToFinishCount = x.Students.Where(y => !y.IsDeleted && !y.IsHardDeleted && y.EducationTrackings.FirstOrDefault(z => z.IsDeleted == false && z.ReasonType == ReasonType.EstimatedFinish).ProcessDate > DateTime.UtcNow && y.EducationTrackings.FirstOrDefault(z => z.IsDeleted == false && z.ReasonType == ReasonType.EstimatedFinish).ProcessDate <= DateTime.UtcNow.AddMonths(6)).Count(),
                ProgramName = x.Hospital.Name + " " + x.ExpertiseBranch.Name,
                IsDeleted = x.IsDeleted,
                IsPrincipal = x.ExpertiseBranch.IsPrincipal,
                IsQuotaRequestable = x.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.IsQuotaRequestable,
                AuthorizationCategory = x.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name,
                Portfolios = mapper.Map<List<PortfolioResponseDTO>>(x.ExpertiseBranch.Portfolios.Where(x => x.IsDeleted == false)),
                ProfessionCode = x.ExpertiseBranch.Profession.Code,
            }); ;
        }

        public async Task<List<ProgramsStaffCount>> GetProgramsForExcelExport(ZoneModel zone, ProgramFilter filter)
        {
            IQueryable<Program> programs = dbContext.Programs.AsNoTracking().AsSplitQuery().Where(x => x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true);

            if (zone.Hospitals != null && zone.Hospitals.Any())
            {
                var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                programs = dbContext.Programs.AsNoTracking().Where(x => hospitalIds.Contains(x.HospitalId.Value));
            }

            if (filter.ProfessionList?.Any() == true)
            {
                programs = programs.Where(a => filter.ProfessionList.Select(x => x.Id).Contains(a.Hospital.Faculty.Profession.Id));
            }
            if (filter.InstitutionList?.Any() == true)
            {
                programs = programs.Where(a => filter.InstitutionList.Select(x => x.Id).Contains(a.Hospital.Faculty.University.Institution.Id) &&
                                               (filter.IsPrivate == null || a.Hospital.Faculty.University.IsPrivate == filter.IsPrivate));
            }
            if (filter.UniversityList?.Any() == true)
            {
                programs = programs.Where(a => filter.UniversityList.Select(x => x.Id).Contains(a.Hospital.Faculty.University.Id));
            }
            if (filter.HospitalList?.Any() == true)
            {
                programs = programs.Where(x => filter.HospitalList.Select(x => x.Id).Contains(x.HospitalId.Value));
            }
            if (filter.IsPrincipal != null)
            {
                programs = programs.Where(a => filter.IsPrincipal == null || filter.IsPrincipal == a.ExpertiseBranch.IsPrincipal);
            }
            if (filter.ExpertiseBranchList?.Any() == true)
            {
                programs = programs.Where(x => filter.ExpertiseBranchList.Select(x => x.Id).Contains(x.ExpertiseBranchId));
            }
            if (filter.ProvinceList?.Any() == true)
            {
                programs = programs.Where(x => filter.ProvinceList.Select(x => x.Id).Contains(x.Hospital.ProvinceId.Value));
            }

            return await (from x in programs
                          let affiliated = dbContext.Affiliations.FirstOrDefault(a => a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId)
                          let authorizationCategory = x.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory
                          select new ProgramsStaffCount()
                          {
                              Id = x.Id,
                              ProvinceName = x.Hospital.Province.Name,
                              ParentInstitutionName = x.Hospital.Faculty.University.Institution.Name,
                              ProfessionName = x.ExpertiseBranch.Profession.Name,
                              UniversityName = x.Hospital.Faculty.University.Name,
                              IsPrivate = x.Hospital.Faculty.University.IsPrivate,
                              FacultyName = x.Hospital.Faculty.Name,
                              HospitalName = x.Hospital.Name,
                              ExpertiseBranchName = x.ExpertiseBranch.Name,
                              AffiliatedUniversityName = affiliated.Faculty.University.Name,
                              AffiliatedHospitalName = affiliated.Faculty.Name,
                              AuthorizationCategory = authorizationCategory.Name,
                              AuthorizationCategoryColorCode = authorizationCategory.ColorCode,
                              EducatorList = x.EducatorPrograms.Where(x => x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false && (x.DutyEndDate == null || DateTime.UtcNow <= x.DutyEndDate) && filter.IsPrincipal == null || (filter.IsPrincipal == true ? x.Program.ExpertiseBranch.IsPrincipal == true : x.Program.ExpertiseBranch.IsPrincipal == false)).Select(x => x.Educator).Select(x => x.AcademicTitle.Name),
                              StudentList = x.OriginalStudents.Where(y => !y.IsDeleted && !y.IsHardDeleted && !y.User.IsDeleted && y.Status != StudentStatus.Gratuated && y.Status != StudentStatus.SentToRegistration && y.Status != StudentStatus.EducationEnded).Select(x => x.EducationTrackings.FirstOrDefault(z => z.IsDeleted == false && z.ReasonType == ReasonType.EstimatedFinish).ProcessDate)
                          }).OrderBy(x => x.HospitalName).ThenBy(x => x.ExpertiseBranchName).ToListAsync();

            return await programs.Select(x => new ProgramsStaffCount()
            {
                Id = x.Id,
                ProvinceName = x.Hospital.Province.Name,
                ParentInstitutionName = x.Faculty.University.Institution.Name,
                ProfessionName = x.ExpertiseBranch.Profession.Name,
                UniversityName = x.Faculty.University.Name,
                FacultyName = x.Faculty.Name,
                HospitalName = x.Hospital.Name,
                ExpertiseBranchName = x.ExpertiseBranch.Name,
                AffiliatedUniversityName = dbContext.Affiliations
                    .FirstOrDefault(a => a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId).Faculty
                    .University.Name ?? null,
                AffiliatedHospitalName = dbContext.Affiliations
                    .FirstOrDefault(a => a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId).Hospital
                    .Name ?? null,
                AuthorizationCategory = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                    .FirstOrDefault().AuthorizationCategory.Name,
                AuthorizationCategoryColorCode = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                    .FirstOrDefault().AuthorizationCategory.ColorCode,
                EducatorList = x.EducatorPrograms.Select(x => x.Educator).Select(x => x.AcademicTitle.Name),
                StudentList = x.OriginalStudents.Where(y => !y.IsDeleted && !y.IsHardDeleted && !y.User.IsDeleted && y.Status != StudentStatus.Gratuated && y.Status != StudentStatus.SentToRegistration && y.Status != StudentStatus.EducationEnded).Select(x => x.EducationTrackings.FirstOrDefault(z => z.IsDeleted == false && z.ReasonType == ReasonType.EstimatedFinish).ProcessDate)
            }).OrderBy(x => x.HospitalName).ThenBy(x => x.ExpertiseBranchName).ToListAsync();
        }


        public IQueryable<ProgramPaginateResponseDTO> PaginateListQuery(ZoneModel zone,
            Expression<Func<Program, bool>> predicate = null, bool isSearch = false)
        {
            IQueryable<Program> programs = predicate != null ? dbContext.Programs.Where(predicate) : dbContext.Programs;

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry && zone.RoleCategory != RoleCategoryType.Registration && isSearch == false)
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

        public IQueryable<ProgramPaginateResponseDTO> AllPaginateListQuery()
        {
            return dbContext.Programs.Select(x => new ProgramPaginateResponseDTO()
            {
                Id = x.Id,
                ProvinceName = x.Hospital.Province.Name,
                ParentInstitutionName = x.Faculty.University.Institution.Name,
                ProfessionName = x.ExpertiseBranch.Profession.Name,
                UniversityName = x.Faculty.University.Name,
                HospitalName = x.Hospital.Name,
                ExpertiseBranchName = x.ExpertiseBranch.Name,
                AuthorizationEndDate = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationEndDate,
                AuthorizationCategory = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.Name,
                AuthorizationCategoryIsActive = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.IsActive,
                AuthorizationCategoryColorCode = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                        .FirstOrDefault().AuthorizationCategory.ColorCode,
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
                ProgramType = x.ParentPrograms.OrderBy(x => x.Id).Select(x => x.Type).ToList(),
                IsDeleted = x.IsDeleted,
                IsPrincipal = x.ExpertiseBranch.IsPrincipal,
                ProtocolStatus =
                        dbContext.Affiliations.FirstOrDefault(a =>
                            a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId) != null ? "BK" :
                        x.ParentPrograms.OrderBy(x => x.Id).FirstOrDefault(x => x.IsDeleted == false).Type ==
                        ProgramType.Protocol ? "PP" :
                        x.ParentPrograms.OrderBy(x => x.Id).FirstOrDefault(x => x.IsDeleted == false).Type ==
                        ProgramType.Complement ? "TP" : null,
            })
                .OrderBy(x => x.HospitalName)
                .ThenBy(x => x.ExpertiseBranchName);
        }

        public async Task<Program> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Programs.AsSplitQuery()
                .Include(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)
                .Include(x => x.ExpertiseBranch).ThenInclude(x => x.SubBranches).ThenInclude(x => x.SubBranch)
                .Include(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Hospital).ThenInclude(x => x.Province)
                .Include(x => x.Hospital).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x =>
                    x.EducatorPrograms.Where(x =>
                        x.EducatorId != null && x.DutyEndDate == null && x.Educator.UserId != null))
                .ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                .Include(x => x.EducationOfficers.Where(x =>
                    x.Educator.IsDeleted == false && x.EndDate == null && x.Educator.User.IsDeleted == false))
                .ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                .Include(x => x.ExpertiseBranch)
                .Include(x =>
                    x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate))
                .ThenInclude(x => x.AuthorizationCategory)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Program>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            return await (from f in dbContext.Faculties
                          join p in dbContext.Programs.Include(p => p.ExpertiseBranch).Include(x => x.Hospital) on f.Id equals p
                              .FacultyId
                          where f.IsDeleted == false && p.IsDeleted == false && f.UniversityId == uniId
                          select p).ToListAsync(cancellationToken);
        }

        public async Task<List<ProgramExpertiseBreadcrumbResponseDTO>> GetListByUniversityIdBreadCrumbModel(
            CancellationToken cancellationToken, long uniId)
        {
            return await (from f in dbContext.Faculties
                          join p in dbContext.Programs.Include(x => x.Hospital).Include(p => p.ExpertiseBranch) on f.Id equals p
                              .FacultyId
                          where f.IsDeleted == false && p.IsDeleted == false && f.UniversityId == uniId
                          select new ProgramExpertiseBreadcrumbResponseDTO
                          {
                              Id = p.Id,
                              HospitalName = p.Hospital.Name,
                              ExpertiseBranchName = p.ExpertiseBranch.Name
                          }).ToListAsync(cancellationToken);
        }

        public async Task<List<Program>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId)
        {
            return await QueryablePrograms(x => x.IsDeleted == false && x.HospitalId == hospitalId).ToListAsync(cancellationToken);
        }

        public async Task<List<ProgramBreadcrumbSimpleDTO>> GetProgramListByHospitalIdBreadCrumb(CancellationToken cancellationToken, long hospitalId)
        {
            //return await dbContext.Programs.Where(x => x.IsDeleted == false && x.HospitalId == hospitalId).Select(x => new ProgramBreadcrumbSimpleDTO()
            //{
            //    Id = x.Id,
            //    ExpertiseBranchId = x.ExpertiseBranch.Id,
            //    ExpertiseBranchName = x.ExpertiseBranch.Name
            //}).ToListAsync(cancellationToken);

            return await (from x in dbContext.Programs.Where(x => x.IsDeleted == false && x.HospitalId == hospitalId)
                          let authorization = x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                                                                    .FirstOrDefault(x => x.IsDeleted == false)
                          where authorization.AuthorizationCategory.IsActive == true
                          select new ProgramBreadcrumbSimpleDTO()
                          {
                              Id = x.Id,
                              ExpertiseBranchName = x.ExpertiseBranch.Name,
                              ExpertiseBranchId = x.ExpertiseBranchId
                          }).OrderBy(x => x.ExpertiseBranchName).ToListAsync(cancellationToken); ;
        }

        public async Task<List<Program>> GetListByExpertiseBranchId(CancellationToken cancellationToken,
            long expertiseBranchId)
        {
            return await QueryablePrograms(x => x.IsDeleted == false && x.ExpertiseBranchId == expertiseBranchId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProgramResponseDTO> GetByHospitalAndBranchId(CancellationToken cancellationToken, long hospitalId,
            long expertiseBranchId)
        {
            return await dbContext.Programs
                .Select(x => new ProgramResponseDTO()
                {
                    Id = x.Id,
                    HospitalId = x.HospitalId,
                    Hospital = new() { Name = x.Hospital.Name, Province = new() { Name = x.Hospital.Province.Name } },
                    ExpertiseBranchId = x.ExpertiseBranchId,
                    ExpertiseBranch = new() { Name = x.ExpertiseBranch.Name },
                    IsDeleted = x.IsDeleted
                }).FirstOrDefaultAsync(x => x.IsDeleted == false && x.ExpertiseBranchId == expertiseBranchId && x.HospitalId == hospitalId, cancellationToken);
        }

        public async Task<Program> GetByStudentId(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.Students.AsQueryable()
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)
                .Include(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Institution)
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.EducatorPrograms).ThenInclude(x => x.Educator)
                .ThenInclude(x => x.User)
                .Include(x => x.Program).ThenInclude(x => x.EducatorPrograms).ThenInclude(x => x.Educator)
                .ThenInclude(x => x.StaffTitle)
                .Where(x => x.Id == studentId)
                .Select(x => x.Program)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<ProgramsLocationResponseDTO>> GetLocationsByExpertiseBranchId(
            CancellationToken cancellationToken, long? expBrId, long? authCategoryId)
        {
            return await (from p in dbContext.Programs
                          let maxAuthDate = p.AuthorizationDetails.Max(x => x.AuthorizationDate)
                          join h in dbContext.Hospitals on p.HospitalId equals h.Id
                          where p.IsDeleted == false && h.IsDeleted == false &&
                                (expBrId == null || p.ExpertiseBranchId == expBrId) && (authCategoryId == null ||
                                                                                        p.AuthorizationDetails
                                                                                            .FirstOrDefault(x =>
                                                                                                x.AuthorizationDate ==
                                                                                                maxAuthDate)
                                                                                            .AuthorizationCategoryId ==
                                                                                        authCategoryId)
                          group p by new
                          {
                              h.Id,
                              h.Name,
                              h.Latitude,
                              h.Longitude,
                              ProvinceName = h.Province.Name,
                              CategoryName = p.AuthorizationDetails.FirstOrDefault(x => x.AuthorizationDate == maxAuthDate)
                                  .AuthorizationCategory.Name,
                              CategoryColorCode = p.AuthorizationDetails.FirstOrDefault(x => x.AuthorizationDate == maxAuthDate)
                                  .AuthorizationCategory.ColorCode,
                          }
                into grp
                          select new ProgramsLocationResponseDTO()
                          {
                              Id = grp.FirstOrDefault().Id,
                              Hospital = new HospitalResponseDTO()
                              {
                                  Id = grp.Key.Id,
                                  Name = grp.Key.Name,
                                  Latitude = grp.Key.Latitude,
                                  Longitude = grp.Key.Longitude,
                                  Province = new ProvinceResponseDTO()
                                  {
                                      Name = grp.Key.ProvinceName,
                                  }
                              },
                              ActiveAuthorizationDetailsAuthorizationCategory = new AuthorizationCategoryResponseDTO
                              {
                                  Name = grp.Key.CategoryName,
                                  ColorCode = grp.Key.CategoryColorCode
                              }
                          }).ToListAsync();
        }

        public IQueryable<ProgramExportResponseDTO> ExportListQuery(ZoneModel zone)
        {
            IQueryable<Program> programs = dbContext.Programs;
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
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

                programs = programs.Where(predicates);
            }

            return programs.Select(x => new ProgramExportResponseDTO()
            {
                Id = x.Id,
                IsDeleted = x.IsDeleted,
                IsPrincipal = x.ExpertiseBranch.IsPrincipal,
                ProvinceName = x.Hospital.Province.Name,
                ParentInstitutionName = x.Hospital.Faculty.University.Institution.Name,
                ProfessionName = x.ExpertiseBranch.Profession.Name,
                UniversityName = x.Hospital.Faculty.University.Name,
                FacultyName = x.Hospital.Faculty.Name,
                HospitalName = x.Hospital.Name,
                IsPrivate = x.Faculty.University.IsPrivate,
                ExpertiseBranchName = x.ExpertiseBranch.Name,
                AffiliatedUniversityName = x.FacultyId != x.Hospital.FacultyId ? x.Faculty.University.Name : "",
                AffiliatedFacultyName = x.FacultyId != x.Hospital.FacultyId ? x.Faculty.Name : "",
                //StudentCount = x.OriginalStudents.Where(x => x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false).Count(),
                AuthorizationCategory = x.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name,
                ProtocolStatus = dbContext.Affiliations.FirstOrDefault(a => a.HospitalId == x.HospitalId && a.FacultyId == x.FacultyId) != null ? "BK" : x.ParentPrograms.OrderBy(x => x.Id).FirstOrDefault(x => x.IsDeleted == false).Type == ProgramType.Protocol ? "P" : x.ParentPrograms.OrderBy(x => x.Id).FirstOrDefault(x => x.IsDeleted == false).Type == ProgramType.Complement ? "T" : null,
                AuthorizationDetails = x.AuthorizationDetails.Where(a => a.IsDeleted == false).Select(c =>
                    new AuthorizationDetailExportResponseDTO
                    {
                        AuthorizationEndDate = c.AuthorizationEndDate,
                        AuthorizationDecisionDate = c.AuthorizationDate,
                        AuthorizationDecisionNo = c.AuthorizationDecisionNo,
                        VisitDate = c.VisitDate,
                        AuthorizationCategory = c.AuthorizationCategory.Name,
                        ColorCode = c.AuthorizationCategory.ColorCode,
                        AuthorizationCategoryIsActive = c.AuthorizationCategory.IsActive
                    }).ToList()
            }).OrderByDescending(x => x.ProfessionName).ThenBy(x => x.HospitalName).ThenBy(x => x.ExpertiseBranchName);
        }

        public async Task<ProgramResponseDTO> CheckProtocolProgram(CancellationToken cancellationToken, long id)
        {
            var protocolProgram =
                await dbContext.ProtocolPrograms.FirstOrDefaultAsync(x =>
                    x.ParentProgramId == id && x.CancelingProtocolNo == null);
            var dependentProgram = await dbContext.DependentPrograms
                .Include(x => x.RelatedDependentProgram.ProtocolProgram)
                .FirstOrDefaultAsync(x => x.ProgramId == id, cancellationToken);
            return new()
            {
                IsMainProgram = protocolProgram != null,
                IsDependentProgram = dependentProgram != null,
                ProtocolOrComplement = protocolProgram?.Type ??
                                       dependentProgram?.RelatedDependentProgram?.ProtocolProgram?.Type
            };
        }

        public long ProgramCountByInstitution(ZoneModel zone, long parentInstitutionId)
        {
            IQueryable<Program> programs = dbContext.Programs.Where(x => x.IsDeleted == false);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Program>();

                if (zone.Programs != null && zone.Programs.Any())
                {
                    return zone.Programs.Count(x => x.Faculty.University.InstitutionId == parentInstitutionId);
                }
            }

            return programs.Count(x => x.Faculty.University.InstitutionId == parentInstitutionId);
        }

        public IQueryable<ProgramChartModel> QueryableProgramsForCharts(ZoneModel zone)
        {
            IQueryable<Program> programs = dbContext.Programs;
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
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

                programs = programs.Where(predicates);
            }

            return programs.Select(x => new ProgramChartModel()
            {
                Id = x.Id,
                ProvinceName = x.Hospital.Province.Name,
                ProvincePlateCode = x.Hospital.Province.Id.ToString(),
                ProvinceId = x.Hospital.ProvinceId,
                ParentInstitutionName = x.Hospital.Faculty.University.Institution.Name,
                ParentInstitutionId = x.Hospital.Faculty.University.InstitutionId,
                ProfessionName = x.ExpertiseBranch.Profession.Name,
                ProfessionId = x.ExpertiseBranch.ProfessionId,
                ProfessionCode = x.ExpertiseBranch.Code,
                UniversityName = x.Hospital.Faculty.University.Name,
                IsUniversityPrivate = x.Hospital.Faculty.University.IsPrivate,
                UniversityId = x.Hospital.Faculty.UniversityId,
                FacultyName = x.Faculty.Name,
                FacultyId = x.Faculty.Id,
                HospitalName = x.Hospital.Name,
                HospitalId = x.HospitalId,
                ExpertiseBranchName = x.ExpertiseBranch.Name,
                ExpertiseBranchId = x.ExpertiseBranchId,
                AuthorizationCategory = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                    .FirstOrDefault().AuthorizationCategory.Name,
                AuthorizationCategoryColorCode = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                    .FirstOrDefault().AuthorizationCategory.ColorCode,
                AuthorizationCategoryIsActive = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                    .FirstOrDefault().AuthorizationCategory.IsActive,
                AuthorizationCategoryId = x.AuthorizationDetails.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                    .FirstOrDefault().AuthorizationCategory.Id,
                IsPrincipal = x.ExpertiseBranch.IsPrincipal,
                IsDeleted = x.IsDeleted,
            });
        }

        public async Task<List<CountByExpertiseBranch>> CountByExpertiseBranch(CancellationToken cancellationToken, ZoneModel zone, ProgramFilter filter)
        {
            IQueryable<ExpertiseBranch> expertiseBranches = dbContext.ExpertiseBranches.AsNoTracking().AsSplitQuery();

            var hospitalIds = zone.Hospitals?.Select(x => x.Id).ToList();

            return await (from x in expertiseBranches
                          let programs = x.Programs.Where(x => (hospitalIds == null || hospitalIds.Contains(x.HospitalId.Value)) &&
                                                               (filter.IsPrincipal == null || filter.IsPrincipal == x.ExpertiseBranch.IsPrincipal) &&
                                                               (filter.ProvinceList == null || filter.ProvinceList.Count <= 0 || filter.ProvinceList.Select(x => x.Id).Contains(x.Hospital.ProvinceId.Value)) &&
                                                               (filter.HospitalList == null || filter.HospitalList.Count <= 0 || filter.HospitalList.Select(x => x.Id).Contains(x.HospitalId.Value)) &&
                                                               (filter.IsPrivate == null || x.Hospital.Faculty.University.IsPrivate == filter.IsPrivate) &&
                                                               (filter.ProfessionList == null || filter.ProfessionList.Count <= 0 || filter.ProfessionList.Select(x => x.Id).Contains(x.Hospital.Faculty.ProfessionId.Value)) &&
                                                               (filter.UniversityList == null || filter.UniversityList.Count <= 0 || filter.UniversityList.Select(x => x.Id).Contains(x.Hospital.Faculty.UniversityId)) &&
                                                               (filter.InstitutionList == null || filter.InstitutionList.Count <= 0 || filter.InstitutionList.Select(x => x.Id).Contains(x.Hospital.Faculty.University.InstitutionId.Value)) &&
                                                               (filter.ExpertiseBranchList == null || filter.ExpertiseBranchList.Count <= 0 || filter.ExpertiseBranchList.Select(x => x.Id).Contains(x.ExpertiseBranchId)) &&
                                                               x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true)
                          select new CountByExpertiseBranch
                          {
                              ExpertiseBranchName = x.Name,
                              ProfessionName = x.Profession.Name,
                              IsPrincipal = x.IsPrincipal ?? false,
                              ProgramCount = programs.Count(),
                              EducatorCount = programs.SelectMany(x => x.EducatorPrograms).Count(x => (x.DutyEndDate == null || DateTime.UtcNow <= x.DutyEndDate) && (filter.IsPrincipal == null || (filter.IsPrincipal == true ? x.Program.ExpertiseBranch.IsPrincipal == true : x.Program.ExpertiseBranch.IsPrincipal == false)) &&
                                x.Educator.User.IsDeleted == false && x.Educator.IsDeleted == false),
                              StudentCount = programs.SelectMany(x => x.OriginalStudents.Where(x => x.IsHardDeleted == false && x.IsDeleted == false && x.User.IsDeleted == false && x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration && x.Status != StudentStatus.EducationEnded))
                                                 .Count()
                          }).OrderBy(x => x.ProfessionName == "Týp" ? 0 : (x.ProfessionName == "Diþ Hekimliði" ? 1 : (x.ProfessionName == "Eczacýlýk" ? 3 : 4)))
                          .ThenBy(x => x.IsPrincipal == true ? 0 : 1).ThenBy(x => x.ExpertiseBranchName).ToListAsync(cancellationToken);

            return await expertiseBranches.Select(x => new CountByExpertiseBranch
            {
                ExpertiseBranchName = x.Name,
                ProfessionName = x.Profession.Name,
                IsPrincipal = x.IsPrincipal ?? false,
                ProgramCount = x.Programs.Where(x => (hospitalIds == null || hospitalIds.Contains(x.HospitalId.Value)) &&
                                                     (filter.HospitalList == null || filter.HospitalList.Count <= 0 || filter.HospitalList.Select(x => x.Id).Contains(x.HospitalId.Value)) &&
                                                     (filter.ExpertiseBranchList == null || filter.ExpertiseBranchList.Count <= 0 || filter.ExpertiseBranchList.Select(x => x.Id).Contains(x.ExpertiseBranchId)) &&
                                                     x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true)
                                         .Count(),
                EducatorCount = x.Programs.Where(x => (hospitalIds == null || hospitalIds.Contains(x.HospitalId.Value)) &&
                                                      (filter.HospitalList == null || filter.HospitalList.Count <= 0 || filter.HospitalList.Select(x => x.Id).Contains(x.HospitalId.Value)) &&
                                                      (filter.ExpertiseBranchList == null || filter.ExpertiseBranchList.Count <= 0 || filter.ExpertiseBranchList.Select(x => x.Id).Contains(x.ExpertiseBranchId)) &&
                                                     x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true)
                                          .SelectMany(x => x.EducatorPrograms)
                                          .Count(),
                StudentCount = x.Programs.Where(x => (hospitalIds == null || hospitalIds.Contains(x.HospitalId.Value)) &&
                                                     (filter.HospitalList == null || filter.HospitalList.Count <= 0 || filter.HospitalList.Select(x => x.Id).Contains(x.HospitalId.Value)) &&
                                                     (filter.ExpertiseBranchList == null || filter.ExpertiseBranchList.Count <= 0 || filter.ExpertiseBranchList.Select(x => x.Id).Contains(x.ExpertiseBranchId)) &&
                                                     x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true)
                                          .SelectMany(x => x.OriginalStudents.Where(x => x.IsHardDeleted == false && x.IsDeleted == false && x.User.IsDeleted == false && x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration && x.Status != StudentStatus.EducationEnded))
                                          .Count()
            }).ToListAsync(cancellationToken);
        }


        public async Task<List<ProgramStaffModel>> ProgramStaffInfo(CancellationToken cancellationToken)
        {
            var programs = dbContext.Programs.Select(x => new ProgramStaffModel()
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
                EducatorCount = x.EducatorPrograms
                    .Where(x => x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false)
                    .Select(x => x.Educator).Count(),
                StudentCount = x.Students.Where(x => x.IsDeleted == false && x.User.IsDeleted == false).Count(),
                EmployeeCount = dbContext.UserRoleHospitals.Where(urp => urp.HospitalId == x.HospitalId)
                    .Select(x => x.UserRole).Count()
            });

            return await programs.ToListAsync(cancellationToken);
        }
    }
}