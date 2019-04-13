using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Domain.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long Vk { get; set; }
        public bool IsAdmin { get; set; }
        public int Group { get; set; }
        public bool Schedule { get; set; }
        public bool Weather { get; set; }
        public string City { get; set; }
        public bool IsErrorsDisabled { get; set; }
    }
}