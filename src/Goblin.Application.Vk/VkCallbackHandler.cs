using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Goblin.Application.Core;
using Goblin.Application.Core.Options;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Application.Vk.Converters;
using Goblin.Application.Vk.Extensions;
using Goblin.Application.Vk.Models;
using Goblin.Application.Vk.Options;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.Extensions.Options;
using Serilog;
using VkNet.Abstractions;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk
{
    public class VkCallbackHandler
    {
        private readonly BotDbContext _db;
        private readonly ILogger _logger;
        private readonly VkOptions _options;
        private readonly CommandsService _commandsService;
        private readonly IVkApi _vkApi;
        private readonly IMapper _mapper;

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
                await MessageNew(upd.MessageNew);
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

        private async Task MessageNew(MessageNew messageNew)
        {
            var msg = _mapper.Map<VkMessage>(messageNew.Message);
            var user = await _db.BotUsers.FindAsync(msg.FromId);
            if(user is null)
            {
                _logger.Debug("Пользователь с id {0} не найден. Создание записи.", msg.FromId);
                user = (await _db.BotUsers.AddAsync(new BotUser(msg.MessageUserId))).Entity;
                await _db.Subscribes.AddAsync(new Subscribe(msg.MessageUserId, false, false));
                await _db.SaveChangesAsync();
                _logger.Debug("Пользователь создан");
            }

            _logger.Debug("Обработка сообщения");
            var result = await _commandsService.ExecuteCommand(msg, user);
            _logger.Information("Обработка сообщения завершена");
            _logger.Debug("Отправка сообщения");

            if(result is FailedResult failed)
            {
                if(result is CommandNotFoundResult && !user.IsErrorsEnabled)
                {
                    // если команда не найдена, и у пользователя отключены ошибки
                    return;
                }

                await _vkApi.Messages.SendError(failed.ToString(), msg.MessageChatId);
            }
            else
            {
                var success = result as SuccessfulResult;
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    Message = success.Message,
                    Keyboard = KeyboardConverter.FromCoreToVk(success.Keyboard),
                    PeerId = msg.MessageChatId
                });
            }
            _logger.Information("Отправка сообщения завершена");
        }

        public async Task GroupLeave(GroupLeave leave)
        {
            const string groupLeaveMessage = "Очень жаль, что ты решил отписаться от группы 😢\n" +
                                             "Если тебе что-то не понравилось или ты не разобрался с ботом, то всегда можешь написать " +
                                             "администрации об этом через команду 'админ *сообщение*' (подробнее смотри в справке).";
            
            _logger.Information("Пользователь id{0} покинул группу", leave.UserId);
            var admins = _db.BotUsers.Where(x => x.IsAdmin).Select(x => x.VkId);
            var vkUser = (await _vkApi.Users.GetAsync(new[] { leave.UserId.Value })).First();
            var userName = $"{vkUser.FirstName} {vkUser.LastName}";
            await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                Message = $"@id{leave.UserId} ({userName}) отписался :С",
                UserIds = admins
            });

            if(leave.IsSelf.HasValue && leave.IsSelf.Value)
            {
                try
                {
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        Message = groupLeaveMessage,
                        PeerId = leave.UserId.Value
                    });
                }
                catch
                {
                    // ignored
                }
            }
        }

        public async Task GroupJoin(GroupJoin join)
        {
            const string groupJoinMessage = "Спасибо за подписку! ❤\n" +
                                            "Если у тебя возникнут вопросы, то ты всегда можешь связаться с администрацией бота " +
                                            "при помощи команды 'админ *сообщение*' (подробнее смотри в справке)";
            
            _logger.Information("Пользователь id{0} вступил в группу", join.UserId);
            var admins = _db.BotUsers.Where(x => x.IsAdmin).Select(x => x.VkId);
            var vkUser = (await _vkApi.Users.GetAsync(new[] { join.UserId.Value })).First();
            var userName = $"{vkUser.FirstName} {vkUser.LastName}";
            await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                Message = $"@id{join.UserId} ({userName}) подписался!",
                UserIds = admins
            });

            if(join.JoinType.HasValue && join.JoinType == GroupJoinType.Join)
            {
                try
                {
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        Message = groupJoinMessage,
                        PeerId = join.UserId.Value
                    });
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}