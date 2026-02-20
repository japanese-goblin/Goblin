using System;

namespace Goblin.Narfu.Settings;

public class NarfuApiOptions
{
    public string HostUrl { get; set; } = "";

    public string NarfuGroupsLink { get; set; } = "";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
}