using Core.Entities;
using Core.Entities.Koru;
using Core.Extentsions;
using Core.Interfaces;
using Core.Models.Authorization;
using Core.Models.ConfigModels;
using Core.Models.LogInformation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.Models.SMSModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        private readonly AppSettingsModel appSettingsModel;
        private readonly ApplicationDbContext applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext, AppSettingsModel appSettingsModel) : base(applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
            this.appSettingsModel = appSettingsModel;
        }
        public User Get(string email)
        {
            return applicationDbContext.Users.Where(x => x.Email == email).FirstOrDefault();
        }

        public async Task<List<User>> GetList(CancellationToken cancellationToken)
        {
            return await applicationDbContext.Users.Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);
        }

        public async Task SendWelcomeMessage(string phone)
        {
            var model = new SMSModel() { PhoneNumber = phone, SmsContent = "Uzmanlık Eğitim Yönetim Sistemi'ne kaydınız yapılmıştır. ueys.saglik.gov.tr adresi üzerinden giriş yapabilirsiniz." };
            await SendMessage(model);
        }

        public async Task SendMessage(SMSModel model)
        {
            HttpClient client_auth = new();
            HttpClient client = new();

            var model_auth = new SMSSettings() { Username = appSettingsModel.SMS.Username, Password = appSettingsModel.SMS.Password };
            client_auth.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client_auth.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            string requestObject_auth = JsonConvert.SerializeObject(model_auth);
            var req_auth = new HttpRequestMessage(HttpMethod.Post, appSettingsModel.SMS.TokenUrl)
            {
                Content = new StringContent(
                requestObject_auth,
                Encoding.UTF8,
                "application/json"
            )
            };
            var response_auth = await client.SendAsync(req_auth);

            string output_auth = await response_auth.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            string requestObject = JsonConvert.SerializeObject(model);
            var req = new HttpRequestMessage(HttpMethod.Post, appSettingsModel.SMS.SMSUrl);
            var smsSettings = JsonConvert.DeserializeObject<SMSSettings>(output_auth);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", smsSettings.Token);
            req.Content = new StringContent(
                requestObject,
                Encoding.UTF8,
                "application/json"
            );
            await client.SendAsync(req);
        }

        public async Task<User> GetUserWithEducationalInfo(CancellationToken cancellationToken, string identityNo)
        {
            return await applicationDbContext.Users.AsSplitQuery()
                                .Include(x => x.Educators).ThenInclude(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                .Include(x => x.Educators).ThenInclude(x => x.EducatorPrograms).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                                .Include(x => x.Students).ThenInclude(x => x.StudentExpertiseBranches).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.PrincipalBranches)
                                .Include(x => x.Students).ThenInclude(x => x.OriginalProgram).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                                .Include(x => x.Students).ThenInclude(x => x.OriginalProgram).ThenInclude(x => x.ExpertiseBranch)
                                .Include(x => x.Students.Where(s => s.IsHardDeleted == false))
                                .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                                .FirstOrDefaultAsync(x => x.IdentityNo == identityNo, cancellationToken);
        }

        public async Task<User> GetUserByIdentity(CancellationToken cancellationToken, string identityNo)
        {
            return await applicationDbContext.Users.FirstOrDefaultAsync(x => x.IdentityNo == identityNo && x.IsDeleted == false, cancellationToken);
        }
        public async Task<User> GetByIdWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await applicationDbContext.Users.AsSplitQuery()
                  .Include(x => x.Institution)
                  .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleFaculties).ThenInclude(x => x.Faculty)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleFaculties).ThenInclude(x => x.ExpertiseBranch)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleHospitals).ThenInclude(x => x.Hospital)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRolePrograms).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRolePrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleProvinces).ThenInclude(x => x.Province)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleUniversities).ThenInclude(x => x.University)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<User> GetByIdentityNoWithSubRecords(CancellationToken cancellationToken, string identityNo)
        {
            return await applicationDbContext.Users.AsSplitQuery()
                  .Include(x => x.Institution)
                  .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleFaculties).ThenInclude(x => x.Faculty)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleFaculties).ThenInclude(x => x.ExpertiseBranch)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleHospitals).ThenInclude(x => x.Hospital)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRolePrograms).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRolePrograms).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleProvinces).ThenInclude(x => x.Province)
                  .Include(x => x.UserRoles).ThenInclude(x => x.UserRoleUniversities).ThenInclude(x => x.University)
                .FirstOrDefaultAsync(x => x.IdentityNo == identityNo, cancellationToken);
        }

        public bool IsExistingIdentity(CancellationToken cancellationToken, string identityNo)
        {
            return applicationDbContext.Users.Any(x => x.IdentityNo == identityNo && x.IsDeleted == false);
        }

        public List<ActivePassiveResponseModel> GetActivePassiveResponse()
        {
            int activeEducators = applicationDbContext.Educators?.Count(x => !x.IsDeleted && x.EducatorType == EducatorType.Instructor) ?? 0;
            int passiveEducators = applicationDbContext.Educators?.Count(x => x.IsDeleted && x.EducatorType == EducatorType.Instructor) ?? 0;

            int activeStudents = applicationDbContext.Students?.Count(x => !x.IsDeleted) ?? 0;
            int passiveStudents = applicationDbContext.Students?.Count(x => x.IsDeleted) ?? 0;

            return new List<ActivePassiveResponseModel>()
            {
                new ActivePassiveResponseModel()
                {
                        SeriesName = "Educators",
                        ActiveRecordsCount = activeEducators,
                        PassiveRecordsCount = passiveEducators,
                },
                new ActivePassiveResponseModel()
                {
                        SeriesName = "Students",
                        ActiveRecordsCount = activeStudents,
                    PassiveRecordsCount = passiveStudents,
                }
            };


        }
        public IQueryable<UserPaginateResponseDTO> PaginateQuery(ZoneModel zone)
        {

            IQueryable<User> users = applicationDbContext.Users.AsNoTracking();
            var predicates = PredicateBuilder.False<User>();

            if (zone.RoleCategory != RoleCategoryType.Admin && zone.RoleCategory != RoleCategoryType.Ministry)
            {
                AddPredicateForZoneEntities(zone.Provinces, x => x.UserRoles.Any(x => x.UserRoleProvinces.Any(a => zone.Provinces.Select(p => p.Id).Contains(a.ProvinceId.Value))));
                AddPredicateForZoneEntities(zone.Universities, x => x.UserRoles.Any(x => x.UserRoleUniversities.Any(a => zone.Universities.Select(u => u.Id).Contains(a.UniversityId.Value))));
                AddPredicateForZoneEntities(zone.Faculties, x => x.UserRoles.Any(x => x.UserRoleFaculties.Any(a => zone.Faculties.Select(f => f.Id).Contains(a.FacultyId.Value))));
                AddPredicateForZoneEntities(zone.Hospitals, x => x.UserRoles.Any(x => x.UserRoleHospitals.Any(a => zone.Hospitals.Select(h => h.Id).Contains(a.HospitalId.Value))));
                AddPredicateForZoneEntities(zone.Programs, x => x.UserRoles.Any(x => x.UserRolePrograms.Any(a => zone.Programs.Select(p => p.Id).Contains(a.ProgramId.Value))));
                if (zone.RoleCategory == RoleCategoryType.Registration)
                    predicates = predicates.Or(x => x.UserRoles.Any(x => x.Role.CategoryId == (int)RoleCategoryType.Registration));


                users = users.Where(predicates);
            }

            void AddPredicateForZoneEntities<T>(List<T> entities, Expression<Func<User, bool>> predicate)
            {
                if (entities != null && entities.Count != 0)
                    predicates = predicates.Or(predicate);
            }

            return users.Select(x =>
                    new UserPaginateResponseDTO()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        IdentityNo = x.IdentityNo,
                        IsDeleted = x.IsDeleted,
                        Name = x.Name,
                        Phone = x.Phone,
                        StudentZone = x.UserRoles.SelectMany(y => y.UserRoleStudents.Select(y => y.Student.OriginalProgram.Hospital.Province.Name + " " +
                                        y.Student.OriginalProgram.Hospital.Name + " " + y.Student.OriginalProgram.ExpertiseBranch.Name)).FirstOrDefault(),
                        Roles = x.UserRoles.Select(x => x.Role.RoleName).ToList(),
                        RoleCodes = x.UserRoles.Select(x => x.Role.Code).ToList(),
                        HospitalZones = x.UserRoles.SelectMany(y => y.UserRoleHospitals.Select(y => y.Hospital.Name)).Distinct().ToList(),
                        UniversityZones = x.UserRoles.SelectMany(y => y.UserRoleUniversities.Select(y => y.University.Name)).Distinct().ToList(),
                        ProgramZones = x.UserRoles.SelectMany(y => y.UserRolePrograms.Select(y => (y.Program.Hospital.Province.Name + " " +
                                        y.Program.Hospital.Name + " " + y.Program.ExpertiseBranch.Name))).Distinct().ToList(),
                        ProvinceZones = x.UserRoles.SelectMany(y => y.UserRoleProvinces.Select(y => y.Province.Name)).Distinct().ToList(),
                        FacultyZones = x.UserRoles.SelectMany(y => y.UserRoleFaculties.Select(y => y.Faculty.Name)).Distinct().ToList(),
                    });





            //if (zone.RoleCategory is RoleCategoryType.Admin or RoleCategoryType.Ministry)
            //{
            //    return from x in applicationDbContext.Users.AsNoTracking()
            //           let educatorProgram = x.Educators.FirstOrDefault(z => z.IsDeleted == false).EducatorPrograms.FirstOrDefault(y => y.ProgramId != null && (y.DutyEndDate == null || y.DutyEndDate <= DateTime.UtcNow)).Program

            //           select new UserPaginateResponseDTO()
            //           {
            //               Id = x.Id,
            //               Email = x.Email,
            //               IdentityNo = x.IdentityNo,
            //               IsDeleted = x.IsDeleted,
            //               Name = x.Name,
            //               Phone = x.Phone,
            //               StudentZone = x.UserRoles.SelectMany(y => y.UserRoleStudents.Select(y => (y.Student.OriginalProgram.Hospital.Province.Name + " " +
            //                               y.Student.OriginalProgram.Hospital.Name + " " + y.Student.OriginalProgram.ExpertiseBranch.Name))).FirstOrDefault(),
            //               EducatorZone = educatorProgram.Hospital.Province.Name + " " +
            //                              educatorProgram.Hospital.Name + " " +
            //                              educatorProgram.ExpertiseBranch.Name,
            //               Roles = x.UserRoles.Select(x => x.Role.RoleName).ToList(),
            //               RoleCodes = x.UserRoles.Select(x => x.Role.Code).ToList(),
            //               HospitalZones = x.UserRoles.SelectMany(y => y.UserRoleHospitals.Select(y => y.Hospital.Name)).Distinct().ToList(),
            //               UniversityZones = x.UserRoles.SelectMany(y => y.UserRoleUniversities.Select(y => y.University.Name)).Distinct().ToList(),
            //               ProgramZones = x.UserRoles.SelectMany(y => y.UserRolePrograms.Select(y => (y.Program.Hospital.Province.Name + " " +
            //                               y.Program.Hospital.Name + " " + y.Program.ExpertiseBranch.Name))).Distinct().ToList(),
            //               ProvinceZones = x.UserRoles.SelectMany(y => y.UserRoleProvinces.Select(y => y.Province.Name)).Distinct().ToList(),
            //               FacultyZones = x.UserRoles.SelectMany(y => y.UserRoleFaculties.Select(y => y.Faculty.Name)).Distinct().ToList(),
            //           };
            //}
            //else
            //{
            //    var roleIds = roles.Select(r => r.Id).ToList();
            //    var userRoles = applicationDbContext.Set<UserRole>()
            //                        .Where(x => x.UserId == currentUserId && roleIds.Any(roleId => roleId == x.RoleId));

            //    var subRoleIds = roles.SelectMany(role => applicationDbContext.Set<Role>()
            //                                .Where(x => (x.CategoryId == role.CategoryId && role.Level >= x.Level) || x.Code == RoleCodeConstants.EGITICI_CODE)
            //                                .Select(x => x.Id)).ToList();

            //    var provinceIds = userRoles.SelectMany(x => x.UserRoleProvinces).Select(x => x.ProvinceId).ToList();
            //    var universityIds = userRoles.SelectMany(x => x.UserRoleUniversities).Select(x => x.UniversityId).ToList();
            //    var facultyIds = userRoles.SelectMany(x => x.UserRoleFaculties).Select(x => x.FacultyId).ToList();
            //    var hospitalIds = userRoles.SelectMany(x => x.UserRoleHospitals).Select(x => x.HospitalId).ToList();
            //    var programIds = userRoles.SelectMany(x => x.UserRolePrograms).Select(x => x.ProgramId).ToList();

            //    return from x in applicationDbContext.Users.Where(x => x.IsDeleted == false)
            //           let student = x.Students.Where(x => !x.IsHardDeleted && !x.IsDeleted &&
            //                                                          x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration &&
            //                                                          x.Status != StudentStatus.EducationEnded &&
            //                                                          x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true).FirstOrDefault()
            //           let educatorPrograms = x.Educators.Where(x => x.EducatorType != EducatorType.NotInstructor && x.EducatorPrograms.Any(x => x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true)).FirstOrDefault().EducatorPrograms.FirstOrDefault(y => y.DutyEndDate == null && y.ProgramId != null)
            //           where x.UserRoles.Any(x => subRoleIds.Contains(x.RoleId.Value))
            //                   && (x.UserRoles.SelectMany(x => x.UserRoleProvinces).Any(x => provinceIds.Contains(x.ProvinceId))
            //                   || x.UserRoles.SelectMany(x => x.UserRoleUniversities).Any(x => universityIds.Contains(x.UniversityId))
            //                   || x.UserRoles.SelectMany(x => x.UserRoleFaculties).Any(x => facultyIds.Contains(x.FacultyId))
            //                   || x.UserRoles.SelectMany(x => x.UserRoleHospitals).Any(x => hospitalIds.Contains(x.HospitalId))
            //                   || x.UserRoles.SelectMany(x => x.UserRolePrograms).Any(x => programIds.Contains(x.ProgramId))
            //                   || x.Educators.SelectMany(x => x.EducatorPrograms).Any(a => hospitalIds.Contains(a.Program.HospitalId))
            //                   || x.Educators.SelectMany(x => x.EducatorPrograms).Any(a => provinceIds.Contains(a.Program.Hospital.ProvinceId))
            //                   || x.Educators.SelectMany(x => x.EducatorPrograms).Any(a => facultyIds.Contains(a.Program.FacultyId))
            //                   || x.Educators.SelectMany(x => x.EducatorPrograms).Any(a => programIds.Contains(a.ProgramId))
            //                   || x.Educators.SelectMany(x => x.EducatorPrograms).Any(a => facultyIds.Contains(a.Program.Faculty.UniversityId))
            //                   || provinceIds.Contains(student.Program.Hospital.ProvinceId)
            //                   || universityIds.Contains(student.Program.Hospital.Faculty.UniversityId)
            //                   || facultyIds.Contains(student.Program.Hospital.FacultyId)
            //                   || hospitalIds.Contains(student.Program.HospitalId)
            //                   || programIds.Contains(student.ProgramId))
            //           select new UserPaginateResponseDTO()
            //           {
            //               Id = x.Id,
            //               Email = x.Email,
            //               IdentityNo = x.IdentityNo,
            //               IsDeleted = x.IsDeleted,
            //               Name = x.Name,
            //               Phone = x.Phone,
            //               Roles = x.UserRoles.Select(x => x.Role.RoleName).ToList(),
            //               RoleCodes = x.UserRoles.Select(x => x.Role.Code).ToList(),
            //               StudentZone = student.Program != null
            //                                ? student.Program.Hospital.Name + " - " + student.Program.ExpertiseBranch.Name.ToLower()
            //                                : null,
            //               EducatorZone = educatorPrograms.Program != null
            //                                ? (educatorPrograms.Program.Hospital.Name + " - " + educatorPrograms.Program.ExpertiseBranch.Name.ToLower())
            //                                : null,
            //               HospitalZones = x.UserRoles.SelectMany(y => y.UserRoleHospitals.Select(y => y.Hospital.Name)).ToList(),
            //               UniversityZones = x.UserRoles.SelectMany(y => y.UserRoleUniversities.Select(y => y.University.Name)).ToList(),
            //               ProgramZones = x.UserRoles.SelectMany(y => y.UserRolePrograms.Select(y => y.Program.Hospital.Name + " " + y.Program.ExpertiseBranch.Name)).ToList(),
            //               ProvinceZones = x.UserRoles.SelectMany(y => y.UserRoleProvinces.Select(y => y.Province.Name)).ToList(),
            //               FacultyZones = x.UserRoles.SelectMany(y => y.UserRoleFaculties.Select(y => y.Faculty.Name)).ToList(),
            //           };
            //}
        }

        public async Task<UserRole> AddUserRole(UserRole userRole)
        {
            var response = await applicationDbContext.UserRoles.AddAsync(userRole);
            await applicationDbContext.SaveChangesAsync();
            return response.Entity;
        }
        public async Task<UserRoleProgram> AddUserRoleProgram(UserRoleProgram userRoleProgram)
        {
            var response = await applicationDbContext.UserRolePrograms.AddAsync(userRoleProgram);
            await applicationDbContext.SaveChangesAsync();
            return response.Entity;
        }


        public async Task<List<DetailedLogInformation>> UserLogInformation()
        {
            var firstReleaseDate = new DateTime(2024, 10, 8).Date;
            var yesterdayStart = DateTime.UtcNow.AddDays(-1).Date;
            var yesterdayEnd = DateTime.UtcNow.Date;

            var logSummary = await (from hospital in applicationDbContext.Hospitals
                                    where hospital.IsDeleted == false
                                    select new DetailedLogInformation
                                    {
                                        // Hospital bilgileri
                                        ProvinceName = hospital.Province.Name,
                                        ParentInstitutionName = hospital.Faculty.University.Institution.Name,
                                        UniversityName = hospital.Faculty.University.Name,
                                        FacultyName = hospital.Faculty.Name,
                                        HospitalName = hospital.Name,

                                        TotalUserCount = 0,
                                        TotalStudentCount = hospital.Programs.SelectMany(p => p.Students)
                                            .Where(x => !x.IsHardDeleted && !x.IsDeleted &&
                                                                      x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration &&
                                                                      x.Status != StudentStatus.EducationEnded &&
                                                                      x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true)
                                            .Count(),
                                        TotalEducatorCount = hospital.Programs.SelectMany(p => p.EducatorPrograms).Select(x => x.Educator)
                                            .Where(x => x.User.IsDeleted == false && x.EducatorType != EducatorType.NotInstructor && x.EducatorPrograms.Any(x => x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true)).Count(),

                                        // Öğrenci ve eğitici verilerini veritabanı tarafında gruplama
                                        TodaysLoggedInStudentCount = hospital.Programs.SelectMany(p => p.Students)
                                            .Where(x => !x.IsHardDeleted && !x.IsDeleted &&
                                                                      x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration &&
                                                                      x.Status != StudentStatus.EducationEnded &&
                                                                      x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true && x.User.LastLoginDate.Value.Date >= yesterdayStart && x.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count(),

                                        TotalLoggedInStudentCount = hospital.Programs.SelectMany(p => p.Students)
                                            .Where(x => !x.IsHardDeleted && !x.IsDeleted &&
                                                                      x.Status != StudentStatus.Gratuated && x.Status != StudentStatus.SentToRegistration &&
                                                                      x.Status != StudentStatus.EducationEnded &&
                                                                      x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true && x.User.LastLoginDate.Value.Date >= firstReleaseDate && x.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count(),

                                        TodaysLoggedInEducatorCount = hospital.Programs.SelectMany(p => p.EducatorPrograms).Select(x => x.Educator)
                                            .Where(x => x.User.IsDeleted == false && x.EducatorType != EducatorType.NotInstructor && x.EducatorPrograms.Any(x => x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true) &&
                                                    x.User.LastLoginDate.Value.Date >= yesterdayStart && x.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count(),

                                        TotalLoggedInEducatorCount = hospital.Programs.SelectMany(p => p.EducatorPrograms).Select(x => x.Educator)
                                            .Where(x => x.EducatorType != EducatorType.NotInstructor && x.EducatorPrograms.Any(x => x.Program.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0).ThenByDescending(x => x.AuthorizationDate).FirstOrDefault(x => !x.IsDeleted).AuthorizationCategory.IsActive == true) &&
                                                    x.User.LastLoginDate.Value.Date >= firstReleaseDate && x.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count(),

                                        // Diğer veriler aynı şekilde gruplama ve filtreleme işlemi ile hesaplanabilir
                                        TodaysLoggedInHeadCount = applicationDbContext.UserRoleHospitals
                                            .Where(urh => urh.UserRole.User.IsDeleted == false && urh.HospitalId == hospital.Id && urh.UserRole.RoleId == 39)
                                            .Where(urh => urh.UserRole.User.LastLoginDate.Value.Date >= yesterdayStart && urh.UserRole.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count(),

                                        TotalLoggedInHeadCount = applicationDbContext.UserRoleHospitals
                                            .Where(urh => urh.UserRole.User.IsDeleted == false && urh.HospitalId == hospital.Id && urh.UserRole.RoleId == 39)
                                            .Where(urh => urh.UserRole.User.LastLoginDate.Value.Date >= firstReleaseDate && urh.UserRole.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count(),

                                        // Agent Counts
                                        TodaysLoggedInAgentCount = applicationDbContext.UserRoleHospitals
                                            .Where(urh => urh.UserRole.User.IsDeleted == false && urh.HospitalId == hospital.Id && urh.UserRole.RoleId == 40)
                                            .Where(urh => urh.UserRole.User.LastLoginDate.Value.Date >= yesterdayStart && urh.UserRole.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count(),

                                        TotalLoggedInAgentCount = applicationDbContext.UserRoleHospitals
                                            .Where(urh => urh.UserRole.User.IsDeleted == false && urh.HospitalId == hospital.Id && urh.UserRole.RoleId == 40)
                                            .Where(urh => urh.UserRole.User.LastLoginDate.Value.Date >= firstReleaseDate && urh.UserRole.User.LastLoginDate.Value.Date < yesterdayEnd)
                                            .Count()
                                    }).OrderBy(x => x.UniversityName).ThenBy(x => x.FacultyName).ThenBy(x => x.HospitalName).ToListAsync();

            return logSummary;
        }
    }
}
