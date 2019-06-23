using System;

namespace Goblin.Domain.Entities
{
    public class Remind
    {
        public int Id { get; set; }

        public long VkId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}