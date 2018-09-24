using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public long Vk { get; set; }
        public int Group { get; set; }
        public bool Schedule { get; set; }
        public bool Weather { get; set; }
        public string City { get; set; }
    }
}