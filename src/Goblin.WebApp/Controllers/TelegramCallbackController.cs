using Goblin.Application.Telegram;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace Goblin.WebApp.Controllers
{
    [ApiController]
    [Route("/api/callback/tg")]
    public class TelegramCallbackController : ControllerBase
    {
        private readonly TelegramCallbackHandler _handler;

        public TelegramCallbackController(TelegramCallbackHandler handler)
        {
            _handler = handler;
        }
        
        [HttpPost]
        public IActionResult Index([FromBody] Update update)
        {
            BackgroundJob.Enqueue(() => _handler.Handle(update));
            return Ok();
        }
    }
}