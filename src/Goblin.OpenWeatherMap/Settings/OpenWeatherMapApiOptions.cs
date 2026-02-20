using System;

namespace Goblin.OpenWeatherMap.Settings;

public class OpenWeatherMapApiOptions
{
    public string HostUrl { get; set; } = "";

    public string AccessToken { get; set; } = "";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    public string Language { get; set; } = "";

    public string Units { get; set; } = "";
}