using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Vk.Converters;
using Goblin.Application.Vk.Options;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.Extensions.Options;
using Serilog;
using VkNet.Abstractions;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using Message = Goblin.Application.Core.Models.Message;

namespace Goblin.Application.Vk;

public class VkCallbackHandler
{
    private readonly CommandsService _commandsService;
    private readonly BotDbContext _db;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly VkOptions _options;
    private readonly IVkApi _vkApi;
    private readonly ISender _sender;

    public VkCallbackHandler(CommandsService commandsService,
                             BotDbContext db,
                             IVkApi vkApi,
                             IEnumerable<ISender> senders,
                             IOptions<VkOptions> options,
                             IMapper mapper)
    {
        _commandsService = commandsService;
        _db = db;
        _vkApi = vkApi;
        _sender = senders.First(x => x.ConsumerType == ConsumerType.Vkontakte);
        _mapper = mapper;
        _options = options.Value;
        _logger = Log.ForContext<VkCallbackHandler>();
    }

    public async Task Handle(GroupUpdate upd)
    {
        if(upd.Secret != _options.SecretKey)
        {
            _logger.Warning("Пришло событие с неправильным секретным ключом ({0})", upd.Secret);
            return;
        }

        _logger.Debug("Обработка события с типом {0}", upd.Type);

        if(upd.Type == GroupUpdateType.MessageNew)
        {
            if(upd.MessageNew.Message.Action?.Type == MessageAction.ChatInviteUser)
            {
                await _sender.Send(upd.MessageNew.Message.PeerId.Value,
                                   "Здравствуйте!\n" +
                                   "Подробности по настройке бота для бесед здесь: vk.com/@japanese.goblin-conversations");
                return;
            }

            var msg = _mapper.Map<Message>(upd.MessageNew.Message);
            ExtractUserIdFromConversation(msg);
            await MessageNew(msg);
        }
        else if(upd.Type == GroupUpdateType.MessageEvent)
        {
            await MessageEvent(upd.MessageEvent);
        }
        else if(upd.Type == GroupUpdateType.GroupLeave)
        {
            await GroupLeave(upd.GroupLeave);
        }
        else if(upd.Type == GroupUpdateType.GroupJoin)
        {
            await GroupJoin(upd.GroupJoin);
        }
        else
        {
            _logger.Fatal("Обработчик для события {0} не найден", upd.Type);
            throw new ArgumentOutOfRangeException(nameof(upd.Type), "Отсутствует обработчик события");
        }

        _logger.Information("Обработка события {0} завершена", upd.Type);

        void ExtractUserIdFromConversation(Message msg)
        {
            if(msg.ChatId == msg.UserId)
            {
                return;
            }

            var regEx = Regex.Match(msg.Text, @"\[club\d+\|.*\] (.*)");
            if(regEx.Groups.Count > 1)
            {
                msg.Text = regEx.Groups[1].Value.Trim();
            }
        }
    }

    private async Task MessageNew(Message message)
    {
        _logger.Debug("Обработка сообщения");
        await _commandsService.ExecuteCommand(message, OnSuccess, OnFailed);
        _logger.Information("Обработка сообщения завершена");

        async Task OnSuccess(IResult res)
        {
            await _sender.Send(message.ChatId, res.Message, res.Keyboard);
        }

        async Task OnFailed(IResult res)
        {
            await _sender.Send(message.ChatId, res.Message, res.Keyboard);
        }
    }

    private async Task MessageEvent(MessageEvent messageEvent)
    {
        var mappedToMessage = _mapper.Map<Message>(messageEvent);
        await _commandsService.ExecuteCommand(mappedToMessage, OnSuccess, OnFailed);
        async Task OnSuccess(IResult res)
        {
            await _vkApi.Messages.EditAsync(new MessageEditParams()
            {
                PeerId = messageEvent.PeerId.GetValueOrDefault(0),
                ConversationMessageId = messageEvent.ConversationMessageId,
                Keyboard = KeyboardConverter.FromCoreToVk(res.Keyboard, true),
                Message = res.Message
            });
        }

        async Task OnFailed(IResult res)
        {
            await _vkApi.Messages.SendMessageEventAnswerAsync(messageEvent.EventId,
                                                              messageEvent.UserId.GetValueOrDefault(0),
                                                              messageEvent.PeerId.GetValueOrDefault(0),
                                                              new EventData()
                                                              {
                                                                  Type = MessageEventType.SnowSnackbar,
                                                                  Text = res.Message
                                                              });
        }
    }

    public async Task GroupLeave(GroupLeave leave)
    {
        const string groupLeaveMessage = "Очень жаль, что ты решил отписаться от группы 😢\n" +
                                         "Если тебе что-то не понравилось или ты не разобрался с ботом, то всегда можешь написать " +
                                         "администрации об этом через команду 'админ *сообщение*' (подробнее смотри в справке).";

        _logger.Information("Пользователь id{0} покинул группу", leave.UserId);
        await SendMessageToAdmins(leave.UserId.Value, "отписался :С");

        if(leave.IsSelf.HasValue && !leave.IsSelf.Value)
        {
            return;
        }

        await TrySendMessageToUser(leave.UserId.Value, groupLeaveMessage);
    }

    public async Task GroupJoin(GroupJoin join)
    {
        const string groupJoinMessage = "Спасибо за подписку! ❤\n" +
                                        "Если у тебя возникнут вопросы, то ты всегда можешь связаться с администрацией бота " +
                                        "при помощи команды 'админ *сообщение*' (подробнее смотри в справке)";

        _logger.Information("Пользователь id{0} вступил в группу", join.UserId);
        await SendMessageToAdmins(join.UserId.Value, "подписался!");

        if(join.JoinType.HasValue && join.JoinType != GroupJoinType.Join)
        {
            return;
        }

        await TrySendMessageToUser(join.UserId.Value, groupJoinMessage);
    }

    private async Task TrySendMessageToUser(long userId, string message)
    {
        try
        {
            await _sender.Send(userId, message);
        }
        catch
        {
            // ignored
        }
    }

    private async Task SendMessageToAdmins(long userId, string message)
    {
        var admins = _db.BotUsers.Where(x => x.IsAdmin &&
                                             x.ConsumerType == ConsumerType.Vkontakte)
                        .Select(x => x.Id);
        var vkUser = (await _vkApi.Users.GetAsync(new[] { userId })).First();
        var userName = $"{vkUser.FirstName} {vkUser.LastName}";
        await _sender.SendToMany(admins, $"@id{userId} ({userName}) {message}");
    }
}