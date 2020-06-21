using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Newtonsoft.Json;
using Serilog;

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
            return await GetSchedule(user.NarfuGroup, DateTime.Parse(date));
        }

        public async Task<IResult> GetSchedule(int narfuGroup, DateTime date)
        {
            if(!_api.Students.IsCorrectGroup(narfuGroup))
            {
                return new FailedResult($"Группа {narfuGroup} не найдена");
            }

            try
            {
                var schedule = await _api.Students.GetScheduleAtDate(narfuGroup, date);

                return new SuccessfulResult
                {
                    Message = schedule.ToString(),
                    Keyboard = DefaultKeyboards.GetScheduleKeyboard()
                };
            }
            catch(FlurlHttpException)
            {
                return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<NarfuApi>().Fatal(ex, "Ошибка при получении расписания на день");
                return new FailedResult(DefaultErrors.NarfuUnexpectedError);
            }
        }
    }
}