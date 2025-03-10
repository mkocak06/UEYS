using Core.Entities;
using Core.Entities.Koru;
using Core.KDSModels;
using Infrastructure.EntityProperties;
using Infrastructure.EntityProperties.AuthorizationProperties;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            // System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        #region DbSet
        public DbSet<User> Users { get; set; }
        #region role
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRoleFaculty> UserRoleFaculties { get; set; }
        public DbSet<UserRoleHospital> UserRoleHospitals { get; set; }
        public DbSet<UserRoleProgram> UserRolePrograms { get; set; }
        public DbSet<UserRoleProvince> UserRoleProvinces { get; set; }
        public DbSet<UserRoleUniversity> UserRoleUniversities { get; set; }
        public DbSet<UserRoleStudent> UserRoleStudents { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<RoleCategory> RoleCategories { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<RoleField> RoleFields { get; set; }
        #endregion

        public DbSet<Province> Provinces { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Affiliation> Affiliations { get; set; }
        public DbSet<AuthorizationCategory> AuthorizationCategories { get; set; }
        public DbSet<AuthorizationDetail> AuthorizationDetails { get; set; }
        public DbSet<Curriculum> Curricula { get; set; }
        public DbSet<Educator> Educators { get; set; }
        public DbSet<ExpertiseBranch> ExpertiseBranches { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<EducatorExpertiseBranch> EducatorExpertiseBranches { get; set; }
        public DbSet<EducatorProgram> EducatorPrograms { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<StudentRotation> StudentRotations { get; set; }
        public DbSet<ProtocolProgram> ProtocolPrograms { get; set; }
        public DbSet<Perfection> Perfections { get; set; }
        public DbSet<StudentPerfection> StudentPerfections { get; set; }
        public DbSet<DependentProgram> DependentPrograms { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Thesis> Theses { get; set; }
        public DbSet<EducatorDependentProgram> EducatorDependentPrograms { get; set; }
        public DbSet<PerformanceRating> PerformanceRatings { get; set; }
        public DbSet<Jury> Juries { get; set; }
        public DbSet<OpinionForm> OpinionForms { get; set; }
        public DbSet<EducationTracking> EducationTrackings { get; set; }
        public DbSet<ProgressReport> ProgressReports { get; set; }
        public DbSet<AdvisorThesis> AdvisorTheses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EthicCommitteeDecision> EthicCommitteeDecisions { get; set; }
        public DbSet<OfficialLetter> OfficialLetters { get; set; }
        public DbSet<ThesisDefence> ThesisDefences { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PerfectionProperty> PerfectionProperties { get; set; }
        public DbSet<RelatedExpertiseBranch> RelatedExpertiseBranches { get; set; }
        public DbSet<StudentExpertiseBranch> StudentExpertiseBranches { get; set; }
        public DbSet<CurriculumRotation> CurriculumRotations { get; set; }
        public DbSet<CurriculumPerfection> CurriculumPerfections { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Demand> Demands { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<ScientificStudy> ScientificStudies { get; set; }
        public DbSet<ExitExam> ExitExams { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<EducatorAdministrativeTitle> EducatorAdministrativeTitles { get; set; }
        public DbSet<GraduationDetail> GraduationDetails { get; set; }
        public DbSet<UserNotification> UserNotifications{ get; set; }
        public DbSet<EducationOfficer> EducationOfficers{ get; set; }
        public DbSet<EducatorStaffInstitution> EducatorStaffInstitutions{ get; set; }
        public DbSet<EducatorStaffParentInstitution> EducatorStaffParentInstitutions{ get; set; }
        public DbSet<StudentRotationPerfection> StudentRotationPerfections { get; set; }
        public DbSet<StudentDependentProgram> StudentDependentPrograms{ get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<StandardCategory> StandardCategories { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormStandard> FormStandards { get; set; }
        public DbSet<OnSiteVisitCommittee> OnSiteVisitCommittees { get; set; }
        public DbSet<SpecificEducation> SpecificEducations { get; set; }
        public DbSet<StudentsSpecificEducation> StudentsSpecificEducations { get; set; }
        public DbSet<SpecificEducationPlace> SpecificEducationPlaces { get; set; }
        public DbSet<Sina> Sina { get; set; }
        public DbSet<QuotaRequest> QuotaRequests { get; set; }
        public DbSet<SubQuotaRequest> SubQuotaRequests { get; set; }
        public DbSet<SubQuotaRequestPortfolio> SubQuotaRequestPortfolios { get; set; }
        public DbSet<EducatorCountContributionFormula> EducatorCountContributionFormulas { get; set; }
        public DbSet<StudentCount> StudentCounts { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        // kds schema
        public DbSet<ENabizPortfolio> ENabizPortfolios { get; set; }


        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Builder
            _ = modelBuilder.Entity<Affiliation>(AffiliationProperties.OnModelCreating);
            _ = modelBuilder.Entity<AuthorizationCategory>(AuthorizationCategoryProperties.OnModelCreating);
            _ = modelBuilder.Entity<AuthorizationDetail>(AuthorizationDetailProperties.OnModelCreating);
            _ = modelBuilder.Entity<Rotation>(RotationProperties.OnModelCreating);
            _ = modelBuilder.Entity<Curriculum>(CurriculumProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducatorExpertiseBranch>(EducatorExpertiseBranchProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducatorProgram>(EducatorProgramProperties.OnModelCreating);
            _ = modelBuilder.Entity<Educator>(EducatorProperties.OnModelCreating);
            _ = modelBuilder.Entity<ExpertiseBranch>(ExpertiseBranchProperties.OnModelCreating);
            _ = modelBuilder.Entity<Profession>(ProfessionProperties.OnModelCreating);
            _ = modelBuilder.Entity<Hospital>(HospitalProperties.OnModelCreating);
            _ = modelBuilder.Entity<Institution>(InstitutionProperties.OnModelCreating);
            _ = modelBuilder.Entity<Program>(ProgramProperties.OnModelCreating);
            _ = modelBuilder.Entity<Province>(ProvinceProperties.OnModelCreating);
            _ = modelBuilder.Entity<Student>(StudentProperties.OnModelCreating);
            _ = modelBuilder.Entity<StudentRotation>(StudentRotationProperties.OnModelCreating);
            _ = modelBuilder.Entity<University>(UniversityProperties.OnModelCreating);
            _ = modelBuilder.Entity<Faculty>(FacultyProperties.OnModelCreating);
            _ = modelBuilder.Entity<User>(UserProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserRole>(UserRoleProperties.OnModelCreating);
            _ = modelBuilder.Entity<Role>(RoleProperties.OnModelCreating);
            _ = modelBuilder.Entity<RolePermission>(RolePermissionProperties.OnModelCreating);
            _ = modelBuilder.Entity<Perfection>(EntityProperties.PerfectionProperties.OnModelCreating);
            _ = modelBuilder.Entity<StudentPerfection>(StudentPerfectionProperties.OnModelCreating);
            _ = modelBuilder.Entity<DependentProgram>(DependentProgramProperties.OnModelCreating);
            _ = modelBuilder.Entity<Document>(DocumentProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducatorDependentProgram>(EducatorDependentProgramProperties.OnModelCreating);
            _ = modelBuilder.Entity<Thesis>(ThesisProperties.OnModelCreating);
            _ = modelBuilder.Entity<PerformanceRating>(PerformanceRatingProperties.OnModelCreating);
            _ = modelBuilder.Entity<Jury>(JuryProperties.OnModelCreating);
            _ = modelBuilder.Entity<OpinionForm>(OpinionFormProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducationTracking>(EducationTrackingProperties.OnModelCreating);
            _ = modelBuilder.Entity<ProgressReport>(ProgressReportProperties.OnModelCreating);
            _ = modelBuilder.Entity<AdvisorThesis>(AdvisorThesisProperties.OnModelCreating);
            _ = modelBuilder.Entity<Notification>(NotificationProperties.OnModelCreating);
            _ = modelBuilder.Entity<EthicCommitteeDecision>(EthicCommitteeDecisionProperties.OnModelCreating);
            _ = modelBuilder.Entity<OfficialLetter>(OfficialLetterProperties.OnModelCreating);
            _ = modelBuilder.Entity<ThesisDefence>(ThesisDefenceProperties.OnModelCreating);
            _ = modelBuilder.Entity<PerfectionProperty>(PerfectionPropertyProperties.OnModelCreating);
            _ = modelBuilder.Entity<RelatedExpertiseBranch>(RelatedExpertiseBranchProperties.OnModelCreating);
            _ = modelBuilder.Entity<StudentExpertiseBranch>(StudentExpertiseBranchProperties.OnModelCreating);
            _ = modelBuilder.Entity<CurriculumRotation>(CurriculumRotationProperties.OnModelCreating);
            _ = modelBuilder.Entity<CurriculumPerfection>(CurriculumPerfectionProperties.OnModelCreating);
            _ = modelBuilder.Entity<Study>(StudyProperties.OnModelCreating);
            _ = modelBuilder.Entity<ScientificStudy>(ScientificStudyProperties.OnModelCreating);
            _ = modelBuilder.Entity<ExitExam>(ExitExamProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducatorAdministrativeTitle>(EducatorAdministrativeTitleProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserRoleFaculty>(UserRoleFacultyProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserRoleHospital>(UserRoleHospitalProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserRoleProgram>(UserRoleProgramProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserRoleProvince>(UserRoleProvinceProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserRoleUniversity>(UserRoleUniversityProperties.OnModelCreating);
            _ = modelBuilder.Entity<GraduationDetail>(GraduationDetailProperties.OnModelCreating);
            _ = modelBuilder.Entity<QuotaRequest>(QuotaRequestProperties.OnModelCreating);
            _ = modelBuilder.Entity<SubQuotaRequest>(SubQuotaRequestProperties.OnModelCreating);
            _ = modelBuilder.Entity<SubQuotaRequestPortfolio>(SubQuotaRequestPortfolioProperties.OnModelCreating);
            _ = modelBuilder.Entity<StudentCount>(StudentCountProperties.OnModelCreating);
            _ = modelBuilder.Entity<Portfolio>(PortfolioProperties.OnModelCreating);

            _ = modelBuilder.Entity<RoleMenu>(RoleMenuProperties.OnModelCreating);
            _ = modelBuilder.Entity<Menu>(MenuProperties.OnModelCreating);
            _ = modelBuilder.Entity<Field>(FieldProperties.OnModelCreating);
            _ = modelBuilder.Entity<RoleField>(RoleFieldProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserRoleStudent>(UserRoleStudentProperties.OnModelCreating);
            _ = modelBuilder.Entity<UserNotification>(UserNotificationProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducationOfficer>(EducationOfficerProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducatorStaffInstitution>(EducatorStaffInstitutionProperties.OnModelCreating);
            _ = modelBuilder.Entity<EducatorStaffParentInstitution>(EducatorStaffParentInstitutionProperties.OnModelCreating);
            _ = modelBuilder.Entity<StudentRotationPerfection>(StudentRotationPerfectionProperties.OnModelCreating);
            _ = modelBuilder.Entity<StudentDependentProgram>(StudentDependentProgramProperties.OnModelCreating);
            _ = modelBuilder.Entity<StandardCategory>(StandardCategoryProperties.OnModelCreating);
            _ = modelBuilder.Entity<Standard>(StandardProperties.OnModelCreating);
            _ = modelBuilder.Entity<Form>(FormProperties.OnModelCreating);
            _ = modelBuilder.Entity<FormStandard>(FormStandardProperties.OnModelCreating);
            _ = modelBuilder.Entity<OnSiteVisitCommittee>(OnSiteVisitCommitteeProperties.OnModelCreating);
            _ = modelBuilder.Entity<SpecificEducation>(SpecificEducationProperties.OnModelCreating);
            _ = modelBuilder.Entity<StudentsSpecificEducation>(StudentsSpecificEducationProperties.OnModelCreating);
            _ = modelBuilder.Entity<SpecificEducationPlace>(SpecificEducationPlaceProperties.OnModelCreating);

            _ = modelBuilder.Entity<ENabizPortfolio>(ENabizPortfolioProperties.OnModelCreating);

            modelBuilder.ApplyUtcDateTimeConverter();
            #endregion
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
