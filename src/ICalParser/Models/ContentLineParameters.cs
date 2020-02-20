using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class ContentLineParameters : Dictionary<string, ContentLineParameter>
    {
        private const string ParameterPattern = "([^;]+)(?=;|$)";

        public ContentLineParameters(string source)
        {
            var matches = Regex.Matches(source, ParameterPattern);
            foreach(Match match in matches)
            {
                var contentLineParameter = new ContentLineParameter(match.Groups[1].ToString());
                this[contentLineParameter.Name] = contentLineParameter;
            }
        }
    }
}