using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class ContentLine
    {
        private const string ContentLineContentPattern = "(.+?)((;.+?)*):(.+)";
        private const RegexOptions ContentLineContentRegexOptions = RegexOptions.Singleline;

        public string Name { get; }
        public string Value { get; }
        public ContentLineParameters Parameters { get; }

        public ContentLine(string source)
        {
            source = UnfoldAndUnescape(source);
            var match = Regex.Match(source, ContentLineContentPattern, ContentLineContentRegexOptions);

            // TODO Error Handling
            Name = match.Groups[1].ToString().Trim();
            Parameters = new ContentLineParameters(match.Groups[2].ToString());
            Value = match.Groups[4].ToString().Trim();
        }

        public static string UnfoldAndUnescape(string s)
        {
            var unfold = Regex.Replace(s, "(\\r\\n )", "");
            var unescaped = Regex.Unescape(unfold);
            return unescaped;
        }
    }
}