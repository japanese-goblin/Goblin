using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class Calendar
    {
        private const string EventPattern = "(BEGIN:VEVENT.+?END:VEVENT)";
        private const RegexOptions EventRegexOptions = RegexOptions.Singleline;

        public List<CalendarEvent> Events { get; }

        public Calendar(string source)
        {
            Events = new List<CalendarEvent>();
            
            foreach(Match vEventMatch in Regex.Matches(source, EventPattern, EventRegexOptions))
            {
                var vEventString = vEventMatch.Groups[1].ToString();
                Events.Add(new CalendarEvent(vEventString));
            }
        }
    }
}