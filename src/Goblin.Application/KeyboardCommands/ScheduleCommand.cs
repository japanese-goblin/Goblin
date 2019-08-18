using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.KeyboardCommands
{
    public class ScheduleCommand : IKeyboardCommand
    {
        public string Trigger => "schedule";
        
        private readonly NarfuApi _api;

        public ScheduleCommand(NarfuApi api)
        {
            _api = api;
        }
        
        public async Task<IResult> Execute(Message msg, BotUser user = null)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult("Для получения расписания сначала необходимо установить группу.");
            }
            
            var date = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)["schedule"];
            try
            {
                var lessons = await _api.Students.GetScheduleAtDate(user.NarfuGroup, DateTime.Parse(date));
                return new SuccessfulResult
                {
                    Message = lessons.ToString()
                };
            }
            catch(FlurlHttpException ex)
            {
                return new FailedResult($"Невозможно получить расписание с сайта. Попробуйте позже. (Код ошибки - {ex.Call.HttpStatus})");
            }
            catch
            {
                return new FailedResult("Непредвиденная ошибка получения расписания с сайта. Попробуйте позже.");
            }
        }
    }
}