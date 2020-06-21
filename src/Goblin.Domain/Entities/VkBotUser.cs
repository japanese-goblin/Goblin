using Goblin.Domain.Abstractions;

namespace Goblin.Domain.Entities
{
    public class VkBotUser : BotUser
    {
        protected VkBotUser()
        {
        }

        public VkBotUser(long id, string city = "", int group = 0, bool isAdmin = false,
                         bool isErrorsEnabled = true, bool hasWeather = false, bool hasSchedule = false) :
                base(id, city, group, isAdmin, isErrorsEnabled, hasWeather, hasSchedule)
        {
        }
    }
}