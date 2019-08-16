using System;
using Flurl;
using Flurl.Http;

namespace Goblin.OpenWeatherMap
{
    internal static class Defaults
    {
        public const string EndPoint = "https://api.openweathermap.org/data/2.5/";

        private const string Language = "ru";
        private const string Units = "metric";
        
        internal const double PressureConvert = 0.75006375541921;

        internal const int MaxDailyWeatherDifference = 16;
        
        internal static IFlurlRequest BuildRequest(string token)
        {
            return EndPoint.SetQueryParam("units", Units)
                           .SetQueryParam("appid", token)
                           .SetQueryParam("lang", Language)
                           .WithTimeout(3)
                           .WithHeaders(new
                           {
                               Accept = "application/json",
                               User_Agent = "Japanese Goblin 1.0"
                           })
                           .AllowAnyHttpStatus();
        }

        internal static DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}