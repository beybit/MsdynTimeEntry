using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(TimeEntryApp.Startup))]
namespace TimeEntryApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ServiceClient>((s) => {
                var configuration = s.GetRequiredService<IConfiguration>();
                return new ServiceClient(configuration.GetValue<string>("DataverseServiceConnectionString"));
            });

            builder.Services.AddSingleton<DataverseServiceClient>((s) => {
                var serviceClient = s.GetRequiredService<ServiceClient>();
                var dataverseServiceClient = new DataverseServiceClient(serviceClient);
                return dataverseServiceClient;
            });

            var sp = builder.Services.BuildServiceProvider();
            var dataverseServiceClient = sp.GetRequiredService<DataverseServiceClient>();
            dataverseServiceClient.ConfgiureTimeEntryDuplicationRules();
        }
    }
}
