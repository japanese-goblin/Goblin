using System.ComponentModel.DataAnnotations;

namespace Goblin.Application.Telegram.Options;

public class TelegramOptions
{
    [Required]
    public string AccessToken { get; set; } = "";

    [Required]
    public string SecretKey { get; set; } = "";
}