using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAffiliationRepository AffiliationRepository { get; }
        IAuthorizationCategoryRepository AuthorizationCategoryRepository { get; }
        ICurriculumRepository CurriculumRepository { get; }
        IRotationRepository RotationRepository { get; }
        IEducatorRepository EducatorRepository { get; }
        IExpertiseBranchRepository ExpertiseBranchRepository { get; }
        IHospitalRepository HospitalRepository { get; }
        IInstitutionRepository InstitutionRepository { get; }
        IProgramRepository ProgramRepository { get; }
        IProvinceRepository ProvinceRepository { get; }
        IStudentRepository StudentRepository { get; }
        IUniversityRepository UniversityRepository { get; }
        IAuthorizationDetailRepository AuthorizationDetailRepository { get; }
        IProfessionRepository ProfessionRepository { get; }
        IFacultyRepository FacultyRepository { get; }
        IPerfectionRepository PerfectionRepository { get; }
        IEducatorExpertiseBranchRepository EducatorExpertiseBranchRepository { get; }
        IEducatorProgramRepository EducatorProgramRepository { get; }
        IStudentRotationRepository StudentRotationRepository { get; }
        IStudentPerfectionRepository StudentPerfectionRepository { get; }
        IProtocolProgramRepository ProtocolProgramRepository { get; }
        ITitleRepository TitleRepository { get; }
        IDependentProgramRepository DependentProgramRepository { get; }
        IDocumentRepository DocumentRepository { get; }
        IThesisRepository ThesisRepository { get; }
        IPerformanceRatingRepository PerformanceRatingRepository { get; }
        IJuryRepository JuryRepository { get; }
        IOpinionFormRepository OpinionFormRepository { get; }
        IEducationTrackingRepository EducationTrackingRepository { get; }
        IProgressReportRepository ProgressReportRepository { get; }
        IAdvisorThesisRepository AdvisorThesisRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IEthicCommitteeDecisionRepository EthicCommitteeDecisionRepository { get; }
        IOfficialLetterRepository OfficialLetterRepository { get; }
        IThesisDefenceRepository ThesisDefenceRepository { get; }
        IPropertyRepository PropertyRepository { get; }
        IStudentExpertiseBranchRepository StudentExpertiseBranchRepository { get; }
        ICurriculumRotationRepository CurriculumRotationRepository { get; }
        ICurriculumPerfectionRepository CurriculumPerfectionRepository { get; }
        IPerfectionPropertyRepository PerfectionPropertyRepository { get; }
        IUserRepository UserRepository { get; }
        ICountryRepository CountryRepository { get; }
        IDemandRepository DemandRepository { get; }
        IStudyRepository StudyRepository { get; }
        IScientificStudyRepository ScientificStudyRepository { get; }
        ISinaRepository SinaRepository { get; }
        ILogRepository LogRepository { get; }
        IMenuRepository MenuRepository { get; }
        IEducatorAdministrativeTitleRepository EducatorAdministrativeTitleRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IEducationOfficerRepository EducationOfficerRepository { get; }
        IEducatorStaffInstitutionRepository EducatorStaffInstitutionRepository { get; }
        IEducatorStaffParentInstitutionRepository EducatorStaffParentInstitutionRepository { get; }
        IStudentRotationPerfectionRepository StudentRotationPerfectionRepository { get; }
        IRelatedDependentProgramRepository RelatedDependentProgramRepository { get; }
        IStandardRepository StandardRepository { get; }
        IStandardCategoryRepository StandardCategoryRepository { get; }
        IFormRepository FormRepository { get; }
        IQuotaRequestRepository QuotaRequestRepository { get; }
        ISubQuotaRequestPortfolioRepository SubQuotaRequestPortfolioRepository { get; }
        ISubQuotaRequestRepository SubQuotaRequestRepository { get; }
        IPortfolioRepository PortfolioRepository { get; }
        IEducatorCountContributionFormulaRepository EducatorCountContributionFormulaRepository { get; }
        IStudentCountRepository StudentCountRepository { get; }
        IAnnouncementRepository AnnouncementRepository { get; }
        IENabizPortfolioRepository ENabizPortfolioRepository { get; }

        Task CommitAsync(CancellationToken cancellationToken);
        void Commit();
        void BeginTransaction();
        Task BeginTransactionAsync();
        Task EndTransactionAsync(CancellationToken cancellationToken);
        void AbortTransaction();
        Task AbortTransactionAsync();
    }
}
