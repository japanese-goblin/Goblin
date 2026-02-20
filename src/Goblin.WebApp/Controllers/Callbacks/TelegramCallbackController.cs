using Goblin.Application.Telegram;
using Goblin.Application.Telegram.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Goblin.WebApp.Controllers.Callbacks;

[ApiController, Route("/api/callback/tg")]
public class TelegramCallbackController(IOptions<TelegramOptions> optionsAccessor, TelegramEventsDispatcher dispatcher) : ControllerBase
{
    private readonly TelegramOptions _options = optionsAccessor.Value;

    [HttpPost]
    public async Task<IActionResult> HandleCallback([FromBody] Update requestModel)
    {
        var passedSecretKey = Request.Headers["X-Telegram-Bot-Api-Secret-Token"];
        if(!_options.SecretKey.Equals(passedSecretKey))
        {
            //TODO: logging
            return NotFound();
        }

        await dispatcher.Publish(requestModel);
        return Ok();
    }
}