using System.Linq;
using System.Text.RegularExpressions;

namespace ICalParser.Models
{
    public class ContentLine
    {
        public string Name { get; }
        public string Value { get; }

        public ContentLine(string source)
        {
            source = UnfoldAndUnescape(source);
            var data = source.Split(':', 2)
                             .Select(x => x.Trim())
                             .ToArray();
            
            Name = data[0];
            Value = data[1];
        }

        private static string UnfoldAndUnescape(string s)
        {
            var unfold = Regex.Replace(s, "(\\r\\n)", "");
            var unescaped = Regex.Unescape(unfold);
            
            return unescaped.Replace("\tn", "\n")
                            .Replace("\t", string.Empty);
        }
    }
}