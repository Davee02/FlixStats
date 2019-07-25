﻿using System;
using AutoMapper;
using FlixStats.Data;
using FlixStats.Data.Repositories;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Services;
using FlixStats.Services.Abstractions;
using FlixStats.Services.Schedule;
using FlixStats.Services.Schedule.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlixStats
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddRazorRuntimeCompilation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new HumanizerMetadataProvider()));

            services.AddDbContext<StatsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<INetflixViewedItemRepository, NetflixViewedItemRepository>();

            services.AddTransient<INetflixStatsCreator, NetflixStatsCreator>();

            services.AddTransient<INetflixViewedItemsFileExporter, NetflixViewedItemsCsvExporter>();
            services.AddTransient<INetflixViewedItemsFileExporter, NetflixViewedItemsJsonExporter>();
            services.AddTransient<INetflixViewedItemsFileExporter, NetflixViewedItemsXmlExporter>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.Add(
                new ServiceDescriptor(
                    typeof(IActionResultExecutor<JsonResult>),
                    Type.GetType("Microsoft.AspNetCore.Mvc.Infrastructure.SystemTextJsonResultExecutor, Microsoft.AspNetCore.Mvc.Core"),
                    ServiceLifetime.Singleton));

            services.AddQuartz(typeof(DeleteOldResultsJob));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseQuartz();
        }
    }
}