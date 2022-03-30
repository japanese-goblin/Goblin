using Goblin.Application.Vk;
using Goblin.Application.Vk.Options;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace Goblin.WebApp.Controllers;

[ApiController]
[Route("/api/callback/vk")]
public class VkCallbackController : ControllerBase
{
    private readonly VkCallbackHandler _handler;
    private readonly VkOptions _vkOptions;

    public VkCallbackController(VkCallbackHandler handler, IOptions<VkOptions> vkOptions)
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