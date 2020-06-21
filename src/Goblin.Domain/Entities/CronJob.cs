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

        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        public ConsumerType ConsumerType { get; private set; }

        protected CronJob()
        {
        }

        public CronJob(string name, long vkId, int narfuGroup, string weatherCity, int hours, int minutes, ConsumerType type)
        {
            SetName(name);
            SetChatId(vkId);
            SetNarfuGroup(narfuGroup);
            SetWeatherCity(weatherCity);
            SetHours(hours);
            SetMinutes(minutes);
            SetConsumerType(type);
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

        public void SetHours(int hours)
        {
            if(hours < 0 || hours > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hours), hours,
                                                      "Параметр должен быть в пределах от 0 до 23");
            }

            Hours = hours;
        }

        public void SetMinutes(int minutes)
        {
            if(minutes < 0 || minutes > 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), minutes,
                                                      "Параметр должен быть в пределах от 0 до 60");
            }

            Minutes = minutes;
        }

        private void SetConsumerType(ConsumerType type)
        {
            ConsumerType = type;
        }
    }
}