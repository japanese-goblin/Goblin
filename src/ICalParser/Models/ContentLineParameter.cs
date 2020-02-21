using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class ContentLineParameter
    {
        private const string NameValuePattern = "(.+?)=(.+)";
        private const string ValueListPattern = "([^,]+)(?=,|$)";

        public string Name { get; }
        public List<string> Values { get; }

        public ContentLineParameter(string source)
        {
            Values = new List<string>();

            var match = Regex.Match(source, NameValuePattern);
            Name = match.Groups[1].ToString().Trim();
            var valueString = match.Groups[2].ToString();
            var matches = Regex.Matches(valueString, ValueListPattern);
            foreach(Match paramMatch in matches)
            {
                Values.Add(paramMatch.Groups[1].ToString().Trim());
            }
        }
    }
}