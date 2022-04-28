using System;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerPlatform.Dataverse.Client;
using TimeEntryApp;

namespace TimeEntryApp.Tests.Infrastructure
{
    public class TestHost
    {
        public TestHost()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .Build();

            var startup = new TestStartup();
            var host = new HostBuilder()
                .ConfigureWebJobs(startup.Configure)
                .ConfigureHostConfiguration(c =>
                {
                    c.AddConfiguration(config.GetSection("Values"));
                })
                .ConfigureServices(ReplaceTestOverrides)
                .Build();

            ServiceProvider = host.Services;
        }

        public IServiceProvider ServiceProvider { get; }

        private void ReplaceTestOverrides(IServiceCollection services)
        {
        }

        private class TestStartup : Startup
        {
            public override void Configure(IFunctionsHostBuilder builder)
            {
                SetExecutionContextOptions(builder);
                base.Configure(builder);
            }

            private static void SetExecutionContextOptions(IFunctionsHostBuilder builder)
            {
                builder.Services.Configure<ExecutionContextOptions>(o => o.AppDirectory = Directory.GetCurrentDirectory());
            }
        }
    }
}
