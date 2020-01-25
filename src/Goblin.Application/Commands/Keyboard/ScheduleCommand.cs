using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.Commands.Keyboard
{
    public class ScheduleCommand : IKeyboardCommand
    {
        public string Trigger => "schedule";

        private readonly NarfuApi _api;

        public ScheduleCommand(NarfuApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                var text = "Для получения расписания установите группу (нужно написать следующее - установить группу 123456).";
                return new FailedResult(text);
            }

            var date = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)["schedule"];

            return await _api.Students.GetScheduleAtDateWithResult(user.NarfuGroup, DateTime.Parse(date));
        }
    }
}