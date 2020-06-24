using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;
using Goblin.Narfu.Abstractions;
using Newtonsoft.Json;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class TeacherScheduleCommand : IKeyboardCommand
    {
        public string Trigger => "teacherSchedule";
        private readonly INarfuApi _narfuApi;

        public TeacherScheduleCommand(INarfuApi narfuApi)
        {
            _narfuApi = narfuApi;
        }

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload);
            var isExists = dict.TryGetValue(Trigger, out var idString);
            if(!isExists)
            {
                return new FailedResult("Невозожно получить ID преподавателя.");
            }

            var isCorrectId = int.TryParse(idString, out var id);
            if(!isCorrectId)
            {
                return new FailedResult("Некорректный ID преподавателя.");
            }

            try
            {
                var schedule = await _narfuApi.Teachers.GetLimitedSchedule(id);

                return new SuccessfulResult
                {
                    Message = schedule.ToString()
                };
            }
            catch(FlurlHttpException)
            {
                return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
            }
            catch(Exception)
            {
                return new FailedResult(DefaultErrors.NarfuUnexpectedError);
            }
        }
    }
}