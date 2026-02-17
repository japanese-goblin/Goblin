using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Goblin.Application.Telegram;

public class TelegramEventsDispatcher
{
    private readonly Channel<Update> _channel = Channel.CreateBounded<Update>(new BoundedChannelOptions(1024)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = false,
        SingleWriter = false
    });

    public async Task Publish(Update update)
    {
        await _channel.Writer.WriteAsync(update);
    }

    public IAsyncEnumerable<Update> ReadAllAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}