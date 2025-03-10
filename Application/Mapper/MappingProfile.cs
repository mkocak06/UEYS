using AutoMapper;
using Core.EkipModels;
using Core.Entities;
using Core.Entities.Koru;
using Core.KDSModels;
using Core.Models;
using Core.Models.Authorization;
using Core.Models.Educator;
using Newtonsoft.Json;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Authorization;
using Shared.ResponseModels.Ekip;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.ResponseModels.Menu;
using Shared.ResponseModels.Mobile;
using Shared.ResponseModels.Program;

namespace Application.Mapper
{

    public class EducatorListToEducatorResponseDTOConverter :
            ITypeConverter<List<Educator>, EducatorResponseDTO>
    {
        public EducatorResponseDTO Convert
          (List<Educator> source, EducatorResponseDTO destination, ResolutionContext context) =>
            context.Mapper.Map<EducatorResponseDTO>(source.FirstOrDefault());
    }

    public class StudentListToStudentResponseDTOConverter :
            ITypeConverter<List<Student>, StudentResponseDTO>
    {
        public StudentResponseDTO Convert
          (List<Student> source, StudentResponseDTO destination, ResolutionContext context) =>
            context.Mapper.Map<StudentResponseDTO>(source.FirstOrDefault());
    }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<KPSResult, UserResponseDTO>()
                .ForMember(x => x.IdentityNo,
                options => options.MapFrom(t => t.TCKN.ToString()))
                .ForMember(x => x.Name,
                options => options.MapFrom(t => t.Name + " " + t.Surname))
            .ForMember(x => x.Address,
                options => options.MapFrom(t => t.AddressInfo.Address));

            CreateMap<List<Educator>, EducatorResponseDTO>()
                .ConvertUsing<EducatorListToEducatorResponseDTOConverter>();
            CreateMap<List<Student>, StudentResponseDTO>()
                .ConvertUsing<StudentListToStudentResponseDTOConverter>();


            CreateMap<UserDTO, User>();
            CreateMap<User, UserResponseDTO>();
            CreateMap<User, UserForLoginDTO>();
            CreateMap<User, UserForRegisterDTO>();
            CreateMap<User, MobileUserResponseDTO>();
            CreateMap<UserForRegisterDTO, User>();
            CreateMap<UpdateUserAccountInfoDTO, UserForRegisterDTO>();
            CreateMap<UserForRegisterDTO, UpdateUserAccountInfoDTO>();
            CreateMap<UserForRegisterDTO, UserAccountDetailInfoResponseDTO>();
            CreateMap<UserAccountDetailInfoResponseDTO, UserForRegisterDTO>();
            CreateMap<UpdateUserAccountInfoDTO, UserAccountDetailInfoResponseDTO>();
            CreateMap<UserAccountDetailInfoResponseDTO, UpdateUserAccountInfoDTO>();
            CreateMap<User, UserAccountDetailInfoResponseDTO>();
            CreateMap<AddUserWithEducatorInfoDTO, User>();
            CreateMap<User, UserWithEducatorInfoResponseDTO>();
            CreateMap<AddUserWithStudentInfoDTO, User>();
            CreateMap<User, UserWithStudentInfoResponseDTO>();
            CreateMap<UpdateUserAccountInfoDTO, User>();
            CreateMap<User, UpdateUserAccountInfoResponseDTO>();
            CreateMap<User, UserForLoginResponseDTO>().ForMember(dto => dto.Id, options => options.MapFrom(x => x.Id));
            CreateMap<User, UserAccountResponseDTO>();
            CreateMap<Permission, PermissionDTO>();
            CreateMap<Role, RoleResponseDTO>()
                .ForMember(dto => dto.Menus,
                    options => options.MapFrom(r => r.RoleMenus.Select(rp => rp.Menu)))
                .ForMember(dto => dto.Permissions,
                    options => options.MapFrom(r => r.RolePermissions.Select(rp => rp.Permission)));
            CreateMap<RolePermission, RolePermissionResponseDTO>();
            CreateMap<Permission, PermissionResponseDTO>();
            CreateMap<Role, RoleDTO>()
                .ForMember(dto => dto.Menus, options => options.MapFrom(r => r.RoleMenus.Select(rp => rp.Menu)))
                .ForMember(dto => dto.Permissions, options => options.MapFrom(r => r.RolePermissions.Select(rp => rp.Permission)));
            CreateMap<UniversityDTO, University>();
            CreateMap<University, UniversityResponseDTO>();
            CreateMap<ProfessionDTO, Profession>();
            CreateMap<FacultyDTO, Faculty>();
            CreateMap<Faculty, FacultyResponseDTO>();
            CreateMap<ThesisDTO, Thesis>();
            CreateMap<Thesis, ThesisResponseDTO>();
            CreateMap<Profession, ProfessionResponseDTO>();
            CreateMap<ExpertiseBranchDTO, ExpertiseBranch>();
            CreateMap<ExpertiseBranch, ExpertiseBranchResponseDTO>();
            CreateMap<HospitalDTO, Hospital>();
            CreateMap<Hospital, HospitalResponseDTO>();
            CreateMap<AuthorizationCategoryDTO, AuthorizationCategory>();
            CreateMap<AuthorizationCategory, AuthorizationCategoryResponseDTO>();
            CreateMap<AuthorizationCategory, MobileAuthCategoryResponseDTO>();
            CreateMap<ProvinceDTO, Province>();
            CreateMap<Province, ProvinceResponseDTO>();
            CreateMap<InstitutionDTO, Institution>();
            CreateMap<Institution, InstitutionResponseDTO>();
            CreateMap<ProgramDTO, Program>();
            CreateMap<Program, ProgramResponseDTO>();
            CreateMap<ProgramPaginateResponseDTO, MobileProgramPaginateResponseDTO>();
            CreateMap<Program, FilterResponse<Program>>();
            CreateMap<AuthorizationDetailDTO, AuthorizationDetail>()
                .ForMember(x => x.Description, y => y.MapFrom(r => JsonConvert.SerializeObject(r.Descriptions)));
            CreateMap<AuthorizationDetail, AuthorizationDetailResponseDTO>()
                .ForMember(x => x.Descriptions, y => y.MapFrom(r => JsonConvert.DeserializeObject<List<string>>(r.Description)));
            CreateMap<AuthorizationDetail, MobileAuthDetailResponseDTO>();
            CreateMap<AffiliationDTO, Affiliation>();
            CreateMap<Affiliation, AffiliationResponseDTO>();
            CreateMap<EducatorDTO, Educator>();
            CreateMap<Educator, EducatorResponseDTO>();
            CreateMap<EducatorPaginateResponseDTO, MobileEducatorPaginateResponseDTO>();

            CreateMap<EducatorExp, EducatorResponseDTO>();

            CreateMap<EducatorExpertiseBranchDTO, EducatorExpertiseBranch>();
            CreateMap<EducatorExpertiseBranch, EducatorExpertiseBranchResponseDTO>();
            CreateMap<StudentDTO, Student>();
            CreateMap<Student, StudentResponseDTO>();
            CreateMap<StudentPaginateResponseDTO, MobileStudentPaginateResponseDTO>();
            CreateMap<ProtocolProgramDTO, ProtocolProgram>();
            CreateMap<ProtocolProgram, ProtocolProgramResponseDTO>();
            CreateMap<ProtocolProgramResponseDTO, ProtocolProgramDTO>();
            CreateMap<EducatorProgramDTO, EducatorProgram>();
            CreateMap<EducatorProgramResponseDTO, EducatorProgram>();
            CreateMap<EducatorProgram, EducatorProgramResponseDTO>();
            CreateMap<PerfectionDTO, Perfection>();
            CreateMap<Perfection, PerfectionResponseDTO>();
            CreateMap<DocumentDTO, Document>();
            CreateMap<Document, DocumentResponseDTO>();
            CreateMap<DocumentResponseDTO, Document>();
            CreateMap<DocumentResponseDTO, DocumentDTO>();
            CreateMap<DependentProgramDTO, DependentProgram>();
            CreateMap<DependentProgram, DependentProgramResponseDTO>();
            CreateMap<DependentProgramResponseDTO, DependentProgramDTO>();
            CreateMap<CurriculumDTO, Curriculum>();
            CreateMap<Curriculum, CurriculumResponseDTO>();
            CreateMap<RotationDTO, Rotation>();
            CreateMap<Rotation, RotationResponseDTO>();
            CreateMap<TitleDTO, Title>();
            CreateMap<Title, TitleResponseDTO>();
            CreateMap<StudentRotationDTO, StudentRotation>();
            CreateMap<StudentRotation, StudentRotationResponseDTO>();
            CreateMap<StudentRotationResponseDTO, StudentRotation>();
            CreateMap<StudentPerfectionDTO, StudentPerfection>();
            CreateMap<StudentPerfection, StudentPerfectionResponseDTO>();
            CreateMap<EducatorDependentProgramDTO, EducatorDependentProgram>();
            CreateMap<EducatorDependentProgram, EducatorDependentProgramResponseDTO>();
            CreateMap<EducatorDependentProgramResponseDTO, EducatorDependentProgramDTO>();
            CreateMap<ThesisDTO, Thesis>();
            CreateMap<Thesis, ThesisResponseDTO>();
            CreateMap<PerformanceRatingDTO, PerformanceRating>();
            CreateMap<PerformanceRating, PerformanceRatingResponseDTO>();
            CreateMap<JuryDTO, Jury>();
            CreateMap<Jury, JuryResponseDTO>();
            CreateMap<OpinionFormDTO, OpinionForm>();
            CreateMap<OpinionForm, OpinionFormResponseDTO>();
            CreateMap<EducationTrackingDTO, EducationTracking>();
            CreateMap<EducationTracking, EducationTrackingDTO>();
            CreateMap<EducationTracking, EducationTrackingResponseDTO>();
            CreateMap<ProgressReportDTO, ProgressReport>();
            CreateMap<ProgressReport, ProgressReportResponseDTO>();
            CreateMap<AdvisorThesisDTO, AdvisorThesis>();
            CreateMap<ChangeCoordinatorAdvisorThesisDTO, AdvisorThesis>();
            CreateMap<AdvisorThesis, AdvisorThesisResponseDTO>();
            CreateMap<NotificationDTO, Notification>();
            CreateMap<Notification, NotificationResponseDTO>();
            CreateMap<EthicCommitteeDecisionDTO, EthicCommitteeDecision>();
            CreateMap<EthicCommitteeDecision, EthicCommitteeDecisionResponseDTO>();
            CreateMap<OfficialLetterDTO, OfficialLetter>();
            CreateMap<OfficialLetter, OfficialLetterResponseDTO>();
            CreateMap<ThesisDefenceDTO, ThesisDefence>();
            CreateMap<ThesisDefence, ThesisDefenceResponseDTO>();
            CreateMap<Property, PropertyResponseDTO>();
            CreateMap<RolePermission2DTO, RolePermission>();
            CreateMap<StudentExpertiseBranch, StudentExpertiseBranchResponseDTO>();
            CreateMap<StudentExpertiseBranchDTO, StudentExpertiseBranch>();
            CreateMap<CurriculumRotation, CurriculumRotationResponseDTO>();
            CreateMap<CurriculumRotationDTO, CurriculumRotation>();
            CreateMap<CurriculumPerfection, CurriculumPerfectionResponseDTO>();
            CreateMap<CurriculumPerfectionDTO, CurriculumPerfection>();
            CreateMap<Country, CountryResponseDTO>();
            CreateMap<CountryDTO, Country>();
            CreateMap<Demand, DemandResponseDTO>();
            CreateMap<DemandDTO, Demand>();
            CreateMap<Study, StudyResponseDTO>();
            CreateMap<StudyDTO, Study>();
            CreateMap<ScientificStudy, ScientificStudyResponseDTO>();
            CreateMap<ScientificStudyDTO, ScientificStudy>();
            CreateMap<ExitExam, ExitExamResponseDTO>();
            CreateMap<ExitExamDTO, ExitExam>();
            CreateMap<EducatorAdministrativeTitle, EducatorAdministrativeTitleResponseDTO>();
            CreateMap<EducatorAdministrativeTitleDTO, EducatorAdministrativeTitle>();
            CreateMap<GraduationDetail, GraduationDetailResponseDTO>();
            CreateMap<GraduationDetailDTO, GraduationDetail>();
            CreateMap<RelatedExpertiseBranch, RelatedExpertiseBranchResponseDTO>();
            CreateMap<RelatedExpertiseBranchDTO, RelatedExpertiseBranch>();
            CreateMap<UserNotification, UserNotificationResponseDTO>();
            CreateMap<UserNotificationDTO, UserNotification>();
            CreateMap<EducationOfficer, EducationOfficerResponseDTO>();
            CreateMap<EducationOfficerDTO, EducationOfficer>();
            CreateMap<EducatorStaffParentInstitution, EducatorStaffParentInstitutionResponseDTO>();
            CreateMap<EducatorStaffParentInstitutionDTO, EducatorStaffParentInstitution>();
            CreateMap<EducatorStaffInstitution, EducatorStaffInstitutionResponseDTO>();
            CreateMap<EducatorStaffInstitutionDTO, EducatorStaffInstitution>();
            CreateMap<StudentDependentProgramPaginateDTO, StudentDependentProgram>();
            CreateMap<StudentDependentProgramDTO, StudentDependentProgram>();
            CreateMap<QuotaRequest, QuotaRequestResponseDTO>()
                .ForMember(x => x.GlobalQuota, y => y.MapFrom(r => JsonConvert.DeserializeObject<List<GlobalQuotaExpertiseBranchModel>>(r.GlobalQuota)));
            CreateMap<QuotaRequestDTO, QuotaRequest>()
                .ForMember(x => x.GlobalQuota, y => y.MapFrom(r => JsonConvert.SerializeObject(r.GlobalQuota)));
            CreateMap<SubQuotaRequest, SubQuotaRequestResponseDTO>();
            CreateMap<SubQuotaRequestDTO, SubQuotaRequest>();
            CreateMap<SubQuotaRequestPortfolio, SubQuotaRequestPortfolioResponseDTO>();
            CreateMap<SubQuotaRequestPortfolioDTO, SubQuotaRequestPortfolio>();
            CreateMap<StudentCount, StudentCountResponseDTO>();
            CreateMap<StudentCountDTO, StudentCount>();
            CreateMap<EducatorCountContributionFormula, EducatorCountContributionFormulaResponseDTO>();
            CreateMap<EducatorCountContributionFormulaDTO, EducatorCountContributionFormula>();
            CreateMap<Portfolio, PortfolioResponseDTO>();
            CreateMap<PortfolioDTO, Portfolio>();
            CreateMap<Announcement, AnnouncementResponseDTO>();
            CreateMap<AnnouncementDTO, Announcement>();
            CreateMap<AuthorizationDetailDTO, AuthorizationDetail>()
                .ForMember(x => x.Description, y => y.MapFrom(r => JsonConvert.SerializeObject(r.Descriptions)));
            CreateMap<AuthorizationDetail, AuthorizationDetailResponseDTO>()
                .ForMember(x => x.Descriptions, y => y.MapFrom(r => JsonConvert.DeserializeObject<List<string>>(r.Description)));

            CreateMap<Property, PropertyResponseDTO>();
            CreateMap<PropertyDTO, Property>();

            CreateMap<KPSResult, KPSResultResponseDTO>();
            CreateMap<AddressInfo, AddressInfoResponseDTO>();

            CreateMap<LogDTO, Log>();
            CreateMap<Log, LogResponseDTO>();

            CreateMap<MenuDTO, Menu>();
            CreateMap<Menu, MenuResponseDTO>();

            CreateMap<UserRoleDTO, UserRole>();
            CreateMap<UserRole, UserRoleResponseDTO>();

            CreateMap<UserRoleFacultyDTO, UserRoleFaculty>();
            CreateMap<UserRoleFaculty, UserRoleFacultyResponseDTO>();

            CreateMap<UserRoleUniversityDTO, UserRoleUniversity>();
            CreateMap<UserRoleUniversity, UserRoleUniversityResponseDTO>();

            CreateMap<UserRoleProgramDTO, UserRoleProgram>();
            CreateMap<UserRoleProgram, UserRoleProgramResponseDTO>();

            CreateMap<UserRoleHospitalDTO, UserRoleHospital>();
            CreateMap<UserRoleHospital, UserRoleHospitalResponseDTO>();

            CreateMap<UserRoleProvinceDTO, UserRoleProvince>();
            CreateMap<UserRoleProvince, UserRoleProvinceResponseDTO>();

            CreateMap<StudentRotationPerfectionDTO, StudentRotationPerfection>();
            CreateMap<StudentRotationPerfection, StudentRotationPerfectionResponseDTO>();

            CreateMap<Field, FieldResponseDTO>();

            CreateMap<RelatedDependentProgramDTO, RelatedDependentProgram>();
            CreateMap<RelatedDependentProgram, RelatedDependentProgramResponseDTO>();
            CreateMap<RelatedDependentProgramResponseDTO, RelatedDependentProgramDTO>();


            CreateMap<ZoneModel, Shared.ResponseModels.Authorization.ZoneModelDTO>();

            CreateMap<StandardDTO, Standard>();
            CreateMap<Standard, StandardResponseDTO>();

            CreateMap<StandardCategoryDTO, StandardCategory>();
            CreateMap<StandardCategory, StandardCategoryResponseDTO>();

            CreateMap<FormDTO, Form>();
            CreateMap<Form, FormResponseDTO>();

            CreateMap<FormStandardDTO, FormStandard>();
            CreateMap<FormStandard, FormStandardResponseDTO>();

            CreateMap<OnSiteVisitCommitteeDTO, OnSiteVisitCommittee>();
            CreateMap<OnSiteVisitCommittee, OnSiteVisitCommitteeResponseDTO>();

            CreateMap<SpecificEducationDTO, SpecificEducation>();
            CreateMap<SpecificEducation, SpecificEducationResponseDTO>();

            CreateMap<StudentSpecificEducationDTO, StudentsSpecificEducation>();
            CreateMap<StudentsSpecificEducation, StudentSpecificEducationResponseDTO>();

            CreateMap<SpecificEducationPlaceDTO, SpecificEducationPlace>();
            CreateMap<SpecificEducationPlace, SpecificEducationPlaceResponseDTO>();

            CreateMap<ENabizPortfolio, ENabizPortfolioResponseDTO>();
            CreateMap<PersonelHareketi, PersonelHareketiResponseDTO>();
            CreateMap<Personel, PersonelResponseDTO>();
        }

    }
}
