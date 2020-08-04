using System;

namespace Goblin.Domain.Entities
{
    public class CronJob
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public long ChatId { get; private set; }

        public int NarfuGroup { get; private set; }
        public string WeatherCity { get; private set; }

        public CronTime Time { get; private set; }

        public ConsumerType ConsumerType { get; private set; }

        protected CronJob()
        {
        }

        public CronJob(string name, long vkId, int narfuGroup, string weatherCity, CronTime time, ConsumerType type)
        {
            SetName(name);
            SetChatId(vkId);
            SetNarfuGroup(narfuGroup);
            SetWeatherCity(weatherCity);
            SetConsumerType(type);
            Time = time;
        }

        public void SetName(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Параметр должен быть непустым", nameof(name));
            }

            Name = name;
        }

        public void SetChatId(long chatId)
        {
            if(chatId <= 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(chatId));
            }

            ChatId = chatId;
        }

        public void SetNarfuGroup(int group)
        {
            if(group < 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(group));
            }

            NarfuGroup = group;
        }

        public void SetWeatherCity(string city)
        {
            WeatherCity = city;
        }

        private void SetConsumerType(ConsumerType type)
        {
            ConsumerType = type;
        }
    }
}