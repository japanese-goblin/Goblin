using System.Threading.Channels;
using VkNet.Model;

namespace Goblin.Application.Vk;

public class VkEventsDispatcher
{
    private readonly Channel<GroupUpdate> _channel = Channel.CreateBounded<GroupUpdate>(new BoundedChannelOptions(1024)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = false,
        SingleWriter = false
    });

    public async Task Publish(GroupUpdate update)
    {
        await _channel.Writer.WriteAsync(update);
    }

    public IAsyncEnumerable<GroupUpdate> ReadAllAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}