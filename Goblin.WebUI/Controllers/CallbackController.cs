using System.Threading.Tasks;
using Goblin.Bot;
using Microsoft.AspNetCore.Mvc;
using Vk.Models;

namespace Goblin.WebUI.Controllers
{
    [Route("api/[controller]")]
    public class CallbackController : ControllerBase
    {
        private readonly Handler _handler;

        public CallbackController(Handler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<string> Handler([FromBody] CallbackResponse resp)
        {
            return await _handler.Handle(resp);
        }
    }
}