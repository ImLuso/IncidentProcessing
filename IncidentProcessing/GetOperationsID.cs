using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IncidentProcessing
{
    public static class GeOperationsID
    {/*
        [FunctionName("GeOperationsID")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string str1 = Helpers.RemoveHtml(requestBody);
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //string str1 = data.ToString();

            string lower = str1.ToLower();
            List<string> stringList = new List<string>();
            int num1 = 0;
            int startIndex1 = lower.IndexOf("title:");
            int num2 = lower.IndexOf("description:");
            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }*/
    }
}
