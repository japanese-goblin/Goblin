using System;

namespace Goblin.Narfu.ICalParser;

public record CalendarEvent(string Uid, DateTime DtStart, DateTime DtEnd, string Description, string Location, string Summary);