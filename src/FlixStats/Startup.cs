using System;
using AutoMapper;
using FlixStats.Data;
using FlixStats.Data.Repositories;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Helper;
using FlixStats.Services;
using FlixStats.Services.Abstractions;
using FlixStats.Services.Schedule.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DaHo.Library.AspNetCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebMarkupMin.AspNetCore3;
using FluffySpoon.AspNet.LetsEncrypt;
using DaHo.Library.AspNetCore;

namespace FlixStats
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddRazorRuntimeCompilation()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new HumanizerMetadataProvider()));

            services.AddDbContext<StatsContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<INetflixViewedItemRepository, NetflixViewedItemRepository>();
            services.AddTransient<ILeaderboardRepository, LeaderboardRepository>();
            services.AddTransient<IQueryResultRepository, QueryResultRepository>();

            services.AddTransient<INetflixStatsCreator, NetflixStatsCreator>();

            services.AddTransient<INetflixViewedItemsFileExporter, NetflixViewedItemsCsvExporter>();
            services.AddTransient<INetflixViewedItemsFileExporter, NetflixViewedItemsJsonExporter>();
            services.AddTransient<INetflixViewedItemsFileExporter, NetflixViewedItemsXmlExporter>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddQuartz(typeof(DeleteOldResultsJob));

            services.AddWebMarkupMin()
                .AddHtmlMinification()
                .AddHttpCompression();

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.Preload = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            if (!_environment.IsDevelopment())
            {
                services.AddLetsEncrypt(_environment);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseFluffySpoonLetsEncrypt();
                app.UseHttpsRedirection();
                app.UseWebMarkupMin();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.MigrateDatabase<StatsContext>();
            app.UseQuartz();
        }
    }
}
