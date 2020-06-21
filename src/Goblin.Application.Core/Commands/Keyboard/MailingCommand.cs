using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Newtonsoft.Json;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class MailingCommand : IKeyboardCommand
    {
        public string Trigger => "mailing";
        private readonly BotDbContext _db;

        public MailingCommand(BotDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> Execute(IMessage msg, BotUser user)
        {
            const string success = "Успешно.";

            user = await _db.BotUsers.FindAsync(user.Id);
            var choose = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger];
            var isSchedule = user.HasScheduleSubscription;
            var isWeather = user.HasWeatherSubscription;
            if(choose.Equals("weather", StringComparison.OrdinalIgnoreCase))
            {
                if(string.IsNullOrWhiteSpace(user.WeatherCity))
                {
                    return new FailedResult(DefaultErrors.CityNotSet);
                }

                user.SetHasWeather(!isWeather);
                await _db.SaveChangesAsync();
                return new SuccessfulResult
                {
                    Message = success,
                    Keyboard = DefaultKeyboards.GetMailingKeyboard(user)
                };
            }

            if(choose.Equals("schedule", StringComparison.OrdinalIgnoreCase))
            {
                if(user.NarfuGroup == 0)
                {
                    return new FailedResult(DefaultErrors.GroupNotSet);
                }

                user.SetHasSchedule(!isSchedule);
                await _db.SaveChangesAsync();
                return new SuccessfulResult
                {
                    Message = success,
                    Keyboard = DefaultKeyboards.GetMailingKeyboard(user)
                };
            }

            return new FailedResult("Действие не найдено");
        }
    }
}