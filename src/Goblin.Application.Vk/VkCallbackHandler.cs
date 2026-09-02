using System.Text.RegularExpressions;
using Goblin.Application.Core;
using Goblin.Application.Vk.Converters;
using Goblin.Domain;
using Microsoft.Extensions.Logging;
using VkNet.Abstractions;
using VkNet.Enums.StringEnums;
using VkNet.Model;
using Message = Goblin.Application.Core.Models.Message;

namespace Goblin.Application.Vk;

public class VkCallbackHandler
{
    private readonly CommandsService _commandsService;
    private readonly ILogger _logger;
    private readonly IVkApi _vkApi;
    private readonly ISender _sender;

    public VkCallbackHandler(
        CommandsService commandsService, 
        IVkApi vkApi,
        IEnumerable<ISender> senders,
        ILogger<VkCallbackHandler> logger)
    {
        _commandsService = commandsService;
        _vkApi = vkApi;

        // TODO: keyed service
        _sender = senders.First(p => p.ConsumerType == ConsumerType.Vkontakte);
        _logger = logger;
    }

    public async Task Handle(GroupUpdate upd, CancellationToken ct)
    {
        _logger.LogDebug("Обработка события с типом {UpdateType}", upd.Type.Value);

        if (upd.Type.Value == GroupUpdateType.MessageNew)
        {
            if (upd.Instance is not MessageNew messageNew)
            {
                _logger.LogWarning("Не удалось преобразовать обновление {GroupUpdateType}", upd.Type.Value);
                return;
            }

            if (messageNew.Message.Action?.Type == MessageAction.ChatInviteUser)
            {
                await _sender.Send(messageNew.Message.PeerId.Value,
                    "Здравствуйте!\n" +
                    "Подробности по настройке бота для бесед здесь: vk.com/@japanese.goblin-conversations");
                return;
            }

            var msg = messageNew.Message.MapToBotMessage();
            ExtractUserIdFromConversation(msg);
            await MessageNew(msg, ct);
        }
        else if (upd.Type.Value == GroupUpdateType.MessageEvent)
        {
            if (upd.Instance is not MessageEvent messageEvent)
            {
                _logger.LogWarning("Не удалось преобразовать обновление {GroupUpdateType}", upd.Type.Value);
                return;
            }

            await MessageEvent(messageEvent, ct);
        }
        // else if(upd.Type.Value == GroupUpdateType.GroupLeave)
        // {
        //     if(upd.Instance is not GroupLeave groupLeaveEvent)
        //     {
        //         _logger.LogWarning("Не удалось преобразовать обновление {GroupUpdateType}", upd.Type.Value);
        //         return;
        //     }
        //
        //     await GroupLeave(groupLeaveEvent);
        // }
        // else if(upd.Type.Value == GroupUpdateType.GroupJoin)
        // {
        //     if(upd.Instance is not GroupJoin groupJoinEvent)
        //     {
        //         _logger.LogWarning("Не удалось преобразовать обновление {GroupUpdateType}", upd.Type.Value);
        //         return;
        //     }
        //
        //     await GroupJoin(groupJoinEvent);
        // }
        else
        {
            _logger.LogCritical("Обработчик для события {UpdateType} не найден", upd.Type);
            throw new ArgumentOutOfRangeException(nameof(upd.Type), "Отсутствует обработчик события");
        }

        _logger.LogDebug("Обработка события {UpdateType} завершена", upd.Type.Value);

        void ExtractUserIdFromConversation(Message msg)
        {
            if (msg.ChatId == msg.UserId)
            {
                return;
            }

            var regEx = Regex.Match(msg.Text, @"\[club\d+\|.*\] (.*)");
            if (regEx.Groups.Count > 1)
            {
                msg.Text = regEx.Groups[1].Value.Trim();
            }
        }
    }

    private async Task MessageNew(Message message, CancellationToken ct)
    {
        _logger.LogDebug("Обработка сообщения");
        var executionResult = await _commandsService.ExecuteAction(message, ct);
        await _sender.Send(message.ChatId, executionResult.Message, executionResult.Keyboard);
        _logger.LogDebug("Обработка сообщения завершена");
    }

    private async Task MessageEvent(MessageEvent messageEvent, CancellationToken ct)
    {
        _logger.LogDebug("Обработка сообщения");
        var mappedToMessage = messageEvent.MapToBotMessage();
        var executionResult = await _commandsService.ExecuteAction(mappedToMessage, ct);

        var peerId = messageEvent.PeerId.GetValueOrDefault(0);
        try
        {
            await _vkApi.Messages.EditAsync(new MessageEditParams
            {
                PeerId = peerId,
                ConversationMessageId = messageEvent.ConversationMessageId,
                Keyboard = KeyboardConverter.FromCoreToVk(executionResult.Keyboard, true),
                Message = executionResult.Message
            }, ct);
        }
        catch
        {
            await _sender.Send(peerId, executionResult.Message, executionResult.Keyboard);
        }

        _logger.LogDebug("Обработка сообщения завершена");
    }

    // private async Task GroupLeave(GroupLeave leave)
    // {
    //     const string groupLeaveMessage = "Очень жаль, что ты решил отписаться от группы 😢\n" +
    //                                      "Если ты не разобрался с ботом, то всегда можешь написать " +
    //                                      "об этом администраторам через команду 'админ *сообщение*' (подробнее смотри в справке).";
    //
    //     _logger.LogInformation("Пользователь id{UserId} покинул группу", leave.UserId);
    //     await SendMessageToAdmins(leave.UserId.Value, "отписался :С");
    //
    //     if(leave.IsSelf.HasValue && !leave.IsSelf.Value)
    //     {
    //         return;
    //     }
    //
    //     await TrySendMessageToUser(leave.UserId.Value, groupLeaveMessage);
    // }
    //
    // private async Task GroupJoin(GroupJoin join)
    // {
    //     const string groupJoinMessage = "Спасибо за подписку! ❤\n" +
    //                                     "Если у тебя возникнут вопросы, то ты всегда можешь связаться с администрацией бота " +
    //                                     "при помощи команды 'админ *сообщение*' (подробнее смотри в справке)";
    //
    //     _logger.LogInformation("Пользователь id{UserId} вступил в группу", join.UserId);
    //     await SendMessageToAdmins(join.UserId.Value, "подписался!");
    //
    //     if(join.JoinType.HasValue && join.JoinType != GroupJoinType.Join)
    //     {
    //         return;
    //     }
    //
    //     await TrySendMessageToUser(join.UserId.Value, groupJoinMessage);
    // }

    // private async Task TrySendMessageToUser(long userId, string message)
    // {
    //     try
    //     {
    //         await _sender.Send(userId, message);
    //     }
    //     catch
    //     {
    //         // ignored
    //     }
    // }

    // private async Task SendMessageToAdmins(long userId, string message)
    // {
    //     var admins = await _db.BotUsers.Where(x => x.IsAdmin &&
    //                                          x.ConsumerType == ConsumerType.Vkontakte)
    //                     .Select(x => x.ConsumerId)
    //                     .ToListAsync();
    //     var vkUser = (await _vkApi.Users.GetAsync([userId])).First();
    //     var userName = $"{vkUser.FirstName} {vkUser.LastName}";
    //     await _sender.SendToMany(admins, $"@id{userId} ({userName}) {message}");
    // }
}
