using Application.Attributes;
using Application.Extentsions;
using Application.Interfaces;
using Application.Mapper;
using Application.Middlewares;
using Application.Models;
using Application.Services;
using AutoMapper;
using ChustaSoft.Tools.SecureConfig;
using Core.Interfaces;
using Core.Models.ConfigModels;
using Core.UnitOfWork;
using Elastic.Apm.NetCoreAll;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Infrastructure.Data;
using Infrastructure.EkipData;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Koru;
using Koru.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SixLabors.ImageSharp;
using SixLaborsCaptcha.Mvc.Core;
using System.Reflection;
using System.Text;

namespace Application
{
    public partial class BusinessStartup
    {
        private AppSettingsModel appSettingModel;
        public IConfiguration Configuration { get; }

        protected IHostEnvironment env { get; }
        public BusinessStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            env = hostEnvironment;
            Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(Configuration)
                 .Enrich.FromLogContext()
                 .CreateLogger();

            //Logging.ClearProviders();
            //Logging.AddSerilog(logger);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <remarks>
        /// It is common to all configurations and must be called. Aspnet core does not call this method because there are other methods.
        /// </remarks>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var apikey = "UeysPrivateKey-ecThWwGUsgXW57bu1.725170825";
            var appSettings = services.SetUpSecureConfig<AppSettings>(Configuration, apikey);
            appSettingModel = appSettings.GetSettingModel();
            //localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddHttpClient("ssoopenid", c =>
            {
                c.BaseAddress = new Uri(appSettingModel.SSO.OpenIdServer);

            });

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Çerezler yalnızca HTTPS üzerinden gönderilir
            //    options.Cookie.HttpOnly = true; // Çerezlere JavaScript ile erişim engellenir
            //    options.Cookie.SameSite = SameSiteMode.Strict; // CSRF saldırılarına karşı koruma
            //});

            // Necessary for Log System
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpContextAccessor();

            //for hangfire
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(appSettings.ConnectionStrings["HangfireConnection"],
                    new PostgreSqlStorageOptions { InvisibilityTimeout = TimeSpan.FromHours(5) }));
            services.AddHangfireServer();

            services.AddSingleton(appSettingModel.TokenOptions);
            //services.Configure<Models.TokenOptions>(Configuration.GetSection("TokenOptions"));
            //var token = Configuration.GetSection("TokenOptions").Get<Models.TokenOptions>();

            //JWT authorization settings
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new() //used for token validation
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettingModel.TokenOptions.Secret)),
                    ValidIssuer = appSettingModel.TokenOptions.Issuer,
                    ValidAudience = appSettingModel.TokenOptions.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                //this is called on successfull validation, for authorization, if you don't need authorization comment following line
                o.EventsType = typeof(UserValidationEvent);
            });

            //authorization required for all controllers
            services.AddControllers(o =>
            {
                //o.AddApplicationLogging();
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                if (env.IsDevelopment())
                {
                    o.Filters.Add<AllowAnonymousFilter>();          // Authentication DISABLED for development purposes
                }
                else
                    o.Filters.Add(new AuthorizeFilter(policy));

                o.Filters.Add<ValidationFilter>();//fluent validation

            }).AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
            .AddFluentValidation(configuration => configuration
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.LoginValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.AuthorizationCategoryDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.ExpertiseBranchDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.ProfessionDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.HospitalDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.InstitutionDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.ProgramDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.ProvinceDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.UniversityDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.UserResponseEducatorValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.AffiliationDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.CurriculumDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.PerfectionDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.RotationDTOValidator>()
                .RegisterValidatorsFromAssemblyContaining<Shared.Validations.TitleDTOValidator>()
                )
            .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);


            services.AddScoped<UserValidationEvent>();

            //automapper configuration
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            //mail
            //EmailConfiguration emailConfig = new()
            //{
            //    From = appSetting.EmailConfiguration["From"],
            //    Password = appSetting.EmailConfiguration["Password"],
            //    Port = Convert.ToInt32(appSetting.EmailConfiguration["Port"]),
            //    SmtpServer = appSetting.EmailConfiguration["SmtpServer"],
            //    UserName = appSetting.EmailConfiguration["Username"]
            //};
            services.AddSingleton(appSettingModel.EmailConfiguration);
            services.AddSingleton(appSettingModel.Environment);

            //var appSettingsModel = Configuration
            //  .GetSection("Settings")
            //  .Get<AppSettingsModel>();
            services.AddSingleton(appSettingModel);

            //razor render
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN"; // CSRF token header'ı
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(appSettings.ConnectionStrings["DefaultConnection"]));
            services.AddDbContext<EkipDbContext>(options => options.UseNpgsql(appSettings.ConnectionStrings["EkipConnection"]));
            services.AddScoped<DbContext, ApplicationDbContext>(); //required to access from Koru project

            //koru authorization 
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            //general dependincy injections
            services.AddScoped<IJWTTokenService, JWTTokenService>();

            //services.AddHealthChecks();
            services.AddEndpointsApiExplorer();
            //Swagger Configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //services.AddAWSService<IAmazonS3>(Configuration.GetAWSOptions());

            services.AddSixLabCaptcha(x =>
            {
                x.DrawLines = 0;
                x.TextColor = new SixLabors.ImageSharp.Color[] { Color.Black };
                x.NoiseRate = 800;
                x.DrawLinesColor = new Color[] { Color.Black };
                //x.FontFamilies = new string[] { "Marlboro" };
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IBackgroundJobService, BackgroundJobService>();

            services.AddOptions();
            //services.AddDistributedMemoryCache();

            //services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            //services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            //services.AddInMemoryRateLimiting();
            //services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISMSSender, SMSSender>();
            services.AddScoped<IKoruRepository, KoruRepository>();
            services.AddScoped<ICKYSService, CKYSService>();
            services.AddScoped<IMaterializedViewService, MaterializedViewService>();

            services.AddAntiforgery(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", policy =>
                {
                    policy.WithOrigins(appSettingModel.Environment.URL)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });

                options.AddPolicy("ExternalIntegrationPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var recurringJobManager = services.BuildServiceProvider().GetRequiredService<IRecurringJobManager>();
            var backJob = services.BuildServiceProvider().GetRequiredService<IBackgroundJobService>();
            //pasife alınacak kullanıcılar için
            recurringJobManager.AddOrUpdate("Create User Check Job", () => backJob.CheckNotLoggedUsers(), Cron.Daily(4));
            recurringJobManager.AddOrUpdate("Sina Table Update Job", () => backJob.UpdateSinaTable(), Cron.Daily);
            //educatorType'ı notInstructor'a dönen kişilerin düzeltimesi için
            recurringJobManager.AddOrUpdate("Educator Type Update Job", () => backJob.UpdateEducatorType(), Cron.Daily);
            //recurringJobManager.AddOrUpdate("User Passive Update Job", () => backJob.MakeUserPassive(), Cron.Hourly());
            recurringJobManager.AddOrUpdate("Student Status Update Job", () => backJob.CheckExpiredStudents(), Cron.Daily());
            recurringJobManager.AddOrUpdate("Educator Program Update Job", () => backJob.CheckEducatorProgramsEndDate(), Cron.Daily());
            recurringJobManager.AddOrUpdate("Educator Program Warning Job", () => backJob.CheckEducatorProgramsEndDateSendWarning(), Cron.Daily());
            recurringJobManager.AddOrUpdate("Specialist Student Job", () => backJob.CheckSpecialistStudents(), Cron.Weekly(DayOfWeek.Friday));
            if (appSettingModel.Environment.Domain == "prod")
            {
                recurringJobManager.AddOrUpdate("Daily Log Info Job", () => backJob.SystemLogInformation(), Cron.Daily(5));
                recurringJobManager.AddOrUpdate("TCKN View Update Job", () => backJob.UpdateMvTcknViewAsync(), () => "0 3 * * *");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
            app.UseAllElasticApm(Configuration);

            #region localization
            var supportedCultures = new[] { "en", "tr" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

            app.UseRequestLocalization(localizationOptions);
            #endregion


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new AllowAllConnectionsFilter() },
                IgnoreAntiforgeryToken = true
            });
            app.UseHttpsRedirection();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
            app.Use(async (context, next) =>
            {
                var allowedOrigin = appSettingModel.Environment.URL;
                var origin = context.Request.Headers["Origin"].ToString();
                var referer = context.Request.Headers["Referer"].ToString();

                if (context.Request.Path.StartsWithSegments("/api/OSYM") || context.Request.Path.StartsWithSegments("/api/ENabiz"))
                {
                    await next();
                    return;
                }

                if ((!string.IsNullOrEmpty(origin) && origin != allowedOrigin) ||
                    (!string.IsNullOrEmpty(referer) && !referer.StartsWith(allowedOrigin)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Unauthorized request.");
                    return;
                }

                var userAgent = context.Request.Headers["User-Agent"].ToString();
                if (userAgent.Contains("Postman") || userAgent.Contains("Swagger"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Unauthorized request from blocked client.");
                    return;
                }

                await next();
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseRouting();

            app.UseSentryTracing();
            app.UseCors("AllowedOrigins");

            app.UseAuthorization();

            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            //app.UseClientRateLimiting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
        private class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                //var httpContext = context.GetHttpContext();
                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                //return httpContext.User.Identity is {IsAuthenticated: true};
                return true;
            }
        }
    }
}
