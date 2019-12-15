using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IncidentProcessing
{
    public static class ProcessData
   {
        [FunctionName("GetCaseData")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            bool bRet = true;
            string content; 

            try
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

                // Title
                string str2;
                if (num2 != -1)
                {
                    string str3 = str1.Substring(startIndex1, num2 - startIndex1);
                    str2 = str3.Substring(str3.IndexOf(":") + 1).Trim();
                }
                else
                {
                    str2 = "Incorrect Title";
                    bRet = false;
                }

                // Description
                stringList.Add(str2);
                int startIndex2 = num2 >= 0 ? num2 : 0;
                int num3 = lower.IndexOf("module:");
                string str4;
                if (num3 != -1)
                {
                    string str3 = str1.Substring(startIndex2, num3 - startIndex2);
                    str4 = str3.Substring(str3.IndexOf(":") + 1).Trim();
                }
                else
                {
                    str4 = "Incorrect Description";
                    bRet = false;
                }

                // Module
                stringList.Add(str4);
                string str5;
                if (num3 != -1)
                {
                    int startIndex3 = num3;
                    num3 = lower.IndexOf("case type:");
                    string str3 = str1.Substring(startIndex3, num3 - startIndex3).Trim();
                    str5 = str3.Substring(str3.IndexOf(":") + 1);
                    if (str5.ToUpper() != "CRM" && str5 != "datainsights" && str5 != "modernapps")
                        str5 = "ModernApps";
                }
                else
                {
                    str5 = "ModernApps";
                    bRet = false;
                }

                // Severity
                stringList.Add(str5);
                num1 = num3 >= 0 ? num3 : 0;
                int startIndex4 = lower.IndexOf("severity:");
                string str6 = str1.Substring(startIndex4);
                int startIndex5 = str6.IndexOf(":") + 1;
                string str7 = str6.Substring(startIndex5).Trim();
                stringList.Add(str7.Substring(0, 1));
                content = JsonConvert.SerializeObject((object)new ProcessData.VSTSElement()
                {
                    title = stringList[0],
                    description = stringList[1],
                    module = stringList[2],
                    severity = Convert.ToInt32(stringList[3])
                });
            }
            catch( Exception ex)
            {
                content = string.Format("Please pass a valid email template in the request body: {0}",ex.Message);
                bRet = false;
            }
            return bRet? (ActionResult)new JsonResult(content): new BadRequestObjectResult(content);
        }

        [DataContract]
        private class VSTSElement
        {
            [DataMember]
            public string title;
            [DataMember]
            public string description;
            [DataMember]
            public string module;
            [DataMember]
            public int severity;
        }
    }
}

