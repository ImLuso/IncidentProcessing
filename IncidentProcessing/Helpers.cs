using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IncidentProcessing
{
    public static class Helpers
    {
        public static String RemoveHtml(string input)
        {
            String output = Regex.Replace(input, @"<.*?>", " ");
            output = Regex.Replace(output, "&nbsp;", " ");
            output = Regex.Replace(output, @"\r", string.Empty);
            output = Regex.Replace(output, @"\n", string.Empty);
            return output;
        }
    }
}
