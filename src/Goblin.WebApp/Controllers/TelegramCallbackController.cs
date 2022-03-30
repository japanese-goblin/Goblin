using Goblin.Application.Telegram;
using Goblin.Application.Telegram.Options;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot.Types;

namespace Goblin.WebApp.Controllers;

[ApiController]
[Route("/api/callback/tg")]
public class TelegramCallbackController : ControllerBase
{
    private readonly TelegramCallbackHandler _handler;
    private readonly TelegramOptions _options;
    private readonly ILogger _logger;

    public TelegramCallbackController(IOptions<TelegramOptions> options, TelegramCallbackHandler handler)
    {
        _handler = handler;
        _options = options.Value;
        _logger = Log.ForContext<TelegramCallbackController>();
    }

    [HttpPost]
    [Route("{secretKey}")]
    public IActionResult Index([FromRoute] string secretKey, [FromBody] Update update)
    {
        if(!_options.SecretKey.Equals(secretKey))
        {
            _logger.Warning("Пришло событие с неправильным секретным ключом: {0}", secretKey);
            return Ok();
        }

        BackgroundJob.Enqueue(() => _handler.Handle(update));
        return Ok();
    }
}