using System.Security.Cryptography;
using Goblin.Application.Core;
using Goblin.Application.Core.Models;
using Goblin.Application.Vk.Converters;
using Goblin.Domain;
using Microsoft.Extensions.Logging;
using VkNet.Abstractions;
using VkNet.Model;

namespace Goblin.Application.Vk;

public class VkSender(IVkApi vkApi, ILogger<VkSender> logger) : ISender
{
    private const int ChunkLimit = 100;
    public int TextLimit => 4096;
    public ConsumerType ConsumerType => ConsumerType.Vkontakte;

    private readonly Dictionary<string, Type> _attachmentTypes = new Dictionary<string, Type>
    {
        ["photo"] = typeof(Photo),
        ["poll"] = typeof(Poll),
        ["video"] = typeof(Video),
        ["wall"] = typeof(Wall),
        ["doc"] = typeof(Document),
        ["audio"] = typeof(Audio),
        ["market"] = typeof(Market)
    };

    private readonly ILogger _logger = logger;
    private readonly RandomNumberGenerator _randomGenerator = RandomNumberGenerator.Create();

    public Task Send(long chatId, string message, CoreKeyboard? keyboard = null, IReadOnlyCollection<string>? attachments = null)
    {
        message = TrimText(message);

        return vkApi.Messages.SendAsync(new MessagesSendParams
        {
            PeerId = chatId,
            Message = message,
            Keyboard = KeyboardConverter.FromCoreToVk(keyboard, true),
            Attachments = ConvertAttachments(attachments),
            RandomId = GetRandomId()
        });
    }

    public async Task SendToMany(IReadOnlyCollection<long> chatIds, string message, CoreKeyboard? keyboard = null,
                                 IReadOnlyCollection<string>? attachments = null)
    {
        message = TrimText(message);

        foreach(var chunk in chatIds.Chunk(ChunkLimit))
        {
            try
            {
                await vkApi.Messages.SendToUserIdsAsync(new MessagesSendParams
                {
                    UserIds = chunk,
                    Message = message,
                    Keyboard = KeyboardConverter.FromCoreToVk(keyboard, true),
                    Attachments = ConvertAttachments(attachments),
                    RandomId = GetRandomId()
                });

                await Task.Delay(100);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Ошибка при отправке сообщения");
            }
        }
    }

    private string TrimText(string text)
    {
        if(text.Length < TextLimit)
        {
            return text;
        }

        const string separator = "...";
        var limit = TextLimit - separator.Length - 2;
        return $"{text[..limit]}...";
    }

    private List<MediaAttachment>? ConvertAttachments(IEnumerable<string>? attachments)
    {
        if(attachments is null)
        {
            return null;
        }

        var attachmentList = new List<MediaAttachment>();

        foreach(var attachType in _attachmentTypes)
        {
            var selected = attachments
                           .Where(x => x.StartsWith(attachType.Key))
                           .Select(x =>
                           {
                               var attach = Activator.CreateInstance(attachType.Value) as MediaAttachment;
                               var data = x.Replace(attachType.Key, string.Empty)
                                           .Split('_');

                               attach.OwnerId = long.Parse(data[0]);
                               attach.Id = long.Parse(data[1]);
                               return attach;
                           }).ToArray();
            if(selected.Length == 0)
            {
                continue;
            }

            attachmentList.AddRange(selected);
        }

        return attachmentList.Count == 0 ? null : attachmentList;
    }

    private int GetRandomId()
    {
        var intBytes = new byte[4];
        _randomGenerator.GetBytes(intBytes);
        return BitConverter.ToInt32(intBytes, 0);
    }
}