using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using FluffySpoon.AspNet.LetsEncrypt;

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
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            if (!IsDevelopment())
            {
                builder.UseKestrel(kestrelOptions => kestrelOptions.ConfigureHttpsDefaults(
                    httpsOptions => httpsOptions.ServerCertificateSelector =
                        (c, s) => LetsEncryptRenewalService.Certificate));
            }

            return builder;
        }

        private static bool IsDevelopment() =>
            string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development");
    }
}
