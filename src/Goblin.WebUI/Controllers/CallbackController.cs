using System.Threading.Tasks;
using Goblin.Bot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vk.Models;

namespace Goblin.WebUI.Controllers
{
    [Route("api/[controller]")]
    public class CallbackController : ControllerBase
    {
        private readonly Handler _handler;
        private readonly ILogger<CallbackController> _logger;

        public CallbackController(Handler handler, ILogger<CallbackController> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        [HttpPost]
        public async Task<string> Handler([FromBody] CallbackResponse resp)
        {
            _logger.LogInformation("Обработка нового запроса с типом {0}", resp.Type);
            return await _handler.Handle(resp);
        }
    }
}