using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Vk.Extensions;
using Goblin.Domain;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk;

public class VkSender : ISender
{
    public ConsumerType ConsumerType => ConsumerType.Vkontakte;

    private readonly IVkApi _vkApi;

    public VkSender(IVkApi vkApi)
    {
        _vkApi = vkApi;
    }

    public Task Send(long chatId, string message)
    {
        return _vkApi.Messages.SendWithRandomId(new MessagesSendParams()
        {
            PeerId = chatId,
            Message = message
        });
    }
}