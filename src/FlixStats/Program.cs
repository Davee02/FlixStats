using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace FlixStats
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';');

            return WebHost.CreateDefaultBuilder()
                .UseKestrel()
                .UseUrls(urls)
                .UseStartup<Startup>();
        }
    }
}
