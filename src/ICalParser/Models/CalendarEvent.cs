using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class CalendarEvent : IEquatable<CalendarEvent>
    {
        private const string EventContentPattern = "BEGIN:VEVENT\\r\\n(.+)\\r\\nEND:VEVENT";
        private const RegexOptions EventContentRegexOptions = RegexOptions.Singleline;
        private const string ContentLinePattern = "(.+?):(.+?)(?=\\r\\n[A-Z]|$)";
        private const RegexOptions ContentLineTRegexOptions = RegexOptions.Singleline;

        private Dictionary<string, ContentLine> ContentLines { get; }

        public string Uid { get; }
        public DateTime DtStart { get; }
        public DateTime DtEnd { get; }

        public string Description { get; }
        public string Location { get; }

        public string Summary { get; }

        public CalendarEvent(string source)
        {
            var contentMatch = Regex.Match(source, EventContentPattern, EventContentRegexOptions);
            var content = contentMatch.Groups[1].ToString();
            var matches = Regex.Matches(content, ContentLinePattern, ContentLineTRegexOptions);

            ContentLines = new Dictionary<string, ContentLine>();
            foreach(Match match in matches)
            {
                var contentLineString = match.Groups[0].ToString();
                var contentLine = new ContentLine(contentLineString);
                ContentLines[contentLine.Name] = contentLine;
            }

            // 20200303T113000Z
            // 2020 03 03 T 11 30 00 Z
            Uid = ContentLines["UID"].Value;
            DtStart = DateTime.ParseExact(ContentLines["DTSTART"].Value, "yyyyMMddTHHmmssZ", CultureInfo.CurrentCulture);
            DtEnd = DateTime.ParseExact(ContentLines["DTEND"].Value, "yyyyMMddTHHmmssZ", CultureInfo.CurrentCulture);
            Description = ContentLines["DESCRIPTION"].Value
                                                     .Replace("\r\n\tn", "\n")
                                                     .Replace("\r\n\t", string.Empty);
            Location = ContentLines["LOCATION"].Value;
            Summary = ContentLines["SUMMARY"].Value;
        }

        public bool Equals(CalendarEvent other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return DtStart.Equals(other.DtStart) && Description == other.Description && Location == other.Location &&
                   Summary == other.Summary;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DtStart, Description, Location, Summary);
        }
    }
}