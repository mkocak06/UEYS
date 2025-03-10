using Application.Interfaces;
using Application.Reports.ExcelReports.EducatorReports;
using Application.Reports.ExcelReports.LogReports.DailyLog;
using Application.Services.Base;
using Core.Interfaces;
using Core.Models;
using Core.Models.ConfigModels;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Models.SMSModels;
using Shared.ResponseModels;
using Shared.Types;
using System.Text;

namespace Application.Services
{
    public class BackgroundJobService : BaseService, IBackgroundJobService
    {
        private readonly IEmailSender _emailSender;
        private readonly ISMSSender _smsSender;
        private readonly IMaterializedViewService _materializedViewService;
        private readonly EnvironmentSettings _environmentSettings;
        private readonly IKoruRepository _koruRepository;
        private readonly ICKYSService _CKYSService;


        public BackgroundJobService(IUnitOfWork unitOfWork, IEmailSender emailSender, IMaterializedViewService materializedViewService, EnvironmentSettings environmentSettings, IKoruRepository koruRepository, ICKYSService cKYSService, ISMSSender smsSender) : base(unitOfWork)
        {
            _emailSender = emailSender;
            _materializedViewService = materializedViewService;
            _environmentSettings = environmentSettings;
            _koruRepository = koruRepository;
            _CKYSService = cKYSService;
            _smsSender = smsSender;
        }

        public async Task CheckNotLoggedUsers()
        {
            var users = await unitOfWork.UserRepository.GetAsync(default,
                x => x.LastLoginDate.Value.Date < DateTime.UtcNow.AddMonths(-6).Date && !x.IsPassive);
            if (users != null && users.Any())
            {
                foreach (var item in users)
                {
                    item.IsPassive = true;
                }

                await unitOfWork.CommitAsync(default);
            }
        }

        public async Task UpdateSinaTable()
        {
            await unitOfWork.SinaRepository.UpdateSinaTable();
        }

        public async Task UpdateEducatorType()
        {
            var educators = await unitOfWork.EducatorRepository.GetAsync(default, x => x.EducatorType == EducatorType.NotInstructor && x.AcademicTitleId != null);

            if (educators != null && educators.Any())
            {
                foreach (var item in educators)
                {
                    item.EducatorType = EducatorType.Instructor;
                }

                await unitOfWork.CommitAsync(default);
            }
        }
        public async Task MakeUserPassive()
        {
            var users = await unitOfWork.StudentRepository.Queryable().Where(x => !x.User.IsDeleted && !x.IsDeleted && !x.IsHardDeleted && x.User.IsPassive == false && x.Program.HospitalId != 145 && x.OriginalProgram.HospitalId != 145).Select(x => x.User).ToListAsync(default);
            var users_1 = await unitOfWork.EducatorProgramRepository.Queryable().Where(x => !x.Educator.IsDeleted && !x.Educator.User.IsDeleted && x.Educator.User.IsPassive == false && x.Program.HospitalId != 145).Select(x => x.Educator.User).ToListAsync(default);

            users = [.. users, .. users_1];

            if (users.Count != 0)
            {
                foreach (var item in users)
                    item.IsPassive = true;

                await unitOfWork.CommitAsync(default);
            }
        }

        public async Task CheckExpiredStudents()
        {
            var expiredStudents = await unitOfWork.StudentRepository.GetAsync(default, x => !x.User.IsDeleted && !x.IsDeleted && !x.IsHardDeleted && x.Status == StudentStatus.EducationContinues && x.EducationTrackings.Any(y => y.ReasonType == ReasonType.EstimatedFinish && y.ProcessDate < DateTime.UtcNow));

            foreach (var item in expiredStudents)
                item.Status = StudentStatus.EstimatedFinishDatePast;

            await unitOfWork.CommitAsync(default);
        }

        public async Task SystemLogInformation()
        {
            if (_environmentSettings.Domain == "prod")
            {
                var logSummary = await unitOfWork.LogRepository.SystemLogInformation();
                var yesterday = DateTime.UtcNow.Date.AddDays(-1);

                var queryableUsers = unitOfWork.UserRepository.Queryable();
                var queryableEducators = unitOfWork.EducatorRepository.Queryable();
                var queryableStudents = unitOfWork.StudentRepository.Queryable();

                var studentQuery = queryableStudents.Where(x => !x.IsHardDeleted && !x.IsDeleted && !x.User.IsDeleted &&
                                                                                                              x.Status != StudentStatus.Gratuated &&
                                                                                                              x.Status != StudentStatus.SentToRegistration &&
                                                                                                              x.Status != StudentStatus.EducationEnded);

                var educatorQuery = queryableEducators.Where(x => x.User.IsDeleted == false && x.IsDeleted == false &&
                                                                x.EducatorType == EducatorType.Instructor);

                var headQuery = queryableUsers.SelectMany(x => x.UserRoles.SelectMany(x => x.UserRoleHospitals))
                                                .Where(urh => urh.UserRole.User.IsDeleted == false && urh.UserRole.RoleId == 39);

                var agentQuery = queryableUsers.SelectMany(x => x.UserRoles.SelectMany(x => x.UserRoleHospitals))
                                                .Where(urh => urh.UserRole.User.IsDeleted == false && urh.UserRole.RoleId == 40);

                var totalUserCount = queryableUsers.Count(x => !x.IsDeleted);
                var totalStudentCount = studentQuery.Count();
                var totalEducatorCount = educatorQuery.Count();

                var studentCount = studentQuery.Count(x => x.User.LastLoginDate.Value.Date == yesterday);
                var eduCount = educatorQuery.Count(x => x.User.LastLoginDate.Value.Date == yesterday);
                var headCount = headQuery.Count(urh => urh.UserRole.User.LastLoginDate.Value.Date == yesterday);
                var agentCount = agentQuery.Count(urh => urh.UserRole.User.LastLoginDate.Value.Date == yesterday);

                var emailBody = new StringBuilder();

                emailBody.Append("<b>Uzmanlık Eğitimi Yönetimi Sistemi (UEYS)</b> günlük kullanım raporunun özetine aşağıda ulaşabilirsiniz.<br>");
                emailBody.Append("Raporun il ve kurum bazlı detay bilgileri ise ekte yer almaktadır.<br><br>");

                emailBody.Append($"<b>Toplam Kullanıcı Sayısı</b>: {totalUserCount}<br>");
                emailBody.Append("<hr>");
                emailBody.Append($"&emsp;Toplam Uzmanlık Öğrencisi Sayısı: {totalStudentCount}<br>");
                emailBody.Append($"&emsp;Toplam Eğitici Sayısı: {totalEducatorCount}<br>");
                emailBody.Append($"&emsp;Diğer: {totalUserCount - totalEducatorCount - totalStudentCount}<br>");

                emailBody.Append($"<br><br><b>Bir Önceki Gün Giriş Yapan Toplam Kullanıcı Sayısı</b>: {studentCount + eduCount + headCount + agentCount}<br>");
                emailBody.Append("<hr>");
                emailBody.Append($"&emsp;Bir Önceki Gün Giriş Yapan Öğrenci Sayısı: {studentCount}<br>");
                emailBody.Append($"&emsp;Bir Önceki Gün Giriş Yapan Eğitici Sayısı: {eduCount}<br>");
                emailBody.Append($"&emsp;Bir Önceki Gün Giriş Yapan Kurum UEYS Sorumlusu Sayısı: {headCount}<br>");
                emailBody.Append($"&emsp;Bir Önceki Gün Giriş Yapan Kurum UEYS Temsilcisi Sayısı: {agentCount}<br>");

                emailBody.Append("<br><br><b>Kritik İşlemler Özeti:</b><br>");
                emailBody.Append("<hr>");
                emailBody.Append($"&emsp;Öğrenci Detay Servisi: {logSummary.Count(x => x.Contains("/api/Student/Get , QueryString"))}<br>");
                emailBody.Append($"&emsp;Eğitici Detay Servisi: {logSummary.Count(x => x.Contains("/api/Educator/Get/"))}<br>");


                var excelArray = DailyLog.DetailLogReport(await unitOfWork.UserRepository.UserLogInformation());

                using (MemoryStream memoryStream = new MemoryStream(excelArray))
                {
                    EmailMessage model = new()
                    {
                        To = { "alpay.tuncar@saglik.gov.tr", "mert.ozcan@saglik.gov.tr", },
                        CC = { "fatos.baygul@saglik.gov.tr", "ismail.topcam@saglik.gov.tr", "mehmet.kocak13@saglik.gov.tr", "elif.avci2@saglik.gov.tr", "ozge.kural@saglik.gov.tr", "ueys@saglik.gov.tr" },
                        Subject = "UEYS Günlük Kullanım Raporu",
                        Body = emailBody.ToString(),
                        AttachedFiles = new()
                        {
                            new()
                            {
                                AttachmentStream = memoryStream,
                                AttachmentTitle = "Log Bilgisi.xlsx",
                                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                            }
                        }
                    };

                    _emailSender.SendEmail(model);
                }
            }
        }

        public async Task UpdateMvTcknViewAsync()
        {
            await _materializedViewService.UpdateMvTcknViewAsync();
            await _materializedViewService.UpdateMvKurumViewAsync();
        }

        public async Task CheckSecondThesisDeadline()
        {
            var students = await unitOfWork.StudentRepository.GetAsync(default, x => x.EducationTrackings.Any(x => !x.IsDeleted && x.SecondThesisDeadline != null && x.SecondThesisDeadline > DateTime.UtcNow) && !x.IsDeleted && !x.IsHardDeleted && !x.User.IsDeleted && x.Status == StudentStatus.EducationContinues);

            foreach (var student in students)
                student.Status = StudentStatus.Dismissed;
            await unitOfWork.CommitAsync(default);
        }

        public async Task CheckEducatorProgramsEndDateSendWarning()
        {
            var educatorPrograms = await unitOfWork.EducatorProgramRepository.Queryable().Where(x => x.DutyEndDate <= DateTime.UtcNow.AddDays(15) && x.DutyEndDate >= DateTime.UtcNow).Select(x => new EducatorProgramResponseDTO()
            {
                Program = new()
                {
                    ManuelProgramName = x.Program.Hospital.Name + " - " + x.Program.ExpertiseBranch.Name,
                },
                Educator = new()
                {
                    User = new()
                    {
                        Phone = x.Educator.User.Phone,
                        Email = x.Educator.User.Email
                    }
                }
            }).ToListAsync();

            if (educatorPrograms.Count > 0)
            {
                foreach (var educatorProgram in educatorPrograms)
                {
                    EmailMessage eMailModel = new()
                    {
                        To = { educatorProgram.Educator.User.Email },
                        Subject = "Görev Yeri Bitiş Uyarısı",
                        Body = $"Görev yapmakta olduğunuz {educatorProgram.Program.ManuelProgramName} programında, görev süreninizin bitmesine 15 gün kalmıştır. Eğer görevinize aynı programda devam edecekseniz Kurum UEYS sorumlunuz ile iletişime geçiniz.",
                    };
                    _emailSender.SendEmail(eMailModel);

                    //BulkSMSModel sMSModel = new()
                    //{
                    //    PhoneNumbers = [educatorProgram.Educator.User.Phone],
                    //    SmsContent = $"{educatorProgram.Program.ManuelProgramName} programında, görev süreninizin bitmesine 15 gün kalmıştır. Kurum UEYS sorumlunuz ile iletişime geçiniz."
                    //};
                    //await _smsSender.SendBulkMessageAsync(sMSModel);
                }
            }
        }

        public async Task CheckSpecialistStudents()
        {
            var studentRole = await _koruRepository.GetRoleByCodeAsync(default, RoleCodeConstants.UZMANLIK_OGRENCISI_CODE);
            var students = unitOfWork.StudentRepository.Queryable().Where(x => (x.Status == StudentStatus.EstimatedFinishDatePast || x.Status == StudentStatus.Dismissed || x.Status == StudentStatus.Gratuated || x.Status == StudentStatus.SentToRegistration) && !x.IsDeleted && !x.User.IsDeleted && !x.IsHardDeleted).Select(x =>
                new StudentResponseDTO()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    User = new()
                    {
                        IdentityNo = x.User.IdentityNo,
                        Name = x.User.Name,
                        UserRoles = x.User.UserRoles.Where(x => x.RoleId == studentRole.Id).Select(y => new UserRoleResponseDTO
                        {
                            Id = y.Id,
                            RoleId = y.RoleId ?? 0,
                            UserId = y.UserId ?? 0
                        }).ToList()
                    },
                    OriginalProgram = new() { ExpertiseBranchId = x.OriginalProgram.ExpertiseBranchId, ExpertiseBranch = new() { Name = x.OriginalProgram.ExpertiseBranch.Name }, Hospital = new() { Name = x.OriginalProgram.Hospital.Name, Province = new() { Name = x.OriginalProgram.Hospital.Province.Name } } },
                }).ToList();

            var specialistStudents = new List<StudentResponseDTO>();

            if (students?.Count > 0)
            {
                foreach (var item in students)
                {
                    var response = await _CKYSService.CKYSDoktorSorgula(item.User.IdentityNo, true);
                    if (response.Result && response.Item?.DoctorExpertiseBranches?.Count > 0 && response.Item.DoctorExpertiseBranches?.Any(x => x.ExpertiseBranchId == item.OriginalProgram.ExpertiseBranchId) == true)
                    {
                        var student = await unitOfWork.StudentRepository.GetByIdAsync(default, item.Id);
                        student.Status = StudentStatus.EducationEnded;
                        student.ConditionallyGraduated = true;
                        unitOfWork.StudentRepository.Update(student);

                        await _koruRepository.RemoveRoleFromUser(default, item.UserId ?? 0, studentRole.Id);

                        var user = await unitOfWork.UserRepository.GetByIdAsync(default, item.UserId ?? 0);
                        user.IsDeleted = true;
                        user.DeleteDate = DateTime.UtcNow;
                        unitOfWork.UserRepository.Update(user);
                        specialistStudents.Add(item);
                    }
                }
                await unitOfWork.CommitAsync(default);
            }

            if (specialistStudents.Count > 0)
            {
                var byteArray = SampleReport.ExportListReport(specialistStudents);
                var emailBody = new StringBuilder();

                emailBody.Append("Eğitimini tamamladığı halde UEYS sisteminde aktif bulunan öğrencilerin durumu, Eğitimini Tamamlayan Öğrenci olarak güncellenmiştir. Bahse konu öğrenciler ekte bulunan listede mevcuttur.");
                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    EmailMessage model = new()
                    {
                        To = { "fatos.baygul@saglik.gov.tr", "ismail.topcam@saglik.gov.tr", "mehmet.kocak13@saglik.gov.tr", "elif.avci2@saglik.gov.tr", "ozge.kural@saglik.gov.tr", "leventsengun@gmail.com", "esin.gungor@saglik.gov.tr" },
                        CC = { "alpay.tuncar@saglik.gov.tr", "mert.ozcan@saglik.gov.tr" },
                        Subject = "Eğitimini Tamamlayan Öğrenciler",
                        Body = emailBody.ToString(),
                        AttachedFiles = new()
                        {
                            new()
                            {
                                AttachmentStream = memoryStream,
                                AttachmentTitle = "Eğitimini Tamamlayan Öğrenciler.xlsx",
                                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                            }
                        }
                    };
                    _emailSender.SendEmail(model);
                }
            }
        }

        public async Task CheckEducatorProgramsEndDate()
        {
            var educatorRole = await _koruRepository.GetRoleByCodeAsync(default, RoleCodeConstants.EGITICI_CODE);
            var educationOfficerRole = await _koruRepository.GetRoleByCodeAsync(default, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);

            var edus = await unitOfWork.EducatorRepository.Queryable().Where(x => !x.IsDeleted && !x.User.IsDeleted).Select(x => new EducatorResponseDTO
            {
                Id = x.Id,
                UserId = x.UserId,
                User = new()
                {
                    IdentityNo = x.User.IdentityNo,
                    Name = x.User.Name,
                    UserRoles = x.User.UserRoles.Select(y => new UserRoleResponseDTO
                    {
                        Id = y.Id,
                        RoleId = y.RoleId ?? 0,
                        UserRolePrograms = y.UserRolePrograms.Select(z => new UserRoleProgramResponseDTO
                        {
                            Id = z.Id,
                            ProgramId = z.ProgramId,
                            UserRoleId = z.UserRoleId
                        }).ToList(),
                        UserId = y.UserId ?? 0
                    }).ToList()
                },
                EducationOfficers = x.EducationOfficers.Select(x => new EducationOfficerResponseDTO
                {
                    Id = x.Id,
                    EducatorId = x.EducatorId,
                    ProgramId = x.ProgramId,
                    Program = new ProgramResponseDTO { Hospital = new HospitalResponseDTO { Name = x.Program.Hospital.Name }, ExpertiseBranch = new ExpertiseBranchResponseDTO { Name = x.Program.ExpertiseBranch.Name } },
                    EndDate = x.EndDate,
                }).ToList(),
                EducatorPrograms = x.EducatorPrograms.Select(y => new EducatorProgramResponseDTO
                {
                    Id = y.Id,
                    EducatorId = y.EducatorId,
                    ProgramId = y.ProgramId,
                    DutyStartDate = y.DutyStartDate,
                    DutyEndDate = y.DutyEndDate
                }).ToList()
            }).Where(x => x.User.UserRoles.Any(x => x.RoleId == educatorRole.Id)).ToListAsync();

            foreach (var educator in edus)
            {
                var activeProgramIds = educator.EducatorPrograms.Where(x => x.DutyEndDate == null || x.DutyEndDate > DateTime.UtcNow).Select(x => x.ProgramId).ToList();
                var activeEducationOfficer = educator.EducationOfficers.FirstOrDefault(x => x.EndDate == null);
                //aktif program bulunmuyorsa
                if (!activeProgramIds.Any())
                {
                    await _koruRepository.RemoveRoleFromUser(default, educator.UserId ?? 0, educatorRole.Id);
                    if (activeEducationOfficer != null)
                        await unitOfWork.EducationOfficerRepository.FinishEducationOfficersDuty(default, activeEducationOfficer.Id ?? 0);
                }
                //userRoleProgramının aktif programı yoksa
                else
                {
                    foreach (var userRoleProgram in educator.User.UserRoles.FirstOrDefault(x => x.RoleId == educatorRole.Id)?.UserRolePrograms)
                    {
                        if (activeProgramIds.Contains(userRoleProgram.ProgramId) == false)
                        {
                            await _koruRepository.RemoveUserRoleProgram(userRoleProgram.Id ?? 0);
                            if (activeEducationOfficer != null && activeEducationOfficer.ProgramId == userRoleProgram.ProgramId)
                                await unitOfWork.EducationOfficerRepository.FinishEducationOfficersDuty(default, activeEducationOfficer.Id ?? 0);
                        }
                    }
                }
            }
        }
    }
}