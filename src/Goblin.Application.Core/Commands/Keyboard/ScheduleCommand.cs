using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Newtonsoft.Json;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class ScheduleCommand : IKeyboardCommand
    {
        public string Trigger => "schedule";
        private readonly NarfuApi _api;

        public ScheduleCommand(NarfuApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(IMessage msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult(DefaultErrors.GroupNotSet);
            }

            var date = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger];

            var schedule = await _api.Students.GetScheduleAtDate(user.NarfuGroup, DateTime.Parse(date));
            
            
            return new SuccessfulResult()
            {
                Message = schedule.ToString()
            };
        }
    }
}