using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.Commands.Keyboard
{
    public class MailingCommand : IKeyboardCommand
    {
        private readonly BotDbContext _db;
        public string Trigger => "mailing";

        public MailingCommand(BotDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            user = _db.BotUsers.Find(user.VkId);
            var choose = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)["mailing"];
            var isSchedule = user.SubscribeInfo.IsSchedule;
            var isWeather = user.SubscribeInfo.IsWeather;
            if(choose == "weather")
            {
                user.SubscribeInfo.SetIsWeather(!isWeather);
                await _db.SaveChangesAsync();
                return new SuccessfulResult
                {
                    Message = "Успешно."
                };
            }

            if(choose == "schedule")
            {
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