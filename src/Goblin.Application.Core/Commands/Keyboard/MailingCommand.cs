using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;
using Newtonsoft.Json;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class MailingCommand : IKeyboardCommand
    {
        private const string Success = "Успешно.";
        public string Trigger => "mailing";
        private readonly BotDbContext _db;

        public MailingCommand(BotDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            user = _db.Entry(user).Entity;
            var choose = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.MessagePayload)[Trigger];
            var isSchedule = user.HasScheduleSubscription;
            var isWeather = user.HasWeatherSubscription;
            if(choose.Equals("weather", StringComparison.OrdinalIgnoreCase))
            {
                return await SetWeatherMailing(user, isWeather);
            }

            if(choose.Equals("schedule", StringComparison.OrdinalIgnoreCase))
            {
                return await SetScheduleMailing(user, isSchedule);
            }

            return new FailedResult("Действие не найдено");
        }

        private async Task<IResult> SetScheduleMailing(BotUser user, bool isSchedule)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult(DefaultErrors.GroupNotSet);
            }

            user.SetHasSchedule(!isSchedule);
            await _db.SaveChangesAsync();
            return new SuccessfulResult
            {
                Message = Success,
                Keyboard = DefaultKeyboards.GetMailingKeyboard(user)
            };
        }

        private async Task<IResult> SetWeatherMailing(BotUser user, bool isWeather)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult(DefaultErrors.CityNotSet);
            }

            user.SetHasWeather(!isWeather);
            await _db.SaveChangesAsync();
            return new SuccessfulResult
            {
                Message = Success,
                Keyboard = DefaultKeyboards.GetMailingKeyboard(user)
            };
        }
    }
}