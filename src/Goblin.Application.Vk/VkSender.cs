using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Core.Models;
using Goblin.Application.Vk.Converters;
using Goblin.Domain;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk;

public class VkSender : ISender
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

    private readonly IVkApi _vkApi;
    private readonly ILogger _logger;
    private readonly RandomNumberGenerator _randomGenerator;

    public VkSender(IVkApi vkApi)
    {
        _vkApi = vkApi;
        _logger = Log.ForContext<VkSender>();
        _randomGenerator = RandomNumberGenerator.Create();
    }

    public Task Send(long chatId, string message, CoreKeyboard keyboard = null, IEnumerable<string> attachments = null)
    {
        message = TrimText(message);

        return _vkApi.Messages.SendAsync(new MessagesSendParams
        {
            PeerId = chatId,
            Message = message,
            Keyboard = KeyboardConverter.FromCoreToVk(keyboard, true),
            Attachments = ConvertAttachments(attachments),
            RandomId = GetRandomId()
        });
    }

    public async Task SendToMany(IEnumerable<long> chatIds, string message, CoreKeyboard keyboard = null,
                                 IEnumerable<string> attachments = null)
    {
        message = TrimText(message);

        foreach(var chunk in chatIds.Chunk(ChunkLimit))
        {
            try
            {
                await _vkApi.Messages.SendToUserIdsAsync(new MessagesSendParams
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
                _logger.Error(e, "Ошибка при отправке сообщения");
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

    private IEnumerable<MediaAttachment> ConvertAttachments(IEnumerable<string> attachments)
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
            if(!selected.Any())
            {
                continue;
            }

            attachmentList.AddRange(selected);
        }

        return attachmentList.Any() ? attachmentList : null;
    }

    private int GetRandomId()
    {
        var intBytes = new byte[4];
        _randomGenerator.GetBytes(intBytes);
        return BitConverter.ToInt32(intBytes, 0);
    }
}