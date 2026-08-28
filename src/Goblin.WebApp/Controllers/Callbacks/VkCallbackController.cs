using Goblin.Application.Vk;
using Goblin.Application.Vk.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace Goblin.WebApp.Controllers.Callbacks;

[ApiController, Route("/api/callback/vk")]
public class VkCallbackController(IOptions<VkCallbackOptions> optionsAccessor, VkEventsDispatcher vkEventsDispatcher) : ControllerBase
{
    private readonly VkCallbackOptions _options = optionsAccessor.Value;

    [HttpPost]
    public async Task<IActionResult> HandleCallback()
    {
        var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var requestModel = JsonConvert.DeserializeObject<GroupUpdate>(rawRequestBody)!;

        if(requestModel.Secret?.Value != _options.SecretKey)
        {
            return NotFound();
        }

        if(requestModel.Type.Value == GroupUpdateType.Confirmation)
        {
            return Ok(_options.ConfirmationCode);
        }

        await vkEventsDispatcher.Publish(requestModel);

        return Ok("ok");
    }
}
