using System;

namespace Goblin.Domain.Entities
{
    public class BotUser
    {
        public int VkId { get; private set; } // Vk id

        public string WeatherCity { get; private set; }
        public int NarfuGroup { get; private set; }

        public bool IsErrorsEnabled { get; private set; }
        public bool IsAdmin { get; private set; }
        
        public virtual Remind[] Reminds { get; private set; }
        public virtual Subscribe SubscribeInfo { get; private set; }

        private BotUser()
        {
        }

        public BotUser(int vkId, string city = "", int group = 0, bool isAdmin = false, bool isErrorsEnabled = true)
        {
            SetVkId(vkId);
            SetCity(city);
            SetNarfuGroup(group);
            SetAdmin(isAdmin);
            SetErrorNotification(isErrorsEnabled);
        }

        private void SetVkId(int vkId)
        {
            if(vkId <= 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(vkId));
            }

            VkId = vkId;
        }

        public void SetCity(string city)
        { 
            WeatherCity = city;
        }

        public void SetNarfuGroup(int group)
        {
            if(group < 0)
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