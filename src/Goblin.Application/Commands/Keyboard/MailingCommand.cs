using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.Commands.Keyboard
{
    public class MailingCommand : IKeyboardCommand
    {
        private readonly BotDbContext _db;

        public MailingCommand(BotDbContext db)
        {
            _db = db;
        }

        public string Trigger => "mailing";

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            user = await _db.BotUsers.FindAsync(user.VkId);
            var choose = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger];
            var isSchedule = user.SubscribeInfo.IsSchedule;
            var isWeather = user.SubscribeInfo.IsWeather;
            if(choose.Equals("weather", StringComparison.CurrentCultureIgnoreCase))
            {
                if(string.IsNullOrWhiteSpace(user.WeatherCity))
                {
                    return new FailedResult(DefaultErrors.CityNotSet);
                }

                user.SubscribeInfo.SetIsWeather(!isWeather);
                await _db.SaveChangesAsync();
                return new SuccessfulResult
                {
                    Message = "Успешно."
                };
            }

            if(choose.Equals("schedule", StringComparison.CurrentCultureIgnoreCase))
            {
                if(user.NarfuGroup == 0)
                {
                    return new FailedResult(DefaultErrors.GroupNotSet);
                }

                user.SubscribeInfo.SetIsSchedule(!isSchedule);
                await _db.SaveChangesAsync();
                return new SuccessfulResult
                {
                    Message = "Успешно."
                };
            }

            return new FailedResult("Действие не найдено");
        }
    }
}