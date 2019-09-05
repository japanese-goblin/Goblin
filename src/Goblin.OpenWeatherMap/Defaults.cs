using System;
using Flurl;
using Flurl.Http;

namespace Goblin.OpenWeatherMap
{
    internal static class Defaults
    {
        public const string EndPoint = "https://api.openweathermap.org/data/2.5/";
        public const string UserAgent = "Japanese Goblin 2.0";

        internal const string Language = "ru";
        internal const string Units = "metric";

        internal const double PressureConvert = 0.75006375541921;

        internal const int MaxDailyWeatherDifference = 16;

        internal static DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}