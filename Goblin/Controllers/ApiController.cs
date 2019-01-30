using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vk.Models;

namespace Goblin.Controllers
{
    public class ApiController : Controller
    {
        public async Task<string> Handler([FromBody] CallbackResponse resp)
        {
            return await Bot.Handler.Handle(resp);
        }
    }
}
