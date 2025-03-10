using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Models.ConfigModels;
using Core.UnitOfWork;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper mapper;
        private readonly AppSettingsModel appSettingsModel;
        IDbContextTransaction transaction = null;

        public UnitOfWork(ApplicationDbContext dbContext, IMapper mapper, AppSettingsModel appSettingsModel)
        {
            _dbContext = dbContext;
            this.mapper = mapper;
            this.appSettingsModel = appSettingsModel;
        }

        private IAffiliationRepository _AffiliationRepository;
        public IAffiliationRepository AffiliationRepository => _AffiliationRepository ??= new AffiliationRepository(_dbContext);

        private IAuthorizationCategoryRepository _AuthorizationCategoryRepository;
        public IAuthorizationCategoryRepository AuthorizationCategoryRepository => _AuthorizationCategoryRepository ??= new AuthorizationCategoryRepository(_dbContext);

        private ICurriculumRepository _CurriculumRepository;
        public ICurriculumRepository CurriculumRepository => _CurriculumRepository ??= new CurriculumRepository(_dbContext);

        private IRotationRepository _RotationRepository;
        public IRotationRepository RotationRepository => _RotationRepository ??= new RotationRepository(_dbContext);

        private IEducatorRepository _EducatorRepository;
        public IEducatorRepository EducatorRepository => _EducatorRepository ??= new EducatorRepository(_dbContext);

        private IExpertiseBranchRepository _ExpertiseBranchRepository;
        public IExpertiseBranchRepository ExpertiseBranchRepository => _ExpertiseBranchRepository ??= new ExpertiseBranchRepository(_dbContext);

        private IHospitalRepository _HospitalRepository;
        public IHospitalRepository HospitalRepository => _HospitalRepository ??= new HospitalRepository(_dbContext);

        private IInstitutionRepository _InstitutionRepository;
        public IInstitutionRepository InstitutionRepository => _InstitutionRepository ??= new InstitutionRepository(_dbContext);

        private IProgramRepository _ProgramRepository;
        public IProgramRepository ProgramRepository => _ProgramRepository ??= new ProgramRepository(_dbContext, mapper);

        private IProvinceRepository _ProvinceRepository;
        public IProvinceRepository ProvinceRepository => _ProvinceRepository ??= new ProvinceRepository(_dbContext);

        private IStudentRepository _StudentRepository;
        public IStudentRepository StudentRepository => _StudentRepository ??= new StudentRepository(_dbContext);

        private IUniversityRepository _UniversityRepository;
        public IUniversityRepository UniversityRepository => _UniversityRepository ??= new UniversityRepository(_dbContext);

        private IAuthorizationDetailRepository _AuthorizationDetailRepository;
        public IAuthorizationDetailRepository AuthorizationDetailRepository => _AuthorizationDetailRepository ??= new AuthorizationDetailRepository(_dbContext);

        private IProfessionRepository _ProfessionRepository;
        public IProfessionRepository ProfessionRepository => _ProfessionRepository ??= new ProfessionRepository(_dbContext);

        private IFacultyRepository _FacultyRepository;
        public IFacultyRepository FacultyRepository => _FacultyRepository ??= new FacultyRepository(_dbContext);

        private IPerfectionRepository _PerfectionRepository;
        public IPerfectionRepository PerfectionRepository => _PerfectionRepository ??= new PerfectionRepository(_dbContext);

        private IEducatorExpertiseBranchRepository _EducatorExpertiseBranchRepository;
        public IEducatorExpertiseBranchRepository EducatorExpertiseBranchRepository => _EducatorExpertiseBranchRepository ??= new EducatorExpertiseBranchRepository(_dbContext);

        private IEducatorProgramRepository _EducatorProgramRepository;
        public IEducatorProgramRepository EducatorProgramRepository => _EducatorProgramRepository ??= new EducatorProgramRepository(_dbContext);

        private IStudentRotationRepository _StudentRotationRepository;
        public IStudentRotationRepository StudentRotationRepository => _StudentRotationRepository ??= new StudentRotationRepository(_dbContext);

        private IStudentPerfectionRepository _StudentPerfectionRepository;
        public IStudentPerfectionRepository StudentPerfectionRepository => _StudentPerfectionRepository ??= new StudentPerfectionRepository(_dbContext);

        private IProtocolProgramRepository _ProtocolProgramRepository;
        public IProtocolProgramRepository ProtocolProgramRepository => _ProtocolProgramRepository ??= new ProtocolProgramRepository(_dbContext);

        private ITitleRepository _TitleRepository;
        public ITitleRepository TitleRepository => _TitleRepository ??= new TitleRepository(_dbContext);

        private IDependentProgramRepository _DependentProgramRepository;
        public IDependentProgramRepository DependentProgramRepository => _DependentProgramRepository ??= new DependentProgramRepository(_dbContext);

        private IDocumentRepository _DocumentRepository;
        public IDocumentRepository DocumentRepository => _DocumentRepository ??= new DocumentRepository(_dbContext);

        private IThesisRepository _ThesisRepository;
        public IThesisRepository ThesisRepository => _ThesisRepository ??= new ThesisRepository(_dbContext);

        private IPerformanceRatingRepository _PerformanceRatingRepository;
        public IPerformanceRatingRepository PerformanceRatingRepository => _PerformanceRatingRepository ??= new PerformanceRatingRepository(_dbContext);

        private IJuryRepository _JuryRepository;
        public IJuryRepository JuryRepository => _JuryRepository ??= new JuryRepository(_dbContext);

        private IOpinionFormRepository _OpinionFormRepository;
        public IOpinionFormRepository OpinionFormRepository => _OpinionFormRepository ??= new OpinionFormRepository(_dbContext);

        private IEducationTrackingRepository _EducationTrackingRepository;
        public IEducationTrackingRepository EducationTrackingRepository => _EducationTrackingRepository ??= new EducationTrackingRepository(_dbContext);

        private IProgressReportRepository _ProgressReportRepository;
        public IProgressReportRepository ProgressReportRepository => _ProgressReportRepository ??= new ProgressReportRepository(_dbContext);

        private IAdvisorThesisRepository _AdvisorThesisRepository;
        public IAdvisorThesisRepository AdvisorThesisRepository => _AdvisorThesisRepository ??= new AdvisorThesisRepository(_dbContext);

        private INotificationRepository _NotificationRepository;
        public INotificationRepository NotificationRepository => _NotificationRepository ??= new NotificationRepository(_dbContext);

        private IEthicCommitteeDecisionRepository _EthicCommitteeDecisionRepository;
        public IEthicCommitteeDecisionRepository EthicCommitteeDecisionRepository => _EthicCommitteeDecisionRepository ??= new EthicCommitteeDecisionRepository(_dbContext);

        private IOfficialLetterRepository _OfficialLetterRepository;
        public IOfficialLetterRepository OfficialLetterRepository => _OfficialLetterRepository ??= new OfficialLetterRepository(_dbContext);

        private IThesisDefenceRepository _ThesisDefenceRepository;
        public IThesisDefenceRepository ThesisDefenceRepository => _ThesisDefenceRepository ??= new ThesisDefenceRepository(_dbContext);

        private IPropertyRepository _PropertyRepository;
        public IPropertyRepository PropertyRepository => _PropertyRepository ??= new PropertyRepository(_dbContext);

        private IStudentExpertiseBranchRepository _StudentExpertiseBranchRepository;
        public IStudentExpertiseBranchRepository StudentExpertiseBranchRepository => _StudentExpertiseBranchRepository ??= new StudentExpertiseBranchRepository(_dbContext);

        private ICurriculumRotationRepository _CurriculumRotationRepository;
        public ICurriculumRotationRepository CurriculumRotationRepository => _CurriculumRotationRepository ??= new CurriculumRotationRepository(_dbContext);

        private ICurriculumPerfectionRepository _CurriculumPerfectionRepository;
        public ICurriculumPerfectionRepository CurriculumPerfectionRepository => _CurriculumPerfectionRepository ??= new CurriculumPerfectionRepository(_dbContext);

        private IPerfectionPropertyRepository _PerfectionPropertyRepository;
        public IPerfectionPropertyRepository PerfectionPropertyRepository => _PerfectionPropertyRepository ??= new PerfectionPropertyRepository(_dbContext);

        private IEducatorAdministrativeTitleRepository _EducatorAdministrativeTitleRepository;
        public IEducatorAdministrativeTitleRepository EducatorAdministrativeTitleRepository => _EducatorAdministrativeTitleRepository ??= new EducatorAdministrativeTitleRepository(_dbContext);

        private IUserRepository _UserRepository;
        public IUserRepository UserRepository => _UserRepository ??= new UserRepository(_dbContext, appSettingsModel);

        private ICountryRepository _CountryRepository;
        public ICountryRepository CountryRepository => _CountryRepository ??= new CountryRepository(_dbContext);

        private IDemandRepository _DemandRepository;
        public IDemandRepository DemandRepository => _DemandRepository ??= new DemandRepository(_dbContext);

        private IStudyRepository _StudyRepository;
        public IStudyRepository StudyRepository => _StudyRepository ??= new StudyRepository(_dbContext);

        private IScientificStudyRepository _ScientificStudyRepository;
        public IScientificStudyRepository ScientificStudyRepository => _ScientificStudyRepository ??= new ScientificStudyRepository(_dbContext);
        
        private ISinaRepository _SinaRepository;
        public ISinaRepository SinaRepository => _SinaRepository ??= new SinaRepository(_dbContext);

        private ILogRepository _LogRepository;
        public ILogRepository LogRepository => _LogRepository ??= new LogRepository(_dbContext);

        private IMenuRepository _MenuRepository;
        public IMenuRepository MenuRepository => _MenuRepository ??= new MenuRepository(_dbContext);

        private IUserNotificationRepository _UserNotificationRepository;
        public IUserNotificationRepository UserNotificationRepository => _UserNotificationRepository ??= new UserNotificationRepository(_dbContext);

        private IEducationOfficerRepository _EducationOfficerRepository;
        public IEducationOfficerRepository EducationOfficerRepository => _EducationOfficerRepository ??= new EducationOfficerRepository(_dbContext);

        private IEducatorStaffInstitutionRepository _EducatorStaffInstitutionRepository;
        public IEducatorStaffInstitutionRepository EducatorStaffInstitutionRepository => _EducatorStaffInstitutionRepository ??= new EducatorStaffInstitutionRepository(_dbContext);

        private IEducatorStaffParentInstitutionRepository _EducatorStaffParentInstitutionRepository;
        public IEducatorStaffParentInstitutionRepository EducatorStaffParentInstitutionRepository => _EducatorStaffParentInstitutionRepository ??= new EducatorStaffParentInstitutionRepository(_dbContext);

        private IStudentRotationPerfectionRepository _StudentRotationPerfectionRepository;
        public IStudentRotationPerfectionRepository StudentRotationPerfectionRepository => _StudentRotationPerfectionRepository ??= new StudentRotationPerfectionRepository(_dbContext);

        private IRelatedDependentProgramRepository _RelatedDependentProgramRepository;
        public IRelatedDependentProgramRepository RelatedDependentProgramRepository => _RelatedDependentProgramRepository ??= new RelatedDependentProgramRepository(_dbContext);
        private IStandardRepository _StandardRepository;
        public IStandardRepository StandardRepository => _StandardRepository ??= new StandardRepository(_dbContext);

        private IStandardCategoryRepository _StandardCategoryRepository;
        public IStandardCategoryRepository StandardCategoryRepository => _StandardCategoryRepository ??= new StandardCategoryRepository(_dbContext);

        private IFormRepository _FormRepository;
        public IFormRepository FormRepository => _FormRepository ??= new FormRepository(_dbContext, mapper);

        private ISpecificEducationRepository _SpecificEducationRepository;
        public ISpecificEducationRepository SpecificEducationRepository => _SpecificEducationRepository ??= new SpecificEducationRepository(_dbContext);

        private IStudentsSpecificEducationRepository _StudentsSpecificEducationRepository;
        public IStudentsSpecificEducationRepository StudentsSpecificEducationRepository => _StudentsSpecificEducationRepository ??= new StudentsSpecificEducationRepository(_dbContext);

        private ISpecificEducationPlaceRepository _SpecificEducationPlaceRepository;
        public ISpecificEducationPlaceRepository SpecificEducationPlaceRepository => _SpecificEducationPlaceRepository ??= new SpecificEducationPlaceRepository(_dbContext);

        private IQuotaRequestRepository _QuotaRequestRepository;
        public IQuotaRequestRepository QuotaRequestRepository => _QuotaRequestRepository ??= new QuotaRequestRepository(_dbContext);

        private ISubQuotaRequestRepository _SubQuotaRequestRepository;
        public ISubQuotaRequestRepository SubQuotaRequestRepository => _SubQuotaRequestRepository ??= new SubQuotaRequestRepository(_dbContext);

        private IPortfolioRepository _PortfolioRepository;
        public IPortfolioRepository PortfolioRepository => _PortfolioRepository ??= new PortfolioRepository(_dbContext);

        private IStudentCountRepository _StudentCountRepository;
        public IStudentCountRepository StudentCountRepository => _StudentCountRepository ??= new StudentCountRepository(_dbContext);

        private IEducatorCountContributionFormulaRepository _EducatorCountContributionFormulaRepository;
        public IEducatorCountContributionFormulaRepository EducatorCountContributionFormulaRepository => _EducatorCountContributionFormulaRepository ??= new EducatorCountContributionFormulaRepository(_dbContext);

        private ISubQuotaRequestPortfolioRepository _SubQuotaRequestPortfolioRepository;
        public ISubQuotaRequestPortfolioRepository SubQuotaRequestPortfolioRepository => _SubQuotaRequestPortfolioRepository ??= new SubQuotaRequestPortfolioRepository(_dbContext);

        private IAnnouncementRepository _AnnouncementRepository;
        public IAnnouncementRepository AnnouncementRepository => _AnnouncementRepository ??= new AnnouncementRepository(_dbContext);

        private IENabizPortfolioRepository _ENabizPortfolioRepository;
        public IENabizPortfolioRepository ENabizPortfolioRepository => _ENabizPortfolioRepository ??= new ENabizPortfolioRepository(_dbContext);

        public void AbortTransaction()
        {
            if (transaction == null)
            {
                transaction.Dispose();
                transaction = null;
            }
        }
        public async Task AbortTransactionAsync()
        {
            if (transaction == null)
            {
                await transaction.DisposeAsync();
                transaction = null;
            }
        }

        public void BeginTransaction()
        {
            transaction = _dbContext.Database.BeginTransaction();
        }
        public async Task BeginTransactionAsync()
        {
            transaction = await _dbContext.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            //try
            //{
            //    if (transaction != null)
            //    {
            //        await transaction.CommitAsync();
            //        await transaction.DisposeAsync();
            //        transaction = null;
            //    }
            //}
            //catch (Exception)
            //{
            //    await transaction.DisposeAsync();
            //    transaction = null;

            //    throw;
            //}
        }
        public async Task EndTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                    await transaction.DisposeAsync();
                    transaction = null;
                }
            }
            catch (Exception)
            {
                await transaction.DisposeAsync();
                transaction = null;

                throw;
            }
        }
        public void Commit()
        {
            _dbContext.SaveChanges();
            try
            {
                if (transaction != null)
                {
                    transaction.Commit();
                    transaction.Dispose();
                    transaction = null;
                }
            }
            catch (Exception)
            {
                transaction.Dispose();
                transaction = null;

                throw;
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }
        }
    }
}
