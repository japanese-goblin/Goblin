using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Models
{
    public class Remind
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int VkID { get; set; }
        public string Text { get; set; }
    }
}
