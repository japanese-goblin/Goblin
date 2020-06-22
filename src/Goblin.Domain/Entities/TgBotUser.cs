using Goblin.Domain.Abstractions;

namespace Goblin.Domain.Entities
{
    public class TgBotUser : BotUser
    {
        protected TgBotUser()
        {
            ConsumerType = ConsumerType.Telegram;
        }

        public TgBotUser(long id, string city = "", int group = 0, bool isAdmin = false,
                         bool isErrorsEnabled = true, bool hasWeather = false, bool hasSchedule = false) :
                base(id, city, group, isAdmin, isErrorsEnabled, hasWeather, hasSchedule)
        {
            ConsumerType = ConsumerType.Telegram;
        }
    }
}