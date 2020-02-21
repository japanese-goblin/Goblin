using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class Calendar
    {
        private const string CalendarParameterPattern = "BEGIN:VCALENDAR\\r\\n(.+?)\\r\\nBEGIN:VEVENT";
        private const RegexOptions CalendarParameterRegexOptions = RegexOptions.Singleline;

        private const string EventPattern = "(BEGIN:VEVENT.+?END:VEVENT)";
        private const RegexOptions EventRegexOptions = RegexOptions.Singleline;

        public string Source { get; }
        public CalendarParameters Parameters { get; }
        public List<CalendarEvent> Events { get; } = new List<CalendarEvent>();

        public Calendar(string source)
        {
            Source = source;

            var parameterMatch = Regex.Match(source, CalendarParameterPattern, CalendarParameterRegexOptions);
            var parameterString = parameterMatch.Groups[1].ToString();
            Parameters = new CalendarParameters(parameterString);

            foreach(Match vEventMatch in Regex.Matches(source, EventPattern, EventRegexOptions))
            {
                var vEventString = vEventMatch.Groups[1].ToString();
                Events.Add(new CalendarEvent(vEventString));
            }
        }
    }
}