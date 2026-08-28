namespace Goblin.Application.Vk.Options;

public class VkLongPollingOptions
{
    public ulong GroupId { get; set; }

    public int WaitTimeout { get; set; } = 25;

    public TimeSpan DelayBetweenUpdates { get; set; } = TimeSpan.FromMilliseconds(250);
}
