using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Domain.Entities
{
    public class Remind
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long VkId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}