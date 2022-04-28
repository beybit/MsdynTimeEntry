using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using Newtonsoft.Json;

namespace TimeEntryApp
{
    public class TimeEntryFunction
    {
        private readonly DataverseServiceClient serviceClient;

        public TimeEntryFunction(DataverseServiceClient serviceClient)
        {
            this.serviceClient = serviceClient;
        }

        [FunctionName("time-entry")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = string.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }

            log.LogInformation("Start parse request body: {body}", requestBody);

            if(string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestResult();
            }

            TimeEntryDto timeEntry;
            try
            {
                timeEntry = JsonConvert.DeserializeObject<TimeEntryDto>(requestBody, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
                if(timeEntry == null)
                {
                    return new BadRequestResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while deserilize body");
                return new BadRequestResult();
            }

            DateTime startDate = timeEntry.StartOn.Date;
            DateTime endDate = timeEntry.EndOn.Date;

            if(startDate > endDate)
            {
                log.LogWarning("StartOn greater than EndOn");
                return new BadRequestResult();
            }


            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                serviceClient.CreateTimeEntryRecord(date);
            }

            return new OkObjectResult(timeEntry);
        }
    }
}
