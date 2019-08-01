using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Domain.Entities
{
    public class BotUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; private set; } // Vk id

        public string WeatherCity { get; private set; }
        public int NarfuGroup { get; private set; }

        public bool IsErrorsEnabled { get; private set; }
        public bool IsAdmin { get; private set; }

        private BotUser()
        {
        }

        public BotUser(string city, int group, bool isAdmin, bool isErrorsEnabled)
        {
            SetCity(city);
            SetNarfuGroup(group);
            SetAdmin(isAdmin);
            SetErrorNotification(isErrorsEnabled);
        }

        public void SetCity(string city)
        {
            if(string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("Параметр должен быть непустым", nameof(city));
            }

            WeatherCity = city;
        }

        public void SetNarfuGroup(int group)
        {
            if(group <= 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(group));
            }

            NarfuGroup = group;
        }

        public void SetErrorNotification(bool enable = true)
        {
            IsErrorsEnabled = enable;
        }

        public void SetAdmin(bool isAdmin = false)
        {
            IsAdmin = isAdmin;
        }
    }
}