using System;
using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
using Microsoft.Extensions.DependencyInjection;

namespace FlixStats.Services.Schedule
{
    public static class FluffySpoonLetsEncryptExtensions
    {
        public static void AddLetsEncrypt(this IServiceCollection services)
        {
            services.AddFluffySpoonLetsEncryptRenewalService(new LetsEncryptOptions
            {
                Email = "davidhodel6@gmail.com",
                UseStaging = false,
                Domains = new[] { "flixstats.com", "www.flixstats.com" },
                TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30),
                TimeAfterIssueDateBeforeRenewal = TimeSpan.FromDays(7),
                CertificateSigningRequest = new CsrInfo
                {
                    CountryName = "CH",
                    Locality = "Lucerne",
                    State = "Lucerne"

                }
            });

            //the following line tells the library to persist the certificate to a file, so that if the server restarts, the certificate can be re-used without generating a new one.
            services.AddFluffySpoonLetsEncryptFileCertificatePersistence();

            //the following line tells the library to persist challenges in-memory. challenges are the "/.well-known" URL codes that LetsEncrypt will call.
            services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();
        }
    }
}
