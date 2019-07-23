﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetflixStatizier.Services.Schedule.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace NetflixStatizier.Services.Schedule
{
    public static class QuartzExtensions
    {
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
