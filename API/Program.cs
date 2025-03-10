using Application.DependencyResolvers.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure.Data;
using Koru.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using ChustaSoft.Tools.SecureConfig;
using Application.Models;
using AspNetCoreRateLimit;

namespace API
{
    public class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            //AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

            using (var scope = host.Services.CreateScope())
            {
                //var services = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await DataSeeder.SeedAsync(context);
                await KoruDataSeeder.SeedAsync(context);

                // get the ClientPolicyStore instance
                //var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
                // seed Client data from appsettings
                //await clientPolicyStore.SeedAsync();
            }
            host.EncryptSettings<AppSettings>(false);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new AutofacBusinessModule());
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Add the following line:
                    webBuilder.UseSentry(o =>
                    {
                        o.Dsn = "https://e1f0d12cace742aab6704af467cd0e15@buggerv2.saglik.gov.tr//65";
                        // When configuring for the first time, to see what the SDK is doing:
                        o.Debug = true;
                        // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
                        // We recommend adjusting this value in production.
                        o.TracesSampleRate = 1.0;
                    });
                    
                    webBuilder.UseStartup<Startup>();
                });
    }
}