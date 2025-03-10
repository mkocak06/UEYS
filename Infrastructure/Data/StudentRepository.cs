using Core.Entities;
using Core.Entities.Koru;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.BaseModels;
using Shared.Constants;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StudentResponseDTO = Shared.ResponseModels.ENabiz.StudentResponseDTO;

namespace Infrastructure.Data
{
    public class StudentRepository : EfRepository<Student>, IStudentRepository
    {
        private readonly ApplicationDbContext dbContext;
        public StudentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Student> FilterByZone(IQueryable<Student> query, ZoneModel zone)
        {
            var predicates = PredicateBuilder.False<Student>();

            if (zone.Provinces != null && zone.Provinces.Any())
            {
                var provinceIds = zone.Provinces.Select(x => x.Id).ToList();
                predicates = predicates.Or(x => provinceIds.Contains(x.Program.Hospital.ProvinceId.Value));
            }
            if (zone.Universities != null && zone.Universities.Any())
            {
                var universityIds = zone.Universities.Select(x => x.Id).ToList();
                predicates = predicates.Or(x => universityIds.Contains(x.Program.Hospital.Faculty.UniversityId));
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
            if (zone.Students != null && zone.Students.Any())
            {
                var studentId = zone.Students.FirstOrDefault()?.Id;
                predicates = predicates.Or(x => x.Id == studentId);
            }
            return query = query.Where(predicates);
        }

        public IQueryable<Student> GetWithSubRecords(ZoneModel zone)
        {
            IQueryable<Student> query = dbContext.Students.AsSplitQuery().AsNoTracking()
                .Include(x => x.User).ThenInclude(x => x.Country)
                .Include(x => x.EducationTrackings.Where(x => x.IsDeleted == false))
                .Include(x => x.Theses.Where(x => x.IsDeleted == false)).ThenInclude(x => x.ThesisDefences.Where(x => x.IsDeleted == false))
                .Include(x => x.OriginalProgram).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                .Include(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Faculty).ThenInclude(x => x.University)
                .Include(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.Program).ThenInclude(x => x.EducationOfficers).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
                .Include(x => x.Curriculum).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.StudentExpertiseBranches).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.PrincipalBranches).Where(x => x.IsHardDeleted == false);

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            return query;
        }

        public IQueryable<Student> GetRegistrationStudentQuery(ZoneModel zone)
        {
            //IQueryable<Student> query = dbContext.Students.AsSplitQuery().AsNoTracking()
            //    .Include(x => x.User).ThenInclude(x => x.Country)
            //    .Include(x => x.EducationTrackings.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Program.Hospital.Province)
            //    .Include(x => x.EducationTrackings.Where(x => x.IsDeleted == false)).ThenInclude(x => x.Program.ExpertiseBranch)
            //    .Include(x => x.Theses.Where(x => x.IsDeleted == false)).ThenInclude(x => x.ThesisDefences.Where(x => x.IsDeleted == false))
            //    .Include(x => x.OriginalProgram.ExpertiseBranch)
            //    .Include(x => x.OriginalProgram.Hospital.Province)
            //    .Include(x => x.OriginalProgram).ThenInclude(x => x.EducationOfficers.Where(x => x.EndDate == null || x.EndDate <= DateTime.UtcNow)).ThenInclude(x => x.Educator).ThenInclude(x => x.User)
            //    .Include(x => x.Curriculum).ThenInclude(x => x.ExpertiseBranch)
            //    .Include(x => x.ExitExams.Where(x => x.IsDeleted == false))
            //    .Include(x => x.StudentRotations).ThenInclude(x => x.Rotation.ExpertiseBranch)
            //    .Include(x => x.StudentRotations).ThenInclude(x => x.Program.ExpertiseBranch)
            //    .Include(x => x.StudentRotations).ThenInclude(x => x.Program.Hospital.Province);

            IQueryable<Student> query = dbContext.Students.AsSplitQuery().AsNoTracking();

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            return query;
        }

        public IQueryable<StudentPaginateResponseDTO> OnlyPaginateListQuery(ZoneModel zone)
        {
            //        var result = dbContext.UserRoles
            //.AsNoTracking()  // Performans için tracking'i kapatýyoruz
            //.Where(ur => ur.RoleId == 66)  // RoleId'si 66 olan kayýtlar
            //.GroupJoin(
            //    dbContext.UserRolePrograms,  // LEFT JOIN: UserRole ile UserRoleProgram
            //    ur => ur.Id,
            //    urp => urp.UserRoleId,
            //    (ur, urpGroup) => new { UserRole = ur, UserRolePrograms = urpGroup }
            //)
            //.SelectMany(
            //    urWithPrograms => urWithPrograms.UserRolePrograms.DefaultIfEmpty(),  // LEFT JOIN'den dolayý null kontrolü
            //    (urWithPrograms, urp) => new { urWithPrograms.UserRole, UserRoleProgram = urp }
            //)
            //.Where(x => x.UserRoleProgram == null)  // UserRoleProgram kaydý olmayanlar
            //.Join(
            //    dbContext.Users,  // User tablosuyla birleþtir
            //    urWithNoProgram => urWithNoProgram.UserRole.UserId,
            //    u => u.Id,
            //    (urWithNoProgram, u) => new { urWithNoProgram.UserRole, User = u }
            //)
            //.Join(
            //    dbContext.Educators.Where(e => !e.IsDeleted),  // Silinmemiþ Educator'larý alýyoruz
            //    userWithRole => userWithRole.User.Id,
            //    e => e.UserId,
            //    (userWithRole, e) => new { userWithRole.UserRole, userWithRole.User, Educator = e }
            //)
            //.GroupJoin(
            //    dbContext.EducatorPrograms,  // Educator ile EducatorProgram iliþkisini kuruyoruz
            //    educatorWithUser => educatorWithUser.Educator.Id,
            //    ep => ep.EducatorId,
            //    (educatorWithUser, educatorPrograms) => new
            //    {
            //        educatorWithUser.UserRole,
            //        educatorWithUser.User,
            //        educatorWithUser.Educator,
            //        EducatorPrograms = educatorPrograms.ToList()  // EducatorProgram'larý listeye dönüþtürüyoruz
            //    }
            //)
            //.ToList();



            //        foreach (var item in result)
            //        {
            //            foreach (var eduProg in item.EducatorPrograms)
            //            {
            //                if (eduProg.DutyEndDate == null || eduProg.DutyEndDate <= DateTime.UtcNow)
            //                    dbContext.UserRolePrograms.Add(new() { UserRoleId = item.UserRole.Id, ProgramId = eduProg.ProgramId });
            //            }
            //        }



            IQueryable<Student> query = dbContext.Students.Where(x => x.IsHardDeleted == false);
            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry && zone.RoleCategory != RoleCategoryType.Registration)
                query = FilterByZone(query, zone);

            return query.Select(x => new StudentPaginateResponseDTO
            {
                Id = x.Id,
                Name = x.User.Name,
                IdentityNo = x.User.IdentityNo,
                ProfessionName = x.OriginalProgram.ExpertiseBranch.Profession.Name,
                HospitalName = x.OriginalProgram.Hospital.Name,
                ExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                CurriculumVersion = x.Curriculum.Version,
                Status = x.Status,
                IsDeleted = x.IsDeleted,
                UserIsDeleted = x.User.IsDeleted,
                AdvisorId = x.AdvisorId,
                CurriculumId = x.CurriculumId,
                DeleteReason = x.DeleteReason,
                ProgramId = x.ProgramId,
                UserId = x.UserId,
                HospitalId = x.OriginalProgram.HospitalId,
                UniversityId = x.OriginalProgram.Hospital.Faculty.UniversityId,
                OriginalProgramId = x.OriginalProgramId,
                OriginalHospitalId = x.OriginalProgram.HospitalId,
                OriginalUniversityId = x.OriginalProgram.Hospital.Faculty.UniversityId,
                OriginalHospitalName = x.OriginalProgram.Hospital.Name,
                OriginalExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                FormerHospitalId = x.EducationTrackings.OrderByDescending(y => y.ProcessDate).FirstOrDefault(y => y.ReasonType == ReasonType.LeavingTheInstitutionDueToRotation && y.ProgramId == x.ProgramId).FormerProgram.HospitalId,
                FormerUniversityId = x.EducationTrackings.OrderByDescending(y => y.ProcessDate).FirstOrDefault(y => y.ReasonType == ReasonType.LeavingTheInstitutionDueToRotation && y.ProgramId == x.ProgramId).FormerProgram.Hospital.Faculty.UniversityId,
                FormerHospitalName = x.EducationTrackings.OrderByDescending(y => y.ProcessDate).FirstOrDefault(y => y.ReasonType == ReasonType.LeavingTheInstitutionDueToRotation && y.ProgramId == x.ProgramId).FormerProgram.Hospital.Name,
                FormerExpertiseBranchName = x.EducationTrackings.OrderByDescending(y => y.ProcessDate).FirstOrDefault(y => y.ReasonType == ReasonType.LeavingTheInstitutionDueToRotation && y.ProgramId == x.ProgramId).FormerProgram.ExpertiseBranch.Name,
                FormerProgramId = x.EducationTrackings.OrderByDescending(y => y.ProcessDate).FirstOrDefault(y => y.ReasonType == ReasonType.LeavingTheInstitutionDueToRotation && y.ProgramId == x.ProgramId).FormerProgramId,
                ProtocolProgramId = x.ProtocolProgramId,
                ProtocolHospitalName = x.ProtocolProgram.Hospital.Name,
                ProtocolExpertiseBranchName = x.ProtocolProgram.ExpertiseBranch.Name,
                ProtocolType = dbContext.DependentPrograms.FirstOrDefault(y => y.ProgramId == x.ProtocolProgramId).RelatedDependentProgram.ProtocolProgram.Type,
                Gender = x.User.Gender,
                RemainingDays = (x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish).ProcessDate - DateTime.UtcNow).Value.Days,
                BeginningDate = x.EducationTrackings.FirstOrDefault(y => y.ReasonType == ReasonType.BeginningSpecializationEducation && y.IsDeleted == false).ProcessDate,
                QuatoType = x.QuotaType,
                QuatoType1 = x.QuotaType_1,
                QuatoType2 = x.QuotaType_2,
                Thesis = new() { Status = x.Theses.OrderByDescending(y => y.UploadDate).FirstOrDefault(y => y.IsDeleted == false).Status, },
                EstimatedFinish = x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish).ProcessDate,
                ExitExam = new() { AbilityExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).AbilityExamNote, PracticeExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).PracticeExamNote, ExamStatus = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).ExamStatus },
                ConditionallyGraduated = x.ConditionallyGraduated
            });
        }

        public IQueryable<BreadCrumbSearchResponseDTO> GetListForBreadCrumb(ZoneModel zone)
        {
            IQueryable<Student> query = dbContext.Students.Where(x => x.IsDeleted == false && x.User.IsDeleted == false && x.IsHardDeleted == false);

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            return query.Select(x => new BreadCrumbSearchResponseDTO { Id = x.Id, Name = x.User.Name });

        }

        public List<CountsByMonthsResponse> GetCountsByMonthsResponse()
        {
            var nowUtc = DateTime.UtcNow;
            var sixMonthsAgoUtc = nowUtc.Date.AddMonths(-6);

            return dbContext.Students
                .Where(s => s.CreateDate >= sixMonthsAgoUtc && s.CreateDate <= nowUtc)
                .GroupBy(s => new { s.CreateDate.Value.Year, s.CreateDate.Value.Month })
                .Select(g => new CountsByMonthsResponse { Month = g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

        }

        public async Task<ProtocolProgram> IsProtocolProgram(long programId, CancellationToken cancellationToken)
        {
            return await dbContext.ProtocolPrograms.FirstOrDefaultAsync(x => x.ParentProgramId == programId && x.RelatedDependentPrograms.Any(x => x.IsActive == true && x.ChildPrograms.Count > 0), cancellationToken);
        }

        public long StudentCountByInstitution(ZoneModel zone, long parentInstitutionId)
        {
            IQueryable<Student> query = dbContext.Students.Where(x => x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false);

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            return query.Count(x => x.Program.Hospital.Institution.Id == parentInstitutionId);
        }

        public IQueryable<StudentChartModel> QueryableStudentsForCharts(ZoneModel zone)
        {
            IQueryable<Student> query = dbContext.Students.Where(x => x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false && x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration && x.Status != StudentStatus.EducationEnded);

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            return query.Select(x => new StudentChartModel()
            {
                Id = x.Id,
                ProvinceName = x.OriginalProgram.Hospital.Province.Name,
                ProvinceId = x.OriginalProgram.Hospital.Province.Id,
                ExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                ExpertiseBranchId = x.OriginalProgram.ExpertiseBranch.Id,
                HospitalName = x.OriginalProgram.Hospital.Name,
                HospitalId = x.OriginalProgram.HospitalId,
                UniversityName = x.OriginalProgram.Hospital.Faculty.University.Name,
                UniversityId = x.OriginalProgram.Hospital.Faculty.University.Id,
                FacultyName = x.OriginalProgram.Faculty.Name,
                FacultyId = x.OriginalProgram.Faculty.Id,
                ProfessionName = x.OriginalProgram.ExpertiseBranch.Profession.Name,
                ProfessionId = x.OriginalProgram.ExpertiseBranch.ProfessionId,
                IsPrincipal = x.OriginalProgram.ExpertiseBranch.IsPrincipal,
                IsDeleted = x.IsDeleted,
                IsUniversityPrivate = x.OriginalProgram.Hospital.Faculty.University.IsPrivate,
                ParentInstitutionId = x.OriginalProgram.Hospital.Institution.Id,
                ParentInstitutionName = x.OriginalProgram.Hospital.Institution.Name,
                CountryName = x.User.Country.Name,
                CountryId = x.User.Country.Id,
                Gender = x.User.Gender,
                PlacementExamType = x.BeginningExam,
                //QuotaType_1 = x.QuotaType_1,
                //QuotaType_2 = x.QuotaType_2,
                QuotaType = x.QuotaType,

                AuthorizationCategory = x.OriginalProgram.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name,
                AuthorizationCategoryIsActive = x.OriginalProgram.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.IsActive,
                AuthorizationCategoryId = x.OriginalProgram.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Id,

                //OriginalProgramName = x.OriginalProgram.Hospital.Province.Name + " - " + x.OriginalProgram.Hospital.Name + " - " + x.OriginalProgram.ExpertiseBranch.Name, // ??

            });
        }

        public async Task<List<StudentResponseDTO>> StudentListEnabiz(CancellationToken cancellationToken, DateTime? createdDate)
        {
            return await dbContext.Students.AsNoTracking()
                                           .Where(x => x.User.IsDeleted == false && x.IsDeleted == false && x.IsHardDeleted == false && (createdDate == null || x.CreateDate > createdDate))
                                           .Select(x => new StudentResponseDTO()
                                           {
                                               Name = x.User.Name,
                                               IdentityNumber = x.User.IdentityNo,
                                               CreateDate = x.CreateDate,
                                               ExpertiseBranchCode = x.OriginalProgram.ExpertiseBranch.Code,
                                               ExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                                               GraduationDate = x.EducationTrackings.FirstOrDefault(x => x.IsDeleted == false && x.ReasonType == ReasonType.SuccessfullyCompleted).ProcessDate
                                           })
                                           .ToListAsync(cancellationToken);
        }

        public IQueryable<StudentExcelExportModel> ExportExcelQuery(ZoneModel zone)
        {
            IQueryable<Student> query = dbContext.Students.AsNoTracking().Where(x => x.IsHardDeleted == false);
            
            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            var hekimQuery = dbContext.ENabizPortfolios
                                        .GroupBy(ah => ah.hekim_kimlik_numarasi)
                                        .Select(g => new
                                        {
                                            KimlikNo = g.Key,
                                            muayene_sayisi = g.Sum(x => x.muayene_sayisi),
                                            recete_sayisi = g.Sum(x => x.recete_sayisi),
                                            islem_sayisi = g.Sum(x => x.islem_sayisi),
                                            tahlil_tetkik_ve_radyoloji_islemleri_sayisi = g.Sum(x => x.tahlil_tetkik_ve_radyoloji_islemleri_sayisi),
                                            a_grubu_ameliyat_sayisi = g.Sum(x => x.a_grubu_ameliyat_sayisi),
                                            b_grubu_ameliyat_sayisi = g.Sum(x => x.b_grubu_ameliyat_sayisi),
                                            c_grubu_ameliyat_sayisi = g.Sum(x => x.c_grubu_ameliyat_sayisi),
                                            d_grubu_ameliyat_sayisi = g.Sum(x => x.d_grubu_ameliyat_sayisi),
                                            e_grubu_ameliyat_sayisi = g.Sum(x => x.e_grubu_ameliyat_sayisi),
                                        });

            return from x in query
                   join ah in hekimQuery on x.User.IdentityNo equals ah.KimlikNo into hekimGroup
                   from studentProgress in hekimGroup.DefaultIfEmpty()
                   select new StudentExcelExportModel
                   {
                       IsDeleted = x.IsDeleted,
                       UserIsDeleted = x.User.IsDeleted,
                       IdentityNo = x.User.IdentityNo,
                       Name = x.User.Name,
                       Gender = x.User.Gender,
                       BirthDate = x.User.BirthDate,
                       Nationality = x.User.Nationality,
                       ProfessionName = x.OriginalProgram.ExpertiseBranch.Profession.Name,
                       OriginalHospitalName = x.OriginalProgram.Hospital.Name,
                       OriginalExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                       ExpertiseBranchIsPrincipal = x.OriginalProgram.ExpertiseBranch.IsPrincipal,
                       ProgramCategory = x.OriginalProgram.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name,
                       QuatoType = x.QuotaType,
                       QuatoType1 = x.QuotaType_1,
                       QuatoType2 = x.QuotaType_2,
                       BeginningExam = x.BeginningExam,
                       BeginningYear = x.BeginningYear,
                       BeginningPeriod = x.BeginningPeriod,
                       BeginningDate = x.EducationTrackings.OrderBy(x => x.ProcessDate).FirstOrDefault(y => y.ProcessType == ProcessType.Start && y.IsDeleted == false).ProcessDate,
                       EstimatedFinish = x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish).ProcessDate,
                       RemainingDays = (x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish).ProcessDate - DateTime.UtcNow).Value.Days,
                       ThesisStatusType = x.Theses.OrderByDescending(y => y.UploadDate).FirstOrDefault(y => y.IsDeleted == false).Status,
                       ExamStatus = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).ExamStatus,
                       AbilityExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).AbilityExamNote,
                       PracticeExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).PracticeExamNote,
                       CurriculumVersion = x.Curriculum.Version,
                       Status = x.Status,
                       ProvinceName = x.OriginalProgram.Hospital.Province.Name,
                       IsThesisSubjectDetermined = x.Theses.OrderByDescending(y => y.UploadDate).FirstOrDefault(y => y.IsDeleted == false).SubjectDetermineDate != null,
                       ThesisAdvisorAssingnDate = x.Theses.OrderByDescending(y => y.UploadDate).FirstOrDefault(y => y.IsDeleted == false).AdvisorTheses.OrderBy(x => x.AdvisorAssignDate).FirstOrDefault().AdvisorAssignDate,
                       ThesisSubjectDetermineDate = x.Theses.OrderByDescending(y => y.UploadDate).FirstOrDefault(y => y.IsDeleted == false).SubjectDetermineDate,

                       MuayeneSayisi = studentProgress.muayene_sayisi,
                       ReceteSayisi = studentProgress.recete_sayisi,
                       IslemSayisi = studentProgress.islem_sayisi,
                       LaboratuvarTetkikÝstemiSayisi = studentProgress.tahlil_tetkik_ve_radyoloji_islemleri_sayisi,
                       ATypeSurgeryCount = studentProgress.a_grubu_ameliyat_sayisi,
                       BTypeSurgeryCount = studentProgress.b_grubu_ameliyat_sayisi,
                       CTypeSurgeryCount = studentProgress.c_grubu_ameliyat_sayisi,
                       DTypeSurgeryCount = studentProgress.d_grubu_ameliyat_sayisi,
                       ETypeSurgeryCount = studentProgress.e_grubu_ameliyat_sayisi,
                   };
        }

        public async Task<List<StudentExcelExportModel>> ExportExcelDetailedQuery(ZoneModel zone, ProgramFilter filter)
        {
            IQueryable<Student> query = dbContext.Students.AsNoTracking().AsSplitQuery().Where(x => !x.IsHardDeleted && !x.User.IsDeleted && !x.IsDeleted &&
                                                                      x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration &&
                                                                      x.Status != StudentStatus.EducationEnded &&
                                                                      x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true);

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            //var selectedStudents = students.Select(x => new StudentExcelExportModel
            //{
            //    IsDeleted = x.IsDeleted,
            //    UserIsDeleted = x.User.IsDeleted,
            //    IdentityNo = x.User.IdentityNo,
            //    Name = x.User.Name,
            //    Gender = x.User.Gender,
            //    Nationality = x.User.Nationality,
            //    ProfessionName = x.OriginalProgram.ExpertiseBranch.Profession.Name,
            //    OriginalHospitalName = x.OriginalProgram.Hospital.Name,
            //    OriginalExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
            //    ExpertiseBranchIsPrincipal = x.OriginalProgram.ExpertiseBranch.IsPrincipal,
            //    ProgramCategory = x.OriginalProgram.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name,
            //    QuatoType = x.QuotaType,
            //    QuatoType1 = x.QuotaType_1,
            //    QuatoType2 = x.QuotaType_2,
            //    BeginningExam = x.BeginningExam,
            //    BeginningYear = x.BeginningYear,
            //    BeginningPeriod = x.BeginningPeriod,
            //    BeginningDate = x.EducationTrackings.FirstOrDefault(y => y.ReasonType == ReasonType.BeginningSpecializationEducation && y.IsDeleted == false).ProcessDate,
            //    EstimatedFinish = x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish).ProcessDate,
            //    RemainingDays = (x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish).ProcessDate - DateTime.UtcNow).Value.Days,
            //    ThesisStatusType = x.Theses.OrderByDescending(y => y.UploadDate).FirstOrDefault(y => y.IsDeleted == false).Status,
            //    ExamStatus = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).ExamStatus,
            //    AbilityExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).AbilityExamNote,
            //    PracticeExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false).PracticeExamNote,
            //    CurriculumVersion = x.Curriculum.Version,
            //    Status = x.Status,
            //});

            var selectedStudents = from x in query
                                   let estimatedFinish = x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish).ProcessDate
                                   let exam = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false)

                                   select new StudentExcelExportModel()
                                   {
                                       IdentityNo = x.User.IdentityNo,
                                       Name = x.User.Name,
                                       Gender = x.User.Gender,
                                       Nationality = x.User.Nationality,
                                       ProfessionName = x.OriginalProgram.ExpertiseBranch.Profession.Name,
                                       OriginalUniversityName = x.OriginalProgram.Hospital.Faculty.University.Name,
                                       OriginalUniversityIsPrivate = x.OriginalProgram.Hospital.Faculty.University.IsPrivate,
                                       OriginalInstitutionName = x.OriginalProgram.Hospital.Faculty.University.Institution.Name,
                                       OriginalHospitalName = x.OriginalProgram.Hospital.Name,
                                       ProvinceName = x.OriginalProgram.Hospital.Province.Name,
                                       OriginalInstitutionId = x.OriginalProgram.Hospital.Faculty.University.Institution.Id,
                                       OriginalExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                                       ExpertiseBranchIsPrincipal = x.OriginalProgram.ExpertiseBranch.IsPrincipal,
                                       ProgramCategory = x.OriginalProgram.AuthorizationDetails.Where(x => x.IsDeleted == false).OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault().AuthorizationCategory.Name,
                                       QuatoType = x.QuotaType,
                                       QuatoType1 = x.QuotaType_1,
                                       QuatoType2 = x.QuotaType_2,
                                       BeginningExam = x.BeginningExam,
                                       BeginningYear = x.BeginningYear,
                                       BeginningPeriod = x.BeginningPeriod,
                                       BeginningDate = x.EducationTrackings.FirstOrDefault(y => y.ReasonType == ReasonType.BeginningSpecializationEducation && y.IsDeleted == false).ProcessDate,
                                       EstimatedFinish = estimatedFinish,
                                       RemainingDays = (estimatedFinish - DateTime.UtcNow).Value.Days,
                                       ThesisStatusType = x.Theses.OrderByDescending(y => y.UploadDate).FirstOrDefault(y => y.IsDeleted == false).Status,
                                       ExamStatus = exam.ExamStatus,
                                       AbilityExamNote = exam.AbilityExamNote,
                                       PracticeExamNote = exam.PracticeExamNote,
                                       CurriculumVersion = x.Curriculum.Version,
                                       Status = x.Status,
                                   };

            if (filter.ProfessionList?.Any() == true)
            {
                selectedStudents = selectedStudents.Where(a => filter.ProfessionList.Select(x => x.Name).Contains(a.ProfessionName));
            }
            if (filter.InstitutionList?.Any() == true)
            {
                selectedStudents = selectedStudents.Where(a => filter.InstitutionList.Select(x => x.Id).Contains(a.OriginalInstitutionId.Value) &&
                                                               (filter.IsPrivate == null || a.OriginalUniversityIsPrivate == filter.IsPrivate));
            }
            if (filter.UniversityList?.Any() == true)
            {
                selectedStudents = selectedStudents.Where(a => filter.UniversityList.Select(x => x.Name).Contains(a.OriginalUniversityName));
            }
            if (filter.HospitalList?.Any() == true)
            {
                selectedStudents = selectedStudents.Where(x => filter.HospitalList.Select(x => x.Name).Contains(x.OriginalHospitalName));
            }
            if (filter.IsPrincipal != null)
            {
                selectedStudents = selectedStudents.Where(a => filter.IsPrincipal == null || filter.IsPrincipal == a.ExpertiseBranchIsPrincipal);
            }
            if (filter.ExpertiseBranchList?.Any() == true)
            {
                selectedStudents = selectedStudents.Where(x => filter.ExpertiseBranchList.Select(x => x.Name).Contains(x.OriginalExpertiseBranchName));
            }
            if (filter.ProvinceList?.Any() == true)
            {
                selectedStudents = selectedStudents.Where(x => filter.ProvinceList.Select(x => x.Name).Contains(x.ProvinceName));
            }

            return await selectedStudents.OrderBy(x => x.OriginalExpertiseBranchName).ThenBy(x => x.Name).ToListAsync();
        }

        public async Task<List<RestartStudentUserModel>> GetRestartStudents(CancellationToken cancellationToken)
        {

            var users = await dbContext.Students.Where(x => !x.IsHardDeleted && x.DeleteReasonType != ReasonType.BranchChange && x.DeleteReasonType != ReasonType.RegistrationByMistake && x.UserId != null).GroupBy(x => new { x.User.IdentityNo, x.User.Name }).Where(x => x.Count() > 1).Select(x => new RestartStudentUserModel()
            {
                Name = x.Key.Name,
                Students = x.Select(y => new RestartStudentModel()
                {
                    StudentId = y.Id,
                    StudentDeleteReasonType = y.DeleteReasonType,
                    IsHardDeleted = y.IsHardDeleted,
                    IsDeleted = y.IsDeleted,
                    ProgramId = y.OriginalProgramId,
                    ProgramName = y.OriginalProgram.ExpertiseBranch.Name,
                    HospitalName = y.OriginalProgram.Hospital.Name
                })
            }).ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                var uniqueStudents = user.Students.GroupBy(s => s.ProgramId)
                                                  .Select(g => g.First())
                                                  .ToList();
                user.Students = uniqueStudents;
            }

            var newUsers = new List<RestartStudentUserModel>();

            newUsers = users.Where(x => x.Students.Count() > 1).ToList();

            return newUsers;
        }

        public async Task<List<StudentExcelExportModel>> ExportExcelDetailedQuery1()
        {
            IQueryable<Student> students = dbContext.Students.Where(x =>
                !x.IsHardDeleted && !x.User.IsDeleted && !x.IsDeleted &&
                x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration &&
                x.Status != StudentStatus.EducationEnded);

            var selectedStudents = students.Select(x => new StudentExcelExportModel
            {
                IsDeleted = x.IsDeleted,
                UserIsDeleted = x.User.IsDeleted,
                IdentityNo = x.User.IdentityNo,
                Name = x.User.Name,
                Gender = x.User.Gender,
                Nationality = x.User.Nationality,
                ProfessionName = x.OriginalProgram.ExpertiseBranch.Profession.Name,
                OriginalHospitalName = x.OriginalProgram.Hospital.Name,
                OriginalExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                ExpertiseBranchIsPrincipal = x.OriginalProgram.ExpertiseBranch.IsPrincipal,
                ProgramCategory = x.OriginalProgram.AuthorizationDetails.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate)
                    .FirstOrDefault().AuthorizationCategory.Name,
                QuatoType = x.QuotaType,
                QuatoType1 = x.QuotaType_1,
                QuatoType2 = x.QuotaType_2,
                BeginningExam = x.BeginningExam,
                BeginningYear = x.BeginningYear,
                BeginningPeriod = x.BeginningPeriod,
                BeginningDate = x.EducationTrackings.FirstOrDefault(y =>
                    y.ReasonType == ReasonType.BeginningSpecializationEducation && y.IsDeleted == false).ProcessDate,
                EstimatedFinish = x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish)
                    .ProcessDate,
                RemainingDays =
                    (x.EducationTrackings.FirstOrDefault(y => y.ProcessType == ProcessType.EstimatedFinish)
                        .ProcessDate - DateTime.UtcNow).Value.Days,
                ThesisStatusType = x.Theses.OrderByDescending(y => y.UploadDate)
                    .FirstOrDefault(y => y.IsDeleted == false).Status,
                ExamStatus = x.ExitExams.OrderByDescending(y => y.ExamDate).FirstOrDefault(y => y.IsDeleted == false)
                    .ExamStatus,
                AbilityExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate)
                    .FirstOrDefault(y => y.IsDeleted == false).AbilityExamNote,
                PracticeExamNote = x.ExitExams.OrderByDescending(y => y.ExamDate)
                    .FirstOrDefault(y => y.IsDeleted == false).PracticeExamNote,
                CurriculumVersion = x.Curriculum.Version,
                Status = x.Status,
            });

            return await selectedStudents.ToListAsync();
        }

        public IQueryable<StudentPaginateResponseDTO> GetExpiredStudents(ZoneModel zone)
        {
            IQueryable<Student> query = dbContext.Students.Where(x => !x.IsHardDeleted && !x.IsDeleted && !x.User.IsDeleted && x.Status == StudentStatus.EstimatedFinishDatePast);

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
                query = FilterByZone(query, zone);

            return query.Select(x => new StudentPaginateResponseDTO
            {
                Id = x.Id,
                Name = x.User.Name,
                IdentityNo = x.User.IdentityNo,
                Status = x.Status,
                IsDeleted = x.IsDeleted,
                UserIsDeleted = x.User.IsDeleted,
                OriginalHospitalName = x.OriginalProgram.Hospital.Name,
                OriginalExpertiseBranchName = x.OriginalProgram.ExpertiseBranch.Name,
                Gender = x.User.Gender,
            });
        }

        public async Task DeleteStudent(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO)
        {
            var studentRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Code == RoleCodeConstants.UZMANLIK_OGRENCISI_CODE, cancellationToken);
            var student = await dbContext.Students
                .Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.UserRolePrograms)
                .FirstOrDefaultAsync(x => x.Id == educationTrackingDTO.StudentId, cancellationToken);

            student.IsDeleted = true;
            student.DeleteReasonType = educationTrackingDTO.ReasonType;
            student.DeleteReason = educationTrackingDTO.Description;
            student.DeleteDate = DateTime.UtcNow;
            student.User.IsDeleted = true;
            student.User.DeleteDate = DateTime.UtcNow;
            if (educationTrackingDTO.ReasonType == ReasonType.RegistrationByMistake)
                student.IsHardDeleted = true;


            if (student.User.UserRoles.FirstOrDefault(x => x.RoleId == studentRole.Id) != null)
                dbContext.UserRoles.Remove(student.User.UserRoles.FirstOrDefault());
            if (student.User.UserRoles.FirstOrDefault(x => x.RoleId == studentRole.Id)?.UserRoleStudents != null)
                dbContext.UserRoleStudents.RemoveRange(student.User.UserRoles.FirstOrDefault().UserRoleStudents);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UnDeleteStudent(CancellationToken cancellationToken, long id)
        {
            var studentRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Code == RoleCodeConstants.UZMANLIK_OGRENCISI_CODE, cancellationToken);
            var student = await dbContext.Students
                .Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.UserRoleStudents)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            UserRole userRole = new(userId: student.UserId ?? 0, roleId: studentRole.Id)
            {
                UserRoleStudents = [new() { StudentId = id }]
            };

            student.IsDeleted = false;
            student.DeleteDate = null;
            student.DeleteReason = null;
            student.DeleteReasonType = null;
            student.User.IsDeleted = false;
            student.User.DeleteDate = null;
            student.User.UserRoles ??= [];
            student.User.UserRoles.Add(userRole);

            var estimatedFinish = await dbContext.EducationTrackings.FirstOrDefaultAsync(x => x.StudentId == id && x.ProcessType == ProcessType.EstimatedFinish, cancellationToken);
            estimatedFinish.IsDeleted = false;
            estimatedFinish.DeleteDate = null;
            var finish = await dbContext.EducationTrackings.FirstOrDefaultAsync(x => x.StudentId == id && x.ProcessType == ProcessType.Finish && !x.IsDeleted, cancellationToken);
            finish.IsDeleted = true;
            finish.DeleteDate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsActiveStudent(CancellationToken cancellationToken, PlacementExamType sinavTuru, string kimlikNo, DateTime? oncekiSinavTarihi, DateTime? mevcutSinavTarihi)
        {
            var isStudent = await dbContext.Students.AnyAsync(x => x.BeginningExam == sinavTuru && x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false && x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration && x.Status != StudentStatus.EducationEnded && x.User.IdentityNo == kimlikNo, cancellationToken);

            if (isStudent)
            {
                return true;
            }
            else
            {
                return await dbContext.EducationTrackings.AnyAsync(x => x.Student.BeginningExam == sinavTuru && x.IsDeleted == false && x.ReasonType == ReasonType.LeftDueToResignation && x.Student.User.IdentityNo == kimlikNo && ((x.ProcessDate.Value.Date > oncekiSinavTarihi && x.ProcessDate.Value.Date <= mevcutSinavTarihi) || (x.ProcessDate.Value.Date < oncekiSinavTarihi || x.ProcessDate.Value.Date >= mevcutSinavTarihi)), cancellationToken);
            }
        }
    }
}
