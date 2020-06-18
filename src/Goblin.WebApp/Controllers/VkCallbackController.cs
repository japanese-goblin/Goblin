using Goblin.Application.Core.Options;
using Goblin.Application.Vk;
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
        private readonly CallbackHandler _handler;
        private readonly VkOptions _vkOptions;

        public VkCallbackController(CallbackHandler handler, IOptions<VkOptions> vkOptions)
        {
            _handler = handler;
            _vkOptions = vkOptions.Value;
        }

        [HttpPost]
        public string Handle([FromBody] object update)
        {
            var response = GroupUpdate.FromJson(new VkResponse(JToken.FromObject(update)));
            if(response.Type == GroupUpdateType.Confirmation)
            {
                return _vkOptions.ConfirmationCode;
            }

            BackgroundJob.Enqueue(() => _handler.Handle(response));

            return "ok";
        }
    }
}