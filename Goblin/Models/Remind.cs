using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Models
{
    public class Remind
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public long VkID { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
