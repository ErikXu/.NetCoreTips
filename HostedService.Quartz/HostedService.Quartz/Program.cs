using Microsoft.Extensions.Hosting;
using System.IO;
using HostedService.Quartz.Jobs;
using HostedService.Quartz.Q1.Foundation.Quartz.HostedService;
using HostedService.Quartz.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Quartz.Spi;

namespace HostedService.Quartz
{
    class Program
    {
        static void Main()
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    //configHost.AddJsonFile("hostsettings.json", true, true);
                    configHost.AddEnvironmentVariables("ASPNETCORE_");
                    //configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                    configApp.AddEnvironmentVariables();
                    //configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    //services.AddHostedService<TimedHostedService>();

                    services.AddSingleton<IJobFactory, JobFactory>();
                    services.AddSingleton(provider =>
                    {
                        var option = new QuartzOption(hostContext.Configuration);
                        var sf = new StdSchedulerFactory(option.ToProperties());
                        var scheduler = sf.GetScheduler().Result;
                        scheduler.JobFactory = provider.GetService<IJobFactory>();
                        return scheduler;
                    });
                    services.AddHostedService<QuartzService>();

                    services.AddSingleton<TestJob, TestJob>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    if (hostContext.HostingEnvironment.EnvironmentName == EnvironmentName.Development)
                    {
                        configLogging.AddDebug();
                    }
                })
                .UseConsoleLifetime()
                .Build();

            host.Run();
        }
    }
}
