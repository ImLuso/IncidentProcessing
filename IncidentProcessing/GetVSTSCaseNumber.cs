using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IncidentProcessing
{
    public static class GetVSTSCaseNumber
    {
        [FunctionName("GetVSTSCaseNumber")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string content;
            bool bRet = true;

            try
            {
                string VSTSCase = req.Query["VSTSCaseNumber"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                VSTSCase = VSTSCase ?? data?.name;

                string temp = VSTSCase.Substring(VSTSCase.IndexOf("VSTS#") + 5);
                VSTSCase = temp.Substring(0, temp.IndexOf(" "));

                content = JsonConvert.SerializeObject((object)new GetVSTSCaseNumber.VSTSNumber()
                {
                    number = VSTSCase
                });
            }
            catch (Exception ex)
            {
                content = string.Format("Please pass a valid email template in the request body: {0}", ex.Message);
                bRet = false;
            }
            return bRet ? (ActionResult)new JsonResult(content) : new BadRequestObjectResult(content);
        }

        [DataContract]
        private class VSTSNumber
        {
            [DataMember]
            public string number;
        }
    }
}
