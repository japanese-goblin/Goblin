using System.Threading.Tasks;
using Goblin.Application;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VkNet.Utils;

namespace Goblin.WebApp.Controllers
{
    [ApiController]
    [Route("/api/callback")]
    public class VkCallbackController : ControllerBase
    {
        private readonly CallbackHandler _handler;

        public VkCallbackController(CallbackHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<string> Handle([FromBody] object update)
        {
            // if(update.Type.ToString() == "confirmation")
            // {
            //     //TODO:
            //     return "Confirmation code";
            // }
            //
            return await _handler.Handle(new VkResponse(JToken.FromObject(update)));
        }
    }
}