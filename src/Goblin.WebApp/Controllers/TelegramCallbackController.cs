using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Goblin.WebApp.Controllers
{
    [ApiController]
    [Route("/api/callback/tg")]
    public class TelegramCallbackController : ControllerBase
    {
        private readonly TelegramBotClient _botClient;

        public TelegramCallbackController(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        
        // GET
        public async Task<IActionResult> Index([FromBody] Update update)
        {
            var message = update.Message;
            var userId = message.Chat.Id;
            var text = message.Text;

            await _botClient.SendTextMessageAsync(userId, text);

            return Ok();
        }
    }
}