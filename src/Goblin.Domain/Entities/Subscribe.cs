using System;

namespace Goblin.Domain.Entities
{
    public class Subscribe
    {
        public int Id { get; private set; }
        public long BotUserId { get; private set; }

        public bool IsWeather { get; private set; }
        public bool IsSchedule { get; private set; }

        public virtual BotUser BotUser { get; private set; }

        protected Subscribe()
        {
        }

        public Subscribe(long botUserId, bool isWeather, bool isSchedule)
        {
            SetBotUserId(botUserId);
            SetIsWeather(isWeather);
            SetIsSchedule(isSchedule);
        }

        private void SetBotUserId(long botUserId)
        {
            if(botUserId <= 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(botUserId));
            }

            BotUserId = botUserId;
        }

        public void SetIsWeather(bool isWeather)
        {
            IsWeather = isWeather;
        }

        public void SetIsSchedule(bool isSchedule)
        {
            IsSchedule = isSchedule;
        }
    }
}