using Goblin.Application.Telegram;
using Goblin.Application.Telegram.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Goblin.WebApp.Controllers.Callbacks;

[ApiController]
[Route("/api/callback/tg")]
public class TelegramCallbackController(IOptions<TelegramOptions> optionsAccessor, TelegramEventsDispatcher dispatcher) : ControllerBase
{
    private readonly TelegramOptions _options = optionsAccessor.Value;

    [HttpPost("{SecretKey}")]
    public async Task<IActionResult> HandleCallback(string secretKey)
    {
        if(!_options.SecretKey.Equals(secretKey))
        {
            //TODO: logging
            return NotFound();
        }

        var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var request = JsonConvert.DeserializeObject<Update>(rawRequestBody)!;

        await dispatcher.Publish(request);
        return Ok();
    }
}