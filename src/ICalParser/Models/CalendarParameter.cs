using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class CalendarParameter
    {
        private const string NameValuePattern = "(.+?):(.+)";

        public string Name { get; set; }
        public string Value { get; set; }

        public CalendarParameter(string source)
        {
            var unfold = ContentLine.UnfoldAndUnescape(source);
            var nameValueMatch = Regex.Match(unfold, NameValuePattern);
            Name = nameValueMatch.Groups[1].ToString().Trim();
            Value = nameValueMatch.Groups[2].ToString().Trim();
        }
    }
}