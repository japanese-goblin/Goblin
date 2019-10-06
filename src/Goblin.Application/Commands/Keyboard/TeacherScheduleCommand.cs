using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.Commands.Keyboard
{
    public class TeacherScheduleCommand : IKeyboardCommand
    {
        public string Trigger => "teacherSchedule";

        private readonly NarfuApi _narfuApi;

        public TeacherScheduleCommand(NarfuApi narfuApi)
        {
            _narfuApi = narfuApi;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(msg.Payload))
            {
                return new FailedResult("Невозожно получить ID преподавателя.");
            }

            var id = int.Parse(JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger]);
            try
            {
                var schedule = await _narfuApi.Teachers.GetLimitedSchedule(id);

                return new SuccessfulResult
                {
                    Message = schedule.ToString()
                };
            }
            catch(FlurlHttpException ex)
            {
                return new
                        FailedResult($"Сайт с расписанием недоступен (код ошибки - {ex.Call.HttpStatus}). Попробуйте позже.");
            }
            catch(Exception)
            {
                return new FailedResult("Непредвиденная ошибка получения расписания. Попробуйте позже.");
            }
        }
    }
}