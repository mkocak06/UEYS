using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.BaseModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class FormRepository : EfRepository<Form>, IFormRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        public FormRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        public async Task<FormResponseDTO> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Forms.AsSplitQuery().AsNoTracking()
                .Include(x => x.Program).ThenInclude(x => x.Hospital)
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.EducatorPrograms)
                .Include(x => x.Program).ThenInclude(x => x.Students.Where(x => x.IsDeleted == false && x.IsHardDeleted == false))
                .Include(x => x.OnSiteVisitCommittees).ThenInclude(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
                .Include("FormStandards.Standard.Curriculum.ExpertiseBranch")
                .Include("FormStandards.Standard.StandardCategory")
                .Select(x => new FormResponseDTO()
                {
                    Id = x.Id,
                    AuthorizationDetailId = x.AuthorizationDetailId,
                    ProgramId = x.ProgramId,
                    Program = mapper.Map<ProgramResponseDTO>(x.Program),
                    AuthorizationDetail = mapper.Map<AuthorizationDetailResponseDTO>(x.AuthorizationDetail),
                    OnSiteVisitCommittees = mapper.Map<List<OnSiteVisitCommitteeResponseDTO>>(x.OnSiteVisitCommittees),
                    FormStandards = mapper.Map<List<FormStandardResponseDTO>>(x.FormStandards),
                    CommitteeDescription = x.CommitteeDescription,
                    CreateDate = x.CreateDate,
                    IsOnSiteVisit = x.IsOnSiteVisit,
                    CommitteeOpinionType = x.CommitteeOpinionType,
                    NumberOfEducator = x.Program.EducatorPrograms.Where(x => (x.DutyEndDate == null || DateTime.UtcNow <= x.DutyEndDate)).Count(),
                    NumberOfStudent = x.Program.Students.Count,
                    VisitDate = x.VisitDate,
                    ApplicationDate = x.ApplicationDate,
                    VisitStatusType = x.VisitStatusType,
                    CurriculumName = x.FormStandards.FirstOrDefault().Standard.Curriculum.ExpertiseBranch.Name+" "+x.FormStandards.FirstOrDefault().Standard.Curriculum.Version,
                    CurrentAuthorizationCategory = x.Program.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name

                })
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public IQueryable<Form> GetByZoneQueryable(ZoneModel zone)
        {
            IQueryable<Form> query = dbContext.Forms.AsNoTracking().AsSplitQuery()
                .Include(x => x.Program).ThenInclude(x => x.Hospital)
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.OnSiteVisitCommittees).ThenInclude(x => x.User)
                .Include(x => x.AuthorizationDetail).ThenInclude(x => x.AuthorizationCategory)
                .Include("FormStandards.Standard.Curriculum.ExpertiseBranch");

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Form>();

                if (zone.Provinces != null && zone.Provinces.Any())
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => provinceIds.Contains(x.Program.Faculty.University.ProvinceId.Value));
                }
                if (zone.Universities != null && zone.Universities.Any())
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => universityIds.Contains(x.Program.Faculty.UniversityId));
                }
                if (zone.Faculties != null && zone.Faculties.Any())
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => facultyIds.Contains(x.Program.FacultyId.Value));
                }
                if (zone.Hospitals != null && zone.Hospitals.Any())
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => hospitalIds.Contains(x.Program.HospitalId.Value));
                }
                if (zone.Programs != null && zone.Programs.Any())
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => programIds.Contains(x.ProgramId.Value));
                }
                query = query.Where(predicates);
            }

            return query;
            //.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == isDeleted && x.IsHardDeleted == false, cancellationToken);
        }
    }
}
