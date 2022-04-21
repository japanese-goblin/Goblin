using FastEndpoints;
using Goblin.Application.Vk;
using Goblin.Application.Vk.Options;
using Hangfire;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace Goblin.WebApp.Endpoints.Callback;

public class Vk : Endpoint<VkRequest>
{
    private readonly VkCallbackHandler _handler;
    private readonly VkOptions _vkOptions;

    public Vk(VkCallbackHandler handler, IOptions<VkOptions> vkOptions)
    {
        _handler = handler;
        _vkOptions = vkOptions.Value;
    }

    public override void Configure()
    {
        Post("/callback/vk");
        AllowAnonymous();
    }

    public override Task HandleAsync(VkRequest req, CancellationToken ct)
    {
        var response = GroupUpdate.FromJson(req.Response);
        if(response.Type == GroupUpdateType.Confirmation)
        {
            return SendStringAsync(_vkOptions.ConfirmationCode, cancellation: ct);
        }

        BackgroundJob.Enqueue(() => _handler.Handle(response));

        return SendStringAsync("ok", cancellation: ct);
    }
}

public class VkRequest : IPlainTextRequest
{
    public string Content { get; set; }

    public VkResponse Response => new VkResponse(JToken.Parse(Content));
}