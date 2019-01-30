using System.ComponentModel.DataAnnotations;

namespace Goblin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан ник")]
        public string Nick { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
