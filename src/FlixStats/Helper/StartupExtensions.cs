using System;
using System.IO;
using Certes;
using FlixStats.Services.Schedule;
using FlixStats.Services.Schedule.Jobs;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.AspNet.LetsEncrypt.Certes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace FlixStats.Helper
{
    public static class StartupExtensions
    {
        public static void AddLetsEncrypt(this IServiceCollection services, IWebHostEnvironment hostEnvironment)
        {
            services.AddFluffySpoonLetsEncrypt(new LetsEncryptOptions
            {
                Email = "davidhodel6@gmail.com",
                UseStaging = false,
                Domains = new[] { "www.flixstats.com" },
                TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30),
                CertificateSigningRequest = new CsrInfo
                {
                    CountryName = "CH",
                    Locality = "Lucerne",
                    State = "Lucerne"
                }
            });

            string certDir = Path.Combine(hostEnvironment.ContentRootPath, "letsencrypt");
            Directory.CreateDirectory(certDir);

            //the following line tells the library to persist the certificate to a file, so that if the server restarts, the certificate can be re-used without generating a new one.
            services.AddFluffySpoonLetsEncryptFileCertificatePersistence(Path.Combine(certDir, "FluffySpoonAspNetLetsEncryptCertificate"));

            //the following line tells the library to persist challenges in-memory. challenges are the "/.well-known" URL codes that LetsEncrypt will call.
            services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();
        }

        public static void AddQuartz(this IServiceCollection services, Type jobType)
        {
            services.Add(new ServiceDescriptor(typeof(IJob), jobType, ServiceLifetime.Transient));
            services.AddSingleton<IJobFactory, ScheduledJobFactory>();
            services.AddSingleton<IJobDetail>(provider => JobBuilder.Create<DeleteOldResultsJob>()
                .WithIdentity("DeleteOldResults.Job", "DeleteOldResultsSchedule")
                .Build());

            services.AddSingleton<ITrigger>(provider =>
            {
                return TriggerBuilder.Create()
                    .WithIdentity($"DeleteOldResults.Trigger", "DeleteOldResultsSchedule")
                    .StartNow()
                    .WithSimpleSchedule
                    (s =>
                        s.WithInterval(TimeSpan.FromMinutes(5))
                            .RepeatForever()
                    )
                    .Build();
            });

            services.AddSingleton<IScheduler>(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();
                return scheduler;
            });

        }

        public static void UseQuartz(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<IScheduler>()
                .ScheduleJob(app.ApplicationServices.GetService<IJobDetail>(),
                    app.ApplicationServices.GetService<ITrigger>()
                );
        }
    }
}
