using idb.Backend.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace idb.Backend
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    ISentryEnviormentProvider sentryEnv = new EnvironmentProvider();
                    webBuilder.UseStartup<Startup>();
                    if(sentryEnv.SentryIntegration)
                        webBuilder.UseSentry(sentry => {
                            sentry.Dsn = sentryEnv.SentryDns;
                            sentry.TracesSampleRate = sentryEnv.SentryTraceSampleRate;
                        });
                });
    }
}
