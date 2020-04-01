using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace FlixStats
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';') ?? new[] { "http://*", "https://*"};

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel()
                        .UseUrls(urls)
                        .UseStartup<Startup>();
                });
        }
    }
}
