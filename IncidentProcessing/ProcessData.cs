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
using System.Diagnostics;

namespace IncidentProcessing
{
    public class errData
    {
        public bool hasErrors;
        public Dictionary<string, bool> errModules; 
        public errData()
        {
            hasErrors = false;
            errModules = new Dictionary<string, bool>();
        }
    }
    public static class ProcessData
   {
        [FunctionName("GetCaseData")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
 
            bool bRet = true;
            string content = String.Empty;
            try
            {
                errData errData = new errData();

                VSTSElement VSTSElement = new VSTSElement();
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                const string SEP = "§§";
                string[] tags = new string[] { "title:", "description:", "module:", "severity:", "case type:"};
                string temp;
                string errModule = string.Empty;
                List<string> tagContent = new List<string>();

                EmailData eData = JsonConvert.DeserializeObject<EmailData>(requestBody);
                string body = Helpers.RemoveHtml(eData.emailbody).ToLower();
                List<string> stringList = new List<string>();

                //Verify all the required items are specified
                foreach (string item in tags)
                {
                    if (body.IndexOf(item) == -1)
                    {
                        errData.hasErrors = true;
                        errData.errModules.Add(item, true);
                    }
                    else
                    {
                        errData.errModules.Add(item, false);
                        temp = body.Replace(item, SEP + item);
                        body = temp;
                    }
                }

                if (!errData.hasErrors)
                {
                    // Split all the elements into strings
                    string[] split = body.Split(SEP, StringSplitOptions.None);

                    // Generate response
                    foreach (string item in split)
                    {
                        if (item != String.Empty)
                        {
                            string[] tagsSplitted = item.Split(":", StringSplitOptions.None);
                            switch (tagsSplitted[0])
                            {
                                case "title":
                                    VSTSElement.title = tagsSplitted[1]; break;
                                case "description":
                                    VSTSElement.description = tagsSplitted[1]; break; 
                                case "module":
                                    VSTSElement.module = tagsSplitted[1].Trim() == "datainsights" ? "DataInsights" : "ModernApps"; break;
                                case "severity":
                                    VSTSElement.severity = Convert.ToInt32(tagsSplitted[1]); break;
                            }
                        }
                    }
                    content = JsonConvert.SerializeObject(VSTSElement);
                }
                else
                {
                    bRet = false;
                    content = "Required tags not specified: ";
                    foreach (var item in errData.errModules)
                    {
                        content += (item.Value ? item.Key : "") + " ";
                    }
                }
            }
            catch (Exception ex)
            {
                content = string.Format(" - ERROR MESSAGE: {0} - {1}", ex.Message, ex.StackTrace);
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


        [DataContract]
        private class EmailData
        {
            [DataMember]
            public string emailbody;
        }
    }
}

