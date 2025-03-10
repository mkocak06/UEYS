using AutoMapper;
using Fluxor;
using Majorsoft.Blazor.Components.Common.JsInterop;
using Majorsoft.Blazor.Components.CssEvents;
using Majorsoft.Blazor.Components.GdprConsent;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using UI.Models;
using UI.Services;

namespace UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services

               .AddScoped<IHttpService, HttpService>()
               .AddScoped<Services.ILocalStorageService, Services.LocalStorageService>()
               .AddScoped<ISweetAlert, SweetAlert>()
               .AddScoped<IIconHelperService, IconHelperService>()
               .AddScoped<IAuthenticationService, AuthenticationService>()
               .AddScoped<IAuthService, AuthService>()
               .AddScoped<IAffiliationService, AffiliationService>()
               .AddScoped<ICurriculumService, CurriculumService>()
               .AddScoped<IUniversityService, UniversityService>()
               .AddScoped<IProgramService, ProgramService>()
               .AddScoped<IProtocolProgramService, ProtocolProgramService>()
               .AddScoped<IHospitalService, HospitalService>()
               .AddScoped<IProfessionService, ProfessionService>()
               .AddScoped<IFacultyService, FacultyService>()
               .AddScoped<IAuthorizationCategoryService, AuthorizationCategoryService>()
               .AddScoped<IAuthorizationDetailService, AuthorizationDetailService>()
               .AddScoped<IExpertiseBranchService, ExpertiseBranchService>()
               .AddScoped<IProvinceService, ProvinceService>()
               .AddScoped<IEducatorDependentProgramService, EducatorDependentProgramService>()
               .AddScoped<IInstitutionService, InstitutionService>()
               .AddScoped<IEducatorService, EducatorService>()
               .AddScoped<IStudentPerfectionService, StudentPerfectionService>()
               .AddScoped<IStudentService, StudentService>()
               .AddScoped<IThesisService, ThesisService>()
               .AddScoped<IRotationService, RotationService>()
               .AddScoped<ITitleService, TitleService>()
               .AddScoped<IUserService, UserService>()
               .AddScoped<IPerfectionService, PerfectionService>()
               .AddScoped<IStudentRotationService, StudentRotationService>()
               .AddScoped<IProtocolProgramService, ProtocolProgramService>()
               .AddScoped<IDependentProgramService, DependentProgramService>()
               .AddScoped<ISvgService, SvgService>()
               .AddScoped<IDocumentService, DocumentService>()
               .AddScoped<IOpinionService, OpinionService>()
               .AddScoped<IAdvisorThesisService, AdvisorThesisService>()
               .AddScoped<IEducationTrackingService, EducationTrackingService>()
               .AddScoped<ICurriculumPerfectionService, CurriculumPerfectionService>()
               .AddScoped<ICurriculumRotationService, CurriculumRotationService>()
               //.AddScoped<IReasonService, ReasonService>()
               .AddScoped<IEducatorProgramService, EducatorProgramService>()
               .AddScoped<IStudentExpertiseBranchService, StudentExpertiseBranchService>()
               .AddScoped<IPropertyService, PropertyService>()
               .AddScoped<IFileUploadService, FileUploadService>()
               .AddScoped<IOfficialLetterService, OfficialLetterService>()
               .AddScoped<IEthicCommitteeService, EthicCommitteeService>()
               .AddScoped<IEducatorExpertiseBranchService, EducatorExpertiseBranchService>()
               .AddScoped<IProgressReportService, ProgressReportService>()
               .AddScoped<IThesisDefenceService, ThesisDefenceService>()
               .AddScoped<IScientificStudyService, ScientificStudyService>()
               .AddScoped<IStudyService, StudyService>()
               .AddScoped<IExitExamService, ExitExamService>()
               .AddScoped<ICaptchaService, CaptchaService>()
               .AddScoped<ILogService, LogService>()
               .AddScoped<IMenuService, MenuService>()
               .AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly))
               .AddLocalization(o => o.ResourcesPath = "Resources")
               .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["apiUrl"]) })
               .AddScoped<IPerformanceRatingService, PerformanceRatingService>()
               .AddScoped<ICountryService, CountryService>()
               .AddScoped<IStudentRotationPerfectionService, StudentRotationPerfectionService>()
               .AddScoped<IEducationOfficerService, EducationOfficerService>()
               .AddScoped<IRelatedDependentProgramService, RelatedDependentProgramService>()
               .AddScoped<IStudentDependentProgramService, StudentDependentProgramService>()
               .AddScoped<IAuditFormService, AuditFormService>()
               .AddScoped<IStandardService, StandardService>()
               .AddScoped<IStandardCategoryService, StandardCategoryService>()
               .AddScoped<ISpecificEducationService, SpecificEducationService>()
               .AddScoped<ISpecificEducationPlaceService, SpecificEducationPlaceService>()
               .AddScoped<IStudentSpecificEducationService, StudentSpecificEducationService>()
               .AddScoped<IPortfolioService, PortfolioService>()
               .AddScoped<ISubQuotaRequestService, SubQuotaRequestService>()
               .AddScoped<IQuotaRequestService, QuotaRequestService>()
               .AddScoped<IAnnouncementService, AnnouncementService>()
               .AddScoped<IStudentCountService, StudentCountService>()
               .AddScoped<IENabizPortfolioService, ENabizPortfolioService>()
               .AddScoped<IEducatorCountContributionFormulaService, EducatorCountContributionFormulaService>()
               .AddScoped<ISpecialistDoctorService, SpecialistDoctorService>();

            //Register dependencies
            builder.Services.AddCssEvents();
            builder.Services.AddJsInteropExtensions();
            builder.Services.AddBrowserStorage();
            builder.Services.AddGdprConsent();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);


            // Begin I18N configuration
            var jsInterop = builder.Services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
            try
            {
                var appLanguage = await jsInterop.InvokeAsync<string>("appCulture.get");
                if (string.IsNullOrEmpty(appLanguage))
                {
                    appLanguage = "tr-TR";
                }
                var cultureInfo = new CultureInfo(appLanguage);
                await jsInterop.InvokeVoidAsync("setLanguage", cultureInfo.TwoLetterISOLanguageName);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }
            catch (Exception)
            {
                // ignored
            }

            var host = builder.Build();
            var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
            await authenticationService.Initialize();

            await host.RunAsync();
        }
    }
}