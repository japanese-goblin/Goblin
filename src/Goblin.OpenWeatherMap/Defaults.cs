using System;

namespace Goblin.OpenWeatherMap
{
    public static class Defaults
    {
        public const string EndPoint = "https://api.openweathermap.org/data/2.5/";

        public const string Language = "ru";
        public const string Units = "metric";

        internal const double PressureConvert = 0.75006375541921;

        internal static DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}