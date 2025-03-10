using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Entities.Koru;
using Core.Helpers;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Shared.Constants;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    internal class UserService : BaseService, IUserService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly IStringLocalizer<Shared.ApiResource> apiResource;
        private readonly IKPSService kPSService;
        private readonly ICKYSService cKYSService;
        private readonly IYOKService yOKService;
        private readonly IDocumentRepository documentRepository;
        private readonly IKoruRepository koruRepository;
        private readonly IEducationOfficerRepository educationOfficerRepository;
        private readonly IDependentProgramRepository dependentProgramRepository;
        private readonly IStudentDependentProgramRepository studentDependentProgramRepository;
        protected IHostEnvironment env { get; }

        public UserService(IMapper mapper, IUnitOfWork unitOfWork, IUserRepository userRepository, IStringLocalizer<Shared.ApiResource> apiResource, IKPSService kPSService, ICKYSService cKYSService, IYOKService yOKService, IDocumentRepository documentRepository, IKoruRepository koruRepository, IEducationOfficerRepository educationOfficerRepository, IDependentProgramRepository dependentProgramRepository, IStudentDependentProgramRepository studentDependentProgramRepository, IHostEnvironment env) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.apiResource = apiResource;
            this.kPSService = kPSService;
            this.cKYSService = cKYSService;
            this.yOKService = yOKService;
            this.documentRepository = documentRepository;
            this.koruRepository = koruRepository;
            this.educationOfficerRepository = educationOfficerRepository;
            this.dependentProgramRepository = dependentProgramRepository;
            this.studentDependentProgramRepository = studentDependentProgramRepository;
            this.env = env;
        }

        public async Task<ResponseWrapper<UserResponseDTO>> GetUserByIdentityNoWithEducationalInfo(CancellationToken cancellationToken, string identityNo)
        {
            User user = await userRepository.GetUserWithEducationalInfo(cancellationToken, identityNo);
            if (user == null)
            {
                var kpsResult = kPSService.GetKPSResultWithAddress(Convert.ToInt64(identityNo));
                if (kpsResult != null)
                {
                    UserResponseDTO userResponse = mapper.Map<UserResponseDTO>(kpsResult);

                    var ckysResponse = await cKYSService.CKYSDoktorSorgula(identityNo);
                    userResponse.CKYSDoctorResult = ckysResponse.Item;
                    userResponse.CKYSStudentResult = await cKYSService.CKYSOgrenciSorgulaAsync(identityNo);
                    userResponse.YOKResult = yOKService.GetAkademikIdariPersonelGorevAsync(Convert.ToInt64(identityNo));
                    userResponse.EgitimBilgisiResult = await yOKService.GetEgitimBilgisiAsync(Convert.ToInt64(identityNo));
                    return new() { Item = userResponse, Result = true };
                }
                else return new() { Result = false, Message = "Bu TCKN'ye sahip kimse bulunamadı." };
            }
            else
            {
                if (user.IsDeleted == false)//kullanıcı varsa ve silinmemişse
                {
                    var educatorProgram = user.Educators?.FirstOrDefault(x => x.IsDeleted == false && x.EducatorType != EducatorType.NotInstructor)?.EducatorPrograms.FirstOrDefault(x => x.DutyEndDate == null || x.DutyEndDate >= DateTime.UtcNow);

                    if (educatorProgram != null)
                        return new() { Result = false, Message = apiResource["ActiveEducator"].Value + " " + educatorProgram?.Program?.Hospital?.Province?.Name + " - " + educatorProgram?.Program?.Hospital?.Name + " - " + educatorProgram?.Program?.ExpertiseBranch?.Name };

                    var activeStudent = user.Students?.FirstOrDefault(x => x.IsDeleted == false && x.IsHardDeleted == false && x.Status != StudentStatus.EducationEnded);

                    if (activeStudent != null)
                        return new() { Result = false, Message = apiResource["ActiveStudent"].Value + " " + activeStudent?.OriginalProgram?.Hospital?.Province?.Name + " - " + activeStudent?.OriginalProgram?.Hospital?.Name + " - " + activeStudent?.OriginalProgram?.ExpertiseBranch?.Name };
                }

                UserResponseDTO userResponse = mapper.Map<UserResponseDTO>(user);
                userResponse.EducatorId = user.Educators?.FirstOrDefault(x => x.IsDeleted == false)?.Id;

                if (!env.IsDevelopment())
                {
                    // dış servislere gitme durumu tekrar değerlendirilmeli
                    var ckysResponse = await cKYSService.CKYSDoktorSorgula(identityNo);
                    userResponse.CKYSDoctorResult = ckysResponse.Item;
                    userResponse.CKYSStudentResult = await cKYSService.CKYSOgrenciSorgulaAsync(identityNo);
                    userResponse.YOKResult = yOKService.GetAkademikIdariPersonelGorevAsync(Convert.ToInt64(identityNo));
                    userResponse.EgitimBilgisiResult = await yOKService.GetEgitimBilgisiAsync(Convert.ToInt64(identityNo));
                }

                return new() { Result = true, Item = userResponse };
            }
        }
        public async Task<ResponseWrapper<UserResponseDTO>> GetUserByIdentityNoForThesis(CancellationToken cancellationToken, string identityNo, bool forAdvisor)
        {
            User user = await userRepository.GetUserWithEducationalInfo(cancellationToken, identityNo);
            if (user == null)
            {
                var kpsResult = kPSService.GetKPSResultWithAddress(Convert.ToInt64(identityNo));
                if (kpsResult != null)
                {
                    UserResponseDTO userResponse = mapper.Map<UserResponseDTO>(kpsResult);

                    return new() { Item = userResponse, Result = true };
                }
                else return new() { Result = false, Message = "Bu TCKN'ye sahip kimse bulunamadı." };
            }
            else
            {
                var type = forAdvisor ? "eğitici olmayan danışman" : "sistem dışı jüri";
                string message = string.Empty;
                if (user.Students.Any(x => !x.IsDeleted && !x.IsHardDeleted))
                    message = $"Bu kişi öğrenci olarak kayıtlıdır, {type} olarak eklenemez!";
                if (user.Educators.Any(x => !x.IsDeleted))
                    message = $"Bu kişi eğitici olarak kayıtlıdır,{type} olarak eklenemez!";
                if (!string.IsNullOrEmpty(message))
                    return new() { Result = false, Message = message };

                return new() { Result = true, Item = mapper.Map<UserResponseDTO>(user) };

            }
        }
        public async Task<ResponseWrapper<UserResponseDTO>> PostWithEdu(CancellationToken cancellationToken, UserDTO userDTO)
        {
            if (userDTO.LastLoginDate == null || userDTO.LastLoginDate == DateTime.MinValue)
                userDTO.LastLoginDate = DateTime.UtcNow;
            var roleToAdd = await koruRepository.GetRoleByCodeAsync(cancellationToken, userDTO.Educator.EducatorType == EducatorType.Instructor ? RoleCodeConstants.EGITICI_CODE : userDTO.Educator.NonInstructorType == NonInstructorType.ThesisAdvisor ? RoleCodeConstants.TEZ_DANISMANI : userDTO.Educator.NonInstructorType == NonInstructorType.ThesisDefenceJury ? RoleCodeConstants.TEZ_SAVUNMASI_JURISI : RoleCodeConstants.BITIRME_SINAVI_JURISI);

            if (userDTO.Id == 0)
            {
                await unitOfWork.BeginTransactionAsync();
                var checkEmailUser = await userRepository.FirstOrDefaultAsync(cancellationToken, x => x.Email.ToLower() == userDTO.Email.ToLower());
                if (checkEmailUser != null)
                    return new() { Result = false, Message = apiResource["ErrorEmailTaken"].Value };
                var checkIdentityUser = await userRepository.FirstOrDefaultAsync(cancellationToken, x => x.IdentityNo == userDTO.IdentityNo);
                if (checkIdentityUser != null)
                    throw new Exception(apiResource["ErrorIdentityExist"].Value);

                var eduOfficers = userDTO.Educator?.EducationOfficers?.Where(x => x.ProgramId != null && x.ProgramId > 0)?.ToList();

                if (eduOfficers?.Count > 0)
                    foreach (var item in eduOfficers)
                        await educationOfficerRepository.FinishCurrentDuty(cancellationToken, item.ProgramId ?? 0);

                User user = mapper.Map<User>(userDTO);
                user.Educators = new List<Educator>() { mapper.Map<Educator>(userDTO.Educator) };//todo: mapping ile halledebilir miyiz

                if (roleToAdd != null && roleToAdd.Id > 0)
                {
                    user.UserRoles = new List<UserRole>() { new UserRole(user.Id, roleToAdd.Id) };
                    user.SelectedRoleId = roleToAdd.Id;
                }

                if (eduOfficers?.Count > 0)
                {
                    var educationOfficerRole = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);
                    if (educationOfficerRole != null && educationOfficerRole.Id > 0)
                        user.UserRoles.Add(new UserRole(user.Id, educationOfficerRole.Id));
                }

                User addedUser = await userRepository.AddAsync(cancellationToken, user);
                await unitOfWork.CommitAsync(cancellationToken);

                var educatorDTO = userDTO.Educator;
                if (educatorDTO != null && educatorDTO.EducatorType != EducatorType.NotInstructor)
                {
                    foreach (var item in educatorDTO.EducatorPrograms)
                        foreach (var doc in item.Documents)
                        {
                            doc.EntityId = addedUser?.Educators?.FirstOrDefault(x => x.IsDeleted == false)?.EducatorPrograms?.FirstOrDefault(x => x.DocumentOrder == item.DocumentOrder)?.Id ?? 0;
                            unitOfWork.DocumentRepository.Update(mapper.Map<Document>(doc));
                        }
                    foreach (var item in educatorDTO.Documents)
                    {
                        item.EntityId = addedUser?.Educators?.FirstOrDefault(x => x.IsDeleted == false)?.Id ?? 0;
                        unitOfWork.DocumentRepository.Update(mapper.Map<Document>(item));
                    }
                    foreach (var item in educatorDTO.EducationOfficers)
                        foreach (var doc in item.Documents)
                        {
                            doc.EntityId = addedUser?.Educators?.FirstOrDefault(x => x.IsDeleted == false)?.EducationOfficers?.FirstOrDefault(x => x.DocumentOrder == item.DocumentOrder)?.Id ?? 0;
                            unitOfWork.DocumentRepository.Update(mapper.Map<Document>(doc));
                        }
                }
                await unitOfWork.EndTransactionAsync(cancellationToken);

                var addedUser1 = await userRepository.GetByIdentityNoWithSubRecords(cancellationToken, user.IdentityNo);

                if (userDTO.Educator.EducatorType != EducatorType.NotInstructor)
                {
                    await userRepository.SendWelcomeMessage(userDTO.Phone);
                    var userRole = addedUser1.UserRoles.FirstOrDefault(x => x.Role.Code == RoleCodeConstants.EGITICI_CODE);
                    var officerUserRole = addedUser1.UserRoles.FirstOrDefault(x => x.Role.Code == RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);

                    userRole.UserRolePrograms ??= new List<UserRoleProgram>();
                    foreach (var item in addedUser.Educators.First().EducatorPrograms)
                        userRole.UserRolePrograms.Add(new() { ProgramId = item.ProgramId });
                    if (officerUserRole != null)
                    {
                        officerUserRole.UserRolePrograms ??= new List<UserRoleProgram>();
                        foreach (var item in addedUser.Educators.First().EducationOfficers)
                            officerUserRole.UserRolePrograms.Add(new() { ProgramId = item.ProgramId });
                    }
                    userRepository.Update(user);
                }

                await unitOfWork.CommitAsync(cancellationToken);
                UserResponseDTO userResponse = mapper.Map<UserResponseDTO>(addedUser);
                userResponse.Educator = mapper.Map<EducatorResponseDTO>(addedUser.Educators.FirstOrDefault());

                return new ResponseWrapper<UserResponseDTO> { Result = true, Item = userResponse };
            }
            else
            {
                if (userDTO.Educator != null)
                {
                    await unitOfWork.BeginTransactionAsync();

                    var eduOfficers = userDTO.Educator?.EducationOfficers?.Where(x => x.ProgramId != null && x.ProgramId > 0)?.ToList();

                    if (eduOfficers?.Count > 0)
                        foreach (var item in eduOfficers)
                            await educationOfficerRepository.FinishCurrentDuty(cancellationToken, item.ProgramId.Value);

                    var educator = mapper.Map<Educator>(userDTO.Educator);
                    educator.UserId = userDTO.Id;

                    var addedEducator = await unitOfWork.EducatorRepository.AddAsync(cancellationToken, educator);

                    if (roleToAdd != null)
                    {
                        koruRepository.AddRoleToUser(roleToAdd, userDTO.Id);

                        User userUpdate = await userRepository.GetByIdAsync(cancellationToken, userDTO.Id);
                        userUpdate.SelectedRoleId = roleToAdd.Id;
                        userRepository.Update(userUpdate);
                    }

                    if (eduOfficers?.Count > 0)
                    {
                        var educationOfficerRole = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.BIRIM_EGITIM_SORUMLUSU_CODE);
                        koruRepository.AddRoleToUser(educationOfficerRole, userDTO.Id);

                        User userUpdate = await userRepository.GetByIdAsync(cancellationToken, userDTO.Id);
                        userUpdate.SelectedRoleId = educationOfficerRole.Id;
                        userRepository.Update(userUpdate);
                    }
                    await unitOfWork.CommitAsync(cancellationToken);

                    if (addedEducator != null && addedEducator.EducatorPrograms?.Count > 0)
                    {
                        foreach (var item in userDTO.Educator.EducatorPrograms)
                        {
                            foreach (var doc in item.Documents)
                            {
                                doc.EntityId = addedEducator?.EducatorPrograms?.FirstOrDefault(x => x.DocumentOrder == item.DocumentOrder)?.Id ?? 0;
                                unitOfWork.DocumentRepository.Update(mapper.Map<Document>(doc));
                            }
                        }
                        foreach (var item in userDTO.Educator.Documents)
                        {
                            item.EntityId = addedEducator.Id;
                            unitOfWork.DocumentRepository.Update(mapper.Map<Document>(item));
                        }
                        foreach (var item in userDTO.Educator.EducationOfficers)
                        {
                            foreach (var doc in item.Documents)
                            {
                                doc.EntityId = addedEducator?.EducationOfficers?.FirstOrDefault(x => x.DocumentOrder == item.DocumentOrder)?.Id ?? 0;
                                unitOfWork.DocumentRepository.Update(mapper.Map<Document>(doc));
                            }
                        }
                    }
                    await unitOfWork.EndTransactionAsync(cancellationToken);
                    User user = await userRepository.GetUserWithEducationalInfo(cancellationToken, userDTO.IdentityNo);

                    var userRole = user.UserRoles.First(x => x.Role.Code == RoleCodeConstants.EGITICI_CODE);
                    userRole.UserRolePrograms ??= new List<UserRoleProgram>();
                    if (user.IsDeleted)
                        user.IsDeleted = false;
                    foreach (var item in user.Educators.First().EducatorPrograms)
                        userRole.UserRolePrograms.Add(new() { ProgramId = item.ProgramId });
                    userRepository.Update(user);

                    UserResponseDTO userResponse = mapper.Map<UserResponseDTO>(user);
                    userResponse.Educator = mapper.Map<EducatorResponseDTO>(addedEducator);

                    return new ResponseWrapper<UserResponseDTO> { Result = true, Item = userResponse };
                }
                else return new ResponseWrapper<UserResponseDTO> { Result = false, Message = "Hiçbir eğitici bulunamadı" };
            }
        }

        public async Task<ResponseWrapper<UserResponseDTO>> PostWithStu(CancellationToken cancellationToken, UserDTO userDTO)
        {
            if (userDTO.LastLoginDate == null || userDTO.LastLoginDate == DateTime.MinValue)
                userDTO.LastLoginDate = DateTime.UtcNow;

            if (userDTO.Id == 0)
            {
                await unitOfWork.BeginTransactionAsync();
                var checkEmailUser = await userRepository.FirstOrDefaultAsync(cancellationToken, x => x.Email.ToLower() == userDTO.Email.ToLower());
                if (checkEmailUser != null)
                    return new() { Result = false, Message = apiResource["ErrorEmailTaken"].Value };
                var checkIdentityUser = await userRepository.FirstOrDefaultAsync(cancellationToken, x => x.IdentityNo == userDTO.IdentityNo);
                if (checkIdentityUser != null)
                    throw new Exception(apiResource["ErrorIdentityExist"].Value);

                User user = mapper.Map<User>(userDTO);
                user.Students = new List<Student>() { mapper.Map<Student>(userDTO.Student) };

                if (userDTO.Student != null)
                {
                    if (user.Students?.FirstOrDefault()?.CurriculumId != null && user.Students?.FirstOrDefault()?.CurriculumId > 0)
                    {
                        var studentsCurriculum = await unitOfWork.CurriculumRepository.GetByIdAsync(cancellationToken, (long)user.Students?.FirstOrDefault()?.CurriculumId);
                        if (user.Students?.FirstOrDefault()?.EducationTrackings != null)
                        {
                            int diff = 0;
                            if (userDTO.Student.IsTransferred == true)
                                diff = (int)(user.Students?.FirstOrDefault()?.EducationTrackings?.FirstOrDefault(x => x.ReasonType != ReasonType.BeginningSpecializationEducation && x.ProcessType == ProcessType.Start)?.ProcessDate - userDTO.Student.FirstInsLeavingDate)?.Days;

                            user.Students?.FirstOrDefault()?.EducationTrackings.Add(new() { ProcessType = ProcessType.EstimatedFinish, ReasonType = ReasonType.EstimatedFinish, ProcessDate = user.Students?.FirstOrDefault()?.EducationTrackings?.FirstOrDefault(x => x.ReasonType == ReasonType.BeginningSpecializationEducation || x.ReasonType == ReasonType.RestartingWithAmnestyLaw || x.ReasonType == ReasonType.StartingOverwithExamfortheSameExpertiseBranch || x.ReasonType == ReasonType.RestartByJudgment)?.ProcessDate?.AddDays(diff).AddYears(studentsCurriculum.Duration.Value) });
                        }
                    }
                }
                var role = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.UZMANLIK_OGRENCISI_CODE);
                if (role != null && role.Id > 0)
                {
                    user.UserRoles = new List<UserRole>() { new UserRole(user.Id, role.Id) };
                    user.SelectedRoleId = role.Id;
                }

                await userRepository.AddAsync(cancellationToken, user);
                await unitOfWork.CommitAsync(cancellationToken);

                await userRepository.SendWelcomeMessage(userDTO.Phone);

                var addedUser = await userRepository.GetIncluding(cancellationToken, x => x.IdentityNo == user.IdentityNo, x => x.UserRoles);

                var dependentPrograms = await dependentProgramRepository.GetAsync(cancellationToken, x => x.RelatedDependentProgram.IsActive == true && x.RelatedDependentProgram.ProtocolProgram.CancelingProtocolNo == null && x.RelatedDependentProgram.ProtocolProgram.ParentProgramId == user.Students.FirstOrDefault().OriginalProgramId);

                //program protokollü ise bağlı programları eklenir
                if (dependentPrograms != null && dependentPrograms.Count > 0)
                    foreach (var item in dependentPrograms)
                        await studentDependentProgramRepository.AddAsync(cancellationToken, new() { DependentProgramId = item.Id, StudentId = addedUser.Students.FirstOrDefault().Id });

                var userRole = addedUser.UserRoles.First(x => x.RoleId == role?.Id);
                userRole.UserRoleStudents ??= new List<UserRoleStudent>();
                userRole.UserRoleStudents.Add(new() { StudentId = addedUser.Students.First(x => x.IsDeleted == false && x.IsHardDeleted == false).Id });
                userRepository.Update(user);

                await unitOfWork.EndTransactionAsync(cancellationToken);

                UserResponseDTO userResponse = mapper.Map<UserResponseDTO>(addedUser);
                userResponse.Student = mapper.Map<StudentResponseDTO>(addedUser.Students.FirstOrDefault());

                return new ResponseWrapper<UserResponseDTO> { Result = true, Item = userResponse };
            }
            else
            {
                if (userDTO.Student != null)
                {
                    var student = mapper.Map<Student>(userDTO.Student);
                    student.UserId = userDTO.Id;

                    if (await unitOfWork.StudentRepository.AnyAsync(cancellationToken, x => x.UserId == student.UserId && !x.IsDeleted && !x.IsHardDeleted && x.Status != StudentStatus.EducationEnded && !x.User.IsDeleted))
                        return new ResponseWrapper<UserResponseDTO> { Result = false, Message = "Student already exists" };
                    else
                    {
                        await unitOfWork.BeginTransactionAsync();
                        if (student.CurriculumId != null)
                        {
                            var studentsCurriculum = await unitOfWork.CurriculumRepository.GetByIdAsync(cancellationToken, student.CurriculumId.Value);
                            if (student.EducationTrackings != null && studentsCurriculum?.Duration != null)
                            {
                                student.EducationTrackings.Add(new() { ProcessType = ProcessType.EstimatedFinish, ReasonType = ReasonType.EstimatedFinish, ProcessDate = student.EducationTrackings.FirstOrDefault()?.ProcessDate?.AddYears(studentsCurriculum.Duration.Value) });
                            }
                        }
                        else
                        {
                            return new() { Result = false, Message = "Müfredat seçerken hata oluştu." };
                        }
                        var addedStudent = await unitOfWork.StudentRepository.AddAsync(cancellationToken, student);

                        var role = await koruRepository.GetRoleByCodeAsync(cancellationToken, RoleCodeConstants.UZMANLIK_OGRENCISI_CODE);
                        if (role != null)
                        {
                            koruRepository.AddRoleToUser(role, userDTO.Id);

                            User userUpdate = await userRepository.GetByIdAsync(cancellationToken, userDTO.Id);
                            userUpdate.SelectedRoleId = role.Id;
                            userRepository.Update(userUpdate);
                        }
                        await unitOfWork.CommitAsync(cancellationToken);

                        var dependentPrograms = await dependentProgramRepository.GetAsync(cancellationToken, x => x.RelatedDependentProgram.IsActive == true && x.RelatedDependentProgram.ProtocolProgram.CancelingProtocolNo == null && x.RelatedDependentProgram.ProtocolProgram.ParentProgramId == student.OriginalProgramId);

                        if (dependentPrograms != null && dependentPrograms.Count > 0)
                            foreach (var item in dependentPrograms)
                                await studentDependentProgramRepository.AddAsync(cancellationToken, new() { DependentProgramId = item.Id, StudentId = addedStudent.Id });

                        await unitOfWork.EndTransactionAsync(cancellationToken);
                        User user = await userRepository.GetUserWithEducationalInfo(cancellationToken, userDTO.IdentityNo);

                        var userRole = user.UserRoles.First(x => x.RoleId == role?.Id);
                        userRole.UserRoleStudents ??= new List<UserRoleStudent>();
                        userRole.UserRoleStudents.Add(new() { StudentId = user.Students.First(x => x.IsDeleted == false && x.IsHardDeleted == false).Id });
                        if (user.IsDeleted)
                            user.IsDeleted = false;
                        userRepository.Update(user);
                        await unitOfWork.CommitAsync(cancellationToken);

                        UserResponseDTO userResponse = mapper.Map<UserResponseDTO>(user);
                        userResponse.Student = mapper.Map<StudentResponseDTO>(addedStudent);

                        return new ResponseWrapper<UserResponseDTO> { Result = true, Item = userResponse };
                    }
                }
                else
                    return new ResponseWrapper<UserResponseDTO> { Result = false, Message = "Hiçbir öğrenci bulunamadı" };
            }
        }

        public async Task<bool> UpdateActivePassiveUser(CancellationToken cancellationToken, long userId)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, userId);

            user.IsDeleted = !user.IsDeleted;

            userRepository.Update(user);
            await unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetUserByIdentityNoAsync(CancellationToken cancellationToken, string identityNo)
        {
            User user = await userRepository.GetByAsync(cancellationToken, x => x.IdentityNo == identityNo);
            if (user == null)
            {
                try
                {
                    var kpsResult = kPSService.GetKPSResultWithAddress(Convert.ToInt64(identityNo));
                    if (kpsResult == null)
                        return new() { Result = false, Message = "Bu T.C. kimlik bilgisine sahip kimse bulunamadı" };
                    UserAccountDetailInfoResponseDTO responseDTO = new() { KPSResult = mapper.Map<KPSResultResponseDTO>(kpsResult) };
                    return new() { Result = true, Item = responseDTO };
                }
                catch (Exception)
                {
                    throw new Exception("Kimlik bilgilerine erişim sağlanamadı");
                }
            }
            else if (user.IsDeleted == true)
                return new() { Result = false, Item = mapper.Map<UserAccountDetailInfoResponseDTO>(user) };
            else
                return new() { Result = false, Message = "User with this identity number already exists" };
        }

        public async Task<ResponseWrapper<bool>> IsExistingUser(CancellationToken cancellationToken, long id)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);

            var result = userRepository.IsExistingIdentity(cancellationToken, user.IdentityNo);

            return new() { Item = result, Result = true };
        }

        public ResponseWrapper<List<ActivePassiveResponseModel>> GetActivePassiveList()
        {
            var result = userRepository.GetActivePassiveResponse();
            return new() { Item = result, Result = true };
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetById(CancellationToken cancellationToken, long id)
        {
            var user = await userRepository.GetByIdWithSubRecords(cancellationToken, id);
            var docs = await documentRepository.GetByEntityId(cancellationToken, id, DocumentTypes.EPKInstitutionalEducationOfficerAppointmentDecision);

            //var roles = await koruRepository.GetRolesByUserIdAsync(cancellationToken, user.Id);

            var response = mapper.Map<UserAccountDetailInfoResponseDTO>(user);

            response.Documents = mapper.Map<List<DocumentResponseDTO>>(docs);

            //response.Roles = mapper.Map<List<RoleResponseDTO>>(roles);
            //response.RoleIds = roles.Select(x=>x.Id).ToList();

            response.IdentityNo = !string.IsNullOrWhiteSpace(user.IdentityNo) ? StringHelpers.MaskIdentityNumber(user.IdentityNo) : "10*******70";

            return new() { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<KPSResultResponseDTO>> GetUserInfoById(CancellationToken cancellationToken, long id)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);

            var response = mapper.Map<KPSResultResponseDTO>(kPSService.GetKPSResultWithAddress(Convert.ToInt64(user.IdentityNo)));

            return new() { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<AcademicAdminStaffResponseDTO>> GetAcademicInfoById(CancellationToken cancellationToken, long id)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);

            var response = yOKService.GetAkademikIdariPersonelGorevAsync(Convert.ToInt64(user.IdentityNo));

            return new() { Item = response, Result = true };
        }

        public async Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoById(CancellationToken cancellationToken, long id)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);

            var response = await cKYSService.CKYSDoktorSorgula(user.IdentityNo);

            if (response.Result)
                return new() { Item = response.Item, Result = true };
            else
                return new() { Result = false, Message = response.Message };
        }

        public async Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoByIdentityNo(string identityNo)
        {
            var response = await cKYSService.CKYSDoktorSorgula(identityNo);

            if (response.Result)
                return new() { Item = response.Item, Result = true };
            else
                return new() { Result = false, Message = response.Message };
        }

        public async Task<ResponseWrapper<List<GraduationDetailResponseDTO>>> GetGraduationInfoById(CancellationToken cancellationToken, long id)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);

            var response = await yOKService.GetEgitimBilgisiAsync(Convert.ToInt64(user.IdentityNo));

            return new() { Item = response, Result = true };
        }
        public async Task<bool> CheckEmailExist(CancellationToken cancellationToken, string email)
        {
            if (await userRepository.AnyAsync(cancellationToken, x => x.Email == email))
                return false;
            else
                return true;
        }

        public async Task<ResponseWrapper<UserResponseDTO>> ChangeIdentityNoAndName(CancellationToken cancellationToken, string identityNo, long id)
        {
            var result = await userRepository.AnyAsync(cancellationToken, x => x.IdentityNo == identityNo);
            if (result)
                return new() { Result = false, Message = "Girdiğiniz kimlik numarasına sahip kişi bulunmaktadır" };

            var kpsResult = kPSService.GetKPSResult(Convert.ToInt64(identityNo));
            if (kpsResult != null)
            {
                var user = await userRepository.GetByIdAsync(cancellationToken, id);

                user.OldIdentityNo = user.IdentityNo;
                user.OldName = user.Name;
                user.IdentityNo = kpsResult.TCKN.ToString();
                user.Name = kpsResult.Name + " " + kpsResult.Surname;

                userRepository.Update(user);
                await unitOfWork.CommitAsync(cancellationToken);
                return new() { Result = true };
            }
            else
                return new() { Result = false, Message = "Girdiğiniz kimlik numarasına sahip kişi bulunmamaktadır!" };
        }


        public async Task<ResponseWrapper<UserResponseDTO>> GetUserForChangeIdentityNoAndName(CancellationToken cancellationToken, string identityNo)
        {
            var result = await userRepository.AnyAsync(cancellationToken, x => x.IdentityNo == identityNo);

            if (result)
                return new() { Result = false, Message = "Bu kimlik numarasına sahip kişi bulunmaktadır" };

            var kpsResult = kPSService.GetKPSResult(Convert.ToInt64(identityNo));

            if (kpsResult != null)
                return new() { Result = true, Item = new() { IdentityNo = identityNo, Name = kpsResult.Name + " " + kpsResult.Surname } };
            else
                return new() { Result = false, Message = "Girdiğiniz kimlik numarasına sahip kişi bulunmamaktadır!" };
        }

    }
}
