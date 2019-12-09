using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace IncidentProcessing
{
    public static class GetVSTSState
    {
        [FunctionName("GetVSTSState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string content;
            bool bRet = true;

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                int iStart = requestBody.IndexOf("State") + 6;
                int iEnd = requestBody.IndexOf("Status");

                string state = requestBody.Substring(iStart, iEnd - iStart);
                string status = requestBody.Substring(iEnd + 7);

                content = JsonConvert.SerializeObject((object)new GetVSTSState.VSTSCaseState()
                {
                    state = state,
                    status = status
                });
            }
            catch (Exception ex)
            {
                content = string.Format("Please pass a valid email template in the request body: {0}-", ex.Message);
                bRet = false;
            }
            return bRet ? (ActionResult)new JsonResult(content) : new BadRequestObjectResult(content);
        }

        [DataContract]
        private class VSTSCaseState
        {
            [DataMember]
            public string state;
            [DataMember]
            public string status;
        }
    }
}
