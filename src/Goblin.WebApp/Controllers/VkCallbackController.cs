using Goblin.Application;
using Goblin.Application.Options;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace Goblin.WebApp.Controllers
{
    [ApiController]
    [Route("/api/callback")]
    public class VkCallbackController : ControllerBase
    {
        private readonly IOptions<VkOptions> _vkOptions;
        private readonly CallbackHandler _handler;

        public VkCallbackController(CallbackHandler handler, IOptions<VkOptions> vkOptions)
        {
            _handler = handler;
            _vkOptions = vkOptions;
        }

        [HttpPost]
        public string Handle([FromBody] object update)
        {
            var response = GroupUpdate.FromJson(new VkResponse(JToken.FromObject(update)));
            if(response.Type == GroupUpdateType.Confirmation)
            {
                return _vkOptions.Value.ConfirmationCode;
            }

            BackgroundJob.Enqueue(() => _handler.Handle(response));

            return "ok";
        }
    }
}