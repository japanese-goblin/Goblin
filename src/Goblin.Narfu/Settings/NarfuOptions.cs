namespace Goblin.Narfu.Settings;

public class NarfuApiOptions
{
    public string HostUrl { get; set; } = "";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan GroupsRefreshInterval { get; set; } = TimeSpan.FromHours(12);
}
