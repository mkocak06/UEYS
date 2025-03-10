using Application.Interfaces;
using Application.Mapper;
using Application.Services;
using Autofac;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Koru;
using Koru.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        public IConfiguration Configuration { get; }
        protected override void Load(ContainerBuilder builder)
        {
            //automapper configuration
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            builder.RegisterInstance(mapper).SingleInstance();

            //builder.register
            //builder.Register(c =>
            //{
            //    var config = c.Resolve<IConfiguration>();

            //    var opt = new DbContextOptionsBuilder<ApplicationDbContext>();
            //    opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));

            //    return new ApplicationDbContext(opt.Options);
            //}).AsSelf().InstancePerLifetimeScope();
            //builder.RegisterType<ApplicationDbContext>().As<DbContext>().AsSelf().InstancePerLifetimeScope();


            //builder.RegisterType<XmlService>().As<IXmlService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerDependency();
            //builder.RegisterType<SMSSender>().As<ISMSSender>().InstancePerDependency();
            //builder.RegisterType<ViewRenderService>().As<IViewRenderService>().InstancePerDependency();

            builder.RegisterType<KoruRepository>().As<IKoruRepository>().InstancePerLifetimeScope();

            builder.RegisterType<AuthRepository>().As<IAuthRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CryptoService>().As<ICryptoService>().InstancePerLifetimeScope();
            //builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
            builder.RegisterType<S3Service>().As<IS3Service>().InstancePerDependency();


            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerDependency();
            builder.RegisterType<UniversityRepository>().As<IUniversityRepository>().InstancePerDependency();
            builder.RegisterType<ProfessionRepository>().As<IProfessionRepository>().InstancePerDependency();
            builder.RegisterType<ExpertiseBranchRepository>().As<IExpertiseBranchRepository>().InstancePerDependency();
            builder.RegisterType<HospitalRepository>().As<IHospitalRepository>().InstancePerDependency();
            builder.RegisterType<AuthorizationCategoryRepository>().As<IAuthorizationCategoryRepository>().InstancePerDependency();
            builder.RegisterType<ProvinceRepository>().As<IProvinceRepository>().InstancePerDependency();
            builder.RegisterType<InstitutionRepository>().As<IInstitutionRepository>().InstancePerDependency();
            builder.RegisterType<ProgramRepository>().As<IProgramRepository>().InstancePerDependency();
            builder.RegisterType<AuthorizationDetailRepository>().As<IAuthorizationDetailRepository>().InstancePerDependency();
            builder.RegisterType<ProfessionRepository>().As<IProfessionRepository>().InstancePerDependency();
            builder.RegisterType<FacultyRepository>().As<IFacultyRepository>().InstancePerDependency();
            builder.RegisterType<EducatorRepository>().As<IEducatorRepository>().InstancePerDependency();
            builder.RegisterType<StudentRepository>().As<IStudentRepository>().InstancePerDependency();
            builder.RegisterType<ProtocolProgramRepository>().As<IProtocolProgramRepository>().InstancePerDependency();
            builder.RegisterType<DependentProgramRepository>().As<IDependentProgramRepository>().InstancePerDependency();
            builder.RegisterType<AffiliationRepository>().As<IAffiliationRepository>().InstancePerDependency();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerDependency();
            builder.RegisterType<PerfectionRepository>().As<IPerfectionRepository>().InstancePerDependency();
            builder.RegisterType<DocumentRepository>().As<IDocumentRepository>().InstancePerDependency();
            builder.RegisterType<CurriculumRepository>().As<ICurriculumRepository>().InstancePerDependency();
            builder.RegisterType<RotationRepository>().As<IRotationRepository>().InstancePerDependency();
            builder.RegisterType<ThesisRepository>().As<IThesisRepository>().InstancePerDependency();
            builder.RegisterType<PerformanceRatingRepository>().As<IPerformanceRatingRepository>().InstancePerDependency();
            builder.RegisterType<OpinionFormRepository>().As<IOpinionFormRepository>().InstancePerDependency();
            builder.RegisterType<EducationTrackingRepository>().As<IEducationTrackingRepository>().InstancePerDependency();
            builder.RegisterType<ProgressReportRepository>().As<IProgressReportRepository>().InstancePerDependency();
            builder.RegisterType<NotificationRepository>().As<INotificationRepository>().InstancePerDependency();
            builder.RegisterType<EthicCommitteeDecisionRepository>().As<IEthicCommitteeDecisionRepository>().InstancePerDependency();
            builder.RegisterType<OfficialLetterRepository>().As<IOfficialLetterRepository>().InstancePerDependency();
            builder.RegisterType<ThesisDefenceRepository>().As<IThesisDefenceRepository>().InstancePerDependency();
            builder.RegisterType<StudyRepository>().As<IStudyRepository>().InstancePerDependency();
            builder.RegisterType<ScientificStudyRepository>().As<IScientificStudyRepository>().InstancePerDependency();
            builder.RegisterType<ExitExamRepository>().As<IExitExamRepository>().InstancePerDependency();
            builder.RegisterType<StandardRepository>().As<IStandardRepository>().InstancePerDependency();
            builder.RegisterType<StandardCategoryRepository>().As<IStandardCategoryRepository>().InstancePerDependency();
            builder.RegisterType<FormRepository>().As<IFormRepository>().InstancePerDependency();
            builder.RegisterType<FormStandardRepository>().As<IFormStandardRepository>().InstancePerDependency();
            builder.RegisterType<EducatorExpertiseBranchRepository>().As<IEducatorExpertiseBranchRepository>().InstancePerDependency();
            builder.RegisterType<EducatorProgramRepository>().As<IEducatorProgramRepository>().InstancePerDependency();
            builder.RegisterType<StudentRotationRepository>().As<IStudentRotationRepository>().InstancePerDependency();
            builder.RegisterType<StudentPerfectionRepository>().As<IStudentPerfectionRepository>().InstancePerDependency();
            builder.RegisterType<TitleRepository>().As<ITitleRepository>().InstancePerDependency();
            builder.RegisterType<EducatorDependentProgramRepository>().As<IEducatorDependentProgramRepository>().InstancePerDependency();
            builder.RegisterType<JuryRepository>().As<IJuryRepository>().InstancePerDependency();
            builder.RegisterType<AdvisorThesisRepository>().As<IAdvisorThesisRepository>().InstancePerDependency();
            builder.RegisterType<EducatorExpertiseBranchRepository>().As<IEducatorExpertiseBranchRepository>().InstancePerDependency();
            builder.RegisterType<PropertyRepository>().As<IPropertyRepository>().InstancePerDependency();
            builder.RegisterType<PerfectionRepository>().As<IPerfectionRepository>().InstancePerDependency();
            builder.RegisterType<RelatedExpertiseBranchRepository>().As<IRelatedExpertiseBranchRepository>().InstancePerDependency();
            builder.RegisterType<StudentExpertiseBranchRepository>().As<IStudentExpertiseBranchRepository>().InstancePerDependency();
            builder.RegisterType<CurriculumRotationRepository>().As<ICurriculumRotationRepository>().InstancePerDependency();
            builder.RegisterType<CurriculumPerfectionRepository>().As<ICurriculumPerfectionRepository>().InstancePerDependency();
            builder.RegisterType<CountryRepository>().As<ICountryRepository>().InstancePerDependency();
            builder.RegisterType<DemandRepository>().As<IDemandRepository>().InstancePerDependency();
            builder.RegisterType<LogRepository>().As<ILogRepository>().InstancePerDependency();
            builder.RegisterType<MenuRepository>().As<IMenuRepository>().InstancePerDependency();
            builder.RegisterType<RoleCategoryRepository>().As<IRoleCategoryRepository>().InstancePerDependency();
            //builder.RegisterType<RoleFieldRepository>().As<IRoleFieldRepository>().InstancePerDependency();
            //builder.RegisterType<FieldRepository>().As<IFieldRepository>().InstancePerDependency();
            builder.RegisterType<UserNotificationRepository>().As<IUserNotificationRepository>().InstancePerDependency();
            builder.RegisterType<EducationOfficerRepository>().As<IEducationOfficerRepository>().InstancePerDependency();
            builder.RegisterType<EducatorStaffInstitutionRepository>().As<IEducatorStaffInstitutionRepository>().InstancePerDependency();
            builder.RegisterType<EducatorStaffParentInstitutionRepository>().As<IEducatorStaffParentInstitutionRepository>().InstancePerDependency();
            builder.RegisterType<StudentRotationPerfectionRepository>().As<IStudentRotationPerfectionRepository>().InstancePerDependency();
            builder.RegisterType<RelatedDependentProgramRepository>().As<IRelatedDependentProgramRepository>().InstancePerDependency();
            builder.RegisterType<StudentDependentProgramRepository>().As<IStudentDependentProgramRepository>().InstancePerDependency();
            builder.RegisterType<SpecificEducationRepository>().As<ISpecificEducationRepository>().InstancePerDependency();
            builder.RegisterType<SpecificEducationPlaceRepository>().As<ISpecificEducationPlaceRepository>().InstancePerDependency();
            builder.RegisterType<StudentsSpecificEducationRepository>().As<IStudentsSpecificEducationRepository>().InstancePerDependency();
            builder.RegisterType<SinaRepository>().As<ISinaRepository>().InstancePerDependency();
            builder.RegisterType<PortfolioRepository>().As<IPortfolioRepository>().InstancePerDependency();
            builder.RegisterType<SubQuotaRequestRepository>().As<ISubQuotaRequestRepository>().InstancePerDependency();
            builder.RegisterType<QuotaRequestRepository>().As<IQuotaRequestRepository>().InstancePerDependency();
            builder.RegisterType<EducatorCountContributionFormulaRepository>().As<IEducatorCountContributionFormulaRepository>().InstancePerDependency();
            builder.RegisterType<SubQuotaRequestPortfolioRepository>().As<ISubQuotaRequestPortfolioRepository>().InstancePerDependency();
            builder.RegisterType<StudentCountRepository>().As<IStudentCountRepository>().InstancePerDependency();
            builder.RegisterType<AnnouncementRepository>().As<IAnnouncementRepository>().InstancePerDependency();

            builder.RegisterType<ENabizPortfolioRepository>().As<IENabizPortfolioRepository>().InstancePerDependency();

            //services injection
            //builder.RegisterType<BackgroundJobService>().As<IBackgroundJobService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerLifetimeScope();
            builder.RegisterType<UniversityService>().As<IUniversityService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfessionService>().As<IProfessionService>().InstancePerLifetimeScope();
            builder.RegisterType<ExpertiseBranchService>().As<IExpertiseBranchService>().InstancePerLifetimeScope();
            builder.RegisterType<HospitalService>().As<IHospitalService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationCategoryService>().As<IAuthorizationCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<ProvinceService>().As<IProvinceService>().InstancePerLifetimeScope();
            builder.RegisterType<InstitutionService>().As<IInstitutionService>().InstancePerLifetimeScope();
            builder.RegisterType<ProgramService>().As<IProgramService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationDetailService>().As<IAuthorizationDetailService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfessionService>().As<IProfessionService>().InstancePerLifetimeScope();
            builder.RegisterType<EducatorService>().As<IEducatorService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentService>().As<IStudentService>().InstancePerLifetimeScope();
            builder.RegisterType<ProtocolProgramService>().As<IProtocolProgramService>().InstancePerLifetimeScope();
            builder.RegisterType<DependentProgramService>().As<IDependentProgramService>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliationService>().As<IAffiliationService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<FacultyService>().As<IFacultyService>().InstancePerLifetimeScope();
            builder.RegisterType<PerfectionService>().As<IPerfectionService>().InstancePerLifetimeScope();
            builder.RegisterType<FileManagementService>().As<IFileManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<DocumentService>().As<IDocumentService>().InstancePerLifetimeScope();
            builder.RegisterType<CurriculumService>().As<ICurriculumService>().InstancePerLifetimeScope();
            builder.RegisterType<TitleService>().As<ITitleService>().InstancePerLifetimeScope();
            builder.RegisterType<RotationService>().As<IRotationService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentRotationService>().As<IStudentRotationService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentPerfectionService>().As<IStudentPerfectionService>().InstancePerLifetimeScope();
            builder.RegisterType<EducatorDependentProgramService>().As<IEducatorDependentProgramService>().InstancePerLifetimeScope();
            builder.RegisterType<ThesisService>().As<IThesisService>().InstancePerLifetimeScope();
            builder.RegisterType<PerformanceRatingService>().As<IPerformanceRatingService>().InstancePerLifetimeScope();
            builder.RegisterType<JuryService>().As<IJuryService>().InstancePerLifetimeScope();
            builder.RegisterType<OpinionFormService>().As<IOpinionFormService>().InstancePerLifetimeScope();
            builder.RegisterType<EducationTrackingService>().As<IEducationTrackingService>().InstancePerLifetimeScope();
            builder.RegisterType<ProgressReportService>().As<IProgressReportService>().InstancePerLifetimeScope();
            builder.RegisterType<AdvisorThesisService>().As<IAdvisorThesisService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<EthicCommitteeDecisionService>().As<IEthicCommitteeDecisionService>().InstancePerLifetimeScope();
            builder.RegisterType<OfficialLetterService>().As<IOfficialLetterService>().InstancePerLifetimeScope();
            builder.RegisterType<ThesisDefenceService>().As<IThesisDefenceService>().InstancePerLifetimeScope();
            builder.RegisterType<EducatorExpertiseBranchService>().As<IEducatorExpertiseBranchService>().InstancePerLifetimeScope();
            builder.RegisterType<EducatorProgramService>().As<IEducatorProgramService>().InstancePerLifetimeScope();
            builder.RegisterType<PerfectionService>().As<IPerfectionService>().InstancePerLifetimeScope();
            builder.RegisterType<PropertyService>().As<IPropertyService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentExpertiseBranchService>().As<IStudentExpertiseBranchService>().InstancePerLifetimeScope();
            builder.RegisterType<CurriculumRotationService>().As<ICurriculumRotationService>().InstancePerLifetimeScope();
            builder.RegisterType<CurriculumPerfectionService>().As<ICurriculumPerfectionService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();
            builder.RegisterType<DemandService>().As<IDemandService>().InstancePerLifetimeScope();
            builder.RegisterType<StudyService>().As<IStudyService>().InstancePerLifetimeScope();
            builder.RegisterType<ScientificStudyService>().As<IScientificStudyService>().InstancePerLifetimeScope();
            builder.RegisterType<ExitExamService>().As<IExitExamService>().InstancePerLifetimeScope();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerLifetimeScope();
            builder.RegisterType<CaptchaService>().As<ICaptchaService>().InstancePerLifetimeScope();
            builder.RegisterType<MenuService>().As<IMenuService>().InstancePerLifetimeScope();
            builder.RegisterType<UserNotificationService>().As<IUserNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<EducationOfficerService>().As<IEducationOfficerService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentRotationPerfectionService>().As<IStudentRotationPerfectionService>().InstancePerLifetimeScope();
            builder.RegisterType<RelatedDependentProgramService>().As<IRelatedDependentProgramService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentDependentProgramService>().As<IStudentDependentProgramService>().InstancePerLifetimeScope();
            builder.RegisterType<MobileService>().As<IMobileService>().InstancePerLifetimeScope();
            builder.RegisterType<ENabizService>().As<IENabizService>().InstancePerLifetimeScope();
            builder.RegisterType<OSYMService>().As<IOSYMService>().InstancePerLifetimeScope();
            builder.RegisterType<SpecialistDoctorService>().As<ISpecialistDoctorService>().InstancePerLifetimeScope();

            builder.RegisterType<StandardService>().As<IStandardService>().InstancePerLifetimeScope();
            builder.RegisterType<StandardCategoryService>().As<IStandardCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<FormService>().As<IFormService>().InstancePerLifetimeScope();
            builder.RegisterType<FormStandardService>().As<IFormStandardService>().InstancePerLifetimeScope();
            builder.RegisterType<SpecificEducationService>().As<ISpecificEducationService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentsSpecificEducationService>().As<IStudentsSpecificEducationService>().InstancePerLifetimeScope();
            builder.RegisterType<SpecificEducationPlaceService>().As<ISpecificEducationPlaceService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotaRequestService>().As<IQuotaRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<SubQuotaRequestService>().As<ISubQuotaRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<PortfolioService>().As<IPortfolioService>().InstancePerLifetimeScope();
            builder.RegisterType<EducatorCountContributionFormulaService>().As<IEducatorCountContributionFormulaService>().InstancePerLifetimeScope();
            builder.RegisterType<SubQuotaRequestPortfolioService>().As<ISubQuotaRequestPortfolioService>().InstancePerLifetimeScope();
            builder.RegisterType<StudentCountService>().As<IStudentCountService>().InstancePerLifetimeScope();
            builder.RegisterType<AnnouncementService>().As<IAnnouncementService>().InstancePerLifetimeScope();

            builder.RegisterType<ENabizPortfolioService>().As<IENabizPortfolioService>().InstancePerLifetimeScope();

            #region externalService
            builder.RegisterType<KPSService>().As<IKPSService>().InstancePerDependency();
            builder.RegisterType<CKYSService>().As<ICKYSService>().InstancePerDependency();
            builder.RegisterType<YOKService>().As<IYOKService>().InstancePerDependency();
            builder.RegisterType<EkipService>().As<IEkipService>().InstancePerDependency();
            builder.RegisterType<SMSSender>().As<ISMSSender>().InstancePerDependency();
            #endregion



            base.Load(builder);
        }
    }
}
