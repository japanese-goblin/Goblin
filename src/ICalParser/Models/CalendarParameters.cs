using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class CalendarParameters : Dictionary<string, CalendarParameter>
    {
        private const string ParameterPattern = "(.+?):(.+?)(?=\\r\\n[A-Z]|$)";
        private const RegexOptions ParameteRegexOptions = RegexOptions.Singleline;

        public CalendarParameters(string source)
        {
            var parameterMatches = Regex.Matches(source, ParameterPattern, ParameteRegexOptions);
            foreach(Match parameterMatch in parameterMatches)
            {
                var parameterString = parameterMatch.Groups[0].ToString();
                var calendarParameter = new CalendarParameter(parameterString);
                this[calendarParameter.Name] = calendarParameter;
            }
        }
    }
}