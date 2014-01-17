using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.Util
{
    public static class Util
    {
        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
    }
}