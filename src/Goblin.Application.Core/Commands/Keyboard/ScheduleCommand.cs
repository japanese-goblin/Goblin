using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Domain.Abstractions;
using Newtonsoft.Json;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class ScheduleCommand : IKeyboardCommand
    {
        public string Trigger => "schedule";
        private readonly IScheduleService _api;

        public ScheduleCommand(IScheduleService api)
        {
            _api = api;
        }

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult(DefaultErrors.GroupNotSet);
            }

            var date = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger];
            return await _api.GetSchedule(user.NarfuGroup, DateTime.Parse(date));
        }
    }
}