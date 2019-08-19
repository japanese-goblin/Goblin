using System;

namespace Goblin.Domain.Entities
{
    public class Remind
    {
        public int Id { get; private set; }
        public long BotUserId { get; private set; }

        public string Text { get; private set; }
        public DateTime Date { get; private set; }

        public virtual BotUser BotUser { get; private set; }

        protected Remind()
        {
        }

        public Remind(long botUserId, string text, DateTime date)
        {
            SetBotUserId(botUserId);
            SetText(text);
            SetDateTime(date);
        }

        private void SetText(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Параметр должен быть непустым", nameof(text));
            }

            Text = text;
        }

        private void SetDateTime(DateTime date)
        {
            if(date < DateTime.Now)
            {
                throw new ArgumentException("Дата должна быть больше текущей", nameof(date));
            }

            Date = date;
        }

        private void SetBotUserId(long botUserId)
        {
            if(botUserId <= 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(botUserId));
            }

            BotUserId = botUserId;
        }
    }
}