using FluffySpoon.AspNet.LetsEncrypt;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FlixStats
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("https://flixstats.com", "http://flixstats.com", "https://www.flixstats.com", "http://www.flixstats.com")
                .UseKestrel(kestrelOptions => kestrelOptions.ConfigureHttpsDefaults(
                    httpsOptions => httpsOptions.ServerCertificateSelector =
                        (c, s) => LetsEncryptRenewalService.Certificate));
    }
}
