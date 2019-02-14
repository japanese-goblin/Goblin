using System.Threading.Tasks;
using Goblin.Bot;
using Microsoft.AspNetCore.Mvc;
using Vk.Models;

namespace Goblin.Controllers
{
    public class ApiController : Controller
    {
        private readonly Handler _handler;

        public ApiController(Handler handler)
        {
            _handler = handler;
        }

        public async Task<string> Handler([FromBody] CallbackResponse resp)
        {
            return await _handler.Handle(resp);
        }
    }
}