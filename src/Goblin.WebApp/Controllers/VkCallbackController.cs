using Goblin.Application;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Goblin.WebApp.Controllers
{
    [ApiController]
    [Route("/api/callback")]
    public class VkCallbackController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CallbackHandler _handler;

        public VkCallbackController(CallbackHandler handler, IConfiguration config)
        {
            _handler = handler;
            _config = config;
        }

        [HttpPost]
        public string Handle([FromBody] object update)
        {
            var temp = update as dynamic; //TODO: пофиксить в следующей версии VkNet
            if(temp["type"] == "confirmation")
            {
                Log.Information("Запрошено подтверждение сервера");

                //TODO:
                return _config["Vk:ConfirmationCode"];
            }

            BackgroundJob.Enqueue(() => _handler.Handle(JToken.FromObject(update)));

            return "ok";
        }
    }
}