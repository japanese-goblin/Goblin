using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Bot.Notifications.Confirmation;
using Goblin.Bot.Notifications.GroupJoin;
using Goblin.Bot.Notifications.GroupLeave;
using Goblin.Bot.Notifications.MessageDeny;
using Goblin.Domain.Entities;
using Goblin.Persistence;
using MediatR;
using Microsoft.Extensions.Configuration;
using Vk;
using Vk.Models;
using Vk.Models.Messages;

namespace Goblin.Bot
{
    public class Handler
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly CommandExecutor _executor;
        private readonly VkApi _api;
        private readonly IMediator _mediator;

        private const string OkResponse = "ok";

        public Handler(ApplicationDbContext db, IConfiguration config, CommandExecutor exec, VkApi api,
                       IMediator mediator)
        {
            _db = db;
            _config = config;
            _executor = exec;
            _api = api;
            _mediator = mediator;
        }

        public async Task<string> Handle(CallbackResponse callbackResponse)
        {
            if(callbackResponse.Secret != _config["Config:Vk_Secret"])
            {
                return "а Вы кто и шо Вы тут делаете?";
            }

            var type = callbackResponse.Type;
            var dict = new Dictionary<string, Func<CallbackResponse, Task<string>>>
            {
                ["confirmation"] = Confirmation,
                ["message_new"] = MessageNew,
                ["message_deny"] = MessageDeny,
                ["message_reply"] = MessageReply,
                ["group_join"] = GroupJoin,
                ["group_leave"] = GroupLeave
            };

            return await dict[type](callbackResponse);
        }

        private async Task<string> Confirmation(CallbackResponse obj)
        {
            await _mediator.Publish(new ConfirmationNotification());
            return _config["Config:Vk_ConfirmCode"];
        }

        private async Task<string> MessageNew(CallbackResponse obj)
        {
            var message = Message.FromJson(obj.Object.ToString());
            if(message.FromId != message.PeerId)
            {
                //TODO: add conv to db
                var match = Regex.Match(message.Text, @"\[club146048760\|.*\] (.*)").Groups[1].Value;
                if(!string.IsNullOrEmpty(match))
                {
                    message.Text = match;
                }

                //TODO: оповещение о том, что гоблину не нужен доступ ко всей переписке?
            }

            var botUser = _db.BotUsers.FirstOrDefault(x => x.Vk == message.FromId);

            if(botUser is null)
            {
                botUser = (await _db.BotUsers.AddAsync(new BotUser { Vk = message.FromId })).Entity;
                await _db.SaveChangesAsync();
            }

            var response = await _executor.ExecuteCommand(message);

            if(botUser.IsErrorsDisabled && response.Text == CommandExecutor.ErrorMessage)
            {
                return OkResponse;
            }

            await _api.Messages.Send(message.PeerId, response.Text, response.Attachments, response.Keyboard);
            return OkResponse;
        }

        private async Task<string> MessageReply(CallbackResponse obj)
        {
            var message = Message.FromJson(obj.Object.ToString());

            const long confId = 2000000000;
            var isConf = message.PeerId / confId > 0;
            if(isConf || !message.Text.StartsWith('!'))
            {
                return OkResponse;
            }

            message.Text = message.Text.Substring(1, message.Text.Length - 1);
            var response = await _executor.ExecuteCommand(message);

            await _api.Messages.Delete(message.Id);
            await _api.Messages.Send(message.PeerId, response.Text, response.Attachments, response.Keyboard);

            return OkResponse;
        }

        private async Task<string> MessageDeny(CallbackResponse obj)
        {
            await _mediator.Publish(new MessageDenyNotification { Response = obj });
            return OkResponse;
        }

        private async Task<string> GroupJoin(CallbackResponse obj)
        {
            await _mediator.Publish(new GroupJoinNotification { Response = obj });
            return OkResponse;
        }

        private async Task<string> GroupLeave(CallbackResponse obj)
        {
            await _mediator.Publish(new GroupLeaveNotification { Response = obj });
            return OkResponse;
        }
    }
}