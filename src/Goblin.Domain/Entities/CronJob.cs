using System;

namespace Goblin.Domain.Entities
{
    public class CronJob
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public long VkId { get; private set; }

        public int NarfuGroup { get; private set; }
        public string WeatherCity { get; private set; }

        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        private CronJob()
        {
        }
        
        public CronJob(string name, long vkId, int narfuGroup, string weatherCity, int hours, int minutes)
        {
            SetName(name);
            SetVkId(vkId);
            SetNarfuGroup(narfuGroup);
            SetWeatherCity(weatherCity);
            SetHours(hours);
            SetMinutes(minutes);
        }
        
        public void SetName(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Параметр должен быть непустым", nameof(name));
            }

            Name = name;
        }

        public void SetVkId(long vkId)
        {
            if(vkId <= 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(vkId));
            }

            VkId = vkId;
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
    }
}