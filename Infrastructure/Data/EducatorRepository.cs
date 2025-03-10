using Core.Entities;
using Core.Entities.Koru;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Core.Models.Educator;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.FilterModels;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Educator;
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
    public class EducatorRepository : EfRepository<Educator>, IEducatorRepository
    {
        private readonly ApplicationDbContext dbContext;
        public EducatorRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<EducatorPaginateResponseDTO> OnlyPaginateListQuery(ZoneModel zone)
        {
            //IQueryable<Educator> educators = dbContext.Educators.AsNoTracking();

            //if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
            //{
            //    var predicates = PredicateBuilder.False<Educator>();

            //    if (zone.Provinces != null && zone.Provinces.Count != 0)
            //    {
            //        var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
            //        predicates = predicates.Or(x => x.EducatorPrograms.Any(a => provinceIds.Contains(a.Program.Hospital.ProvinceId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
            //    }
            //    if (zone.Universities != null && zone.Universities.Count != 0)
            //    {
            //        var universityIds = zone.Universities.Select(x => x.Id).ToList();
            //        predicates = predicates.Or(x => x.EducatorPrograms.Any(a => universityIds.Contains(a.Program.Hospital.Faculty.UniversityId) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
            //    }
            //    if (zone.Faculties != null && zone.Faculties.Count != 0)
            //    {
            //        var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
            //        predicates = predicates.Or(x => x.EducatorPrograms.Any(a => facultyIds.Contains(a.Program.Hospital.FacultyId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
            //    }
            //    if (zone.Hospitals != null && zone.Hospitals.Any())
            //    {
            //        var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
            //        predicates = predicates.Or(x => x.EducatorPrograms.Any(a => hospitalIds.Contains(a.Program.HospitalId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
            //    }
            //    if (zone.Programs != null && zone.Programs.Count != 0)
            //    {
            //        var programIds = zone.Programs.Select(x => x.Id).ToList();
            //        predicates = predicates.Or(x => x.EducatorPrograms.Any(a => programIds.Contains(a.ProgramId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
            //    }
            //    if (zone.Students != null && zone.Students.Count != 0)
            //    {
            //        var programId = zone.Students.FirstOrDefault()?.OriginalProgramId;
            //        predicates = predicates.Or(x => x.EducatorPrograms.Any(a => a.ProgramId == programId && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
            //    }
            //    educators = educators.Where(predicates)/*.Where(x => x.EducatorPrograms.Any(y => y.DutyEndDate == null || y.DutyEndDate >= DateTime.UtcNow))*/;
            //}

            IQueryable<Educator> educators = dbContext.Educators.AsNoTracking();
            var predicates = PredicateBuilder.False<Educator>();

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry && zone.RoleCategory != RoleCategoryType.Registration)
            {

                AddPredicateForZoneEntities(zone.Provinces, x => x.EducatorPrograms.Any(a => zone.Provinces.Select(p => p.Id).Contains(a.Program.Hospital.ProvinceId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
                AddPredicateForZoneEntities(zone.Universities, x => x.EducatorPrograms.Any(a => zone.Universities.Select(u => u.Id).Contains(a.Program.Hospital.Faculty.UniversityId) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
                AddPredicateForZoneEntities(zone.Faculties, x => x.EducatorPrograms.Any(a => zone.Faculties.Select(f => f.Id).Contains(a.Program.Hospital.FacultyId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
                AddPredicateForZoneEntities(zone.Hospitals, x => x.EducatorPrograms.Any(a => zone.Hospitals.Select(h => h.Id).Contains(a.Program.HospitalId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
                AddPredicateForZoneEntities(zone.Programs, x => x.EducatorPrograms.Any(a => zone.Programs.Select(p => p.Id).Contains(a.ProgramId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));

                if (zone.Students != null && zone.Students.Count != 0)
                {
                    var programId = zone.Students.FirstOrDefault()?.OriginalProgramId;
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => a.ProgramId == programId && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
                }

                educators = educators.Where(predicates);
            }

            void AddPredicateForZoneEntities<T>(List<T> entities, Expression<Func<Educator, bool>> predicate)
            {
                if (entities != null && entities.Count != 0)
                {
                    predicates = predicates.Or(predicate);
                }
            }

            return (from x in educators
                    let principalProgram = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program
                    let subProgram = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).Program
                    select new EducatorPaginateResponseDTO()
                    {
                        Id = x.Id,
                        EducatorType = x.EducatorType,
                        IdentityNo = x.User.IdentityNo,
                        Name = x.User.Name,
                        UserId = x.User.Id,
                        AcademicTitle = x.AcademicTitle.Name,
                        Email = x.User.Email,
                        PrincipalBranchName = principalProgram.ExpertiseBranch.Name,
                        PrincipalBranchDutyPlaceId = principalProgram.Id,
                        PrincipalBranchDutyPlace = principalProgram.Hospital.Name,
                        PrincipalBranchDutyPlaceHospitalId = principalProgram.HospitalId,
                        PrincipalBranchDutyPlaceUniversityId = principalProgram.Hospital.Faculty.UniversityId,
                        PrincipalBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y => (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) && y.Program.ExpertiseBranch.IsPrincipal == true).DutyType,
                        SubBranchName = subProgram.ExpertiseBranch.Name,
                        SubBranchDutyPlaceId = subProgram.Id,
                        SubBranchDutyPlace = subProgram.Hospital.Name,
                        SubBranchDutyPlaceHospitalId = subProgram.HospitalId,
                        SubBranchDutyPlaceUniversityId = subProgram.Hospital.Faculty.UniversityId,
                        SubBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y => (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) && y.Program.ExpertiseBranch.IsPrincipal == false).DutyType,
                        IsDeleted = x.IsDeleted,
                        UserIsDeleted = x.User.IsDeleted,
                        DeleteReason = x.DeleteReason,
                        DeleteReasonExplanation = x.DeleteReasonExplanation,
                        Roles = x.User.UserRoles.Select(x => x.Role.RoleName).ToList(),
                        EducatorAdministrativeTitles = x.EducatorAdministrativeTitles.Select(x => x.AdministrativeTitle.Name).ToList(),
                        IsConditionalEducator = x.IsConditionalEducator,
                        ExpertiseBranches = x.EducatorExpertiseBranches.Select(x => x.ExpertiseBranch.Name).ToList(),
                        Phone = x.User.Phone,
                        StaffTitle = x.StaffTitle.Name,
                        EducationOfficerProgramId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).ProgramId,
                        EducationOfficerId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).Id,
                        DutyPlaceHospital = x.EducatorPrograms.FirstOrDefault(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate).Program.Hospital.Name,
                        DutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate).Program.HospitalId
                    });

            return educators.Select(x => new EducatorPaginateResponseDTO()
            {
                Id = x.Id,
                EducatorType = x.EducatorType,
                IdentityNo = x.User.IdentityNo,
                Name = x.User.Name,
                UserId = x.User.Id,
                AcademicTitle = x.AcademicTitle.Name,
                Email = x.User.Email,
                PrincipalBranchName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program.ExpertiseBranch.Name,
                PrincipalBranchDutyPlaceId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).ProgramId,
                PrincipalBranchDutyPlace = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program.Hospital.Name,
                PrincipalBranchDutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program.HospitalId,
                PrincipalBranchDutyPlaceUniversityId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program.Hospital.Faculty.UniversityId,
                PrincipalBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y => (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) && y.Program.ExpertiseBranch.IsPrincipal == true).DutyType,
                SubBranchName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).Program.ExpertiseBranch.Name,
                SubBranchDutyPlaceId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).ProgramId,
                SubBranchDutyPlace = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).Program.Hospital.Name,
                SubBranchDutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).Program.HospitalId,
                SubBranchDutyPlaceUniversityId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).Program.Hospital.Faculty.UniversityId,
                SubBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y => (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) && y.Program.ExpertiseBranch.IsPrincipal == false).DutyType,
                IsDeleted = x.IsDeleted,
                UserIsDeleted = x.User.IsDeleted,
                DeleteReason = x.DeleteReason,
                DeleteReasonExplanation = x.DeleteReasonExplanation,
                Roles = x.User.UserRoles.Select(x => x.Role.RoleName).ToList(),
                EducatorAdministrativeTitles = x.EducatorAdministrativeTitles.Select(x => x.AdministrativeTitle.Name).ToList(),
                IsConditionalEducator = x.IsConditionalEducator,
                ExpertiseBranches = x.EducatorExpertiseBranches.Select(x => x.ExpertiseBranch.Name).ToList(),
                Phone = x.User.Phone,
                StaffTitle = x.StaffTitle.Name,
                EducationOfficerProgramId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).ProgramId,
                EducationOfficerId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).Id,
                DutyPlaceHospital = x.EducatorPrograms.FirstOrDefault(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate).Program.Hospital.Name,
                DutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate).Program.HospitalId
            });
        }

        public async Task<List<EducatorPaginateResponseDTO>> ListForExcelQuery(ZoneModel zone, ProgramFilter filter)
        {
            IQueryable<Educator> educators = dbContext.Educators.AsNoTracking().AsSplitQuery().Where(x => x.EducatorType != EducatorType.NotInstructor && x.EducatorPrograms.Any(x => x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true));

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Educator>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => provinceIds.Contains(a.Program.Hospital.ProvinceId.Value)));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => universityIds.Contains(a.Program.Hospital.Faculty.UniversityId)));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => facultyIds.Contains(a.Program.Hospital.FacultyId.Value)));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => hospitalIds.Contains(a.Program.HospitalId.Value) && (a.DutyEndDate == null || a.DutyEndDate >= DateTime.UtcNow)));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => programIds.Contains(a.ProgramId.Value)));
                }
                if (zone.Students != null && zone.Students.Count != 0)
                {
                    var programId = zone.Students.FirstOrDefault()?.OriginalProgramId;
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => a.ProgramId == programId));
                }
                educators = educators.Where(predicates).Where(x => x.EducatorPrograms.Any(y => y.DutyEndDate == null || y.DutyEndDate >= DateTime.UtcNow));
            }

            //var selectedEducators = educators.Select(x => new EducatorPaginateResponseDTO()
            //{
            //    Id = x.Id,
            //    EducatorType = x.EducatorType,
            //    IdentityNo = x.User.IdentityNo,
            //    Name = x.User.Name,
            //    UserId = x.User.Id,
            //    AcademicTitle = x.AcademicTitle.Name,
            //    Email = x.User.Email,
            //    PrincipalBranchName = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == true).Program.ExpertiseBranch.Name,
            //    PrincipalBranchDutyPlaceId = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == true).ProgramId,
            //    PrincipalBranchDutyPlace = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == true).Program.Hospital.Name,
            //    PrincipalBranchDutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == true).Program.HospitalId,
            //    PrincipalBranchDutyPlaceUniversityId = x.EducatorPrograms
            //        .FirstOrDefault(a =>
            //            (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //            a.Program.ExpertiseBranch.IsPrincipal == true).Program.Hospital.Faculty.UniversityId,
            //    PrincipalBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y =>
            //        (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) &&
            //        y.Program.ExpertiseBranch.IsPrincipal == true).DutyType,
            //    SubBranchName = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == false).Program.ExpertiseBranch.Name,
            //    SubBranchDutyPlaceId = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == false).ProgramId,
            //    SubBranchDutyPlace = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == false).Program.Hospital.Name,
            //    SubBranchDutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a =>
            //        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //        a.Program.ExpertiseBranch.IsPrincipal == false).Program.HospitalId,
            //    SubBranchDutyPlaceUniversityId = x.EducatorPrograms
            //        .FirstOrDefault(a =>
            //            (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
            //            a.Program.ExpertiseBranch.IsPrincipal == false).Program.Hospital.Faculty.UniversityId,
            //    SubBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y =>
            //        (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) &&
            //        y.Program.ExpertiseBranch.IsPrincipal == false).DutyType,
            //    IsDeleted = x.IsDeleted,
            //    UserIsDeleted = x.User.IsDeleted,
            //    DeleteReason = x.DeleteReason,
            //    DeleteReasonExplanation = x.DeleteReasonExplanation,
            //    Roles = x.User.UserRoles.Select(x => x.Role.RoleName).ToList(),
            //    EducatorAdministrativeTitles =
            //        x.EducatorAdministrativeTitles.Select(x => x.AdministrativeTitle.Name).ToList(),
            //    IsConditionalEducator = x.IsConditionalEducator,
            //    ExpertiseBranches = x.EducatorExpertiseBranches.Select(x => x.ExpertiseBranch.Name).ToList(),
            //    Phone = x.User.Phone,
            //    StaffTitle = x.StaffTitle.Name,
            //    EducationOfficerProgramId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).ProgramId,
            //    EducationOfficerId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).Id
            //});

            var selectedEducators = from x in educators
                                    let principalProgram = x.EducatorPrograms.FirstOrDefault(a =>
                                            (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                                            a.Program.ExpertiseBranch.IsPrincipal == true)
                                    let subProgram = x.EducatorPrograms.FirstOrDefault(a =>
                                            (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                                            a.Program.ExpertiseBranch.IsPrincipal == false)
                                    let educationOfficer = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null)

                                    select new EducatorPaginateResponseDTO()
                                    {
                                        Id = x.Id,
                                        EducatorType = x.EducatorType,
                                        IdentityNo = x.User.IdentityNo,
                                        Name = x.User.Name,
                                        UserId = x.User.Id,
                                        AcademicTitle = x.AcademicTitle.Name,
                                        Email = x.User.Email,
                                        DutyPlaceProfession = principalProgram.Program.Hospital.Faculty.Profession.Name,
                                        PrincipalBranchName = principalProgram.Program.ExpertiseBranch.Name,
                                        PrincipalInstitutionName = principalProgram.Program.Hospital.Faculty.University.Institution.Name,
                                        PrincipalInstitutionId = principalProgram.Program.Hospital.Faculty.University.Institution.Id,
                                        PrincipalBranchDutyPlaceId = principalProgram.ProgramId,
                                        PrincipalBranchDutyPlace = principalProgram.Program.Hospital.Name,
                                        PrincipalBranchDutyPlaceHospitalId = principalProgram.Program.HospitalId,
                                        PrincipalBranchDutyPlaceProvinceId = principalProgram.Program.Hospital.ProvinceId,
                                        PrincipalBranchDutyPlaceUniversityId = principalProgram.Program.Hospital.Faculty.UniversityId,
                                        PrincipalBranchDutyPlaceUniversityIsPrivate = principalProgram.Program.Hospital.Faculty.University.IsPrivate,
                                        PrincipalBranchDutyType = (int)principalProgram.DutyType,
                                        SubBranchName = subProgram.Program.ExpertiseBranch.Name,
                                        SubBranchDutyPlaceId = subProgram.ProgramId,
                                        SubBranchDutyPlace = subProgram.Program.Hospital.Name,
                                        SubBranchDutyPlaceHospitalId = subProgram.Program.HospitalId,
                                        SubBranchDutyPlaceUniversityId = subProgram.Program.Hospital.Faculty.UniversityId,
                                        SubBranchDutyType = (int)subProgram.DutyType,
                                        IsDeleted = x.IsDeleted,
                                        UserIsDeleted = x.User.IsDeleted,
                                        DeleteReason = x.DeleteReason,
                                        DeleteReasonExplanation = x.DeleteReasonExplanation,
                                        Roles = x.User.UserRoles.Select(x => x.Role.RoleName).ToList(),
                                        EducatorAdministrativeTitles = x.EducatorAdministrativeTitles.Select(x => x.AdministrativeTitle.Name).ToList(),
                                        IsConditionalEducator = x.IsConditionalEducator,
                                        ExpertiseBranches = x.EducatorExpertiseBranches.Select(x => x.ExpertiseBranch.Name).ToList(),
                                        Phone = x.User.Phone,
                                        StaffTitle = x.StaffTitle.Name,
                                        EducationOfficerProgramId = educationOfficer.ProgramId,
                                        EducationOfficerId = educationOfficer.Id,
                                        StaffInstitution = x.StaffInstitutions.FirstOrDefault(x => x.EndDate == null) != null ? x.StaffInstitutions.FirstOrDefault(x => x.EndDate == null).StaffInstitution.Name : "",
                                        StaffParentInstitution = x.StaffParentInstitutions.FirstOrDefault(x => x.EndDate == null) != null ? x.StaffParentInstitutions.FirstOrDefault(x => x.EndDate == null).StaffParentInstitution.Name : "",
                                    };

            if (filter.ProfessionList?.Any() == true)
            {
                selectedEducators = selectedEducators.Where(a => filter.ProfessionList.Select(x => x.Name).Contains(a.DutyPlaceProfession));
            }
            if (filter.InstitutionList?.Any() == true)
            {
                selectedEducators = selectedEducators.Where(a => filter.InstitutionList.Select(x => x.Id).Contains(a.PrincipalInstitutionId.Value) &&
                                                                 (filter.IsPrivate == null || a.PrincipalBranchDutyPlaceUniversityIsPrivate == filter.IsPrivate));
            }
            if (filter.UniversityList?.Any() == true)
            {
                selectedEducators = selectedEducators.Where(a => filter.UniversityList.Select(x => x.Id).Contains(a.PrincipalBranchDutyPlaceUniversityId.Value));
            }
            if (filter.HospitalList?.Any() == true)
            {
                selectedEducators = selectedEducators.Where(a => filter.HospitalList.Select(x => x.Id).Contains(a.PrincipalBranchDutyPlaceHospitalId.Value));
            }
            if (filter.IsPrincipal != null)
            {
                if (filter.IsPrincipal == true)
                {
                    selectedEducators = selectedEducators.Where(a => filter.ExpertiseBranchList.Select(x => x.Name).Contains(a.PrincipalBranchName));
                }
                else
                {
                    selectedEducators = selectedEducators.Where(a => filter.ExpertiseBranchList.Select(x => x.Name).Contains(a.SubBranchName));
                }
            }
            if (filter.ExpertiseBranchList?.Any() == true)
            {
                selectedEducators = selectedEducators.Where(a => a.ExpertiseBranches.Any(a => filter.ExpertiseBranchList.Select(x => x.Name).Contains(a)));
            }
            if (filter.ProvinceList?.Any() == true)
            {
                selectedEducators = selectedEducators.Where(a => filter.ProvinceList.Select(x => x.Id).Contains(a.PrincipalBranchDutyPlaceProvinceId.Value));
            }

            return await selectedEducators.OrderBy(x => x.PrincipalBranchName).ThenBy(x => x.Name).ToListAsync();
        }

        public IQueryable<Educator> GetByHospitalIdQuery(long hospitalId)
        {
            return (from p in dbContext.Programs
                    join ep in dbContext.EducatorPrograms on p.Id equals ep.ProgramId
                    join e in dbContext.Educators.Include(x => x.User).Include(x => x.AcademicTitle).Include(x => x.StaffTitle) on ep.EducatorId equals e.Id

                    where hospitalId == p.HospitalId && p.IsDeleted == false && e.IsDeleted == false
                    select e);
        }

        public IQueryable<Educator> QueryList(ZoneModel zone)
        {
            IQueryable<Educator> educators = dbContext.Educators.Include(x => x.User).Where(x => !x.IsDeleted);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Educator>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => provinceIds.Contains(a.Program.Hospital.ProvinceId.Value)));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => universityIds.Contains(a.Program.Hospital.Faculty.UniversityId)));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => facultyIds.Contains(a.Program.Hospital.FacultyId.Value)));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => hospitalIds.Contains(a.Program.HospitalId.Value)));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => programIds.Contains(a.ProgramId.Value)));
                }
                educators = educators.Where(predicates);
            }
            return educators;
        }

        public IQueryable<Educator> PaginateListQuery()
        {
            return dbContext.Educators.AsNoTracking().AsSplitQuery()
                                               .Include(x => x.User)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                                               .Include(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.AcademicTitle)
                                               .Include(x => x.StaffTitle);
        }

        public IQueryable<EducatorExp> PaginateListForAdvisorQuery(long eduplaceId)
        {
            return (from e in dbContext.Educators
                                               .Include(x => x.User)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                                               .Include(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.AcademicTitle)
                                               .Include(x => x.StaffTitle)
                    select new EducatorExp
                    {
                        Educator = e,
                        IsExistEducationPlace = e.EducatorPrograms.Any(x => (x.DutyEndDate == null || DateTime.UtcNow <= x.DutyEndDate) && x.Program.HospitalId == eduplaceId),
                    }).AsNoTracking().AsSplitQuery();
        }

        public IQueryable<Educator> PaginateListQueryByCore(long studentId)
        {

            var student = dbContext.Students.AsNoTracking().AsSplitQuery()
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .FirstOrDefault(x => x.Id == studentId);

            return dbContext.Educators.AsNoTracking().AsSplitQuery()
                                               .Include(x => x.User)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                                               .Include(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.AcademicTitle)
                                               .Include(x => x.StaffTitle)
                                               .Where(x => x.EducatorExpertiseBranches.Any(x => x.ExpertiseBranchId == student.Program.ExpertiseBranchId));
        }

        public async Task<Educator> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Educators.AsNoTracking().AsSplitQuery()
                                               .Include(x => x.User)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.EducationOfficers.Where(y => y.EndDate == null))
                                               .Include(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.SubBranches)
                                               .Include(x => x.AcademicTitle)
                                               .Include(x => x.StaffTitle)
                                               .Include(x => x.EducatorAdministrativeTitles).ThenInclude(x => x.AdministrativeTitle)
                                               .Include(x => x.StaffInstitutions).ThenInclude(x => x.StaffInstitution)
                                               .Include(x => x.StaffParentInstitutions).ThenInclude(x => x.StaffParentInstitution)
                                               .Include(x => x.GraduationDetails)
                                               .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public IQueryable<Educator> GetByUniversityIdQuery(long uniId)
        {
            return (from f in dbContext.Faculties
                    join p in dbContext.Programs on f.Id equals p.FacultyId
                    join ep in dbContext.EducatorPrograms.Include(x => x.Educator).ThenInclude(x => x.AcademicTitle).Include(x => x.Educator).ThenInclude(x => x.StaffTitle).Include(x => x.Educator).ThenInclude(x => x.User) on p.Id equals ep.ProgramId
                    where f.UniversityId == uniId && ep.Educator.IsDeleted == false
                    select ep.Educator);
        }

        public IQueryable<Educator> GetByProgramIdQuery(long programId)
        {
            return dbContext.Educators.Where(x => x.EducatorPrograms.Any(y => y.ProgramId == programId && (y.DutyEndDate == null || y.DutyEndDate >= DateTime.UtcNow)) && x.IsDeleted == false && x.User.IsDeleted == false).AsSplitQuery()
                        .Include(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                        .Include(x => x.EducatorPrograms.Where(y => y.DutyEndDate == null || y.DutyEndDate >= DateTime.UtcNow)).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                        .Include(x => x.EducatorPrograms.Where(y => y.DutyEndDate == null || y.DutyEndDate >= DateTime.UtcNow)).ThenInclude(x => x.Program).ThenInclude(x => x.AuthorizationDetails).ThenInclude(x => x.AuthorizationCategory)
                        .Include(x => x.EducatorPrograms.Where(y => y.DutyEndDate == null || y.DutyEndDate >= DateTime.UtcNow)).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                        .Include(x => x.EducationOfficers.Where(y => y.EndDate == null))
                        .Include(x => x.EducatorAdministrativeTitles).ThenInclude(x => x.AdministrativeTitle)
                        .Include(x => x.AcademicTitle)
                        .Include(x => x.StaffTitle)
                        .Include(x => x.User);


            //return (from p in dbContext.Programs
            //        join ep in dbContext.EducatorPrograms.Include(x => x.Educator).ThenInclude(x => x.AcademicTitle)
            //        .Include(x => x.Educator).ThenInclude(x => x.StaffTitle)
            //        .Include(x => x.Educator).ThenInclude(x => x.User)
            //        .Include(x => x.Educator).ThenInclude(x => x.EducatorPrograms.Where(y=>y.DutyEndDate == null)).ThenInclude(x => x.Program)
            //        .Include(x => x.Educator).ThenInclude(x => x.EducationOfficers.Where(y=>y.EndDate == null)) on p.Id equals ep.ProgramId // TODO
            //        where p.Id == programId && p.IsDeleted == false && (ep.DutyEndDate == null || DateTime.UtcNow <= ep.DutyEndDate) && ep.Educator.User.IsDeleted == false
            //        select ep.Educator);
        }

        public async Task<Educator> UpdateWithSubRecords(CancellationToken cancellationToken, long id, Educator educator)
        {
            Educator existEducator = await GetWithSubRecords(cancellationToken, id);

            if (existEducator.IsDeleted == false && educator.IsDeleted == true)
            {
                educator.DeleteDate = DateTime.UtcNow;
            }
            dbContext.Entry(educator).State = EntityState.Modified;

            //if (educator.EducatorPrograms != null)
            //{
            //    foreach (var item in educator.EducatorPrograms)
            //    {
            //        var existEducatorProgram = existEducator.EducatorPrograms.FirstOrDefault(x => x.Id == item.Id);
            //        dbContext.Entry(item).State = existEducatorProgram == null ? EntityState.Added : EntityState.Modified;

            //        var educationOfficer = educator.EducationOfficers.FirstOrDefault(x => x.ProgramId == item.ProgramId && x.EndDate == null);
            //        if (educationOfficer != null && item.DutyEndDate <= DateTime.UtcNow)
            //            educationOfficer.EndDate = item.DutyEndDate;
            //    }

            //    var EducatorProgramsIds = educator.EducatorPrograms.Select(x => x.Id).ToList();
            //    if (existEducator?.EducatorPrograms != null)
            //    {
            //        foreach (var item in existEducator.EducatorPrograms.Where(x => !EducatorProgramsIds.Contains(x.Id)))
            //        {
            //            dbContext.Entry(item).State = EntityState.Deleted;
            //        }
            //    }
            //}

            //if (educator.EducationOfficers != null)
            //{
            //    foreach (var item in educator.EducationOfficers)
            //    {
            //        var existEducationOfficer = existEducator.EducationOfficers.FirstOrDefault(x => x.Id == item.Id);
            //        dbContext.Entry(item).State = existEducationOfficer == null ? EntityState.Added : EntityState.Modified;
            //    }

            //    var EducationOfficerIds = educator.EducationOfficers.Select(x => x.Id).ToList();
            //    if (existEducator?.EducationOfficers != null)
            //    {
            //        foreach (var item in existEducator.EducationOfficers.Where(x => !EducationOfficerIds.Contains(x.Id)))
            //        {
            //            item.EndDate = System.DateTime.UtcNow;
            //            dbContext.Entry(item).State = EntityState.Modified;
            //        }
            //    }
            //}

            if (educator.EducatorExpertiseBranches != null)
            {
                foreach (var item in educator.EducatorExpertiseBranches)
                {
                    var existEducatorExpertiseBranch = existEducator.EducatorExpertiseBranches.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existEducatorExpertiseBranch == null ? EntityState.Added : EntityState.Modified;
                }

                var EducatorExpertiseBranchesIds = educator.EducatorExpertiseBranches.Select(x => x.Id).ToList();
                if (existEducator?.EducatorExpertiseBranches != null)
                {
                    foreach (var item in existEducator.EducatorExpertiseBranches.Where(x => !EducatorExpertiseBranchesIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }

            if (educator.StaffParentInstitutions != null)
            {
                foreach (var item in educator.StaffParentInstitutions)
                {
                    var existStaffParentInstitution = existEducator.StaffParentInstitutions.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existStaffParentInstitution == null ? EntityState.Added : EntityState.Modified;
                }

                var staffParentInstitutionIds = educator.StaffParentInstitutions.Select(x => x.Id).ToList();
                if (existEducator?.StaffParentInstitutions != null)
                {
                    foreach (var item in existEducator.StaffParentInstitutions.Where(x => !staffParentInstitutionIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }

            if (educator.StaffInstitutions != null)
            {
                foreach (var item in educator.StaffInstitutions)
                {
                    var existStaffInstitution = existEducator.StaffInstitutions.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existStaffInstitution == null ? EntityState.Added : EntityState.Modified;
                }

                var staffInstitutionIds = educator.StaffInstitutions.Select(x => x.Id).ToList();
                if (existEducator?.StaffInstitutions != null)
                {
                    foreach (var item in existEducator.StaffInstitutions.Where(x => !staffInstitutionIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }

            if (educator.EducatorAdministrativeTitles != null)
            {
                foreach (var item in existEducator.EducatorAdministrativeTitles)
                {
                    dbContext.Entry(item).State = EntityState.Deleted;
                }
                foreach (var item in educator.EducatorAdministrativeTitles)
                {
                    dbContext.Entry(item).State = EntityState.Added;
                }
            }

            if (educator.GraduationDetails != null)
            {
                foreach (var item in existEducator.GraduationDetails)
                {
                    dbContext.Entry(item).State = EntityState.Deleted;
                }
                foreach (var item in educator.GraduationDetails)
                {
                    dbContext.Entry(item).State = EntityState.Added;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return educator;
        }

        public long EducatorCountByInstitution(ZoneModel zone, long parentInstitutionId)
        {
            IQueryable<Educator> educators = dbContext.Educators.Where(x => x.IsDeleted == false && x.User.IsDeleted == false);
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Educator>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => provinceIds.Contains(a.Program.Hospital.ProvinceId.Value)));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => universityIds.Contains(a.Program.Hospital.Faculty.UniversityId)));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => facultyIds.Contains(a.Program.Hospital.FacultyId.Value)));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => hospitalIds.Contains(a.Program.HospitalId.Value)));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => programIds.Contains(a.ProgramId.Value)));
                }
                educators = educators.Where(predicates);
            }

            return educators.Count(x => x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program.Hospital.Institution.Id == parentInstitutionId);
        }

        public IQueryable<EducatorChartModel> QueryableEducatorsForCharts(ZoneModel zone)
        {
            IQueryable<Educator> educators = dbContext.Educators.Where(x => x.IsDeleted == false && x.User.IsDeleted == false && x.EducatorType == EducatorType.Instructor && x.EducatorPrograms.Any(x => /*x.Program.AuthorizationDetails.Any(x => x.AuthorizationCategory.IsActive == true ) &&*/ (x.DutyEndDate == null || x.DutyEndDate >= DateTime.UtcNow)));
            if (zone.RoleCategory is not RoleCategoryType.Admin or RoleCategoryType.Ministry)
            {
                var predicates = PredicateBuilder.False<Educator>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => provinceIds.Contains(a.Program.Hospital.ProvinceId.Value)));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => universityIds.Contains(a.Program.Hospital.Faculty.UniversityId)));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => facultyIds.Contains(a.Program.Hospital.FacultyId.Value)));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => hospitalIds.Contains(a.Program.HospitalId.Value)));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => programIds.Contains(a.ProgramId.Value)));
                }
                educators = educators.Where(predicates);
            }
            return educators.Select(x => new EducatorChartModel()
            {
                Id = x.Id,
                AcademicTitle = x.AcademicTitle.Name,
                ProgramId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Id,
                ProvinceName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Province.Name,
                ProvinceId = x.EducatorPrograms.FirstOrDefault(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate).Program.Hospital.Province.Id,
                ExpertiseBranchName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.ExpertiseBranch.Name,
                ExpertiseBranchId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.ExpertiseBranch.Id,
                HospitalName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Name,
                HospitalId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.HospitalId,
                UniversityName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Faculty.University.Name,
                UniversityId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Faculty.University.Id,
                FacultyName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Faculty.Name,
                FacultyId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Faculty.Id,
                ProfessionName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.ExpertiseBranch.Profession.Name,
                ProfessionId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.ExpertiseBranch.ProfessionId,
                IsPrincipal = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.ExpertiseBranch.IsPrincipal,
                IsDeleted = x.IsDeleted,
                IsUniversityPrivate = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Faculty.University.IsPrivate,
                ParentInstitutionId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Institution.Id,
                ParentInstitutionName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.Hospital.Institution.Name,
                CountryName = x.User.Country.Name,
                CountryId = x.User.Country.Id,
                Gender = x.User.Gender,

                AuthorizationCategory = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name,
                AuthorizationCategoryIsActive = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.IsActive,
                AuthorizationCategoryId = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate)).Program.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Id,

                //ProgramName = x.Program.Hospital.Province.Name + " - " + x.Program.Hospital.Name + " - " + x.Program.ExpertiseBranch.Name, // ??

            });

        }

        public async Task<Educator> GetByIdForJuryList(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Educators.AsSplitQuery()
                                                 .Include(x => x.User)
                                                 .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital)
                                                 .Include(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.SubBranches)
                                                 .Include(x => x.AcademicTitle).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        }

        public async Task<Educator> GetWithSubRecordsWithZone(CancellationToken cancellationToken, ZoneModel zone, long id)
        {
            IQueryable<Educator> query = dbContext.Educators.AsSplitQuery()
                                               .Include(x => x.User)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                               .Include(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                               .Include(x => x.EducationOfficers.Where(y => y.EndDate == null))
                                               .Include(x => x.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.SubBranches)
                                               .Include(x => x.AcademicTitle)
                                               .Include(x => x.StaffTitle)
                                               .Include(x => x.EducatorAdministrativeTitles).ThenInclude(x => x.AdministrativeTitle)
                                               .Include(x => x.StaffInstitutions).ThenInclude(x => x.StaffInstitution)
                                               .Include(x => x.StaffParentInstitutions).ThenInclude(x => x.StaffParentInstitution)
                                               .Include(x => x.GraduationDetails).Where(x => x.IsDeleted == false);

            var educator = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            var hasActiveDuty = educator.EducatorPrograms.Any(x => x.DutyEndDate == null || x.DutyEndDate > DateTime.UtcNow);

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry && hasActiveDuty)
            {
                var predicates = PredicateBuilder.False<Educator>();

                if (zone.Provinces != null && zone.Provinces.Count != 0)
                {
                    var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => provinceIds.Contains(a.Program.Hospital.ProvinceId.Value)));
                }
                if (zone.Universities != null && zone.Universities.Count != 0)
                {
                    var universityIds = zone.Universities.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => universityIds.Contains(a.Program.Hospital.Faculty.UniversityId)));
                }
                if (zone.Faculties != null && zone.Faculties.Count != 0)
                {
                    var facultyIds = zone.Faculties.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => facultyIds.Contains(a.Program.Hospital.FacultyId.Value)));
                }
                if (zone.Hospitals != null && zone.Hospitals.Count != 0)
                {
                    var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => hospitalIds.Contains(a.Program.HospitalId.Value)));
                }
                if (zone.Programs != null && zone.Programs.Count != 0)
                {
                    var programIds = zone.Programs.Select(x => x.Id).ToList();
                    predicates = predicates.Or(x => x.EducatorPrograms.Any(a => programIds.Contains(a.ProgramId.Value)));
                }
                return await query.Where(predicates).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }
            return educator;
        }

        public IQueryable<EducatorPaginateResponseDTO> GetAllEducatorsForExport()
        {
            return dbContext.Educators.Select(x => new EducatorPaginateResponseDTO()
            {
                Name = x.User.Name,
                Email = x.User.Email,
                Phone = x.User.Phone,
                PrincipalBranchName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == true).Program.ExpertiseBranch.Name,
                SubBranchName = x.EducatorPrograms.FirstOrDefault(a => (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) && a.Program.ExpertiseBranch.IsPrincipal == false).Program.ExpertiseBranch.Name,
                DutyPlaceHospital = x.EducatorPrograms.FirstOrDefault(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate).Program.Hospital.Name,
                DutyPlaceProfession = x.EducatorPrograms.FirstOrDefault(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate).Program.ExpertiseBranch.Profession.Name,
                IsDeleted = x.IsDeleted,
                UserIsDeleted = x.User.IsDeleted,
                EducatorType = x.EducatorType
            });
        }

        public async Task<List<EducatorPaginateResponseDTO>> ListForExcelQuery1()
        {
            IQueryable<Educator> educators = dbContext.Educators;

            var selectedEducators = educators.Select(x => new EducatorPaginateResponseDTO()
            {
                Id = x.Id,
                EducatorType = x.EducatorType,
                IdentityNo = x.User.IdentityNo,
                Name = x.User.Name,
                UserId = x.User.Id,
                AcademicTitle = x.AcademicTitle.Name,
                Email = x.User.Email,
                ProfessionName = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == true).Program.ExpertiseBranch.Profession.Name,
                PrincipalBranchName = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == true).Program.ExpertiseBranch.Name,
                PrincipalBranchDutyPlaceId = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == true).ProgramId,
                PrincipalBranchDutyPlace = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == true).Program.Hospital.Name,
                PrincipalBranchDutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == true).Program.HospitalId,
                PrincipalBranchDutyPlaceUniversityId = x.EducatorPrograms
                    .FirstOrDefault(a =>
                        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                        a.Program.ExpertiseBranch.IsPrincipal == true).Program.Hospital.Faculty.UniversityId,
                PrincipalBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y =>
                    (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) &&
                    y.Program.ExpertiseBranch.IsPrincipal == true).DutyType,
                SubBranchName = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == false).Program.ExpertiseBranch.Name,
                SubBranchDutyPlaceId = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == false).ProgramId,
                SubBranchDutyPlace = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == false).Program.Hospital.Name,
                SubBranchDutyPlaceHospitalId = x.EducatorPrograms.FirstOrDefault(a =>
                    (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                    a.Program.ExpertiseBranch.IsPrincipal == false).Program.HospitalId,
                SubBranchDutyPlaceUniversityId = x.EducatorPrograms
                    .FirstOrDefault(a =>
                        (a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate) &&
                        a.Program.ExpertiseBranch.IsPrincipal == false).Program.Hospital.Faculty.UniversityId,
                SubBranchDutyType = (int)x.EducatorPrograms.FirstOrDefault(y =>
                    (y.DutyEndDate == null || DateTime.UtcNow <= y.DutyEndDate) &&
                    y.Program.ExpertiseBranch.IsPrincipal == false).DutyType,
                IsDeleted = x.IsDeleted,
                UserIsDeleted = x.User.IsDeleted,
                DeleteReason = x.DeleteReason,
                DeleteReasonExplanation = x.DeleteReasonExplanation,
                Roles = x.User.UserRoles.Select(x => x.Role.RoleName).ToList(),
                EducatorAdministrativeTitles =
                    x.EducatorAdministrativeTitles.Select(x => x.AdministrativeTitle.Name).ToList(),
                IsConditionalEducator = x.IsConditionalEducator,
                ExpertiseBranches = x.EducatorExpertiseBranches.Select(x => x.ExpertiseBranch.Name).ToList(),
                Phone = x.User.Phone,
                StaffTitle = x.StaffTitle.Name,
                EducationOfficerProgramId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).ProgramId,
                EducationOfficerId = x.EducationOfficers.FirstOrDefault(x => x.EndDate == null).Id
            });

            return await selectedEducators.ToListAsync();
        }

        public async Task DeleteEducator(CancellationToken cancellationToken, EducatorDTO educatorDTO)
        {
            var educatorRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Code == RoleCodeConstants.EGITICI_CODE, cancellationToken);
            var educator = await dbContext.Educators
                .Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.UserRolePrograms)
                .FirstOrDefaultAsync(x => x.Id == educatorDTO.Id, cancellationToken);

            educator.IsDeleted = true;
            educator.DeleteReason = educatorDTO.DeleteReason;
            educator.DeleteReasonExplanation = educatorDTO.DeleteReasonExplanation;
            educator.DeleteDate = DateTime.UtcNow;
            educator.User.IsDeleted = true;
            educator.User.DeleteDate = DateTime.UtcNow;


            if (educator.User.UserRoles.FirstOrDefault(x => x.RoleId == educatorRole.Id) != null)
                dbContext.UserRoles.Remove(educator.User.UserRoles.FirstOrDefault());
            if (educator.User.UserRoles.FirstOrDefault(x => x.RoleId == educatorRole.Id)?.UserRolePrograms != null)
                dbContext.UserRolePrograms.RemoveRange(educator.User.UserRoles.FirstOrDefault().UserRolePrograms);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UnDeleteEducator(CancellationToken cancellationToken, long id)
        {
            var educatorRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Code == RoleCodeConstants.EGITICI_CODE, cancellationToken);
            var educator = await dbContext.Educators
                .Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.UserRolePrograms)
                .Include(x => x.EducatorPrograms.Where(a => a.DutyEndDate == null || DateTime.UtcNow <= a.DutyEndDate))
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            UserRole userRole = new(userId: educator.UserId ?? 0, roleId: educatorRole.Id);

            if (educator.EducatorPrograms != null && educator.EducatorPrograms?.Count > 0)
            {
                foreach (var item in educator.EducatorPrograms)
                {
                    userRole.UserRolePrograms ??= [];
                    userRole.UserRolePrograms.Add(new() { ProgramId = item.ProgramId });
                }
            }

            educator.IsDeleted = false;
            educator.DeleteDate = null;
            educator.DeleteReason = null;
            educator.DeleteReasonExplanation = null;
            educator.User.IsDeleted = false;
            educator.User.DeleteDate = null;
            educator.User.UserRoles ??= [];
            educator.User.UserRoles.Add(userRole);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<EducatorModel> EducatorTest(CancellationToken cancellationToken, string tckn)
        {
            return await dbContext.Educators.Where(x => x.User.IdentityNo == tckn)
                                                    .Select(x => new EducatorModel()
                                                    {
                                                        kadroUnvani = x.StaffTitle.Name,
                                                        akademikUnvani = x.AcademicTitle.Name,
                                                        ustKurumlar = x.StaffParentInstitutions.Select(x => new ustKurum()
                                                        {
                                                            EndDate = x.EndDate,
                                                            StartDate = x.StartDate,
                                                            StaffParentName = x.StaffParentInstitution.Name
                                                        }).ToList(),
                                                    }).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
