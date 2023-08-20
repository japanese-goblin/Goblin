using FastEndpoints;
using Goblin.Application.Telegram;
using Goblin.Application.Telegram.Options;
using Hangfire;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Goblin.WebApp.Endpoints.Callback;

public class TelegramCallbackEndpoint : Endpoint<TelegramRequest>
{
    private readonly TelegramCallbackHandler _handler;
    private readonly TelegramOptions _options;
    private readonly ILogger _logger;

    public TelegramCallbackEndpoint(IOptions<TelegramOptions> options, TelegramCallbackHandler handler,
                                    ILogger<TelegramCallbackEndpoint> logger)
    {
        _handler = handler;
        _options = options.Value;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/callback/tg/{SecretKey}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(TelegramRequest req, CancellationToken ct)
    {
        if(!_options.SecretKey.Equals(req.SecretKey))
        {
            _logger.LogWarning("Пришло событие с неправильным секретным ключом: {RequestSecretKey}", req.SecretKey);
            await SendNotFoundAsync(ct);
            return;
        }

        BackgroundJob.Enqueue(() => _handler.Handle(req.Update));

        await SendOkAsync(ct);
    }
}

public class TelegramRequest : IPlainTextRequest
{
    public string SecretKey { get; set; }
    public string Content { get; set; }
    public Update Update => JsonConvert.DeserializeObject<Update>(Content)!;
}