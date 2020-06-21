using System;

namespace Goblin.Domain.Entities
{
    public class Remind
    {
        public int Id { get; private set; }
        public long ChatId { get; private set; }

        public string Text { get; private set; }
        public DateTime Date { get; private set; }

        public ConsumerType ConsumerType { get; private set; }

        protected Remind()
        {
        }

        public Remind(long chatId, string text, DateTime date, ConsumerType type)
        {
            SetChatId(chatId);
            SetText(text);
            SetDateTime(date);
            SetConsumerType(type);
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

        private void SetChatId(long botUserId)
        {
            if(botUserId <= 0)
            {
                throw new ArgumentException("Параметр должен быть больше 0", nameof(botUserId));
            }

            ChatId = botUserId;
        }

        private void SetConsumerType(ConsumerType type)
        {
            ConsumerType = type;
        }
    }
}