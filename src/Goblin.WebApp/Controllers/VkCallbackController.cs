using System.Threading.Tasks;
using Goblin.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using VkNet.Utils;

namespace Goblin.WebApp.Controllers
{
    [ApiController]
    [Route("/api/callback")]
    public class VkCallbackController : ControllerBase
    {
        private readonly CallbackHandler _handler;
        private readonly IConfiguration _config;

        public VkCallbackController(CallbackHandler handler, IConfiguration config)
        {
            _handler = handler;
            _config = config;
        }

        [HttpPost]
        public async Task<string> Handle([FromBody] object update)
        {
            var temp = update as dynamic; //TODO: пофиксить в следующей версии VkNet
            if(temp["type"] == "confirmation")
            {
                //TODO:
                return _config["Vk:ConfirmationCode"];
            }
            return await _handler.Handle(new VkResponse(JToken.FromObject(update)));
        }
    }
}