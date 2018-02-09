using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int Vk { get; set; }
        public short Group { get; set; } // TODO: short -> int?
        public bool Schedule { get; set; }
        public bool Weather { get; set; }
        public string City { get; set; }
        public int CityNumber { get; set; }
    }
}