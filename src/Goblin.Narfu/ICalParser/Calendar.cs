using System.Globalization;
using System.Text.RegularExpressions;

namespace Goblin.Narfu.ICalParser;

public partial class Calendar(string source)
{
    public CalendarEvent[] Events { get; } = GetEvents(source).ToArray();

    private static Dictionary<string, string> ParseVEventFields(string source)
    {
        var regex = EventFieldsRegex();

        var eventDictionary = new Dictionary<string, string>();
        var lastAddedKey = "";

        source = source.Replace("\r\n", "\n")
                       .Replace("\n\t", string.Empty)
                       .Replace("\\,", ",");

        foreach(var line in source.Split("\n", StringSplitOptions.RemoveEmptyEntries))
        {
            var match = regex.Match(line);
            if(match.Success)
            {
                var split = line.Split(":", 2, StringSplitOptions.RemoveEmptyEntries);
                lastAddedKey = split[0].Trim();
                eventDictionary.Add(lastAddedKey, split[1].Trim());
            }
            else
            {
                var lastValue = eventDictionary[lastAddedKey];
                eventDictionary.Remove(lastAddedKey);
                eventDictionary.Add(lastAddedKey, lastValue + "\n" + line.Trim());
            }
        }

        return eventDictionary;
    }

    private static IEnumerable<CalendarEvent> GetEvents(string source)
    {
        const string vEventContentPattern = @"BEGIN:VEVENT\s(?<vevent>.+?)\sEND:VEVENT";
        const RegexOptions vEventContentRegexOptions = RegexOptions.Singleline;

        var contentMatch = Regex.Matches(source, vEventContentPattern, vEventContentRegexOptions);

        foreach(Match match in contentMatch)
        {
            var field = ParseVEventFields(match.Groups["vevent"].Value);

            var startDate = DateTime.ParseExact(field["DTSTART"], "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(field["DTEND"], "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
            yield return new CalendarEvent(field["UID"], startDate, endDate, field["DESCRIPTION"],
                                           field["LOCATION"], field["SUMMARY"]);
        }
    }

    [GeneratedRegex(@"[^|\n][A-Z-]+:", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex EventFieldsRegex();
}