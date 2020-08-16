using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Vk.Converters;
using Goblin.Application.Vk.Extensions;
using Goblin.Application.Vk.Options;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.Extensions.Options;
using Serilog;
using VkNet.Abstractions;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using Message = Goblin.Application.Core.Models.Message;

namespace Goblin.Application.Vk
{
    public class VkCallbackHandler
    {
        private readonly CommandsService _commandsService;
        private readonly BotDbContext _db;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly VkOptions _options;
        private readonly IVkApi _vkApi;

        public VkCallbackHandler(CommandsService commandsService, BotDbContext db, IVkApi vkApi, IOptions<VkOptions> options,
                                 IMapper mapper)
        {
            _commandsService = commandsService;
            _db = db;
            _vkApi = vkApi;
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
                var msg = _mapper.Map<Message>(upd.MessageNew.Message);
                if(msg.ChatId != msg.UserId)
                {
                    var regEx = Regex.Match(msg.Text, @"\[club\d+\|.*\] (.*)");
                    if(regEx.Groups.Count > 1)
                    {
                        msg.Text = regEx.Groups[1].Value.Trim();
                    }
                }

                await MessageNew(msg, upd.MessageNew.ClientInfo);
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
        }

        private async Task MessageNew(Message message, ClientInfo clientInfo)
        {
            _logger.Debug("Обработка сообщения");
            await _commandsService.ExecuteCommand<VkBotUser>(message, OnSuccess, OnFailed);
            _logger.Information("Обработка сообщения завершена");

            async Task OnSuccess(IResult res)
            {
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    Message = res.Message,
                    Keyboard = KeyboardConverter.FromCoreToVk(res.Keyboard, clientInfo.InlineKeyboard),
                    PeerId = message.ChatId
                });
            }

            async Task OnFailed(IResult res)
            {
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    Message = res.Message,
                    PeerId = message.ChatId
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

            await SendMessageToUserWithTry(leave.UserId.Value, groupLeaveMessage);
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

            await SendMessageToUserWithTry(join.UserId.Value, groupJoinMessage);
        }

        private async Task SendMessageToUserWithTry(long userId, string message)
        {
            try
            {
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    Message = message,
                    PeerId = userId
                });
            }
            catch
            {
                // ignored
            }
        }

        private async Task SendMessageToAdmins(long userId, string message)
        {
            var admins = _db.VkBotUsers.Where(x => x.IsAdmin).Select(x => x.Id);
            var vkUser = (await _vkApi.Users.GetAsync(new[] { userId })).First();
            var userName = $"{vkUser.FirstName} {vkUser.LastName}";
            await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                Message = $"@id{userId} ({userName}) {message}",
                UserIds = admins
            });
        }
    }
}